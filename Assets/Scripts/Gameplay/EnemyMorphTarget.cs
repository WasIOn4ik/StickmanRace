using SR.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	[RequireComponent(typeof(EnemyAnimationCallbacks))]
	public class EnemyMorphTarget : MonoBehaviour
	{
		public List<SpriteRenderer> sprites;
		public Transform bulletSpawnTransform;
		public bool updateAnimator = true;
		[NonSerialized] public EnemyAnimationCallbacks callbacks;

		private void Awake()
		{
			callbacks = GetComponent<EnemyAnimationCallbacks>();
			if (updateAnimator)
				callbacks.animator = GetComponent<Animator>();
		}
	}
}
