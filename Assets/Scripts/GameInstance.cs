using SR.Customization;
using SR.Library;
using SR.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Purchasing;
using YG;
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

	public class GameInstance : MonoBehaviour
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

		[Header("Components")]
		[SerializeField] private MenusListSO menusLibrary;
		[SerializeField] private ShopLibrarySO shopLibrary;

		[Header("Properties")]
		[SerializeField] private string recordSaveSlot;
		[SerializeField] private string carConfigSaveSlot;
		[SerializeField] private string unlockedDetailsSaveSlot;
		[SerializeField] private string gameSettingsSaveSlot;
		[SerializeField] private List<string> distanceMarkers;

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

		private void Awake()
		{
#if !UNITY_WEBGL
			LoadRecords();
			LoadCarConfig();
			LoadUnlockedDetails();
			LoadGameSettings();
#elif UNITY_WEBGL
			Application.focusChanged += Application_focusChanged;
#endif
			RangeEnemy.onAnyEnemyShoot += RangeEnemy_onAnyEnemyShoot;
			InitializeShop();
			MenuBase.menusLibrary = menusLibrary;
			Obstacle.onObstacleDestroyed += Obstacle_onObstacleDestroyed;
		}

#if UNITY_WEBGL
		private void Start()
		{
			YandexGame.GetDataEvent = OnLoad;
		}

		public void OnLoad()
		{
			YandexGame.StickyAdActivity(true);
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
			YandexGame.NewLeaderboardScores("RoadKing", (int)YandexGame.savesData.records.totalDistance);
			UpdateUnlockedDetails();
			if (GameSettings.bSoundsOn)
				soundSystem.EnableSound();
			else
				soundSystem.DisableSound();
		}
#endif

		private void Application_focusChanged(bool obj)
		{
			if (!obj)
				soundSystem.Mute();
			else
				soundSystem.Unmute();
		}

		#endregion

		#region Functions

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

		public string GetDistanceString()
		{
			float tempDistance = records.totalDistance;

			StringBuilder sb = new StringBuilder();

			return GetShortStringDistance(tempDistance);
		}

		public string GetShortStringDistance(float value)
		{
			int val = 0;

			while (value >= 1000f)
			{
				value /= 1000f;
				val++;
			}

			return $"{value: 0.0}{distanceMarkers[val]} m";
		}

		public string GetShortString(int value)
		{
			int val = 0;

			while (value >= 1000)
			{
				value /= 1000;
				val++;
			}

			return $"{value}{distanceMarkers[val]}";
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

		public void SaveRecords()
		{
#if !UNITY_WEBGL
			string str = JsonUtility.ToJson(records);
			PlayerPrefs.SetString(recordSaveSlot, str);
#else
			YandexGame.savesData.records = records;
#endif
		}

		public void SaveCarConfig()
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

		public void SaveUnlockedDetails()
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
				var defaultCar = shopLibrary.GetStandartCar();
				unlockedDetails = new UnlockedDetails();
				unlockedDetails.unlockedWeapons.Add(defaultCar.weapon);
				unlockedDetails.unlockedWheels.Add(defaultCar.wheels);
				unlockedDetails.unlockedBumpers.Add(defaultCar.bumper);
				unlockedDetails.unlockedStickmans.Add(defaultCar.stickman);
				unlockedDetails.unlockedBackdoors.Add(defaultCar.backdoor);
				SaveUnlockedDetails();
			}
			UpdateUnlockedDetails();
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
