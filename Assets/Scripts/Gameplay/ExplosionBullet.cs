using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class ExplosionBullet : Bullet
	{
		#region Variables

		[SerializeField] private Explosion objectToSpawnOnDestroy;
		[SerializeField] private float explosionScale;

		#endregion

		#region Overrides

		protected override void OnCollisionEnter2D(Collision2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, targetLayerMask) || SRUtils.IsInLayerMask(collision.gameObject.layer, destroyLayerMask))
			{
				var explosion = Instantiate(objectToSpawnOnDestroy, transform.position, Quaternion.identity);
				explosion.Explode(explosionScale, targetLayerMask);
				Destroy(gameObject);
			}
		}

		#endregion
	}
}
