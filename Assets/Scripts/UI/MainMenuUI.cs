using SR.Core;
using SR.SceneManagement;
using SR.UI;
using System.Collections;
using System.Collections.Generic;
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

		[Inject] private SoundSystem soundsSystem;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			startButton.onClick.AddListener(() =>
			{
				soundsSystem.PlayButton2(true);
#if UNITY_WEBGL
				YandexGame.Instance.ResetTimerFullAd();
				YandexGame.Instance.CloseFullscreenAd.AddListener(HandleStartGameYandex);
				YandexGame.Instance.ErrorFullscreenAd.AddListener(HandleFullAdError);
				YandexGame.FullscreenShow();

#elif UNITY_ANDROID
				startButton.interactable = false;
				StartCoroutine(DelayedStart());
#endif
			});

			settingsButton.onClick.AddListener(() =>
			{
				soundsSystem.PlayButton1();
				OpenMenu(MenuType.SettingsMenu, this);
			});
			soundsSystem.PlayMenuMusic();
		}

		private void HandleFullAdError()
		{
			YandexGame.Instance.CloseFullscreenAd.RemoveListener(HandleStartGameYandex);
			YandexGame.Instance.ErrorFullscreenAd.RemoveListener(HandleFullAdError);
			SceneLoader.LoadScene(SRScene.GameScene);
		}
		private void HandleStartGameYandex()
		{
			YandexGame.Instance.CloseFullscreenAd.RemoveListener(HandleStartGameYandex);
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
