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

		[Header("Tabs")]
		[SerializeField] private Button wheelsTabButton;
		[SerializeField] private Button weaponTabBUtton;
		[SerializeField] private Button backDoorTabButton;
		[SerializeField] private Button bumperTabButton;
		[SerializeField] private Button stickmanTabButton;

		[Inject] private GameInstance gameInstance;

		private List<ShopItemSlot> spawnedSlots = new List<ShopItemSlot>();

		private CarDetailType currentCategory;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			gameInstance.onGemsCountChanged += GameInstance_onGemsCountChanged;
			gameInstance.onDetailChanged += GameInstance_onDetailChanged;

			wheelsTabButton.onClick.AddListener(() =>
			{
				gameInstance.Sounds.PlayButton1();
				SetCategory(CarDetailType.Wheels);
				wheelsTabButton.interactable = false;
				weaponTabBUtton.interactable = true;
				backDoorTabButton.interactable = true;
				bumperTabButton.interactable = true;
				stickmanTabButton.interactable = true;
			});

			weaponTabBUtton.onClick.AddListener(() =>
			{
				gameInstance.Sounds.PlayButton1();
				SetCategory(CarDetailType.Weapon);
				wheelsTabButton.interactable = true;
				weaponTabBUtton.interactable = false;
				backDoorTabButton.interactable = true;
				bumperTabButton.interactable = true;
				stickmanTabButton.interactable = true;
			});

			backDoorTabButton.onClick.AddListener(() =>
			{
				gameInstance.Sounds.PlayButton1();
				SetCategory(CarDetailType.BackDoor);
				wheelsTabButton.interactable = true;
				weaponTabBUtton.interactable = true;
				backDoorTabButton.interactable = false;
				bumperTabButton.interactable = true;
				stickmanTabButton.interactable = true;
			});

			bumperTabButton.onClick.AddListener(() =>
			{
				gameInstance.Sounds.PlayButton1();
				SetCategory(CarDetailType.Bumper);
				wheelsTabButton.interactable = true;
				weaponTabBUtton.interactable = true;
				backDoorTabButton.interactable = true;
				bumperTabButton.interactable = false;
				stickmanTabButton.interactable = true;
			});

			stickmanTabButton.onClick.AddListener(() =>
			{
				gameInstance.Sounds.PlayButton1();
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

		private void OnDestroy()
		{
			gameInstance.onDetailChanged -= GameInstance_onDetailChanged;
			gameInstance.onGemsCountChanged -= GameInstance_onGemsCountChanged;
		}

		#endregion

		#region Functions

		public void SetCategory(CarDetailType category)
		{
			currentCategory = category;
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
				spawnedSlots[i].InitSlot(this, gameInstance, list[i]);
			}
		}

		#endregion

		#region Callbacks

		private void GameInstance_onGemsCountChanged(object sender, System.EventArgs e)
		{
			SetCategory(currentCategory);
		}

		private void GameInstance_onDetailChanged(object sender, GameInstance.DetailEventArgs e)
		{
			SetCategory(currentCategory);
		}

		#endregion
	}
}
