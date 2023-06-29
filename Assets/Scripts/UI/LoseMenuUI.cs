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
				gameInstance.Sounds.Mute();
				YandexGame.ErrorVideoEvent = () =>
				{
					gameInstance.Sounds.Unmute();
					YandexGame.ErrorVideoEvent = null;
					YandexGame.CloseFullAdEvent = null;
					YandexGame.OpenVideoEvent = null;
				};
				YandexGame.OpenVideoEvent = null;
				YandexGame.CloseVideoEvent = () =>
				{
					gameInstance.Sounds.Unmute();
					gameInstance.AddBoughtGems(gems);
					YandexGame.ErrorVideoEvent = null;
					YandexGame.CloseFullAdEvent = null;
					YandexGame.OpenVideoEvent = null;
					HandleEndgame();
				};
				YG._RewardedShow(0);
#elif UNITY_ANDROID
				gameInstance.ShowRewarded(HandleX2);
#endif
			});

			buttonPlus3HP.onClick.AddListener(() =>
			{
#if UNITY_WEBGL
				gameInstance.Sounds.Mute();
				bPlus3HPUsed = true;
				YandexGame.ErrorVideoEvent = () =>
				{
					gameInstance.Sounds.Unmute();
					YandexGame.ErrorVideoEvent = null;
					YandexGame.CloseFullAdEvent = null;
					YandexGame.OpenVideoEvent = null;
				};
				YandexGame.OpenVideoEvent = null;
				YandexGame.CloseVideoEvent = () =>
				{
					gameInstance.Sounds.Unmute();
					YandexGame.ErrorVideoEvent = null;
					YandexGame.CloseFullAdEvent = null;
					YandexGame.OpenVideoEvent = null;
					gameplayBase.ResetPlayer(3);
					BackToPrevious();
				};
				YG._RewardedShow(0);
#elif UNITY_ANDROID
				gameInstance.ShowRewarded(HandleRespawn3HP);
#endif
			});

			buttonRestart.onClick.AddListener(() =>
			{
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
#endif
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
