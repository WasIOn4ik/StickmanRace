using SR.Customization;
using SR.Library;
using SR.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using YG;
using YG.Example;
using Zenject;

namespace SR.Core
{
	[Serializable]
	public struct GameRecords
	{
		public float totalDistance;
		public float maxTime;
		public int gems;
	}

	[Serializable]
	public class UnlockedDetails
	{
		public List<string> unlockedWheels = new();
		public List<string> unlockedBumpers = new();
		public List<string> unlockedBackdoors = new();
		public List<string> unlockedWeapons = new();
		public List<string> unlockedStickmans = new();
	}

	[Serializable]
	public struct CarConfig
	{
		public string wheels;
		public string bumper;
		public string backdoor;
		public string weapon;
		public string stickman;
	}

	[Serializable]
	public struct GameSettings
	{
		public bool bSoundsOn;
	}

#if UNITY_ANDROID
	public class GameInstance : MonoBehaviour, IDetailedStoreListener
#elif UNITY_WEBGL
	public class GameInstance : MonoBehaviour
#endif
	{
		#region HelperClasses

		public class DetailEventArgs : EventArgs
		{
			public CarDetailSO detail;
		}

		#endregion

		#region Variables

		public event EventHandler<DetailEventArgs> onDetailChanged;
		public event EventHandler onGemsCountChanged;

#if UNITY_ANDROID

		#region Variables

		public const string NO_ADS_ID = "com.no_ads";
		public const string NO_ADS_SUBTYPE = "no_ads";

		public Action onPurchaseCompleted;

		public BannerAdObject banner;
		public InterstitialAdObject interstitial;
		public RewardedAdObject rewarded;

		public static IStoreController storeController;
		public static IExtensionProvider extensionProvider;

		public bool bNoAdsBought;
		private bool isSaving;

		#endregion

		#region IStoreListener

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			Debug.Log("Android IAP initialized");
			storeController = controller;
			List<Product> sortedProducts = storeController.products.all.OrderBy(item => item.metadata.localizedPrice).ToList();
			foreach (var p in sortedProducts)
			{
				if (p.definition.id == NO_ADS_ID)
				{
					Debug.Log($"Handling NO-AD {p.hasReceipt}");
					bNoAdsBought = p.hasReceipt;
					if (banner)
						SetBannerActivity(true);
				}
			}

			extensionProvider = extensions;
			if (!StoreItemProvider.IsInitialized())
			{
				StoreItemProvider.Initialize(storeController.products);
			}
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			Debug.LogError(error.ToString());
		}

