using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class ExplosionBullet : Bullet
	{
		[SerializeField] private Explosion objectToSpawnOnDestroy;
		[SerializeField] private float explosionScale;

		protected override void OnCollisionEnter2D(Collision2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, targetLayerMask))
			{
				var explosion = Instantiate(objectToSpawnOnDestroy, transform.position, Quaternion.identity);
				explosion.Explode(explosionScale, targetLayerMask);
				Destroy(gameObject);
			}
		}
	}
}
