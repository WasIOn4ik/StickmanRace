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
		[SerializeField] private Transform bulletSpawnPoint;
		[SerializeField] private float aimVelocity = 3f;
		[SerializeField] private string buildingDestroyBulletLayerName = "BuildingDestroyBullet";

		private WeaponSO weaponBase;

		private List<Obstacle> targets = new List<Obstacle>();
		private bool targetIsBuilding = false;

		#endregion

		#region Functions

		private void Update()
		{
			targetIsBuilding = true;
			while (targets.Count > 0 && targets[0] == null)
				targets.RemoveAt(0);

			if (targets.Count > 0)
			{
				if (targets[0].transform.position.x > transform.position.x)
				{
					float currentZ = transform.eulerAngles.z;
					currentZ = Mathf.MoveTowardsAngle(currentZ, SRUtils.GetRotationTo(transform.position, targets[0].GetAimPosition()), aimVelocity);
					transform.rotation = Quaternion.Euler(0, 0, currentZ);
					targetIsBuilding = false;
				}
				else
				{
					targets.RemoveAt(0);
				}
			}
			else
			{
				float currentZ = transform.eulerAngles.z;
				currentZ = Mathf.MoveTowardsAngle(currentZ, 0, aimVelocity);
				transform.rotation = Quaternion.Euler(0, 0, currentZ);
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
			if (targetIsBuilding)
				bullet.gameObject.layer = LayerMask.NameToLayer(buildingDestroyBulletLayerName);
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
