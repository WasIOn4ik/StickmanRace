using SR.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

namespace SR.UI
{
	public class LoseMenuUI : MenuBase
	{
		#region Variables

		[SerializeField] private TMP_Text distanceText;
		[SerializeField] private TMP_Text timeText;
		[SerializeField] private TMP_Text gemsText;

		[Inject] GameInstance gameInstance;

		#endregion

		#region Functions

		public void UpdateDisplay(float distance, float time)
		{
			gameInstance.TryUpdateRecords(distance, time);
			distanceText.text = Mathf.Max(0f, distance).ToString("0.0");
			timeText.text = time.ToString("0.0");
			gemsText.text = gameInstance.DistanceToGems(distance).ToString();
		}

		#endregion
	}
}
