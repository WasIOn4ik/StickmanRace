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
		#region Variables
		[Header("Main")]
		[SerializeField] private Button startButton;
		[SerializeField] private Button settingsButton;

		#endregion

		#region UnityMessages

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

		#endregion
	}
}
