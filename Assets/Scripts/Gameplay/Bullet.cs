using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public enum BulletType
	{
		Standart,
		Sniper,
		Rifle,
		Shotgun,
		Explosion
	}
	public class Bullet : MonoBehaviour
	{
		#region Variables

		[SerializeField] private int damage = 1;
		[SerializeField] private float velocity = 3f;
		[SerializeField] protected LayerMask targetLayerMask;
		[SerializeField] protected LayerMask destroyLayerMask;
		[SerializeField] protected float maxSpeed = 10f;
		[SerializeField] private GameObject objectToSpawnOnDestroy;

		protected int scaledDamage;
		protected float scaledVelocity;
		private BulletType bulletType;
		private int bulletValue;
		private int collisions = 0;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			scaledDamage = damage;
			scaledVelocity = velocity;
		}

		private void Update()
		{
			transform.position += transform.right * scaledVelocity * Time.deltaTime;
		}

		protected virtual void OnCollisionEnter2D(Collision2D collision)
		{
			if (bulletType == BulletType.Explosion)
			{
				var explosion = Instantiate(objectToSpawnOnDestroy, transform.position, Quaternion.identity).GetComponent<Explosion>();
				explosion.Explode(bulletValue, targetLayerMask);
				Destroy(gameObject);
			}
			else if (bulletType == BulletType.Sniper)
			{
				var target = collision.collider.GetComponent<IDamageable>();

				if (target == null)
				{
					Destroy(gameObject);
					return;
				}
				collisions++;

				target.ApplyDamage(scaledDamage);

				if (collisions > bulletValue)
				{
					Destroy(gameObject);
				}
			}
			else
			{
				var target = collision.collider.GetComponent<IDamageable>();

				if (target == null)
				{
					Destroy(gameObject);
					return;
				}

				target.ApplyDamage(scaledDamage);
				Destroy(gameObject);
			}
		}

		#endregion

		#region Functions

		public void InitBullet(BulletType type, int bulletValue, float difficulty, float shootDistance, GameInstance gameInstance = null)
		{
			this.bulletType = type;
			this.bulletValue = bulletValue;

			if(gameInstance != null)
			{
				if (bulletType == BulletType.Explosion)
				{
					gameInstance.Sounds.PlayRocketLauncher();
				}
				else if (bulletType == BulletType.Rifle)
				{
					gameInstance.Sounds.PlayWeapon(bulletValue);
				}
				else if (bulletType == BulletType.Shotgun && bulletValue != 0)
				{
					gameInstance.Sounds.PlayWeapon(1);
				}
				else
				{
					gameInstance.Sounds.PlayWeapon(1);
				}
			}

			if (bulletType == BulletType.Shotgun)
			{
				float degrees = 30f;
				for (int i = 0; i < bulletValue; i++)
				{
					var bullet = Instantiate(this, transform.position, Quaternion.Euler(
						transform.eulerAngles + Vector3.forward * degrees - Vector3.forward * (degrees / bulletValue * i * 2)));
					bullet.SetVelocity(scaledVelocity);
					bullet.InitBullet(type, 0, difficulty, shootDistance, gameInstance);
				}

				if (bulletValue > 0)
					Destroy(gameObject);
			}

			scaledVelocity = Mathf.Min(maxSpeed, scaledVelocity * difficulty);
			Destroy(gameObject, shootDistance);
		}

		public void SetVelocity(float vel)
		{
			scaledVelocity = vel;
		}

		public void SetDamage(int dmg)
		{
			scaledDamage = dmg;
		}

		#endregion

	}
}
