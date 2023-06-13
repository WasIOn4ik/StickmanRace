using SR.Core;
using SR.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SR.UI
{
	public class GameplayBase : MonoBehaviour
	{
		#region Variables
		[SerializeField] private float afterDeathTimeout;

		public event EventHandler onGameStarted;

		[Inject] PlayerVehicle playerVehicle;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			playerVehicle.onDeath += PlayerVehicle_onDeath;
		}

		#endregion

		#region Functions

		public void StartGame()
		{
			onGameStarted?.Invoke(this, EventArgs.Empty);
		}

		private void RestartGame()
		{
			SceneLoader.LoadScene(SRScene.GameScene);
		}

		#endregion

		#region Callbacks

		private void PlayerVehicle_onDeath(object sender, EventArgs e)
		{
			Invoke("RestartGame", afterDeathTimeout);
		}

		#endregion
	}
}
