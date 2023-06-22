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
using Zenject;

namespace SR.UI
{
	public class DonateShopMenuUI : MenuBase, IDetailedStoreListener
	{
		#region Variables

		[SerializeField] private DonateItemUI itemPrefab;
		[SerializeField] private RectTransform itemsHolder;
		[SerializeField] private Button backButton;

		[Inject] GameInstance gameInstance;

		private static IStoreController storeController;
		private static IExtensionProvider extensionProvider;
		List<DonateItemUI> spawnedItems = new List<DonateItemUI>();

		private Action onPurchaseCompleted;

		[Inject] private SoundSystem soundsSystem;

		#endregion

		#region Functions

		private bool IsInitialized()
		{
			return storeController != null && extensionProvider != null;
		}

		private async void Awake()
		{
			backButton.onClick.AddListener(() =>
			{
				soundsSystem.PlayButton2();
				BackToPrevious();
			});
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
		}

		private void HandleIAPPCatalog(AsyncOperation operation)
		{
			var request = operation as ResourceRequest;
			ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);

			StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
			StandardPurchasingModule.Instance().useFakeStoreAlways = true;

#if UNITY_ANDROID
			ConfigurationBuilder builder = ConfigurationBuilder.Instance(
				StandardPurchasingModule.Instance(AppStore.GooglePlay));
#elif UNITY_WEBGL

#else
			ConfigurationBuilder builder = ConfigurationBuilder.Instance(
				StandardPurchasingModule.Instance(AppStore.NotSpecified));
#endif

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
			foreach(var el in spawnedItems)
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
	}
}
