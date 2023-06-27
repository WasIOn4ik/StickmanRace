using SR.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;
using YG;
using Zenject;

namespace SR.UI
{
#if UNITY_ANDROID
	public class DonateShopMenuUI : MenuBase, IDetailedStoreListener
#elif UNITY_WEBGL
	public class DonateShopMenuUI : MenuBase
#endif
	{
		#region Variables

		[SerializeField] private DonateItemUI itemPrefab;
		[SerializeField] private RectTransform itemsHolder;
		[SerializeField] private Button backButton;
		[SerializeField] private PaymentsYG payments;

		[Inject] GameInstance gameInstance;

#if UNITY_ANDROID
		private static IStoreController storeController;
		private static IExtensionProvider extensionProvider;
#elif UNITY_WEBGL
		private static bool bInitialized;
		private string currentPurchase;
#endif
		List<DonateItemUI> spawnedItems = new List<DonateItemUI>();

		private Action onPurchaseCompleted;

		[Inject] private SoundSystem soundsSystem;

		#endregion

		#region Functions

		private bool IsInitialized()
		{
#if UNITY_ANDROID
			return storeController != null && extensionProvider != null;
#elif UNITY_WEBGL
			return bInitialized;
#endif
		}

		private async void Awake()
		{
			backButton.onClick.AddListener(() =>
			{
				soundsSystem.PlayButton2();
				BackToPrevious();
			});
#if UNITY_ANDROID
			if (IsInitialized())
			{
				StoreItemProvider_onLoadComplete();
				return;
			}
			InitializationOptions options = new InitializationOptions()
#if UNITY_EDITOR || DEVELOPMENT_BUILD
				.SetEnvironmentName("test");
#else
				.SetEnvironmentName("production");
#endif
			await UnityServices.InitializeAsync(options);
			ResourceRequest request = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
			request.completed += HandleIAPPCatalog;
			payments.gameObject.SetActive(false);
#elif UNITY_WEBGL
			itemsHolder.gameObject.SetActive(false);
			YandexGame.PurchaseSuccessEvent = (id) =>
			{
				switch (id)
				{
					case "gems_100":
						gameInstance.AddBoughtGems(100);
						break;
					case "gems_1000":
						gameInstance.AddBoughtGems(1000);
						break;
					case "gems_5000":
						gameInstance.AddBoughtGems(5000);
						break;
					case "gems_20000":
						gameInstance.AddBoughtGems(20000);
						break;
					case "gems_100000":
						gameInstance.AddBoughtGems(100000);
						break;
					case "no_ads":
						YandexGame.savesData.noAdsBought = true;
						YandexGame.SaveProgress();
						break;
				}
				BackToPrevious();
				/*foreach (var el in spawnedItems)
				{
					el.Activate();
				}*/
			};/*
			if (!StoreItemProvider.IsInitialized())
			{
				StoreItemProvider.Initialize(YandexGame.PaymentsData.id);
				//StoreItemProvider.onLoadComplete += StoreItemProvider_onLoadComplete;
			}
			else
			{
				StoreItemProvider_onLoadComplete();
			}*/
#endif
		}

		private void OnDestroy()
		{
			StoreItemProvider.onLoadComplete -= StoreItemProvider_onLoadComplete;
		}

#if UNITY_ANDROID
		private void HandleIAPPCatalog(AsyncOperation operation)
		{
			var request = operation as ResourceRequest;
			ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);

			StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
			StandardPurchasingModule.Instance().useFakeStoreAlways = true;

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
#endif


		#endregion

#if UNITY_ANDROID
		#region IStoreListener

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			storeController = controller;
			extensionProvider = extensions;
			if (!StoreItemProvider.IsInitialized())
				StoreItemProvider.Initialize(storeController.products);
			StoreItemProvider.onLoadComplete += StoreItemProvider_onLoadComplete;
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
			onPurchaseCompleted?.Invoke();
			onPurchaseCompleted = null;

			Debug.Log("Purchase!");

			var def = purchaseEvent.purchasedProduct.definition;
			foreach (var pay in def.payouts)
			{
				Debug.Log($"Payouts: {pay.subtype} x {pay.quantity}");
				if (pay.subtype == "Gems")
				{
					gameInstance.AddBoughtGems((int)pay.quantity);
				}
			}

			return PurchaseProcessingResult.Complete;
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
		{
			foreach (var el in spawnedItems)
			{
				el.Activate();
			}
			onPurchaseCompleted?.Invoke();
			onPurchaseCompleted = null;
		}

		#endregion

		#region Callbacks

		private void StoreItemProvider_onLoadComplete()
		{
			List<Product> sortedProducts = storeController.products.all.OrderBy(item => item.metadata.localizedPrice).ToList();

			foreach (var product in sortedProducts)
			{
				var item = Instantiate(itemPrefab);
				ProjectContext.Instance.Container.Inject(item);
				spawnedItems.Add(item);
				item.transform.SetParent(itemsHolder, false);
				item.onPurchase += Item_onPurchase;
				item.Initialize(product);
			}
		}
		private void Item_onPurchase(Product product, Action onComplete)
		{
			onPurchaseCompleted = onComplete;
			storeController.InitiatePurchase(product);
		}

		#endregion
#elif UNITY_WEBGL

		#region Callbacks

		private void StoreItemProvider_onLoadComplete()
		{
			for (int i = 0; i < YandexGame.PaymentsData.title.Length; i++)
			{
				var item = Instantiate(itemPrefab);
				ProjectContext.Instance.Container.Inject(item);
				spawnedItems.Add(item);
				item.transform.SetParent(itemsHolder, false);
				item.onPurchase += Item_onPurchase;
				item.Initialize(YandexGame.PaymentsData.id[i], YandexGame.PaymentsData.title[i], YandexGame.PaymentsData.priceValue[i]);
			}
		}
		private void Item_onPurchase(string item, Action onComplete)
		{
			onPurchaseCompleted = onComplete;
			YandexGame.BuyPayments(item);
		}

		#endregion

#endif
	}
}
