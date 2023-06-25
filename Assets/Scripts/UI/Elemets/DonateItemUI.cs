using SR.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using Zenject;

namespace SR.UI
{
	public class DonateItemUI : MonoBehaviour
	{
		#region Variables

		[SerializeField] private Image icon;
		[SerializeField] private TMP_Text priceText;
		[SerializeField] private TMP_Text titleText;
		[SerializeField] private Button purchaseButton;

#if UNITY_ANDROID
		public delegate void PurchaseDelegate(Product product, Action onComplete);

		private Product product;
#elif UNITY_WEBGL
		public delegate void PurchaseDelegate(string id, Action onComplete);

		private string product;
#endif
		public event PurchaseDelegate onPurchase;

		[Inject] private SoundSystem soundsSystem;

#endregion

		#region UnityMessages

		private void Awake()
		{
			purchaseButton.onClick.AddListener(() =>
			{
				soundsSystem.PlayButton1();
				Purchase();
			});
		}

		#endregion

		#region Functions

		public void Activate()
		{
			purchaseButton.interactable = true;
		}

#if UNITY_ANDROID
		public void Initialize(Product product)
		{
			this.product = product;
			titleText.text = product.metadata.localizedTitle;
			priceText.text = $"{product.metadata.localizedPriceString} {product.metadata.isoCurrencyCode}";
			var tex = StoreItemProvider.GetIcon(product.definition.id);
			icon.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2f);
		}
#elif UNITY_WEBGL
		public void Initialize(string id, string title, string price)
		{
			product = id;
			titleText.text = title;
			priceText.text = price;
			var tex = StoreItemProvider.GetIcon(id);
			icon.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2f);
		}

#endif

		public void Purchase()
		{
			purchaseButton.interactable = false;
			onPurchase?.Invoke(product, HandlePurchaseCompleted);
		}

		private void HandlePurchaseCompleted()
		{
			purchaseButton.interactable = true;
		}

#endregion
	}
}

