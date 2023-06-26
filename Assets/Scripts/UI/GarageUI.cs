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

	[Inject] private YandexGame YG;
	[Inject] private GameplayBase gameplayBase;
	[Inject] private GameInstance gameInstance;


	private static int startsCount = 0;

	#endregion

	#region UnityMessages

	private void Awake()
	{
		Enemy.killsInRound = 0;
#if UNITY_WEBGL
		YandexGame.StickyAdActivity(false);
#endif
		LoseMenuUI.bPlus3HPUsed = false;
		gameInstance.onGemsCountChanged += GameInstance_onGemsCountChanged;

		plus50Button.onClick.AddListener(() =>
		{
#if UNITY_WEBGL
			YandexGame.OpenVideoEvent = null;
			YandexGame.CloseVideoEvent = () =>
			{
				gameInstance.AddBoughtGems(50);
				YandexGame.CloseVideoEvent = null;
			};
			YG._RewardedShow(1);
#endif
		});
		playButton.onClick.AddListener(() =>
		{
			startsCount++;
			gameInstance.Sounds.PlayButton1();
			if (startsCount % 2 == 0)
			{
#if UNITY_WEBGL
				YG.ResetTimerFullAd();
				YG.ErrorFullscreenAd.AddListener(() =>
				{
					gameplayBase.StartGame();
				});
				YandexGame.OpenFullAdEvent = () =>
				{
					YandexGame.CloseFullAdEvent = () =>
					{
						YandexGame.StickyAdActivity(true);
						gameplayBase.StartGame();
						YandexGame.CloseFullAdEvent = null;
					};
					YandexGame.OpenFullAdEvent = null;

				};
				YG._FullscreenShow();
#elif UNITY_ANDROID
				gameplayBase.StartGame();
#endif
			}
			else
			{
				gameplayBase.StartGame();
			}
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
