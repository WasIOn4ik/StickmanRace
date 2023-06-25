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
		[SerializeField] private float aimChangeTargetDelay = 0.25f;

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
			/*
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
						}*/
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
			var bullet = Instantiate(weaponBase.bulletPrefab);
			if (currentTarget == null)
				bullet.gameObject.layer = LayerMask.NameToLayer(buildingDestroyBulletLayerName);
			bullet.SetDamage(weaponBase.weaponStats.damage);
			bullet.SetVelocity(playerVehicle.GetVelocity() * 1.25f + weaponBase.weaponStats.velocity);
			bullet.transform.rotation = transform.rotation;
			bullet.transform.position = bulletSpawnPoint.position;
			bullet.InitBullet(1f, weaponBase.weaponStats.shootDistance);
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
