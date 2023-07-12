using SR.Core;
using SR.SceneManagement;
using SR.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Zenject;

namespace SR.UI
{
	public class MainMenuUI : MenuBase
	{
		#region Variables
		[Header("Main")]
		[SerializeField] private Button startButton;
		[SerializeField] private Button settingsButton;
		[SerializeField] private TextMeshProUGUI MessageAboutLogin;

		[Inject] private GameInstance gameInstance;
		[Inject] private SoundSystem soundsSystem;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			startButton.onClick.AddListener(() =>
			{
				soundsSystem.PlayButton2(true);
#if UNITY_WEBGL
				if(YandexGame.savesData.noAdsBought)
				{
					StartCoroutine(DelayedStart());
				}
				else
				{
					YandexGame.Instance.ResetTimerFullAd();
					YandexGame.Instance.CloseFullscreenAd.AddListener(HandleStartGameYandex);
					YandexGame.Instance.ErrorFullscreenAd.AddListener(HandleFullAdError);
					soundsSystem.Mute();
					YandexGame.FullscreenShow();
				}

#elif UNITY_ANDROID
				if (Social.localUser.authenticated)
				{
					gameInstance.ShowInterstitial(GoToGarage);
				}
				else
				{
					gameInstance.TryLogin(() => { gameInstance.ShowInterstitial(GoToGarage); });
					MessageAboutLogin.color = Color.green;
				}
#endif
			});

			settingsButton.onClick.AddListener(() =>
			{
				soundsSystem.PlayButton1();
				OpenMenu(MenuType.SettingsMenu, this);
			});
			soundsSystem.PlayMenuMusic();
		}

#if UNITY_WEBGL
		private void HandleFullAdError()
		{
			YandexGame.Instance.CloseFullscreenAd.RemoveListener(HandleStartGameYandex);
			YandexGame.Instance.ErrorFullscreenAd.RemoveListener(HandleFullAdError);
			SceneLoader.LoadScene(SRScene.GameScene);
		}
		private void HandleStartGameYandex()
		{
			soundsSystem.Unmute();
			YandexGame.Instance.CloseFullscreenAd.RemoveListener(HandleStartGameYandex);
			SceneLoader.LoadScene(SRScene.GameScene);
		}
#endif
		private void Update()
		{
			var col = MessageAboutLogin.color;
			col.a -= Time.deltaTime * 0.75f;
			col.a = Mathf.Max(col.a, 0);
			MessageAboutLogin.color = col;
		}

		private void GoToGarage()
		{
			SceneLoader.LoadScene(SRScene.GameScene);
		}

		private IEnumerator DelayedStart()
		{
			yield return new WaitForSeconds(0.5f);
			SceneLoader.LoadScene(SRScene.GameScene);
		}

		#endregion
	}
}
