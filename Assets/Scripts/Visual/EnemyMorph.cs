using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	public class EnemyMorph : MonoBehaviour
	{
		#region Variables

		[SerializeField] private Enemy enemy;
		[SerializeField] private Transform spawnTransform;
		[SerializeField] private EnemyMorphsListSO morphsList;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			foreach (Transform ch in spawnTransform)
			{
				Destroy(ch.gameObject);
			}

			int index = Random.Range(0, morphsList.MorphTargets.Count);
			var morph = morphsList.MorphTargets[index];
			var instantiated = Instantiate(morph, spawnTransform);
			instantiated.callbacks.enemy = enemy;

			instantiated.transform.localPosition = Vector3.zero;//new Vector3(0, morph.sprites[0].bounds.size.y / 2, 0);

			if (instantiated.bulletSpawnTransform != null && enemy is RangeEnemy range)
			{
				range.SetBulletSpawn(instantiated.bulletSpawnTransform);
			}
		}

		#endregion
	}
}

