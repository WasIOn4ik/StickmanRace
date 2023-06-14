using SR.Customization;
using SR.Library;
using SR.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	[Serializable]
	public struct GameRecords
	{
		public float maxDistance;
		public float maxTime;
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

	public class GameInstance : MonoBehaviour
	{
		#region HelperClasses

		public class DetailEventArgs : EventArgs
		{
			public CarDetailSO detail;
		}

		#endregion

		#region Variables

		[Header("Components")]
		[SerializeField] private MenusListSO menusLibrary;
		[SerializeField] private ShopLibrarySO shopLibrary;

		[Header("Properties")]
		[SerializeField] private string recordSaveSlot;
		[SerializeField] private string carConfigSaveSlot;

		public event EventHandler<DetailEventArgs> onDetailChanged;

		private GameRecords records = new GameRecords() { maxDistance = 0f, maxTime = 0f };
		private CarConfig carConfig = new CarConfig();

		#endregion

		#region UnityMessages

		private void Awake()
		{
			MenuBase.menusLibrary = menusLibrary;
			LoadRecords();
			LoadCarConfig();
		}

		#endregion

		#region Functions

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

		public void TryUpdateRecords(float distance, float time)
		{
			bool changed = false;

			if (distance > records.maxDistance)
			{
				records.maxDistance = distance;
				changed = true;
			}

			if (time > records.maxTime)
			{
				records.maxTime = time;
				changed = true;
			}

			if (changed)
				SaveRecords();
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
			string str = JsonUtility.ToJson(records);
			PlayerPrefs.SetString(recordSaveSlot, str);
		}

		public void SaveCarConfig()
		{
			string str = JsonUtility.ToJson(carConfig);
			PlayerPrefs.SetString(carConfigSaveSlot, str);
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


		#endregion
	}
}
