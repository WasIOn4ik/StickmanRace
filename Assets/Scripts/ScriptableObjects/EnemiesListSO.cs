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

		public List<Enemy> Enemies { get { return enemies; } }
	}
}
