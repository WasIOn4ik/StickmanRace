using SR.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Zenject;

namespace SR.UI
{
	public class LoseMenuUI : MenuBase
	{
		#region Variables

		[SerializeField] private TMP_Text distanceText;
		[SerializeField] private TMP_Text timeText;
		[SerializeField] private TMP_Text gemsText;
		[SerializeField] private Button buttonX2;
		[SerializeField] private Button buttonPlus3HP;
		[SerializeField] private Button buttonRestart;

		[Inject] private GameInstance gameInstance;
		[Inject] private GameplayBase gameplayBase;
		[Inject] private YandexGame YG;

		public static bool bPlus3HPUsed = false;
		private static int racesCount = 0;

		private int gems;
		private float time;
		private float distance;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			buttonPlus3HP.interactable = !bPlus3HPUsed;

			buttonX2.onClick.AddListener(() =>
			{
				gameInstance.ShowRewarded(HandleX2);
			});

			buttonPlus3HP.onClick.AddListener(() =>
			{
				gameInstance.ShowRewarded(HandleRespawn3HP);
			});

			buttonRestart.onClick.AddListener(() =>
			{
				racesCount++;
				HandleEndgame();
			});
		}

		#endregion

		#region Functions

		private void HandleRespawn3HP()
		{
			bPlus3HPUsed = true;
			gameplayBase.ResetPlayer(3);
			BackToPrevious();
		}

		private void HandleX2()
		{
			gameInstance.AddBoughtGems(gems);
			HandleEndgame();
		}

		private void HandleEndgame()
		{
#if UNITY_WEBGL
			Debug.Log($"player {YandexGame.playerName} finished race");

			if (time > YandexGame.savesData.records.maxTime && YandexGame.auth && YandexGame.playerName != "anonymous")
			{
				Debug.Log("Updating Expresses");
				YandexGame.NewLeaderboardScores("Expresses", (int)time);
			}
			else
			{
				Debug.Log("Rejected update Expresses without login");
			}

			gameInstance.TryUpdateRecords(distance, time, Enemy.killsInRound);


			if (YandexGame.auth && YandexGame.playerName != "anonymous")
			{
				Debug.Log("Updating Road");
				YandexGame.NewLeaderboardScores("Road", (int)YandexGame.savesData.records.totalDistance);
			}
			else
			{
				Debug.Log("Rejected update Road without login");
			}
#elif UNITY_ANDROID
			if (gameInstance.GetRecords().maxTime < time)
			{
				AndroidLeaderboard.PostLEaderboardResult(GPGSIds.leaderboard_expresses, (int)time);
			}

			gameInstance.TryUpdateRecords(distance, time, Enemy.killsInRound);

			AndroidLeaderboard.PostLEaderboardResult(GPGSIds.leaderboard_roads, (int)gameInstance.GetRecords().totalDistance);
#endif
			//Каждую третью гонку, в которой игрок не смотрел рекламу
			if (racesCount % 3 == 2)
			{
				gameInstance.ShowInterstitial(gameplayBase.RestartGame);
			}
			else
			{
				gameplayBase.RestartGame();
			}
		}

		public void UpdateDisplay(float distance, float time)
		{
			this.time = time;
			this.distance = distance;

			buttonPlus3HP.interactable = !bPlus3HPUsed;/*

			if (time >= YandexGame.savesData.records.maxTime)
			{
				//buttonRestart.interactable = false;
				StartCoroutine(UpdateSecondLeaderBoard("KingExpresses", (int)time));
			}*/

			gems = Enemy.killsInRound;//gameInstance.DistanceToGems(distance);

			distanceText.text = Mathf.Max(0f, distance).ToString("0.0");
			timeText.text = time.ToString("0.0");
			gemsText.text = gems.ToString();
			gameInstance.Sounds.PlayCarMovement(false);
#if UNITY_ANDROID
			gameInstance.SetBannerActivity(false);
#endif
		}

		#endregion

		#region Coroutines

#if UNITY_WEBGL
		private IEnumerator UpdateSecondLeaderBoard(string techTitle, int value)
		{
			yield return new WaitForSeconds(2f);
			YandexGame.NewLeaderboardScores(techTitle, value);
			//buttonRestart.interactable = true;
		}
#endif

		#endregion
	}
}
