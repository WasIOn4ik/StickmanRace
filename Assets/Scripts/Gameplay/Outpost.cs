using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.UI
{
	public class Outpost : MonoBehaviour
	{
		#region Variables

		[SerializeField] private List<Obstacle> obstacles = new List<Obstacle>();

		#endregion

		#region Functions

		public void DestroyOutpost()
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
