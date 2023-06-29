using SR.Core;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SR.UI
{
	public class HUD_UI : MonoBehaviour
	{
		#region Variables

		[Header("Components")]
		[SerializeField] private TMP_Text distanceText;
		[SerializeField] private TMP_Text timeText;
		[SerializeField] private TMP_Text healthText;
		[SerializeField] private TMP_Text killsText;

		[Header("Properties")]
		[SerializeField] private HoldableButton ForwardButton;
		[SerializeField] private HoldableButton BackwardButton;
		[SerializeField] private HoldableButton RotateButton;
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
			playerVehicle.onHealthChanged += PlayerVehicle_onHealthChanged;
			playerVehicle.onDeath += PlayerVehicle_onDeath;
			gameplayBase.onGameStarted += GameplayBase_onGameStarted;
			Enemy.onEnemyDeath += Enemy_onEnemyDeath;
			bGameStarted = false;

			distanceText.text = "";
			timeText.text = "";
			maxDistance = 0;
			killsText.text = "0 X";

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

		private void UpdateHealthDisplay(int health)
		{
			healthText.text = $"X {health}";
		}

		private void UpdateInputs()
		{
			bool move = Input.GetKey(KeyCode.D);
			bool rot = Input.GetKey(KeyCode.A);

			if (!move)
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
			else
			{
				gameInputs.SetMovement(move ? 1f : 0);
			}

			if (!rot)
			{
				if (RotateButton.isPressed)
				{
					gameInputs.SetRotation(true);
				}
				else
				{
					gameInputs.SetRotation(false);
				}
			}
			else
			{
				gameInputs.SetRotation(rot);
			}
		}

		private void UpdateDisplay()
		{
			float distance = GetDistance();

			if (distance < 0)
				return;

			if (distance > maxDistance)
				maxDistance = distance;

			distanceText.text = $"{maxDistance:N1} m";
			timeText.text = $"{GetTime():N1} s";
		}

		private float GetDistance()
		{
			return (playerVehicle.transform.position - startPosition.position).x;
		}

		private float GetTime()
		{
			return Time.time - startTime;
		}

		#endregion

		#region Callbacks

		private void GameplayBase_onGameStarted(object sender, System.EventArgs e)
		{
			bGameStarted = true;
			gameObject.SetActive(true);
			startTime = Time.time;

			UpdateHealthDisplay(playerVehicle.GetHP());
		}

		private void PlayerVehicle_onHealthChanged(object sender, PlayerVehicle.HealthEventArgs e)
		{
			UpdateHealthDisplay(e.hp);
		}

		private void PlayerVehicle_onDeath(object sender, System.EventArgs e)
		{
			gameObject.SetActive(false);
			var menu = MenuBase.OpenMenu(MenuType.LoseMenu, false) as LoseMenuUI;
			menu.UpdateDisplay(GetDistance(), GetTime());
		}

		private void Enemy_onEnemyDeath(object sender, System.EventArgs e)
		{
			Enemy.killsInRound++;

			killsText.text = Enemy.killsInRound.ToString() + " X";
		}

		#endregion
	}
}
