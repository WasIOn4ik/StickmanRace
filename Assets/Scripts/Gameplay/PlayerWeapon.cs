using SR.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SR.Core
{
	public class PlayerWeapon : MonoBehaviour
	{
		#region Variables

		[SerializeField] private PlayerVehicle playerVehicle;
		[SerializeField] private Transform bulletSpawnPoint;
		[SerializeField] private float aimVelocity = 3f;
		[SerializeField] private string buildingDestroyBulletLayerName = "BuildingDestroyBullet";
		[SerializeField] private float aimChangeTargetDelay = 0.25f;
		[Inject] GameInstance gameInstance;

		private WeaponSO weaponBase;

		private List<Obstacle> targets = new List<Obstacle>();
		//private bool targetIsBuilding = false;
		private Obstacle currentTarget = null;

		private bool bAim = false;

		#endregion

		#region UnityMessages

		private void Update()
		{
			HandleAim();
		}

		#endregion

		#region Functions

		public void StartAim()
		{
			bAim = true;

			StartCoroutine(HandleAimCoroutine());
		}

		public void StopAim()
		{
			bAim = false;

			StopCoroutine(HandleAimCoroutine());
		}

		private void HandleAim()
		{
			float currentZ = transform.eulerAngles.z;

			if (currentTarget != null && transform.position.x < currentTarget.transform.position.x)
			{
				currentZ = Mathf.MoveTowardsAngle(currentZ, SRUtils.GetRotationTo(transform.position, currentTarget.GetAimPosition()), aimVelocity);
			}
			else
			{
				currentZ = Mathf.MoveTowardsAngle(currentZ, 0, aimVelocity);
				currentTarget = null;
			}

			transform.rotation = Quaternion.Euler(0, 0, currentZ);
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
			float coef = 1.5f;
			var bullet = Instantiate(weaponBase.bulletPrefab);
			if (currentTarget == null)
				bullet.gameObject.layer = LayerMask.NameToLayer(buildingDestroyBulletLayerName);
			bullet.SetDamage(weaponBase.weaponStats.damage);
			bullet.SetVelocity(playerVehicle.GetVelocity() * coef + weaponBase.weaponStats.velocity);
			bullet.transform.rotation = transform.rotation;
			bullet.transform.position = bulletSpawnPoint.position;
			bullet.InitBullet(weaponBase.bulletType, weaponBase.bulletValue, 1f, weaponBase.weaponStats.shootDistance, gameInstance);
		}

		#endregion

		#region Coroutines

		private IEnumerator HandleAimCoroutine()
		{
			while (bAim)
			{
				float minDist = float.MaxValue;
				int index = -1;

				for (int i = 0; i < targets.Count;)
				{
					if (targets[i] == null)
					{
						targets.RemoveAt(i);
						continue;
					}
					var targetPos = targets[i].transform.position;
					if (targets[i].IsAlive() && transform.position.x < targetPos.x)
					{

						float currentDist = (transform.position - targetPos).magnitude;
						if (currentDist < minDist)
						{
							minDist = currentDist;
							index = i;
						}
					}

					i++;
				}

				if (index != -1)
				{
					currentTarget = targets[index];
				}
				yield return new WaitForSeconds(aimChangeTargetDelay);
			}
		}

		private IEnumerator HandleShoot()
		{
			while (playerVehicle.IsAlive())
			{
				yield return new WaitForSeconds(60f / weaponBase.weaponStats.fireRate);
				if (currentTarget != null && !currentTarget.IsAlive())
				{
					currentTarget = null;
				}
				if (weaponBase.bulletType == BulletType.Rifle)
				{
					float delay = 0.1f;
					int spawned = 0;
					while (spawned < weaponBase.bulletValue)
					{
						spawned++;
						yield return new WaitForSeconds(delay);
						Shoot();
					}
				}
				else
				{
					Shoot();
				}
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

		private void OnTriggerExit2D(Collider2D collision)
		{
			var target = collision.gameObject.GetComponent<Obstacle>();
			if (target)
			{
				targets.Remove(target);
			}
		}

		#endregion
	}
}
