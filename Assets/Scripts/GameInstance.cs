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

	public class GameInstance : MonoBehaviour
	{
		#region Variables

		[Header("Components")]
		[SerializeField] private MenusListSO menusLibrary;

		[Header("Properties")]
		[SerializeField] private string recordSaveSlot;

		private GameRecords records = new GameRecords() { maxDistance = 0f, maxTime = 0f };

		#endregion

		#region UnityMessages

		private void Awake()
		{
			MenuBase.menusLibrary = menusLibrary;
			LoadRecords();
		}

		#endregion

		#region Functions

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

		public void SaveRecords()
		{
			string str = JsonUtility.ToJson(records);
			PlayerPrefs.SetString(recordSaveSlot, str);
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

		#endregion
	}
}
