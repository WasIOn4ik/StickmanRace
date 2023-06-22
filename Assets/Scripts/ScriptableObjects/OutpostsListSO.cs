using SR.Core;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	[CreateAssetMenu(menuName = "SR/Enemies/OutpostsList", fileName = "OutpostsList")]
	public class OutpostsListSO : ScriptableObject
	{
		[SerializeField] private List<Outpost> outposts;

		public Outpost GetRandomOutpost()
		{
			return outposts[Random.Range(0, outposts.Count)];
		}
	}
}
