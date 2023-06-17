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

		private int scaledDamage;
		private float scaledVelocity;

		#endregion

		#region Functions

		private void Awake()
		{
			scaledDamage = damage;
			scaledVelocity = velocity;
		}

		public void InitBullet(float difficulty)
		{/*
			scaledDamage = (int)(damage * difficulty);*/
			scaledVelocity = velocity * difficulty;
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

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, targetLayerMask))
			{
				var target = collision.gameObject.GetComponent<IDamageable>();
				target.ApplyDamage(scaledDamage);
				Destroy(gameObject);
			}
		}

		#endregion

	}
}
