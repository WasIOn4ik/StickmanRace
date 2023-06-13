using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class Obstacle : MonoBehaviour
	{
		#region Variables

		[SerializeField] protected float destroyVelocity;
		[SerializeField] protected LayerMask playerLayerMask;

		#endregion

		#region UnityMessages

		protected virtual void OnPlayerCollisionConfirmed()
		{
			Destroy(gameObject);
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, playerLayerMask) && collision.rigidbody.velocity.magnitude > destroyVelocity)
			{
				OnPlayerCollisionConfirmed();
			}
		}

		#endregion
	}
}
