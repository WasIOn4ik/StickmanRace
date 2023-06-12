using SR.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class StartupScript : MonoBehaviour
{
	private void Start()
	{
		MenuBase.OpenMenu(MenuType.MainMenu);
	}
}
