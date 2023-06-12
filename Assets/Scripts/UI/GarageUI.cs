using SR.Core;
using SR.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GarageUI : MonoBehaviour
{
	#region Variables

	[SerializeField] private Button playButton;
	[SerializeField] private Button shopButton;
	[SerializeField] private TMP_Text distanceText;
	[SerializeField] private TMP_Text timeText;
	[SerializeField] private HUD_UI hud;

	[Inject] GameplayBase gameplayBase;

	#endregion

	#region UnityMessages

	private void Awake()
	{
		playButton.onClick.AddListener(() =>
		{
			gameplayBase.StartGame();
			Destroy(gameObject);
		});
	}

	#endregion
}
