using SR.Customization;
using SR.SceneManagement;
using SR.UI;
using System;
using System.Collections;
using System.Collections.Generic;
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
		public float rotationSpeed;
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

		[Header("Properties")]
		[SerializeField] private CarDescriptor carDescriptor;
		[SerializeField] private float cameraUpperOffset = 3f;
		[SerializeField] private float cameraBlendDelta = 0.25f;

		public event EventHandler<HealthEventArgs> onHealthChanged;
		public event EventHandler onDeath;

		[Inject] GameplayBase gameplayBase;
		[Inject] GameInputs gameInputs;
		[Inject] GameInstance gameInstance;

		private bool bFrozen;
		private bool bAlive;

		private int currentHP;

		#endregion

		#region UnityMessages

		private void Start()
		{
			Freeze();
			gameplayBase.onGameStarted += GameplayBase_onGameStarted;
			//DEBUG
			ApplyCar(carDescriptor);
		}

		private void FixedUpdate()
		{
			if (bFrozen || !bAlive)
				return;

			float input = gameInputs.GetMovement();
			frontTireRB.AddTorque(-input * carDescriptor.velocity * Time.fixedDeltaTime);
			backTireRB.AddTorque(-input * carDescriptor.velocity * Time.fixedDeltaTime);
			carRB.AddTorque(input * carDescriptor.rotationSpeed * Time.fixedDeltaTime);
		}

		#endregion

		#region Functions

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
			currentHP -= damage;
			if (currentHP <= 0)
			{
				Death();
			}
			else
			{
				onHealthChanged?.Invoke(this, new HealthEventArgs() { hp = currentHP });
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
			return currentHP;
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
			tempPos.x = transform.position.x;
			cameraFollow.position = tempPos;
		}

		private void ApplyCar(CarDescriptor newCarDescriptor)
		{
			carDescriptor = newCarDescriptor;
			currentHP = carDescriptor.health;
		}

		#endregion

		#region Callbacks

		private void GameplayBase_onGameStarted(object sender, System.EventArgs e)
		{
			var car = gameInstance.GetCarConfig();
			carCustomizer.SetDetail(gameInstance.GetShopLibrary().GetBumper(car.bumper));
			carCustomizer.SetDetail(gameInstance.GetShopLibrary().GetBackdoor(car.backdoor));
			carCustomizer.SetDetail(gameInstance.GetShopLibrary().GetWheels(car.wheels));
			carCustomizer.SetDetail(gameInstance.GetShopLibrary().GetWeapon(car.weapon));
			carCustomizer.SetDetail(gameInstance.GetShopLibrary().GetStickman(car.stickman));
			bAlive = true;
			UnFreeze();
		}

		#endregion
	}
}
