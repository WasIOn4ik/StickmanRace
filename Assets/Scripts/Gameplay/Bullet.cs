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
		[SerializeField] protected LayerMask targetLayerMask;

		protected int scaledDamage;
		protected float scaledVelocity;

		#endregion

		#region Functions

		private void Awake()
		{
			scaledDamage = damage;
			scaledVelocity = velocity;
		}

		public virtual void InitBullet(float difficulty, float shootDistance)
		{/*
			scaledDamage = (int)(damage * difficulty);*/
			scaledVelocity = scaledVelocity * difficulty;
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

		private void Update()
		{
			transform.position += transform.right * scaledVelocity * Time.deltaTime;
		}

		protected virtual void OnCollisionEnter2D(Collision2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, targetLayerMask))
			{
				Debug.LogWarning(collision.gameObject.layer);
				var target = collision.gameObject.GetComponent<IDamageable>();

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

	}
}
