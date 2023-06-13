using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class Bullet : MonoBehaviour
	{
		#region Variables

		[SerializeField] private int damage = 1;
		[SerializeField] private float velocity = 3f;
		[SerializeField] private LayerMask targetLayerMask;

		#endregion

		#region Functions

		private void Update()
		{
			transform.position += transform.right * velocity * Time.deltaTime;
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, targetLayerMask))
			{
				var target = collision.gameObject.GetComponent<IDamageable>();
				target.ApplyDamage(damage);
				Destroy(gameObject);
			}
		}

		#endregion

	}
}
