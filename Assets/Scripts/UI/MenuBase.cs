using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SR.UI
{
	public enum MenuType
	{
		StartupMenu,
		MainMenu,
		SettingsMenu,
		InGameMenu,
		LoseMenu,
		DonateShop
	}

	public interface MenusLibrary
	{
		public IEnumerable<MenuBase> GetMenus();
	}

	public abstract class MenuBase : MonoBehaviour
	{
		#region Variables

		public MenuType menuType;

		protected MenuBase previousMenu;

		#endregion

		#region StaticVariables

		/// <summary>
		/// Must be set manually
		/// </summary>
		public static MenusLibrary menusLibrary;

		#endregion

		#region StaticFunctions

		public static MenuBase OpenMenu(MenuType type, bool inject, MenuBase caller = null)
		{
			foreach (var m in menusLibrary.GetMenus())
			{
				if (m.menuType == type)
				{
					MenuBase newMenu = null;
					if (inject)
					{
						newMenu = ProjectContext.Instance.Container.InstantiatePrefabForComponent<MenuBase>(m);
						newMenu.transform.parent = null;
						SceneManager.MoveGameObjectToScene(newMenu.gameObject, SceneManager.GetActiveScene());
					}
					else
					{
						newMenu = Instantiate(m);
					}

					newMenu.Show(caller);
					return newMenu;
				}
			}
			return null;
		}

		#endregion

		#region Functions

		public virtual void Show(MenuBase callerMenu = null)
		{
			if (callerMenu != null)
			{
				previousMenu = callerMenu;
				callerMenu.Hide();
			}

			gameObject.SetActive(true);
		}

		public virtual void Hide()
		{
			gameObject.SetActive(false);
		}

		public virtual void Close()
		{
			Destroy(gameObject);
		}

		public virtual void BackToPrevious()
		{
			if (previousMenu != null)
			{
				previousMenu.Show();
			}

			Hide();
		}

		#endregion
	}
}

