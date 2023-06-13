using SR.Core;
using SR.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GarageUI : MonoBehaviour
{
	#region Variables

	[SerializeField] private Button playButton;
	[SerializeField] private Button shopButton;
	[SerializeField] private TMP_Text distanceText;
	[SerializeField] private TMP_Text timeText;

	[Inject] GameplayBase gameplayBase;
	[Inject] GameInstance gameInstance;

	#endregion

	#region UnityMessages

	private void Awake()
	{
		playButton.onClick.AddListener(() =>
		{
			gameplayBase.StartGame();
			Destroy(gameObject);
		});

		UpdateDisplay();
	}

	#endregion

	#region Functions

	private void UpdateDisplay()
	{
		var records = gameInstance.GetRecords();
		distanceText.text = records.maxDistance.ToString("#.#") + " m";
		timeText.text = records.maxTime.ToString("#.#") + " s";
	}

	#endregion
}
