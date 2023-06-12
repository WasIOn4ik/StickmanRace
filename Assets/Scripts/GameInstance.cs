using SR.Library;
using SR.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class GameInstance : MonoBehaviour
	{
		#region Variables

		[SerializeField] private MenusListSO menusLibrary;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			MenuBase.menusLibrary = menusLibrary;
		}

		#endregion
	}
}
