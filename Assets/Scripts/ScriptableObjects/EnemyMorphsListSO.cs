using SR.Extras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	[CreateAssetMenu(menuName = "SR/Enemies/MorphsList", fileName = "MorphList")]
	public class EnemyMorphsListSO : ScriptableObject
	{
		[SerializeField] private List<EnemyMorphTarget> morphs;

		public List<EnemyMorphTarget> MorphTargets { get { return morphs; } }
	}
}
