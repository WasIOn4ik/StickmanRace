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

		protected virtual void OnPlayerCollisionConfirmed()
		{
			HandleDestroy();
		}

		public virtual void HandleDestroy()
		{
			Destroy(gameObject);
		}

		public virtual void SetDifficulty(float difficulty)
		{
			scaledDestroyVelocity = destroyVelocity * difficulty;
		}

		#endregion

		#region IDamageable

		public virtual void ApplyDamage(int value)
		{
			OnPlayerCollisionConfirmed();
		}

		#endregion
	}
}
