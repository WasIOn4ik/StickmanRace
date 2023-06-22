using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class ShotgunBullet : Bullet
	{
		#region Variables

		public int pieces;
		[SerializeField] private float degrees;

		#endregion

		#region Overrides

		public override void InitBullet(float difficulty, float shootDistance)
		{
			base.InitBullet(difficulty, shootDistance);
			for (int i = 0; i < pieces; i++)
			{
				var bullet = Instantiate(this, transform.position, Quaternion.Euler(
					transform.eulerAngles + Vector3.forward * degrees - Vector3.forward * (degrees / pieces * i * 2)));
				bullet.pieces = 0;
				bullet.SetVelocity(scaledVelocity);
				bullet.InitBullet(difficulty, shootDistance);
			}
			if (pieces > 0)
				Destroy(gameObject);
		}

		#endregion
	}
}
