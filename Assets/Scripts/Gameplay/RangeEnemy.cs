using SR.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SR.Core
{
	public class RangeEnemy : Enemy
	{
		#region Variables

		public static event EventHandler onAnyEnemyShoot;

		[SerializeField] private Bullet bulletPrefab;
		[SerializeField] private Transform bulletSpawnpoint;
		[SerializeField] private BulletType bulletType = BulletType.Standart;
		[SerializeField] private int bulletValue = 4;

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
			if (IsAlive())
			{
				onAnyEnemyShoot?.Invoke(this, EventArgs.Empty);
				float rot_z = SRUtils.GetRotationTo(bulletSpawnpoint.position, target.GetHeadPosition());
				var bullet = Instantiate(bulletPrefab);
				bullet.transform.position = bulletSpawnpoint.position;
				bullet.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
				Debug.DrawLine(bulletSpawnpoint.position, target.GetHeadPosition(), Color.red, 10f);
				var bulletRot = bullet.transform.eulerAngles;
				bulletRot.x = 0;
				bullet.transform.eulerAngles = bulletRot;
				bullet.InitBullet(bulletType, bulletValue, difficultyCoef, 3f);
			}
		}

		#endregion
	}
}
