using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	public class BumperLaser : MonoBehaviour
	{
		[SerializeField] private float leftOffset;
		[SerializeField] private float length;
		[SerializeField] private float damageTimer;
		[SerializeField] private LayerMask layerMask;

		private float localTImer = 0;

		private void Update()
		{
			localTImer += Time.deltaTime;
			if (localTImer > damageTimer)
			{
				localTImer = 0;
				Vector3 pos = transform.position + Vector3.left * leftOffset;
				Debug.DrawRay(pos, Vector2.right * length, Color.cyan, 0.1f);
				var targets = Physics2D.RaycastAll(pos, Vector2.right, length, layerMask);

				foreach (var target in targets)
				{
					var dmgT = target.collider.gameObject.GetComponent<IDamageable>();
					if (dmgT != null)
					{
						dmgT.ApplyDamage(1);
					}
				}
			}
		}
	}
}
