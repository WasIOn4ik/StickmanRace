using ModestTree;
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
		[SerializeField] public Transform visualWeapon;
		[SerializeField] public SpriteRenderer weaponSpriteRenderer;
		[SerializeField] public Animator burstAnimator;
		[Inject] GameInstance gameInstance;

		private WeaponSO weaponBase;

		private List<Obstacle> targets = new List<Obstacle>();
		//private bool targetIsBuilding = false;
		private Obstacle currentTarget = null;

		private bool bAim = false;
		private int weaponLayer;

		private Coroutine weaponDropCoroutine;

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
			bulletSpawnPoint.localPosition = bulletSpawnPoint.localPosition + new Vector3(weapon.barrelOffset.x, weapon.barrelOffset.y, 0);
		}

		public void StartShooting()
		{
			StartCoroutine(HandleShoot());
		}

		private void Shoot()
		{
			burstAnimator.Play("Burst");
			float coef = 1.5f;
			var bullet = Instantiate(weaponBase.bulletPrefab);
			if (currentTarget is Outpost)
				bullet.gameObject.layer = LayerMask.NameToLayer(buildingDestroyBulletLayerName);
			bullet.SetDamage(weaponBase.weaponStats.damage);
			bullet.SetVelocity(playerVehicle.GetVelocity() * coef + weaponBase.weaponStats.velocity);
			bullet.transform.rotation = transform.rotation;
			bullet.transform.position = new Vector3(bulletSpawnPoint.position.x,
				bulletSpawnPoint.position.y, bulletSpawnPoint.position.z);
			bullet.SetSprite(weaponBase.bulletSprite);
			bullet.InitBullet(weaponBase.bulletType, weaponBase.bulletValue, 1f, weaponBase.weaponStats.shootDistance, gameInstance);
		}

		#endregion

		#region Coroutines

		private IEnumerator WeaponAnim()
		{
			Vector3 dir = new Vector3(Random.Range(-0.25f, 0.25f), -1f, 0);
			while (true)
			{
				visualWeapon.transform.Rotate(new Vector3(0, 0, 50 * Time.deltaTime));
				visualWeapon.transform.position += (dir * Time.deltaTime * 5f);
				yield return null;
			}
		}

		public void DropWeapon()
		{
			weaponLayer = weaponSpriteRenderer.sortingOrder;
			weaponSpriteRenderer.sortingOrder = 200;
			weaponDropCoroutine = StartCoroutine(WeaponAnim());
		}

		public void ResetWeapon()
		{
			StopCoroutine(weaponDropCoroutine);
			weaponSpriteRenderer.sortingOrder = weaponLayer;
		}

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
					while (currentTarget != null && spawned < weaponBase.bulletValue)
					{
						spawned++;
						yield return new WaitForSeconds(delay);
						Shoot();
					}
				}
				else
				{
					if (currentTarget != null)
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
