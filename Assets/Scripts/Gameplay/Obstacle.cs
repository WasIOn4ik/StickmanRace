using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class Obstacle : MonoBehaviour, IDamageable
	{
		#region Variables

		[SerializeField] protected float destroyVelocity;
		[SerializeField] protected LayerMask playerLayerMask;

		private float scaledDestroyVelocity;

		#endregion

		#region UnityMessages

		protected virtual void OnPlayerCollisionConfirmed()
		{
			Destroy(gameObject);
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, playerLayerMask))
			{
				var player = collision.gameObject.GetComponent<PlayerVehicle>();
				if (player.GetDamage() > scaledDestroyVelocity)
				{
					OnPlayerCollisionConfirmed();
				}
			}
		}

		#endregion

		#region Functions

		public virtual void SetDifficulty(float difficulty)
		{
			scaledDestroyVelocity = destroyVelocity * difficulty;
		}

		#endregion

		#region IDamageable

		public void ApplyDamage(int value)
		{
			scaledDestroyVelocity -= value;

			if (scaledDestroyVelocity <= 0)
			{
				OnPlayerCollisionConfirmed();
			}
		}

		#endregion
	}
}
