using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class MeleeEnemy : Enemy
	{
		#region Variables

		[SerializeField] private Vector2 attackDistance = new Vector2(3f, 1f);

		#endregion

		#region Overrides

		public override void Attack()
		{
			if(IsAlive())
			{
				var result = Physics2D.BoxCast(transform.position, attackDistance, 0, Vector2.left, 0.1f, destroyLayerMask);
				if (result.collider != null)
				{
					var player = result.collider.gameObject.GetComponentInParent<PlayerVehicle>();
					player.ApplyDamage(damage);
				}
			}
		}

		#endregion
	}
}

