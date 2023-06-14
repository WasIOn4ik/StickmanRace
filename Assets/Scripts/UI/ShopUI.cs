using JetBrains.Annotations;
using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SR.UI
{
	public class ShopUI : MonoBehaviour
	{
		#region Variables

		[Header("Components")]
		[SerializeField] private ShopItemSlot slotPrefab;
		[SerializeField] private Transform itemsHolder;
		[Inject] private GameInstance gameInstance;

		[Header("Tabs")]
		[SerializeField] private Button wheelsTabButton;
		[SerializeField] private Button weaponTabBUtton;
		[SerializeField] private Button backDoorTabButton;
		[SerializeField] private Button bumperTabButton;
		[SerializeField] private Button stickmanTabButton;

		private List<ShopItemSlot> spawnedSlots = new List<ShopItemSlot>();

		#endregion

		#region UnityMessages

		private void Awake()
		{
			wheelsTabButton.onClick.AddListener(() =>
			{
				SetCategory(CarDetailType.Wheels);
				wheelsTabButton.interactable = false;
				weaponTabBUtton.interactable = true;
				backDoorTabButton.interactable = true;
				bumperTabButton.interactable = true;
				stickmanTabButton.interactable = true;
			});

			weaponTabBUtton.onClick.AddListener(() =>
			{
				SetCategory(CarDetailType.Weapon);
				wheelsTabButton.interactable = true;
				weaponTabBUtton.interactable = false;
				backDoorTabButton.interactable = true;
				bumperTabButton.interactable = true;
				stickmanTabButton.interactable = true;
			});

			backDoorTabButton.onClick.AddListener(() =>
			{
				SetCategory(CarDetailType.BackDoor);
				wheelsTabButton.interactable = true;
				weaponTabBUtton.interactable = true;
				backDoorTabButton.interactable = false;
				bumperTabButton.interactable = true;
				stickmanTabButton.interactable = true;
			});

			bumperTabButton.onClick.AddListener(() =>
			{
				SetCategory(CarDetailType.Bumper);
				wheelsTabButton.interactable = true;
				weaponTabBUtton.interactable = true;
				backDoorTabButton.interactable = true;
				bumperTabButton.interactable = false;
				stickmanTabButton.interactable = true;
			});

			stickmanTabButton.onClick.AddListener(() =>
			{
				SetCategory(CarDetailType.Stickman);
				wheelsTabButton.interactable = true;
				weaponTabBUtton.interactable = true;
				backDoorTabButton.interactable = true;
				bumperTabButton.interactable = true;
				stickmanTabButton.interactable = false;
			});

			SetCategory(CarDetailType.Wheels);
			wheelsTabButton.interactable = false;
			weaponTabBUtton.interactable = true;
			backDoorTabButton.interactable = true;
			bumperTabButton.interactable = true;
			stickmanTabButton.interactable = true;
		}

		#endregion

		#region Functions

		public void SetCategory(CarDetailType category)
		{
			var list = gameInstance.GetShopLibrary().GetCategoryList(category);

			//Disabling slots
			foreach (var el in spawnedSlots)
			{
				el.gameObject.SetActive(false);
			}

			//Creating new slots
			int dif = list.Count - spawnedSlots.Count;

			if (dif > 0)
			{
				for (int i = 0; i < dif; i++)
				{
					var slot = Instantiate(slotPrefab, itemsHolder);
					spawnedSlots.Add(slot);
				}
			}

			for (int i = 0; i < list.Count; i++)
			{
				spawnedSlots[i].InitSlot(gameInstance, list[i]);
			}
		}

		#endregion
	}
}
