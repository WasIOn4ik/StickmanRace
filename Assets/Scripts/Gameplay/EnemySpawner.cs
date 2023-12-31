using SR.Core;
using SR.Extras;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	[Serializable]
	public class EnemySpawner
	{
		#region Variables

		public Transform spawnTransform;
		public EnemiesListSO enemiesToSpawn;
		public float spawnDelay;
		public int maxSpawnedAtOnce = 1;
		[HideInInspector] public float currentTimer = 0;

		public List<Enemy> spawnedEnemies = new List<Enemy>();

		#endregion

		#region Functions

		public bool CanSpawn()
		{
			return spawnedEnemies.Count < maxSpawnedAtOnce;

		}

		public Enemy SpawnEnemy()
		{
			var enemy = GameObject.Instantiate(enemiesToSpawn.GetRandomEnemy(), spawnTransform);
			spawnedEnemies.Add(enemy);
			enemy.onDeathStarted += Enemy_onDeathStarted;
			return enemy;
		}

		private void Enemy_onDeathStarted(object sender, EventArgs e)
		{
			spawnedEnemies.Remove(sender as Enemy);
		}

		#endregion
	}
}