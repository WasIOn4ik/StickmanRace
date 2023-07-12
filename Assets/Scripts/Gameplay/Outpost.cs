using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class Outpost : Obstacle
	{
		#region Variables

		[Header("Outpost")]
		[SerializeField] protected List<Obstacle> obstacles = new List<Obstacle>();

		protected float currentDifficulty;

		#endregion

		#region Functions

		public override void SetDifficulty(float difficulty)
		{
			currentDifficulty = difficulty;
			foreach(var obstacle in obstacles)
			{
				obstacle.SetDifficulty(difficulty);
			}
		}

		public virtual void DestroyOutpost()
		{
			foreach (var obstacle in obstacles)
			{
				Destroy(obstacle);
			}
			Destroy(gameObject);
		}

		#endregion
	}
}
