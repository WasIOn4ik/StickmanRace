using SR.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using YG;
using Zenject;

namespace SR.UI
{
#if UNITY_ANDROID
	public class DonateShopMenuUI : MenuBase
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

#if UNITY_WEBGL
		private static bool bInitialized;
		private string currentPurchase;
#endif
		List<DonateItemUI> spawnedItems = new List<DonateItemUI>();

		[Inject] private SoundSystem soundsSystem;

		#endregion

		#region Functions

#if UNITY_WEBGL
		private bool IsInitialized()
		{
			return bInitialized;
		}
#endif

		private async void Awake()
		{
			backButton.onClick.AddListener(() =>
			{
				soundsSystem.PlayButton2();
				BackToPrevious();
			});
#if UNITY_ANDROID
			Debug.Log("Donate shop awake");
			StoreItemProvider_onLoadComplete();
			InitializationOptions options = new InitializationOptions()
#if UNITY_EDITOR || DEVELOPMENT_BUILD
				.SetEnvironmentName("test");
#else
				.SetEnvironmentName("production");
#endif
			await UnityServices.InitializeAsync(options);
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

		#endregion

#if UNITY_ANDROID

		#region Callbacks

		private void StoreItemProvider_onLoadComplete()
		{
			Debug.Log("Shop initialized, creating purchases");
			List<Product> sortedProducts = GameInstance.storeController.products.all.OrderBy(item => item.definition.id).ToList();

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
			//gameInstance.onPurchaseCompleted = onComplete;
			gameInstance.onPurchaseCompleted = OnPurchaseSuccess;
			GameInstance.storeController.InitiatePurchase(product);
		}
		private void OnPurchaseSuccess()
		{
			foreach (var item in spawnedItems)
			{
				item.Activate();
			}
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
