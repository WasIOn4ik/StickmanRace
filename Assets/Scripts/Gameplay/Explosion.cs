using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class Explosion : MonoBehaviour
	{
		#region Variables

		[SerializeField] private Collider2D explosionCollider;

		private LayerMask targets;
		private float targetScale;

		#endregion

		#region Functions

		public void Explode(float scale, LayerMask mask)
		{
			targets = mask;
			targetScale = scale;
			transform.localScale = Vector3.one * targetScale;
			Destroy(gameObject, 0.3f);
			//StartCoroutine(HandleExplosion());
		}

		#endregion

		#region AnimationCallbacks

		private void HandleExplode()
		{
			RaycastHit2D[] collisions = new RaycastHit2D[10];
			if (explosionCollider.Cast(Vector2.zero, collisions) > 0)
			{
				foreach (var collision in collisions)
				{
					if (!collision)
						return;

					var target = collision.collider.gameObject.GetComponent<IDamageable>();

					if (target != null)
					{
						target.ApplyDamage(1);
					}
				}
			}
		}

		private void DestroyExplosion()
		{
			Destroy(gameObject);
		}

		#endregion
	}
}
