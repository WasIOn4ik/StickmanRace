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
#if UNITY_WEBGL
				YandexGame.OpenVideoEvent = null;
				YandexGame.CloseVideoEvent = () =>
				{
					gameInstance.AddBoughtGems(gems);
					YandexGame.CloseVideoEvent = null;
					HandleEndgame();
				};
				YG._RewardedShow(0);
#endif
			});

			buttonPlus3HP.onClick.AddListener(() =>
			{
#if UNITY_WEBGL
				bPlus3HPUsed = true;
				YandexGame.OpenVideoEvent = null;
				YandexGame.CloseVideoEvent = () =>
				{
					YandexGame.CloseVideoEvent = null;
					gameplayBase.ResetPlayer(3);
					BackToPrevious();
				};
				YG._RewardedShow(0);
#endif
			});

			buttonRestart.onClick.AddListener(() =>
			{
				HandleEndgame();
			});
		}

		#endregion

		#region Functions

		private void HandleEndgame()
		{
			Debug.Log($"player {YandexGame.playerName} finished race");

			if (time > YandexGame.savesData.records.maxTime && YandexGame.auth && YandexGame.playerName != "anonymous")
			{
				Debug.Log("Updating KingExpresses");
				YandexGame.NewLeaderboardScores("KingExpresses", (int)YandexGame.savesData.records.maxTime);
			}
			else
			{
				Debug.Log("Rejected update KingExpresses without login");
			}

			gameInstance.TryUpdateRecords(distance, time, Enemy.killsInRound);

			if (YandexGame.auth && YandexGame.playerName != "anonymous")
			{
				Debug.Log("Updating RoadKing");
				YandexGame.NewLeaderboardScores("RoadKing", (int)YandexGame.savesData.records.totalDistance);
			}
			else
			{
				Debug.Log("Rejected update RoadKing without login");
			}
			gameplayBase.RestartGame();
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
		}

		#endregion

		#region Coroutines

		private IEnumerator UpdateSecondLeaderBoard(string techTitle, int value)
		{
			yield return new WaitForSeconds(2f);
			YandexGame.NewLeaderboardScores(techTitle, value);
			//buttonRestart.interactable = true;
		}

		#endregion
	}
}
