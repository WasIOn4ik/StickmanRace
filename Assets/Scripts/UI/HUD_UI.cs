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

		[Header("Components")]
		[SerializeField] private TMP_Text distanceText;
		[SerializeField] private TMP_Text timeText;
		[SerializeField] private TMP_Text healthText;

		[Header("Properties")]
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
			playerVehicle.onHealthChanged += PlayerVehicle_onHealthChanged;
			playerVehicle.onDeath += PlayerVehicle_onDeath;
			gameplayBase.onGameStarted += GameplayBase_onGameStarted;
			bGameStarted = false;

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

		private void UpdateHealthDisplay(int health)
		{
			healthText.text = $"X {health}";
		}

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
			var menu = MenuBase.OpenMenu(MenuType.LoseMenu) as LoseMenuUI;
			ProjectContext.Instance.Container.InjectGameObject(menu.gameObject);
			menu.UpdateDisplay(GetDistance(), GetTime());
		}

		#endregion
	}
}
