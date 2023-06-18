using SR.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class PlayerWeapon : MonoBehaviour
	{
		#region Variables

		[SerializeField] private PlayerVehicle playerVehicle;
		[SerializeField] private Bullet bulletPrefab;
		[SerializeField] private Transform bulletSpawnPoint;

		private WeaponSO weaponBase;

		#endregion

		#region Functions

		public void SetWeapon(WeaponSO weapon)
		{
			weaponBase = weapon;
		}

		public void StartShooting()
		{
			StartCoroutine(HandleShoot());
		}

		private void Shoot()
		{
			var bullet = Instantiate(weaponBase.bulletPrefab);
			bullet.SetDamage(weaponBase.weaponStats.damage);
			bullet.SetVelocity(playerVehicle.GetVelocity() + weaponBase.weaponStats.velocity);
			bullet.transform.rotation = transform.rotation;
			bullet.transform.position = bulletSpawnPoint.position;
			bullet.InitBullet(1f, weaponBase.weaponStats.shootDistance);
		}

		#endregion

		#region Coroutines

		private IEnumerator HandleShoot()
		{
			while (playerVehicle.IsAlive())
			{
				yield return new WaitForSeconds(60f / weaponBase.weaponStats.fireRate);
				Shoot();
			}
		}

		#endregion
	}
}
