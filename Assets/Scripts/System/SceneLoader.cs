using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SR.SceneManagement
{
	public enum SRScene
	{
		MainMenuScene,
		GameScene
	}

	public class SceneLoader : MonoBehaviour
	{
		public static void LoadScene(SRScene scene, LoadSceneMode mode = LoadSceneMode.Single, Action onSceneLoad = null)
		{
			var loadOperation = SceneManager.LoadSceneAsync(scene.ToString(), mode);

			if (onSceneLoad != null)
			{
				loadOperation.completed += x =>
				{
					onSceneLoad?.Invoke();
				};
			}
		}
	}
}