		public void OnInitializeFailed(InitializationFailureReason error, string message)
		{
			Debug.LogError(error.ToString() + " " + message);
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			Debug.LogError("Failed to purchase product: " + failureReason.ToString());
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
		{
			Debug.Log("Purchase!");

			var def = purchaseEvent.purchasedProduct.definition;
			foreach (var pay in def.payouts)
			{
				Debug.Log($"Payouts: {pay.subtype} x {pay.quantity}");
				if (pay.subtype == "Gems")
				{
					AddBoughtGems((int)pay.quantity);
				}
				else if (pay.subtype == NO_ADS_SUBTYPE)
				{
					bNoAdsBought = true;
					Debug.Log("No ads bought");
				}
			}

			onPurchaseCompleted?.Invoke();
			onPurchaseCompleted = null;

			return PurchaseProcessingResult.Complete;
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
		{
			onPurchaseCompleted?.Invoke();
			onPurchaseCompleted = null;
		}

		#endregion

		#region Purchases

		private void HandleIAPPCatalog(AsyncOperation operation)
		{
			Debug.Log("Handling IAP catalog");
			var request = operation as ResourceRequest;
			ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);

#if UNITY_EDITOR
			StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
			StandardPurchasingModule.Instance().useFakeStoreAlways = true;
#endif

			ConfigurationBuilder builder = ConfigurationBuilder.Instance(
				StandardPurchasingModule.Instance(AppStore.GooglePlay));
			foreach (ProductCatalogItem item in catalog.allProducts)
			{
				List<PayoutDefinition> pays = new List<PayoutDefinition>();

				foreach (var pd in item.Payouts)
				{
					var pay = new PayoutDefinition(pd.subtype, pd.quantity, pd.data);
					pays.Add(pay);
				}

				builder.AddProduct(item.id, item.type, null, pays);
			}

			UnityPurchasing.Initialize(this, builder);
		}

		#endregion

		#region AD

		public void ClearBannerCallbacks()
		{
			banner.OnAdFailedToLoad.RemoveAllListeners();
			banner.OnAdClicked.RemoveAllListeners();
			banner.OnAdHidden.RemoveAllListeners();
			banner.OnAdImpression.RemoveAllListeners();
			banner.OnAdLoaded.RemoveAllListeners();
			banner.OnAdShown.RemoveAllListeners();
		}

		public void SetBannerActivity(bool active)
		{
			banner.gameObject.SetActive(active && !bNoAdsBought);
		}

		public void ClearInterstitialCallbacks()
		{
			interstitial.OnAdFailedToLoad.RemoveAllListeners();
			interstitial.OnAdFailedToShow.RemoveAllListeners();
			interstitial.OnAdClicked.RemoveAllListeners();
			interstitial.OnAdClosed.RemoveAllListeners();
			interstitial.OnAdImpression.RemoveAllListeners();
			interstitial.OnAdLoaded.RemoveAllListeners();
			interstitial.OnAdShown.RemoveAllListeners();
		}

		public void ClearRewardedCallbacks()
		{
			rewarded.OnAdClicked.RemoveAllListeners();
			rewarded.OnAdClosed.RemoveAllListeners();
			rewarded.OnAdFailedToLoad.RemoveAllListeners();
			rewarded.OnAdFailedToShow.RemoveAllListeners();
			rewarded.OnAdImpression.RemoveAllListeners();
			rewarded.OnAdLoaded.RemoveAllListeners();
			rewarded.OnAdShown.RemoveAllListeners();
			rewarded.OnReward.RemoveAllListeners();
		}

		#endregion

		#region SaveGames

		[Serializable]
		private struct AndroidSaveGame
		{
			public GameRecords records;
			public CarConfig carConfig;
			public UnlockedDetails unlockedDetails;
			public GameSettings settings;
		}

		public void OpenSave(bool save)
		{
			if (Social.localUser.authenticated)
			{
				isSaving = save;
				((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution("PlayerSave", DataSource.ReadCacheOrNetwork,
					GooglePlayGames.BasicApi.SavedGame.ConflictResolutionStrategy.UseLongestPlaytime, SaveGameOpen);
			}
		}

		private string GetSaveString()
		{
			AndroidSaveGame saveGameRaw = new AndroidSaveGame() { records = records, carConfig = carConfig, settings = GameSettings, unlockedDetails = unlockedDetails };
			return JsonUtility.ToJson(saveGameRaw);
		}

		private void SaveGameOpen(SavedGameRequestStatus status, ISavedGameMetadata metadata)
		{
			if (status == SavedGameRequestStatus.Success)
			{
				if (isSaving)
				{
					byte[] playerData = System.Text.ASCIIEncoding.ASCII.GetBytes(GetSaveString());
					SavedGameMetadataUpdate updateForMetadata = new SavedGameMetadataUpdate.Builder().Build();
					((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(metadata, updateForMetadata, playerData, SaveCallback);
				}
				else
				{
					((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(metadata, LoadCallback);
				}
			}
			else
			{
				Debug.Log($"Error while open save: {status.ToString()}");
			}
		}

		private void LoadCallback(SavedGameRequestStatus status, byte[] data)
		{
			if (status == SavedGameRequestStatus.Success)
			{
				string playerData = System.Text.Encoding.ASCII.GetString(data);
				if (!String.IsNullOrEmpty(playerData))
				{
					AndroidSaveGame saveGameRaw = JsonUtility.FromJson<AndroidSaveGame>(playerData);

					records = saveGameRaw.records;
					GameSettings = saveGameRaw.settings;
					unlockedDetails = saveGameRaw.unlockedDetails;
					carConfig = saveGameRaw.carConfig;

					Debug.Log("Loaded successfully");
				}
				else
				{
					Debug.Log("Loaded empty data slot");
				}
			}
			else
			{
				Debug.Log($"Error while loading: {status.ToString()}");/*
				try
				{
					if(TryLoadLocalData()) // Гугл недоступен, но есть локальные данные
					{

					}
					else // Первый вход, но гугл недоступен
					{
						CreateInitialData();
					}
				}
				catch (Exception e) // Гугл недоступен, а локальные данные повреждены
				{
					CreateInitialData();
					Debug.LogException(e);
				}*/
			}
			UpdateUnlockedDetails();
			if (GameSettings.bSoundsOn)
			{
				soundSystem.EnableSound();
				soundSystem.PlayMenuMusic();
			}
			else
				soundSystem.DisableSound();
		}

		private void SaveCallback(SavedGameRequestStatus status, ISavedGameMetadata metadata)
		{
			isSaving = false;
			if (status == SavedGameRequestStatus.Success)
			{
				Debug.Log("Player data successfully saved to cloud");
			}
			else
			{
				Debug.LogError($"Error while saving: {status.ToString()}");
			}
		}

		#endregion
#endif

		[Header("Components")]
		[SerializeField] private MenusListSO menusLibrary;
		[SerializeField] private ShopLibrarySO shopLibrary;

		[Header("Properties")]
		[SerializeField] private string recordSaveSlot;
		[SerializeField] private string carConfigSaveSlot;
		[SerializeField] private string unlockedDetailsSaveSlot;
		[SerializeField] private string gameSettingsSaveSlot;

		[Header("Gems")]
		[SerializeField] private float distanceToGemsPow = 1.5f;
		[SerializeField] private float distanceDemultiplier = 50f;

		public SoundSystem Sounds { get { return soundSystem; } }

		[Inject] private SoundSystem soundSystem;

		private GameRecords records = new GameRecords() { totalDistance = 0f, maxTime = 0f };
		private CarConfig carConfig = new CarConfig();
		private UnlockedDetails unlockedDetails;
		private GameSettings GameSettings = new GameSettings() { bSoundsOn = true };

		#endregion

		#region UnityMessages

#if UNITY_ANDROID

		private async void Awake()
		{
			CreateInitialData();
			UpdateUnlockedDetails();
			if (!Social.localUser.authenticated)
			{
				PlayGamesPlatform.Activate();
				PlayGamesPlatform.Instance.ManuallyAuthenticate(code =>
				{
					if (code == SignInStatus.Success)
					{
						Debug.Log("Auth success");
						OpenSave(false);
					}
					else
					{
						Debug.LogError($"Error while auth {code}");
					}
				});
			}
			await UnityServices.InitializeAsync();
			ResourceRequest request = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
			request.completed += HandleIAPPCatalog;
#elif UNITY_WEBGL
		private void Awake()
		{
			Application.focusChanged += Application_focusChanged;
#endif
			InitializeShop();
			RangeEnemy.onAnyEnemyShoot += RangeEnemy_onAnyEnemyShoot;
			MenuBase.menusLibrary = menusLibrary;
			Obstacle.onObstacleDestroyed += Obstacle_onObstacleDestroyed;
		}

		public void ShowInterstitial(Action onSuccess)
		{
#if UNITY_ANDROID
			if (bNoAdsBought)
			{
				onSuccess?.Invoke();
				return;
			}
			interstitial.OnAdClosed.AddListener(() =>
			{
				soundSystem.Unmute();
				ClearInterstitialCallbacks();
				onSuccess?.Invoke();
			});
			interstitial.OnAdFailedToLoad.AddListener(reason =>
			{
				Debug.Log($"Failed to load interstitial {reason}");
				soundSystem.Unmute();
				ClearInterstitialCallbacks();
				onSuccess?.Invoke();

			});
			interstitial.OnAdFailedToShow.AddListener(reason =>
			{
				Debug.Log($"Failed to show interstitial {reason}");
				soundSystem.Unmute();
				ClearInterstitialCallbacks();
				onSuccess?.Invoke();
			});
			soundSystem.Mute();
			interstitial.Present();
#elif UNITY_WEBGL

			if (YandexGame.savesData.noAdsBought)
			{
				onSuccess?.Invoke();
				return;
			}
			else
			{
				YandexGame.Instance.ResetTimerFullAd();
				YandexGame.OpenFullAdEvent = () =>
				{
					Debug.Log("Interstitial opened");
					Sounds.Mute();
				};
				YandexGame.ErrorFullAdEvent = () =>
				{
					Debug.Log("Error full ad event");
					Sounds.Unmute();
					YandexGame.StickyAdActivity(!YandexGame.savesData.noAdsBought);
					onSuccess?.Invoke();
				};
				YandexGame.CloseFullAdEvent = () =>
				{
					Debug.Log("Interstitial AD closed");
					Sounds.Unmute();
					YandexGame.StickyAdActivity(!YandexGame.savesData.noAdsBought);
					onSuccess?.Invoke();
				};
				YandexGame.FullscreenShow();
			}
#endif
		}

		public void ShowRewarded(Action onSuccess)
		{
#if UNITY_ANDROID
			rewarded.OnReward.AddListener(() =>
			{
				ClearRewardedCallbacks();
				soundSystem.Unmute();
				onSuccess?.Invoke();
			});
			rewarded.OnAdClosed.AddListener(() =>
			{
				ClearRewardedCallbacks();
				soundSystem.Unmute();
			});
			rewarded.OnAdFailedToLoad.AddListener(reason =>
			{
				Debug.Log($"Rewarded ad failed to load {reason}");
				ClearRewardedCallbacks();
				soundSystem.Unmute();
			});
			rewarded.OnAdFailedToShow.AddListener(reason =>
			{
				Debug.Log($"Rewarded ad failed to show {reason}");
				ClearRewardedCallbacks();
				soundSystem.Unmute();

			});
			soundSystem.Mute();
			rewarded.Present();
#elif UNITY_WEBGL
			YandexGame.OpenVideoEvent = () =>
			{
				Debug.Log("VideoEvent active");
				Sounds.Mute();
			};
			YandexGame.ErrorVideoEvent = () =>
			{
				Debug.Log("Error with video event");
				Sounds.Unmute();
			};
			YandexGame.CloseFullAdEvent = () =>
			{
				Debug.Log("FullAddClosed");
				Sounds.Unmute();
			};
			YandexGame.RewardVideoEvent = (x) =>
			{
				Debug.Log($"Reward video event {x}");
				Sounds.Unmute();
				onSuccess?.Invoke();
			};
			YandexGame.Instance._RewardedShow(0);
#endif
		}

#if UNITY_WEBGL

		private void Start()
		{
			YandexGame.GetDataEvent = OnLoad;
		}

		public void OnLoad()
		{
			if (YandexGame.savesData.details == null)
			{
				YandexGame.savesData.records = new GameRecords() { totalDistance = 0f, maxTime = 0f };
				YandexGame.savesData.settings = new GameSettings() { bSoundsOn = true };
				YandexGame.savesData.carConfig = shopLibrary.GetStandartCar();
				var defaultCar = shopLibrary.GetStandartCar();
				unlockedDetails = new UnlockedDetails();
				unlockedDetails.unlockedWeapons.Add(defaultCar.weapon);
				unlockedDetails.unlockedWheels.Add(defaultCar.wheels);
				unlockedDetails.unlockedBumpers.Add(defaultCar.bumper);
				unlockedDetails.unlockedStickmans.Add(defaultCar.stickman);
				unlockedDetails.unlockedBackdoors.Add(defaultCar.backdoor);
				YandexGame.savesData.details = unlockedDetails;
				YandexGame.SaveProgress();
			}

			records = YandexGame.savesData.records;
			carConfig = YandexGame.savesData.carConfig;
			GameSettings = YandexGame.savesData.settings;
			unlockedDetails = YandexGame.savesData.details;

			if (YandexGame.auth && YandexGame.playerName != "anonymous")
			{
				Debug.Log("Updating Road");
				YandexGame.NewLeaderboardScores("Road", (int)YandexGame.savesData.records.totalDistance);
			}
			UpdateUnlockedDetails();
			if (GameSettings.bSoundsOn)
				soundSystem.EnableSound();
			else
				soundSystem.DisableSound();

			if (!YandexGame.savesData.noAdsBought)
			{
				for (int i = 0; i < YandexGame.PaymentsData.id.Length; i++)
				{
					if (YandexGame.PaymentsData.id[i] == "no_ads" && YandexGame.PaymentsData.purchased[i] > 0)
					{
						Debug.Log("No-ADs reovered");
						YandexGame.savesData.noAdsBought = true;
						YandexGame.SaveProgress();
					}
				}
			}
			else
			{
				Debug.Log("No-ADs active");
			}
			if (!YandexGame.savesData.noAdsBought)
			{
				Debug.Log("No-ADs not active");
			}
			YandexGame.StickyAdActivity(!YandexGame.savesData.noAdsBought);
		}
		private void Application_focusChanged(bool obj)
		{
			if (!obj)
				soundSystem.Mute();
			else if (!YandexGame.nowVideoAd && !YandexGame.nowFullAd)
				soundSystem.Unmute();
		}

#endif

		#endregion

		#region Functions

#if UNITY_ANDROID

		private void SaveLocalData()
		{
			SaveGameSettings();
			SaveCarConfig();
			SaveUnlockedDetails();
			SaveRecords();
		}

		private bool TryLoadLocalData()
		{
			if (PlayerPrefs.HasKey(carConfigSaveSlot) && PlayerPrefs.HasKey(unlockedDetailsSaveSlot))
			{
				LoadRecords();
				LoadCarConfig();
				LoadUnlockedDetails();
				LoadGameSettings();

				return true;
			}

			return false;
		}

#endif

		public void SetSounds(bool sounds)
		{
			GameSettings.bSoundsOn = sounds;
		}

		public void AddBoughtGems(int count)
		{
			records.gems += count;
			onGemsCountChanged?.Invoke(this, EventArgs.Empty);
			SaveRecords();
			ConfirmSave();
		}

		public int DistanceToGems(float distance)
		{
			return (int)Mathf.Pow(distance / distanceDemultiplier, distanceToGemsPow);
		}

		public bool IsEquipped(CarDetailSO detail)
		{
			switch (detail.type)
			{
				case CarDetailType.Wheels:
					return detail.identifier == carConfig.wheels;
				case CarDetailType.Bumper:
					return detail.identifier == carConfig.bumper;
				case CarDetailType.BackDoor:
					return detail.identifier == carConfig.backdoor;
				case CarDetailType.Weapon:
					return detail.identifier == carConfig.weapon;
				case CarDetailType.Stickman:
					return detail.identifier == carConfig.stickman;
			}

			return false;
		}

		public bool TryBuyDetail(CarDetailSO detail)
		{
			if (detail.price <= records.gems)
			{
				records.gems -= detail.price;
				detail.bUnlocked = true;
				switch (detail.type)
				{
					case CarDetailType.Wheels:
						unlockedDetails.unlockedWheels.Add(detail.identifier);
						break;
					case CarDetailType.Bumper:
						unlockedDetails.unlockedBumpers.Add(detail.identifier);
						break;
					case CarDetailType.BackDoor:
						unlockedDetails.unlockedBackdoors.Add(detail.identifier);
						break;
					case CarDetailType.Weapon:
						unlockedDetails.unlockedWeapons.Add(detail.identifier);
						break;
					case CarDetailType.Stickman:
						unlockedDetails.unlockedStickmans.Add(detail.identifier);
						break;
				}
				SaveUnlockedDetails();
				SaveRecords();
				TryUpdateCarConfig(detail);
				ConfirmSave();
				onGemsCountChanged?.Invoke(this, EventArgs.Empty);
				return true;
			}
			return false;
		}

		public string GetShortStringDistance(float value)
		{
			var str = GetShortString((int)value);

			LocalizedString localString = new LocalizedString();
			localString.TableReference = "UI";
			localString.TableEntryReference = "DIstanceMarker";

			return $"{str} {localString.GetLocalizedString()}";
		}
		public string GetShortStringTime(float value)
		{
			var str = GetShortString((int)value);

			LocalizedString localString = new LocalizedString();
			localString.TableReference = "UI";
			localString.TableEntryReference = "TimeMarker";

			return $"{str} {localString.GetLocalizedString()}";
		}

		public string GetShortString(int valueToShort)
		{
			int markerNum = 0;

			while (valueToShort >= 1000)
			{
				valueToShort /= 1000;
				markerNum++;
			}

			LocalizedString str = new LocalizedString();
			str.TableReference = "UI";
			str.TableEntryReference = $"MarkerReduction_{markerNum}";
			return $"{valueToShort}{str.GetLocalizedString()}";
		}

		public ShopLibrarySO GetShopLibrary()
		{
			return shopLibrary;
		}

		public CarConfig GetCarConfig()
		{
			return carConfig;
		}

		public GameRecords GetRecords()
		{
			return records;
		}

		public void TryUpdateRecords(float distance, float time, int kills)
		{
			records.totalDistance += distance;
			records.gems += kills;// DistanceToGems(distance);

			if (time > records.maxTime)
			{
				records.maxTime = time;
			}

			SaveRecords();
			ConfirmSave();
		}

		public void TryUpdateCarConfig(CarDetailSO newDetail)
		{
			switch (newDetail.type)
			{
				case CarDetailType.Wheels:
					carConfig.wheels = newDetail.identifier;
					break;
				case CarDetailType.Bumper:
					carConfig.bumper = newDetail.identifier;
					break;
				case CarDetailType.BackDoor:
					carConfig.backdoor = newDetail.identifier;
					break;
				case CarDetailType.Weapon:
					carConfig.weapon = newDetail.identifier;
					break;
				case CarDetailType.Stickman:
					carConfig.stickman = newDetail.identifier;
					break;
			}

			onDetailChanged?.Invoke(this, new DetailEventArgs() { detail = newDetail });
			SaveCarConfig();
		}

		private void SaveRecords()
		{
#if UNITY_ANDROID
			string str = JsonUtility.ToJson(records);
			PlayerPrefs.SetString(recordSaveSlot, str);
#else
			YandexGame.savesData.records = records;
#endif
		}

		private void SaveCarConfig()
		{
#if !UNITY_WEBGL
			string str = JsonUtility.ToJson(carConfig);
			PlayerPrefs.SetString(carConfigSaveSlot, str);
#else
			YandexGame.savesData.carConfig = carConfig;
#endif
		}

		public void SaveGameSettings()
		{
#if !UNITY_WEBGL
			string str = JsonUtility.ToJson(GameSettings);
			PlayerPrefs.SetString(gameSettingsSaveSlot, str);
#else
			YandexGame.savesData.settings = GameSettings;
#endif
		}

		private void SaveUnlockedDetails()
		{
#if !UNITY_WEBGL
			string str = JsonUtility.ToJson(unlockedDetails);
			PlayerPrefs.SetString(unlockedDetailsSaveSlot, str);
#else
			if (unlockedDetails == null)
				unlockedDetails = new UnlockedDetails();

			YandexGame.savesData.details = unlockedDetails;
#endif
		}

		public void ConfirmSave()
		{
#if UNITY_WEBGL
			YandexGame.SaveProgress();
#elif UNITY_ANDROID
			OpenSave(true);
#endif
		}

		public void LoadGameSettings()
		{
			if (PlayerPrefs.HasKey(gameSettingsSaveSlot))
			{
				var str = PlayerPrefs.GetString(gameSettingsSaveSlot);
				GameSettings = JsonUtility.FromJson<GameSettings>(str);
			}
			else
			{
				SaveGameSettings();
			}

			if (GameSettings.bSoundsOn)
				soundSystem.EnableSound();
			else
				soundSystem.DisableSound();
		}

		private void CreateInitialData()
		{
			carConfig = shopLibrary.GetStandartCar();
			unlockedDetails = CreateUnlockedDetails();
			GameSettings = new GameSettings() { bSoundsOn = true };
			records = new GameRecords() { gems = 0, maxTime = 0, totalDistance = 0 };
		}

		private void LoadRecords()
		{
			if (PlayerPrefs.HasKey(recordSaveSlot))
			{
				string str = PlayerPrefs.GetString(recordSaveSlot);
				records = JsonUtility.FromJson<GameRecords>(str);
			}
			else
			{
				SaveRecords();
			}
		}

		private void LoadCarConfig()
		{
			if (PlayerPrefs.HasKey(carConfigSaveSlot))
			{
				string str = PlayerPrefs.GetString(carConfigSaveSlot);
				carConfig = JsonUtility.FromJson<CarConfig>(str);
			}
			else
			{
				carConfig = shopLibrary.GetStandartCar();
				SaveCarConfig();
			}
		}

		private void LoadUnlockedDetails()
		{
			if (PlayerPrefs.HasKey(unlockedDetailsSaveSlot))
			{
				string str = PlayerPrefs.GetString(unlockedDetailsSaveSlot);
				unlockedDetails = JsonUtility.FromJson<UnlockedDetails>(str);
			}
			else
			{
				unlockedDetails = CreateUnlockedDetails();
				SaveUnlockedDetails();
			}
			UpdateUnlockedDetails();
		}

		private UnlockedDetails CreateUnlockedDetails()
		{
			var defaultCar = shopLibrary.GetStandartCar();
			var result = new UnlockedDetails();
			result.unlockedWeapons.Add(defaultCar.weapon);
			result.unlockedWheels.Add(defaultCar.wheels);
			result.unlockedBumpers.Add(defaultCar.bumper);
			result.unlockedStickmans.Add(defaultCar.stickman);
			result.unlockedBackdoors.Add(defaultCar.backdoor);
			return result;
		}
		public void CallUpdateCallbacks()
		{
			onGemsCountChanged?.Invoke(this, EventArgs.Empty);
		}

		private void UpdateUnlockedDetails()
		{
			foreach (var w in unlockedDetails.unlockedWeapons)
			{
				shopLibrary.GetWeapon(w).bUnlocked = true;
			}

			foreach (var w in unlockedDetails.unlockedWheels)
			{
				shopLibrary.GetWheels(w).bUnlocked = true;
			}

			foreach (var b in unlockedDetails.unlockedBumpers)
			{
				shopLibrary.GetBumper(b).bUnlocked = true;
			}

			foreach (var b in unlockedDetails.unlockedBackdoors)
			{
				shopLibrary.GetBackdoor(b).bUnlocked = true;
			}

			foreach (var s in unlockedDetails.unlockedStickmans)
			{
				shopLibrary.GetStickman(s).bUnlocked = true;
			}
		}

		private void InitializeShop()
		{
			shopLibrary.Initialize();
		}

		#endregion

		#region Callbacks

		private void RangeEnemy_onAnyEnemyShoot(object sender, EventArgs e)
		{
			soundSystem.PlayEnemyShoot();
		}

		private void Obstacle_onObstacleDestroyed(object sender, EventArgs e)
		{
			soundSystem.PlayObstacleDestruction();
		}

		#endregion
	}
}
