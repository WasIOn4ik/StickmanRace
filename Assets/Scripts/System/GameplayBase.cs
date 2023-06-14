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

		private const float DIFFICULTY_BASE = 0.025f;
		[SerializeField] private float afterDeathTimeout;
		[SerializeField] private float startDifficulty = 1f;
		[SerializeField] private float difficultyIncreaseCoef = 1f;

		public event EventHandler onGameStarted;

		[Inject] PlayerVehicle playerVehicle;

		private float difficulty;

		private float difficultyCoef;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			difficulty = startDifficulty;
			difficultyCoef = difficultyIncreaseCoef * DIFFICULTY_BASE + 1;
			playerVehicle.onDeath += PlayerVehicle_onDeath;
		}

		#endregion

		#region Functions

		public float GetDifficulty()
		{
			return difficulty;
		}

		public void StartGame()
		{
			onGameStarted?.Invoke(this, EventArgs.Empty);
			StartCoroutine(HandleDifficulty());
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

		#region Coroutines

		private IEnumerator HandleDifficulty()
		{
			while (playerVehicle.IsAlive())
			{
				yield return new WaitForSeconds(1f);

				difficulty *= difficultyCoef;
			}
		}

		#endregion
	}
}
