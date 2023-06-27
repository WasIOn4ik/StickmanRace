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
		[SerializeField] private ParticleSystem particles;

		[Header("Peoperties")]
		[SerializeField] private float attackDelay = 3f;
		[SerializeField] private float firstAttackDelay = 0.3f;
		[SerializeField] protected int damage = 1;
		[SerializeField] private float minimalShootDistance = 1f;
		[SerializeField] protected LayerMask stickmanLayerMask;
		[SerializeField] private float dissapearTimer = 1.25f;

		[Header("MaxDIfficultySettings")]
		[SerializeField] private float minFirstAttackDelay = 0.05f;
		[SerializeField] private float minAttackDelay = 0.4f;

		protected PlayerVehicle target;
		protected float difficultyCoef = 1f;

		#endregion

		#region StaticVariables

		public static int killsInRound = 0;

		public static event EventHandler onEnemyDeath;

		#endregion

		#region UnityMessages

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (target == null)
			{
				target = collision.gameObject.GetComponent<PlayerVehicle>();

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
			yield return new WaitForSeconds(Mathf.Max(minFirstAttackDelay, firstAttackDelay / difficultyCoef));

			while (IsAlive() && target != null && target.IsAlive() && (target.transform.position - transform.position).magnitude
				> minimalShootDistance && target.transform.position.x < transform.position.x)
			{

				if (target != null)
				{
					StartAttack();
				}

				yield return new WaitForSeconds(Mathf.Max(minAttackDelay, attackDelay / difficultyCoef));
			}
		}

		#endregion

		#region Overrides

		protected override void OnPlayerCollisionConfirmed()
		{
			if (IsAlive())
			{
				CallDestroySound();
				rigidBody2D.freezeRotation = false;
				onEnemyDeath?.Invoke(this, EventArgs.Empty);
				bDestroyed = true;
				Invoke("FinishDestroy", dissapearTimer);
			}
			particles.Play();
		}

		private void FinishDestroy()
		{
			onDeathStarted?.Invoke(this, EventArgs.Empty);
			rigidBody2D.simulated = false;
		}

		public override void SetDifficulty(float difficulty)
		{
			difficultyCoef = difficulty;
		}

		#endregion
	}
}
