using SR.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SR.Extras
{
	public class SleepableObstacle : Obstacle
	{
		#region Variables

		public Action onUnsleep;

		[Header("SleepableObstacle")]
		[SerializeField] protected Rigidbody2D rigidbody2DRef;
		[SerializeField] protected LayerMask unsleepLayerMask;

		#endregion

		#region Functions

		public void Freeze()
		{
			rigidbody2DRef.bodyType = RigidbodyType2D.Kinematic;
		}

		public void UnFreeze()
		{
			rigidbody2DRef.bodyType = RigidbodyType2D.Dynamic;
		}

		#endregion

		#region Overrides

		protected override void OnCollisionEnter2D(Collision2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, unsleepLayerMask))
			{
				if (rigidbody2DRef.isKinematic)
				{
					var vehicle = collision.gameObject.GetComponent<PlayerVehicle>();
					vehicle.BackVelocity();
					vehicle.ResetTorque();
				}
				UnFreeze();
				onUnsleep?.Invoke();
			}
			base.OnCollisionEnter2D(collision);
		}

		public override void ApplyDamage(int value)
		{
			onUnsleep?.Invoke();
			base.ApplyDamage(value);
		}

		#endregion
	}
}
