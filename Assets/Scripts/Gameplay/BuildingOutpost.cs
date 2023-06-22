using SR.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace SR.Core
{
	public class BuildingOutpost : Outpost, IDamageable
	{
		#region HelperClasses

		public class HPEventArgs : EventArgs
		{
			public float hpRatio;
		}

		#endregion

		#region Variables

		public event EventHandler<HPEventArgs> onHPChanged;
		[SerializeField] private HealthBar healthBar;
		[SerializeField] private List<EnemySpawner> spawners = new List<EnemySpawner>();
		[SerializeField] private float baseHP = 10f;
		[SerializeField] private ParticleSystem destroyPS;
		[SerializeField] private Collider2D colliderToDisable;
		[SerializeField] private SpriteRenderer spriteToDisable;
		private float maxHP;
		private float currentHP;
		private bool bSpawning = false;
		[Inject] private SoundSystem soundSystem;

		#endregion

		#region Overrides

		public override void SetDifficulty(float difficulty)
		{
			base.SetDifficulty(difficulty);
			maxHP = baseHP * difficulty;
			currentHP = maxHP;
		}

		#endregion

		#region IDamageable

		public void ApplyDamage(int value)
		{
			currentHP -= value;
			Debug.Log($"BUilding reached {value} when hp is {currentHP}");
			if (currentHP > 0)
			{
				soundSystem.PlayBuildingDamage();
				onHPChanged?.Invoke(this, new HPEventArgs() { hpRatio = currentHP / maxHP });
				return;
			}

			DestroyOutpost();
		}

		#endregion

		#region UnityMessages

		private void Awake()
		{
			onHPChanged += healthBar.OnHPChanged;
			SetDifficulty(1f);
			SpawnRandomIgnoreTimer();
		}

		private void Update()
		{
			if (!bSpawning)
				return;

			SpawnEnemies();
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			var player = collision.gameObject.GetComponent<PlayerVehicle>();
			if (player)
			{
				ApplyDamage((int)player.GetDamage());
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			var player = collision.gameObject.GetComponentInParent<PlayerVehicle>();
			if (player)
			{
				bSpawning = true;
			}
		}

		#endregion

		#region Overrides

		public override void DestroyOutpost()
		{
			bSpawning = false;
			if (spriteToDisable)
			{
				FreeEnemies();

				healthBar.gameObject.SetActive(false);
				spriteToDisable.gameObject.SetActive(false);
				colliderToDisable.enabled = false;
				destroyPS.Play();
				soundSystem.PlayHighVelocityDamage();
				Destroy(gameObject, destroyPS.main.duration);
			}
		}

		#endregion

		#region Functions

		private void SpawnEnemies()
		{
			for (int i = 0; i < spawners.Count; i++)
			{
				spawners[i].currentTimer += Time.deltaTime * currentDifficulty;
				if (spawners[i].currentTimer >= spawners[i].spawnDelay && spawners[i].CanSpawn())
				{
					spawners[i].currentTimer = 0;
					spawners[i].SpawnEnemy();
				}
			}
		}

		private void SpawnRandomIgnoreTimer()
		{
			int i = UnityEngine.Random.Range(0, spawners.Count);
			spawners[i].currentTimer = 0;
			spawners[i].SpawnEnemy();
		}

		private void FreeEnemies()
		{
			foreach (var sp in spawners)
			{
				foreach (var e in sp.spawnedEnemies)
				{
					e.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
					transform.parent.GetComponent<TerrainGenerator>().AddToEnemies(e);
					e.transform.parent = transform.parent;
				}
			}
		}

		#endregion
	}
}
