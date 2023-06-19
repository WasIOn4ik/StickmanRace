using SR.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SR.Core
{
	public class StickmanHead : MonoBehaviour, IDamageable
	{
		#region Variables

		public PlayerVehicle playerVehicle;
		[SerializeField] private LayerMask terrainLayerMask;
		[SerializeField] private LayerMask bulletLayerMask;
		[Inject] GameplayBase gameplayBase;

		public void ApplyDamage(int value)
		{
			playerVehicle.ApplyDamage(value);
		}

		#endregion

		#region UnityMessages

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, terrainLayerMask))
			{
				playerVehicle.Death();
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, bulletLayerMask))
			{
				gameplayBase.StartSlowMotion();
			}
		}

		#endregion
	}
}
