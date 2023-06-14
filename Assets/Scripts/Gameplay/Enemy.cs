using SR.Core;
using System;
using System.Collections;
using UnityEngine;

namespace SR.Core
{
	public class Enemy : Obstacle
	{
		#region Variables

		[Header("Peoperties")]
		[SerializeField] private float attackDelay = 3f;
		[SerializeField] private Bullet bulletPrefab;
		[SerializeField] private Transform bulletSpwnpoint;

		private PlayerVehicle target;
		private float difficultyCoef;

		#endregion

		#region StaticVariables

		public static event EventHandler onEnemyDeath;

		#endregion

		#region UnityMessages

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, playerLayerMask))
			{
				target = collision.gameObject.GetComponent<PlayerVehicle>();
				StartCoroutine(HandleAttack());
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, playerLayerMask))
			{
				target = null;
			}
		}

		#endregion

		#region Functions

		private void Attack()
		{
			Vector3 diff = target.GetHeadPosition() - bulletSpwnpoint.position;
			diff.Normalize();

			float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

			var bullet = Instantiate(bulletPrefab);
			bullet.InitBullet(difficultyCoef);
			bullet.transform.position = bulletSpwnpoint.position;
			bullet.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
			Debug.DrawLine(bulletSpwnpoint.position, target.GetHeadPosition(), Color.red, 10f);
			var bulletRot = bullet.transform.eulerAngles;
			bulletRot.x = 0;
			bullet.transform.eulerAngles = bulletRot;
		}

		#endregion

		#region Coroutines

		private IEnumerator HandleAttack()
		{
			yield return new WaitForSeconds(0.2f);

			while (target != null && target.IsAlive())
			{

				if (target != null)
				{
					Attack();
				}

				yield return new WaitForSeconds(attackDelay);
			}
		}

		#endregion

		#region Overrides

		protected override void OnPlayerCollisionConfirmed()
		{
			onEnemyDeath?.Invoke(this, EventArgs.Empty);
			base.OnPlayerCollisionConfirmed();
		}

		public override void SetDifficulty(float difficulty)
		{
			difficultyCoef = difficulty;
			base.SetDifficulty(difficulty);
		}

		#endregion
	}
}
