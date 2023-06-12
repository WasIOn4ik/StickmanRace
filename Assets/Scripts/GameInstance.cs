using SR.Library;
using SR.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
	[SerializeField] private MenusListSO menusLibrary;

	private void Awake()
	{
		MenuBase.menusLibrary = menusLibrary;
	}
}
