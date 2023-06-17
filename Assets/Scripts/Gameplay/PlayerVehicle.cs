using SR.Customization;
using SR.SceneManagement;
using SR.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using Zenject;

namespace SR.Core
{
	[Serializable]
	public struct CarDescriptor
	{
		public int health;
		public float velocity;
		public float meleeDamage;
		public float acceleration;

		public static CarDescriptor operator +(CarDescriptor a, CarDetailStats stats)
		{
			a.health += stats.health;
			a.velocity += stats.velocity;
			a.meleeDamage += stats.meleeDamage;
			a.acceleration += stats.acceleration;

			return a;
		}

		public override string ToString()
		{
			return $"hp: {health}, vel: {velocity}, mel: {meleeDamage}, acc: {acceleration}";
		}
	}

	public interface IDamageable
	{
		public void ApplyDamage(int value);
	}

	public class PlayerVehicle : MonoBehaviour, IDamageable
	{
		#region HelperClasses

		public class HealthEventArgs : EventArgs
		{
			public int hp;
		}

		#endregion
		#region Variables

		[Header("Components")]
		[SerializeField] private Rigidbody2D frontTireRB;
		[SerializeField] private Rigidbody2D backTireRB;
		[SerializeField] private Rigidbody2D carRB;
		[SerializeField] private Transform cameraFollow;
		[SerializeField] private StickmanHead head;
		[SerializeField] private InGameCarCustomizer carCustomizer;
		[SerializeField] private PlayerWeapon weaponController;

		[Header("Properties")]
		[SerializeField] private CarDescriptor baseCarDescriptor;
		[SerializeField] private float cameraUpperOffset = 3f;
		[SerializeField] private float cameraRightOffset = 3f;
		[SerializeField] private float cameraBlendDelta = 0.25f;

		private CarDescriptor fullCarDescriptor;

		public event EventHandler<HealthEventArgs> onHealthChanged;
		public event EventHandler onDeath;

		[Inject] GameplayBase gameplayBase;
		[Inject] GameInputs gameInputs;
		[Inject] GameInstance gameInstance;

		private bool bFrozen;
		private bool bAlive;

		#endregion

		#region UnityMessages

		private void Start()
		{
			Freeze();
			gameplayBase.onGameStarted += GameplayBase_onGameStarted;
		}

		private void FixedUpdate()
		{
			if (bFrozen || !bAlive)
				return;

			float input = gameInputs.GetMovement();
			frontTireRB.AddTorque(-input * fullCarDescriptor.acceleration * Time.fixedDeltaTime);
			backTireRB.AddTorque(-input * fullCarDescriptor.acceleration * Time.fixedDeltaTime);
			carRB.AddTorque(input * fullCarDescriptor.acceleration * Time.fixedDeltaTime);
			carRB.velocity = Vector2.ClampMagnitude(carRB.velocity, fullCarDescriptor.velocity);
		}

		#endregion

		#region Functions

		public float GetVelocity()
		{
			Debug.Log(carRB.velocity.magnitude);
			return carRB.velocity.magnitude;
		}

		public float GetDamage()
		{
			return carRB.velocity.magnitude + fullCarDescriptor.meleeDamage;
		}

		public Vector3 GetHeadPosition()
		{
			return head.transform.position;
		}

		public bool IsAlive()
		{
			return bAlive;
		}

		public void ApplyDamage(int damage)
		{
			Debug.Log($"Received {damage} damage");
			fullCarDescriptor.health -= damage;
			if (fullCarDescriptor.health <= 0)
			{
				Death();
			}
			else
			{
				onHealthChanged?.Invoke(this, new HealthEventArgs() { hp = fullCarDescriptor.health });
			}
		}

		public void Death()
		{
			if (!bAlive)
				return;

			Freeze();
			bAlive = false;
			onDeath?.Invoke(this, EventArgs.Empty);
			Debug.Log("Dead");
		}

		public int GetHP()
		{
			return fullCarDescriptor.health;
		}

		public void Freeze()
		{
			bFrozen = true;
		}

		public void UnFreeze()
		{
			bFrozen = false;
		}

		public void UpdateCameraFollow(Vector3 upperLimit)
		{
			var tempPos = cameraFollow.position;
			tempPos.y = Mathf.MoveTowards(tempPos.y, upperLimit.y + cameraUpperOffset, cameraBlendDelta);
			tempPos.x = transform.position.x + cameraRightOffset;
			cameraFollow.position = tempPos;
		}

		#endregion

		#region Callbacks

		private void GameplayBase_onGameStarted(object sender, System.EventArgs e)
		{
			var car = gameInstance.GetCarConfig();

			fullCarDescriptor = baseCarDescriptor;

			var bumper = gameInstance.GetShopLibrary().GetBumper(car.bumper);
			fullCarDescriptor += bumper.stats;
			carCustomizer.SetDetail(bumper);

			var backdoor = gameInstance.GetShopLibrary().GetBackdoor(car.backdoor);
			fullCarDescriptor += backdoor.stats;
			carCustomizer.SetDetail(backdoor);

			var wheels = gameInstance.GetShopLibrary().GetWheels(car.wheels);
			fullCarDescriptor += wheels.stats;
			carCustomizer.SetDetail(wheels);

			var weapon = gameInstance.GetShopLibrary().GetWeapon(car.weapon);
			fullCarDescriptor += weapon.stats;
			carCustomizer.SetDetail(weapon);
			weaponController.SetWeapon(weapon);

			var stickman = gameInstance.GetShopLibrary().GetStickman(car.stickman);
			fullCarDescriptor += stickman.stats;
			carCustomizer.SetDetail(stickman);

			onHealthChanged?.Invoke(this, new HealthEventArgs() { hp = fullCarDescriptor.health });
			bAlive = true;
			UnFreeze();

			weaponController.StartShooting();

			Debug.Log(baseCarDescriptor);
			Debug.LogWarning(fullCarDescriptor);
		}

		#endregion
	}
}
