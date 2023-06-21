using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class AdvancedBullet : Bullet
	{
		#region Variables

		[Header("Advanced Bullet")]
		[SerializeField] private int collisionsCount = 1;

		#endregion

		#region Overrides

		protected override void OnCollisionEnter2D(Collision2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, targetLayerMask))
			{
				var target = collision.gameObject.GetComponent<IDamageable>();
				if (target == null)
				{
					Destroy(gameObject);
					return;
				}
				target.ApplyDamage(scaledDamage);
				collisionsCount--;

				if (collisionsCount <= 0)
					Destroy(gameObject);
			}
			else if(SRUtils.IsInLayerMask(collision.gameObject.layer, destroyLayerMask))
			{
				Destroy(gameObject);
			}
		}

		#endregion
	}
}
