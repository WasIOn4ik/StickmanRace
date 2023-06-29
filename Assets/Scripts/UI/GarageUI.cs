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
#if UNITY_WEBGL
		YandexGame.StickyAdActivity(false);
#elif UNITY_ANDROID
		gameInstance.SetBannerActivity(false);
#endif
		plus50Button.onClick.AddListener(() =>
		{
#if UNITY_WEBGL
			gameInstance.Sounds.Mute();
			YandexGame.OpenVideoEvent = null;
			YandexGame.ErrorFullAdEvent = () =>
			{
				YandexGame.CloseFullAdEvent = null;
				YandexGame.ErrorFullAdEvent = null;
				gameInstance.Sounds.Unmute();
			};
			YandexGame.CloseVideoEvent = () =>
			{
				gameInstance.AddBoughtGems(50);
				YandexGame.CloseFullAdEvent = null;
				YandexGame.ErrorFullAdEvent = null;
				gameInstance.Sounds.Unmute();
			};
			YG._RewardedShow(1);
#elif UNITY_ANDROID
			gameInstance.ShowRewarded(OnPlus50Reward);
#endif
		});
		playButton.onClick.AddListener(() =>
		{
			startsCount++;
			gameInstance.Sounds.PlayButton1();
			if (startsCount % 2 == 0)
			{
#if UNITY_WEBGL
				if (YandexGame.savesData.noAdsBought)
				{
					gameInstance.Sounds.StartBackgroundMusic();
					gameplayBase.StartGame();
				}
				else
				{
					gameInstance.Sounds.Mute();
					YG.ResetTimerFullAd();
					YG.ErrorFullscreenAd.AddListener(() =>
					{
						gameInstance.Sounds.StartBackgroundMusic();
						gameplayBase.StartGame();
					});
					YandexGame.OpenFullAdEvent = () =>
					{
						YandexGame.ErrorFullAdEvent = () =>
						{
							gameInstance.Sounds.Unmute();
							YandexGame.StickyAdActivity(!YandexGame.savesData.noAdsBought);
							gameInstance.Sounds.StartBackgroundMusic();
							gameplayBase.StartGame();
							YandexGame.CloseFullAdEvent = null;
							YandexGame.ErrorFullAdEvent = null;
						};
						YandexGame.CloseFullAdEvent = () =>
						{
							gameInstance.Sounds.Unmute();
							YandexGame.StickyAdActivity(!YandexGame.savesData.noAdsBought);
							gameInstance.Sounds.StartBackgroundMusic();
							gameplayBase.StartGame();
							YandexGame.CloseFullAdEvent = null;
							YandexGame.ErrorFullAdEvent = null;
						};
						YandexGame.OpenFullAdEvent = null;
					};
					YandexGame.FullscreenShow();
				}
#elif UNITY_ANDROID
				gameInstance.ShowInterstitial(OnGameStart);
#endif
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

	#endregion

	#region Functions

	private void OnGameStart()
	{
		gameInstance.SetBannerActivity(true);
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
