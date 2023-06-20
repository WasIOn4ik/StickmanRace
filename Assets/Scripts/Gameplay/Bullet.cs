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
		[SerializeField] protected LayerMask destroyLayerMask;
		[SerializeField] protected float maxSpeed = 10f;

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

		private void Update()
		{
			transform.position += transform.right * scaledVelocity * Time.deltaTime;
		}

		protected virtual void OnCollisionEnter2D(Collision2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.collider.gameObject.layer, targetLayerMask))
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
			else if (SRUtils.IsInLayerMask(collision.gameObject.layer, destroyLayerMask))
			{
				Destroy(gameObject);
			}
		}

		#endregion

	}
}
