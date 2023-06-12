using SR.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Library
{
	[CreateAssetMenu(menuName = "SR/MenusList")]
	public class MenusListSO : ScriptableObject, MenusLibrary
	{
		public List<MenuBase> menus;

		public IEnumerable<MenuBase> GetMenus()
		{
			return menus;
		}
	}
}
