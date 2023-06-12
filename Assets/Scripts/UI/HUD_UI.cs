using SR.Core;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SR.UI
{
	public class HUD_UI : MonoBehaviour
	{
		#region Variables

		[SerializeField] private TMP_Text distanceText;
		[SerializeField] private TMP_Text timeText;
		[SerializeField] private HoldableButton ForwardButton;
		[SerializeField] private HoldableButton BackwardButton;
		[SerializeField] private Transform startPosition;

		[Inject] GameInputs gameInputs;
		[Inject] PlayerVehicle playerVehicle;
		[Inject] GameplayBase gameplayBase;

		private bool bGameStarted;

		private float startTime;
		private float maxDistance;

		#endregion

		#region UnityMessages

		private void Start()
		{
			bGameStarted = false;
			gameplayBase.onGameStarted += GameplayBase_onGameStarted;

			distanceText.text = "";
			timeText.text = "";
			maxDistance = 0;

			gameObject.SetActive(false);
		}

		private void Update()
		{
			if (!bGameStarted)
				return;

			UpdateInputs();
			UpdateDisplay();
		}

		#endregion

		#region Functions

		private void UpdateInputs()
		{
			if (ForwardButton.isPressed)
			{
				gameInputs.SetMovement(1f);
			}
			else if (BackwardButton.isPressed)
			{
				gameInputs.SetMovement(-1f);
			}
			else
			{
				gameInputs.SetMovement(0f);
			}
		}

		private void UpdateDisplay()
		{
			Vector3 difference = playerVehicle.transform.position - startPosition.position;

			if (difference.x < 0)
				return;

			if (difference.x > maxDistance)
				maxDistance = difference.x;

			distanceText.text = $"{maxDistance:N1} m";
			timeText.text = $"{Time.time - startTime:N1} s";
		}

		#endregion

		#region Callbacks

		private void GameplayBase_onGameStarted(object sender, System.EventArgs e)
		{
			bGameStarted = true;
			gameObject.SetActive(true);
			startTime = Time.time;
		}

		#endregion
	}
}
