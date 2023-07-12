using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidLeaderboard : MonoBehaviour
{
	public static void ShowLeadeboardUI()
	{
		Social.ShowLeaderboardUI();
	}

	public static void PostLEaderboardResult(string leaderboardName, int score)
	{
		if (Social.localUser.authenticated)
		{
			Debug.Log($"GPGS_ANDROID: Trying to update {leaderboardName} WITH SCORE {score}");
			Social.ReportScore(score, leaderboardName, HandlePostResults);
		}
		else
			Debug.Log("Can't update leaderboard while not authenticated");
	}

	private static void HandlePostResults(bool success)
	{
		if (success)
		{
			Debug.Log("Successfully updated scores");
		}
		else
		{
			Debug.Log("Error while updatin leaderboard");
		}
	}
}
