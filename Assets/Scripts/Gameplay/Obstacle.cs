using SR.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class Obstacle : MonoBehaviour, IDamageable
	{
		#region Variables

		public static event EventHandler onObstacleDestroyed;

		[SerializeField] protected float destroyVelocity;
		[SerializeField] protected LayerMask destroyLayerMask;
		[SerializeField] private float aimVerticalOffset = 1f;
		[SerializeField] private ParticleSystem destroyParticles;
		[SerializeField] private float highSpeedDestruction = 5f;
		protected bool bDestroyed = false;

		private float scaledDestroyVelocity;

		#endregion

		#region UnityMessages

		private void OnCollisionEnter2D(Collision2D collision)
		{
			var player = collision.gameObject.GetComponent<PlayerVehicle>();
			if (player && player.GetDamage() > scaledDestroyVelocity)
			{
				if (player.GetVelocity() > highSpeedDestruction)
				{
					player.BackVelocity();
					HandleHighSpeedDestruction();
				}
				OnPlayerCollisionConfirmed();
			}
		}

		#endregion

		#region Functions

		public virtual void HandleHighSpeedDestruction()
		{
			if (destroyParticles == null)
				return;

			var ps = Instantiate(destroyParticles);
			Destroy(ps.gameObject, ps.main.duration);
			ps.gameObject.SetActive(true);
			ps.transform.position = destroyParticles.transform.position;
			ps.Play();
		}

		public Vector3 GetAimPosition()
		{
			return transform.position + Vector3.up * aimVerticalOffset;
		}

		protected void CallDestroySound()
		{
			onObstacleDestroyed?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnPlayerCollisionConfirmed()
		{
			CallDestroySound();
			HandleDestroy();
		}

		public bool IsAlive()
		{
			return !bDestroyed;
		}

		public void HandleDestroy()
		{
			bDestroyed = true;
			Destroy(gameObject);
		}

		public virtual void SetDifficulty(float difficulty)
		{
			scaledDestroyVelocity = destroyVelocity * difficulty;
		}

		#endregion

		#region IDamageable

		public virtual void ApplyDamage(int value)
		{
			OnPlayerCollisionConfirmed();
		}

		#endregion
	}
}
