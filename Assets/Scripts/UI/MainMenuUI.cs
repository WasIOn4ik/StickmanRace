using SR.Core;
using SR.SceneManagement;
using SR.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
				startButton.interactable = false;
				StartCoroutine(DelayedStart());
			});

			settingsButton.onClick.AddListener(() =>
			{
				soundsSystem.PlayButton1();
				OpenMenu(MenuType.SettingsMenu, this);
			});
			soundsSystem.PlayMenuMusic();
		}

		private IEnumerator DelayedStart()
		{
			yield return new WaitForSeconds(0.5f);
			SceneLoader.LoadScene(SRScene.GameScene);
		}

		#endregion
	}
}
