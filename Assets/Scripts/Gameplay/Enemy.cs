using SR.Core;
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
			var bullet = Instantiate(bulletPrefab);
			bullet.transform.position = bulletSpwnpoint.position;
			bullet.transform.right = target.GetHeadPosition() - bulletSpwnpoint.position;
		}

		#endregion

		#region Coroutines

		private IEnumerator HandleAttack()
		{
			while (target != null && target.IsAlive())
			{
				yield return new WaitForSeconds(attackDelay);

				if (target != null)
				{
					Attack();
				}
			}
		}

		#endregion
	}
}
