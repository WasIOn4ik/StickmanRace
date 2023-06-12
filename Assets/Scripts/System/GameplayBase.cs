using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SR.UI
{
	public class GameplayBase : MonoBehaviour
	{
		public event EventHandler onGameStarted;

		public void StartGame()
		{
			onGameStarted?.Invoke(this, EventArgs.Empty);
		}
	}
}
