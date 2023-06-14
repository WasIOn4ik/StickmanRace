using SR.Core;
using SR.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SR.UI
{
	public class ShopItemSlot : MonoBehaviour
	{
		#region Variables

		[SerializeField] private Image iconImage;
		[SerializeField] private Button slotButton;

		private GameInstance gameInstance;
		private CarDetailSO carDetailSO;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			slotButton.onClick.AddListener(() =>
			{
				gameInstance.TryUpdateCarConfig(carDetailSO);
			});
		}

		#endregion

		#region Functions

		public void InitSlot(GameInstance gi, CarDetailSO newCarDetailSO)
		{
			iconImage.sprite = newCarDetailSO.sprite;
			iconImage.color = newCarDetailSO.color;
			carDetailSO = newCarDetailSO;
			gameInstance = gi;
			gameObject.SetActive(true);
		}

		#endregion
	}
}
