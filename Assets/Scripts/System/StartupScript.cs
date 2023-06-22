using SR.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace SR.Extras
{
	public class StartupScript : MonoBehaviour
	{
		#region UnityMessages

		private void Start()
		{
			//TODO: Loading screen
			var localeOperation = LocalizationSettings.SelectedLocaleAsync;

			if (localeOperation.IsDone)
			{
				ShowMainMenu();
			}
			else
			{
				localeOperation.Completed += x =>
				{
					ShowMainMenu();
				};
			}
		}

		#endregion

		#region Functions

		private void ShowMainMenu()
		{
			MenuBase.OpenMenu(MenuType.MainMenu, true);
		}

		#endregion
	}
}
