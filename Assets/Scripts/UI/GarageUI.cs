using CAS.AdObject;
using SR.Core;
using SR.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Zenject;

public class GarageUI : MonoBehaviour
{
	#region Variables

	[SerializeField] private GameObject objectToDestroy;
	[SerializeField] private Button playButton;
	[SerializeField] private Button shopButton;
	[SerializeField] private Button settingsButton;
	[SerializeField] private Button plus50Button;
	[SerializeField] private TMP_Text distanceText;
	[SerializeField] private TMP_Text timeText;
	[SerializeField] private TMP_Text gemsText;
	[SerializeField] private LeaderboardYG roadsLeaderboard;
	[SerializeField] private LeaderboardYG expressesLeaderboard;
	[SerializeField] private Button androidLeaderboard;

	[Inject] private YandexGame YG;
	[Inject] private GameplayBase gameplayBase;
	[Inject] private GameInstance gameInstance;


	private static int startsCount = 0;

	#endregion

	#region UnityMessages

	private void Awake()
	{
		Enemy.killsInRound = 0;
		LoseMenuUI.bPlus3HPUsed = false;
		gameInstance.onGemsCountChanged += GameInstance_onGemsCountChanged;
		SettingsMenuUI.onLanguageChanged += SettingsMenuUI_onLanguageChanged;
#if UNITY_WEBGL
		YandexGame.StickyAdActivity(false);
		androidLeaderboard.gameObject.SetActive(false);
#elif UNITY_ANDROID
		roadsLeaderboard.gameObject.SetActive(false);
		expressesLeaderboard.gameObject.SetActive(false);
		gameInstance.SetBannerActivity(false);
		androidLeaderboard.onClick.AddListener(() =>
		{
			AndroidLeaderboard.ShowLeadeboardUI();
		});
#endif
		plus50Button.onClick.AddListener(() =>
		{
			gameInstance.ShowRewarded(OnPlus50Reward);
		});

		playButton.onClick.AddListener(() =>
		{
			startsCount++;
			gameInstance.Sounds.PlayButton1();
			if (startsCount % 2 == 0)
			{
				gameInstance.ShowInterstitial(OnGameStart);
			}
			else
			{
				gameInstance.Sounds.StartBackgroundMusic();
				gameplayBase.StartGame();
			}
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

	private void OnDestroy()
	{
		SettingsMenuUI.onLanguageChanged -= SettingsMenuUI_onLanguageChanged;
	}
	#endregion

	#region Functions

	private void OnGameStart()
	{
#if UNITY_ANDORID
		gameInstance.SetBannerActivity(true);
#endif
		gameInstance.Sounds.StartBackgroundMusic();
		gameplayBase.StartGame();
	}

	private void OnPlus50Reward()
	{
		gameInstance.AddBoughtGems(50);
		gameInstance.Sounds.Unmute();
	}

	public void DestroyGarage()
	{
		Destroy(objectToDestroy);
	}

	private void UpdateDisplay()
	{
		var records = gameInstance.GetRecords();
		distanceText.text = gameInstance.GetShortStringDistance(records.totalDistance);
		timeText.text = gameInstance.GetShortStringTime(records.maxTime);
		gemsText.text = gameInstance.GetShortString(records.gems);
	}

	#endregion

	#region Callbacks
	private void SettingsMenuUI_onLanguageChanged(object sender, System.EventArgs e)
	{
		UpdateDisplay();
	}

	private void GameInstance_onGemsCountChanged(object sender, System.EventArgs e)
	{
		UpdateDisplay();
	}

	#endregion
}
