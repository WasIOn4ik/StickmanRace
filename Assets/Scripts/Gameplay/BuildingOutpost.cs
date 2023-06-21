using SR.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace SR.Core
{
	[Serializable]
	public class EnemySpawner
	{
		public Transform spawnTransform;
		public List<Enemy> enemiesToSpawn;
		public float spawnDelay;
		public int maxSpawnedAtOnce = 2;
		[HideInInspector] public float currentTimer = 0;

		List<Enemy> spawnedEnemies = new List<Enemy>();

		public bool CanSpawn()
		{
			return spawnedEnemies.Count < maxSpawnedAtOnce;

		}

		public Enemy SpawnEnemy()
		{
			var enemy = GameObject.Instantiate(enemiesToSpawn[UnityEngine.Random.Range(0, enemiesToSpawn.Count)], spawnTransform);
			spawnedEnemies.Add(enemy);
			enemy.onDeathStarted += Enemy_onDeathStarted;
			return enemy;
		}

		public void Clear()
		{
			foreach (var el in spawnedEnemies)
			{
				GameObject.Destroy(el.gameObject);
			}
		}

		private void Enemy_onDeathStarted(object sender, EventArgs e)
		{
			spawnedEnemies.Remove(sender as Enemy);
		}
	}
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
		}

		private void Update()
		{
			if (!bSpawning)
				return;
			for (int i = 0; i < spawners.Count; i++)
			{
				spawners[i].currentTimer += Time.deltaTime;
				if (spawners[i].currentTimer >= spawners[i].spawnDelay && spawners[i].CanSpawn())
				{
					spawners[i].currentTimer = 0;
					spawners[i].SpawnEnemy();
				}
			}
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

		public override void DestroyOutpost()
		{
			if (spriteToDisable)
			{
				foreach (var s in spawners)
				{
					s.Clear();
				}
				healthBar.gameObject.SetActive(false);
				spriteToDisable.gameObject.SetActive(false);
				colliderToDisable.enabled = false;
				destroyPS.Play();
				Destroy(gameObject, destroyPS.main.duration);
			}
		}

		#endregion
	}
}
