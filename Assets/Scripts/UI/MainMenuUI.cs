using SR.SceneManagement;
using SR.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SR.UI
{
	public class MainMenuUI : MenuBase
	{
		[Header("Main")]
		[SerializeField] private Button startButton;
		[SerializeField] private Button settingsButton;

		private void Awake()
		{
			startButton.onClick.AddListener(() =>
			{
				SceneLoader.LoadScene(SRScene.GameScene);
			});

			settingsButton.onClick.AddListener(() =>
			{
				OpenMenu(MenuType.SettingsMenu, this);
			});
		}
	}
}
