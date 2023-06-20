using JetBrains.Annotations;
using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SR.Extras
{
	public enum PlayerEffectLevel
	{
		Nothing,
		OnlySmoke,
		SmokeAndFire,
		Maximum
	}

	public class PlayerThrusterVisual : MonoBehaviour
	{
		#region Variables

		[SerializeField] private ParticleSystem smokePS;
		[SerializeField] private Animator fireFX;

		private PlayerEffectLevel effectLevel;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			GameInputs.onMovementStarted += GameInputs_onMovementStarted;
			GameInputs.onMovementEnded += GameInputs_onMovementEnded;
		}

		private void OnDestroy()
		{
			GameInputs.onMovementStarted -= GameInputs_onMovementStarted;
			GameInputs.onMovementEnded -= GameInputs_onMovementEnded;
		}

		#endregion

		#region Functions

		public void InitEffect(PlayerEffectLevel level)
		{
			effectLevel = level;
			bool fire = level > PlayerEffectLevel.OnlySmoke;
			bool smoke = level > PlayerEffectLevel.Nothing;
			smokePS.gameObject.SetActive(smoke);

			if (smoke)
				smokePS.Play();
		}

		#endregion

		#region Callbacks

		private void GameInputs_onMovementEnded(object sender, System.EventArgs e)
		{
			fireFX.gameObject.SetActive(false);
		}

		private void GameInputs_onMovementStarted(object sender, System.EventArgs e)
		{
			if (effectLevel > PlayerEffectLevel.OnlySmoke)
			{
				fireFX.gameObject.SetActive(true);
				fireFX.Play("Play");
			}
		}

		#endregion
	}
}
