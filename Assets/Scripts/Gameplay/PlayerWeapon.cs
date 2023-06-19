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

		private List<Obstacle> targets = new List<Obstacle>();

		#endregion

		#region Functions

		private void Update()
		{
			while (targets.Count > 0 && targets[0] == null)
				targets.RemoveAt(0);

			if (targets.Count > 0)
			{
				if (targets[0].transform.position.x > transform.position.x)
				{
					transform.rotation = Quaternion.Euler(0, 0, SRUtils.GetRotationTo(transform.position, targets[0].transform.position + Vector3.up * 1f));
				}
				else
				{
					targets.RemoveAt(0);
				}
			}
			else
			{
				transform.rotation = Quaternion.identity;
			}
		}

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

		private void OnTriggerEnter2D(Collider2D collision)
		{
			var target = collision.gameObject.GetComponent<Obstacle>();
			if (target)
			{
				targets.Add(target);
			}
		}

		#endregion
	}
}
