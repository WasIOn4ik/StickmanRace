using SR.Core;
using SR.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SR.UI
{
	public class GameplayBase : MonoBehaviour
	{
		#region Variables

		private const float DIFFICULTY_BASE = 0.025f;

		public event EventHandler onGameStarted;

		[Header("Components")]
		[SerializeField] private Image faderImage;
		[SerializeField] private GarageUI garageUI;

		[Header("Properties")]
		[SerializeField] private float afterDeathTimeout = 5f;
		[SerializeField] private float startDifficulty = 1f;
		[SerializeField] private float difficultyIncreaseCoef = 1f;
		[SerializeField] private float fadeTime = 1f;
		[SerializeField] private float gameStartUnfadeTime = 0.5f;
		[SerializeField] private float gameStartCarForce = 100f;
		[SerializeField] private float slowMotionMultiplier = 0.25f;
		[SerializeField] private float slowMotionDuration = 0.5f;

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
			StartCoroutine(FadeImage(faderImage, fadeTime));
		}

		#endregion

		#region Functions

		public void StartSlowMotion()
		{
			Debug.Log("Slowmotion");
			Time.timeScale = slowMotionMultiplier;
			StartCoroutine(HandleSlowMotion());
		}

		public float GetDifficulty()
		{
			return difficulty;
		}

		public void StartGame()
		{
			StartCoroutine(FadeImage(false, fadeTime, () =>
			{
				garageUI.DestroyGarage();
				StartGameInternal();
				StartCoroutine(FadeImage(true, gameStartUnfadeTime));
			}));
		}

		private void StartGameInternal()
		{
			onGameStarted?.Invoke(this, EventArgs.Empty);
			StartCoroutine(HandleDifficulty());
			playerVehicle.GetComponent<Rigidbody2D>().AddForce(Vector2.right * gameStartCarForce);
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

		private IEnumerator HandleSlowMotion()
		{
			while(Time.timeScale > slowMotionMultiplier)
			{
				Time.timeScale = Mathf.MoveTowards(Time.timeScale, slowMotionMultiplier, 0.1f);
				yield return null;
			}

			yield return new WaitForSeconds(slowMotionDuration * slowMotionMultiplier);

			Time.timeScale = 1f;
		}

		private IEnumerator HandleDifficulty()
		{
			while (playerVehicle.IsAlive())
			{
				yield return new WaitForSeconds(1f);

				difficulty *= difficultyCoef;
			}
		}

		IEnumerator FadeImage(bool fadeAway, float time, Action onFade = null)
		{
			var color = faderImage.color;
			if (fadeAway)
			{
				for (float i = 1; i >= 0; i -= Time.deltaTime / time)
				{
					color.a = i;
					faderImage.color = color;
					yield return null;
				}
				onFade?.Invoke();
			}
			else
			{
				for (float i = 0; i <= 1; i += Time.deltaTime / time)
				{
					color.a = i;
					faderImage.color = color;
					yield return null;
				}
				onFade?.Invoke();
			}
		}

		#endregion
	}
}
