using SR.Core;
using SR.Customization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SR.UI
{
	public class ShopItemSlot : MonoBehaviour
	{
		#region Variables

		[SerializeField] private Image iconImage;
		[SerializeField] private Button slotButton;
		[SerializeField] private TMP_Text priceText;
		[SerializeField] private GameObject lockedGameObject;
		[SerializeField] private Image lockImage;
		[SerializeField] private Image backgroundImage;
		[SerializeField] private Sprite lockedSprite;
		[SerializeField] private Sprite unlockedSprite;
		[SerializeField] private Color nonSelectedColor = Color.white;
		[SerializeField] private Color selectedColor = Color.gray;

		private GameInstance gameInstance;
		private CarDetailSO carDetailSO;
		private ShopUI shopUI;

		private bool canBuy;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			slotButton.onClick.AddListener(() =>
			{
				gameInstance.Sounds.PlayButton1();
				if (carDetailSO.bUnlocked)
				{
					gameInstance.TryUpdateCarConfig(carDetailSO);
					shopUI.SetCategory(carDetailSO.type);
				}
				else if (canBuy)
				{
					gameInstance.TryBuyDetail(carDetailSO);
					shopUI.SetCategory(carDetailSO.type);
				}
			});
		}

		#endregion

		#region Functions

		public void InitSlot(ShopUI shop, GameInstance gi, CarDetailSO newCarDetailSO)
		{
			gameObject.SetActive(true);
			lockedGameObject.SetActive(!newCarDetailSO.bUnlocked);

			shopUI = shop;

			iconImage.sprite = newCarDetailSO.sprite;
			iconImage.color = newCarDetailSO.color;

			carDetailSO = newCarDetailSO;

			gameInstance = gi;
			priceText.text = gameInstance.GetShortString(newCarDetailSO.price);
			canBuy = gameInstance.GetRecords().gems >= newCarDetailSO.price;
			lockImage.sprite = canBuy ? unlockedSprite : lockedSprite;

			backgroundImage.color = (gameInstance.IsEquipped(newCarDetailSO)) ? selectedColor : nonSelectedColor;
		}

		#endregion
	}
}
