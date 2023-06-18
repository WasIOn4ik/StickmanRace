using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class Explosion : MonoBehaviour
	{
		private LayerMask targets;
		private float targetScale;
		[SerializeField] private Collider2D explosionCollider;
		public void Explode(float scale, LayerMask mask)
		{
			targets = mask;
			targetScale = scale;
			transform.localScale = Vector3.one * targetScale;
			Destroy(gameObject, 0.3f);
			//StartCoroutine(HandleExplosion());
		}

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
		/*
				private void OnTriggerEnter2D(Collider2D collision)
				{
					if (SRUtils.IsInLayerMask(collision.gameObject.layer, targets))
					{
						var target = collision.gameObject.GetComponent<IDamageable>();

						if (target != null)
						{
							target.ApplyDamage(1);
						}
					}
				}*/

		/*
				private IEnumerator HandleExplosion()
				{
					float scale = 0f;
					float delta = targetScale / 30f;
					while (scale < targetScale)
					{
						scale = Mathf.MoveTowards(scale, targetScale, delta);

						transform.localScale = Vector3.one * scale;

						var cast = Physics2D.CircleCastAll(transform.position, scale / 2, Vector2.zero, 0f, targets);
						Debug.LogError(cast.Length);
						if (cast.Length > 0)
						{
							foreach (var el in cast)
							{
								var collision = el.collider;

								var target = collision.gameObject.GetComponent<IDamageable>();
								if (target != null)
								{
									target.ApplyDamage(1);
								}
							}
						}

						yield return null;
					}

					Destroy(gameObject);
				}*/
	}
}
