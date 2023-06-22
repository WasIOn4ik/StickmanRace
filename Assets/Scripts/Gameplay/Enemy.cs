using SR.Core;
using System;
using System.Collections;
using UnityEngine;

namespace SR.Core
{
	public class Enemy : Obstacle
	{
		#region Variables

		public event EventHandler onAttackStarted;
		public event EventHandler onDeathStarted;

		[Header("Components")]
		[SerializeField] private Rigidbody2D rigidBody2D;

		[Header("Peoperties")]
		[SerializeField] private float attackDelay = 3f;
		[SerializeField] private float firstAttackDelay = 0.3f;
		[SerializeField] protected int damage = 1;
		[SerializeField] private float minimalShootDistance = 1f;
		[SerializeField] protected LayerMask stickmanLayerMask;

		[Header("MaxDIfficultySettings")]
		[SerializeField] private float minFirstAttackDelay = 0.05f;
		[SerializeField] private float minAttackDelay = 0.4f;

		protected PlayerVehicle target;
		protected float difficultyCoef = 1f;

		#endregion

		#region StaticVariables

		public static event EventHandler onEnemyDeath;

		#endregion

		#region UnityMessages

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, stickmanLayerMask))
			{
				target = collision.gameObject.GetComponentInParent<PlayerVehicle>();
				StartCoroutine(HandleAttack());
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, destroyLayerMask))
			{
				target = null;
			}
		}

		#endregion

		#region Functions

		public virtual void Attack()
		{
			throw new NotImplementedException();
		}

		private void StartAttack()
		{
			onAttackStarted?.Invoke(this, EventArgs.Empty);
		}

		#endregion

		#region Coroutines

		private IEnumerator HandleAttack()
		{
			yield return new WaitForSeconds(Mathf.Max(minFirstAttackDelay,firstAttackDelay/ difficultyCoef));

			while (IsAlive() && target != null && target.IsAlive() && (target.transform.position - transform.position).magnitude
				> minimalShootDistance && target.transform.position.x < transform.position.x)
			{

				if (target != null)
				{
					StartAttack();
				}

				yield return new WaitForSeconds(Mathf.Max(minAttackDelay,attackDelay/difficultyCoef));
			}
		}

		#endregion

		#region Overrides

		protected override void OnPlayerCollisionConfirmed()
		{
			onEnemyDeath?.Invoke(this, EventArgs.Empty);
			onDeathStarted?.Invoke(this, EventArgs.Empty);
			rigidBody2D.simulated = false;
			bDestroyed = true;
		}

		public override void SetDifficulty(float difficulty)
		{
			difficultyCoef = difficulty;
		}

		#endregion
	}
}
