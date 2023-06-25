using SR.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
					gameplayBase.RestartGame();
				};
				YG._RewardedShow(0);
#endif
			});

			buttonPlus3HP.onClick.AddListener(() =>
			{
				Enemy.killsInRound = 0;
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
				gameplayBase.RestartGame();
			});
		}

		#endregion

		#region Functions

		public void UpdateDisplay(float distance, float time)
		{
			buttonPlus3HP.interactable = !bPlus3HPUsed;

			if(time > YandexGame.savesData.records.maxTime)
			{
				YandexGame.NewLeaderboardScores("KingExpresses", (int)YandexGame.savesData.records.maxTime);
			}

			gameInstance.TryUpdateRecords(distance, time, Enemy.killsInRound);

			YandexGame.NewLeaderboardScores("RoadKing", (int)YandexGame.savesData.records.totalDistance);/*

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
