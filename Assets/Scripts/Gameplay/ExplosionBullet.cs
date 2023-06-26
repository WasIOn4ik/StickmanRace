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
		}

		#endregion
	}
}
