using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	[CreateAssetMenu(menuName = "SR/Enemies/EnemiesList", fileName = "EnemiesList")]
	public class EnemiesListSO : ScriptableObject
	{
		[SerializeField] private List<Enemy> enemies;

		public Enemy GetRandomEnemy()
		{
			return enemies[Random.Range(0, enemies.Count)];
		}
	}
}
