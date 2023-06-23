using SR.Core;
using SR.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GarageUI : MonoBehaviour
{
	#region Variables

	[SerializeField] private GameObject objectToDestroy;
	[SerializeField] private Button playButton;
	[SerializeField] private Button shopButton;
	[SerializeField] private Button settingsButton;
	[SerializeField] private TMP_Text distanceText;
	[SerializeField] private TMP_Text timeText;
	[SerializeField] private TMP_Text gemsText;

	[Inject] GameplayBase gameplayBase;
	[Inject] GameInstance gameInstance;

	#endregion

	#region UnityMessages

	private void Awake()
	{
		gameInstance.onGemsCountChanged += GameInstance_onGemsCountChanged;
		playButton.onClick.AddListener(() =>
		{
			gameInstance.Sounds.PlayButton1();
			gameplayBase.StartGame();
			gameInstance.Sounds.StartBackgroundMusic();
		});

		settingsButton.onClick.AddListener(() =>
		{
			gameInstance.Sounds.PlayButton1();
			MenuBase.OpenMenu(MenuType.SettingsMenu, true);
		});

		shopButton.onClick.AddListener(() =>
		{
			gameInstance.Sounds.PlayButton1();
			MenuBase.OpenMenu(MenuType.DonateShop, true);
		});

		UpdateDisplay();
		gameInstance.Sounds.PlayGarageMusic();
	}

	#endregion

	#region Functions

	public void DestroyGarage()
	{
		Destroy(objectToDestroy);
	}

	private void UpdateDisplay()
	{
		var records = gameInstance.GetRecords();
		distanceText.text = gameInstance.GetDistanceString();
		timeText.text = records.maxTime.ToString("0.0") + " s";
		gemsText.text = gameInstance.GetRecords().gems.ToString();
	}

	#endregion

	#region Callbacks

	private void GameInstance_onGemsCountChanged(object sender, System.EventArgs e)
	{
		UpdateDisplay();
	}

	#endregion
}
