using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class RangeEnemy : Enemy
	{
		#region Variables

		[SerializeField] private Bullet bulletPrefab;
		[SerializeField] private Transform bulletSpawnpoint;

		#endregion

		#region Functions

		public void SetBulletSpawn(Transform spawnPoint)
		{
			bulletSpawnpoint = spawnPoint;
		}

		#endregion

		#region Overrides

		public override void Attack()
		{
			float rot_z = SRUtils.GetRotationTo(bulletSpawnpoint.position, target.GetHeadPosition());
			var bullet = Instantiate(bulletPrefab);
			bullet.InitBullet(difficultyCoef, 3f);
			bullet.transform.position = bulletSpawnpoint.position;
			bullet.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
			Debug.DrawLine(bulletSpawnpoint.position, target.GetHeadPosition(), Color.red, 10f);
			var bulletRot = bullet.transform.eulerAngles;
			bulletRot.x = 0;
			bullet.transform.eulerAngles = bulletRot;
		}

		#endregion
	}
}
