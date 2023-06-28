using ModestTree;
using SR.Customization;
using SR.Extras;
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

	public class PlayerVehicle : MonoBehaviour
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
		[SerializeField] private PlayerThrusterVisual thrusterVisual;
		[SerializeField] private Transform meleeDamageStart;

		[Header("Properties")]
		[SerializeField] private CarDescriptor baseCarDescriptor;
		[SerializeField] private float cameraUpperOffset = 3f;
		[SerializeField] private float cameraRightOffset = 3f;
		[SerializeField] private Vector3 headPositionOffset = new Vector3(0, 0.4f, 0);
		[SerializeField] private float meleeDamageCheckDelay = 0.25f;
		[SerializeField] private float meleeDamageDistance = 1f;
		[SerializeField] private LayerMask meleeLayerMask;

		private CarDescriptor fullCarDescriptor;

		public event EventHandler<HealthEventArgs> onHealthChanged;
		public event EventHandler onDeath;

		[Inject] private GameplayBase gameplayBase;
		[Inject] private GameInputs gameInputs;
		[Inject] private GameInstance gameInstance;
		[Inject] private SoundSystem soundSystem;

		private bool bFrozen;
		private bool bAlive;
		private Vector2 cachedVelocity;
		private float cachedAngularVelocity;
		private StickmanSO stickman;

		#endregion

		#region UnityMessages

		private void Start()
		{
			Freeze();
			gameplayBase.onGameStarted += GameplayBase_onGameStarted;
			GameInputs.onMovementStarted += GameInputs_onMovementStarted;
			GameInputs.onMovementEnded += GameInputs_onMovementEnded;
		}

		private void FixedUpdate()
		{
			if (bFrozen || !bAlive)
				return;

			cachedAngularVelocity = carRB.angularVelocity;
			cachedVelocity = carRB.velocity;

			float input = gameInputs.GetMovement();
			frontTireRB.AddTorque(-input * fullCarDescriptor.acceleration * Time.fixedDeltaTime);
			backTireRB.AddTorque(-input * fullCarDescriptor.acceleration * Time.fixedDeltaTime);
			if (gameInputs.GetRotation())
			{
				carRB.AddTorque(1600f * Time.fixedDeltaTime);
			}
			carRB.velocity = Vector2.ClampMagnitude(carRB.velocity, fullCarDescriptor.velocity);
			soundSystem.SetMaxCarSound(GetVelocity() / 10f);
		}

		public void OnDestroy()
		{
			GameInputs.onMovementStarted -= GameInputs_onMovementStarted;
			GameInputs.onMovementEnded -= GameInputs_onMovementEnded;
		}

		#endregion

		#region Functions

		public void Respawn(int hp)
		{
			transform.rotation = Quaternion.identity;
			transform.position = transform.position + Vector3.up * 2f;
			fullCarDescriptor.health = hp;
			onHealthChanged?.Invoke(this, new HealthEventArgs() { hp = hp });
			bAlive = true;
			weaponController.ResetWeapon();
			weaponController.visualWeapon.transform.localPosition = Vector3.zero;
			weaponController.visualWeapon.transform.localRotation = Quaternion.identity;
			weaponController.StartShooting();
			weaponController.StartAim();
			UnFreeze();
		}

		public float GetVelocity()
		{
			return cachedVelocity.magnitude;
		}

		public void ResetTorque()
		{
			Debug.Log($"Reset angular from {carRB.angularVelocity} to: {cachedAngularVelocity}");
			carRB.angularVelocity = cachedAngularVelocity + 10;
		}

		public void BackVelocity()
		{
			Debug.Log($"Reset velocity from {carRB.velocity.magnitude} to: {cachedVelocity.magnitude}");
			carRB.velocity = cachedVelocity * 1.2f;
		}

		public float GetDamage()
		{
			return GetVelocity() / 2f + GetVelocity() / 4 * fullCarDescriptor.meleeDamage;
		}

		public Vector3 GetHeadPosition()
		{
			return head.transform.position + headPositionOffset;
		}

		public bool IsAlive()
		{
			return bAlive;
		}

		public void ApplyDamage(int damage)
		{
			if (bAlive)
			{
				soundSystem.PlayDeath(stickman.deathSound);
			}
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

			soundSystem.PlayDeath(stickman.deathSound);
			Freeze();
			bAlive = false;
			weaponController.StopAim();
			weaponController.DropWeapon();
			onDeath?.Invoke(this, EventArgs.Empty);
			Debug.Log("Dead");
		}

		public int GetHP()
		{
			return fullCarDescriptor.health;
		}

		public void Freeze()
		{
			carRB.bodyType = RigidbodyType2D.Kinematic;
			carRB.velocity = Vector2.zero;
			carRB.angularVelocity = 0f;
			gameInputs.SetMovement(0);
			gameInstance.Sounds.PlayCarMovement(false);
			frontTireRB.angularVelocity = 0f;
			backTireRB.angularVelocity = 0f;
			bFrozen = true;
		}

		public void UnFreeze()
		{
			carRB.bodyType = RigidbodyType2D.Dynamic;
			bFrozen = false;
		}

		public void UpdateCameraFollow(Vector3 upperLimit)
		{
			var tempPos = cameraFollow.position;
			tempPos.y = upperLimit.y + cameraUpperOffset;// Mathf.MoveTowards(tempPos.y, upperLimit.y + cameraUpperOffset, cameraBlendDelta);
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
			this.stickman = stickman;

			onHealthChanged?.Invoke(this, new HealthEventArgs() { hp = fullCarDescriptor.health });
			bAlive = true;
			UnFreeze();

			if (weapon.weaponStats.fireRate != 0)
			{
				weaponController.StartShooting();
				weaponController.StartAim();
			}
			thrusterVisual.InitEffect(backdoor.effects);

			StartCoroutine(HandleMelee());
		}

		private void GameInputs_onMovementEnded(object sender, EventArgs e)
		{
			soundSystem.PlayCarMovement(false);
		}

		private void GameInputs_onMovementStarted(object sender, EventArgs e)
		{
			soundSystem.PlayCarMovement(true);
		}

		#endregion

		#region Coroutines

		private IEnumerator HandleMelee()
		{
			while (IsAlive())
			{
				var targets = Physics2D.RaycastAll(meleeDamageStart.position, meleeDamageStart.TransformDirection(Vector2.right), meleeDamageDistance, meleeLayerMask);
				foreach (var target in targets)
				{
					if (target.collider.isTrigger)
						continue;
					Debug.Log(target.collider.name);

					var dmgt = target.rigidbody.gameObject.GetComponent<IDamageable>();
					if (dmgt != null)
					{
						dmgt.ApplyDamage((int)(fullCarDescriptor.meleeDamage));
					}
				}

				yield return new WaitForSeconds(meleeDamageCheckDelay);
			}
		}

		#endregion
	}
}
