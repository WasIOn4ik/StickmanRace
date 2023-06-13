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

		[Inject] GameInstance gameInstance;

		#endregion

		#region Functions

		public void UpdateDisplay(float distance, float time)
		{
			gameInstance.TryUpdateRecords(distance, time);
			distanceText.text = distance.ToString("#.#");
			timeText.text = time.ToString("#.#");
		}

		#endregion
	}
}
