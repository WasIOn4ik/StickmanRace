using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	[CreateAssetMenu(menuName = "SR/Enemies/ObstaclesList", fileName = "ObstaclesList")]
	public class ObstaclesListSO : ScriptableObject
	{
		[SerializeField] private List<Obstacle> obstacles;

		public Obstacle GetRandomObstacle()
		{
			return obstacles[Random.Range(0, obstacles.Count)];
		}
	}
}

