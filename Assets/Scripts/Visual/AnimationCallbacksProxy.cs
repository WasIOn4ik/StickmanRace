using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCallbacksProxy : MonoBehaviour
{
	[SerializeField] private EnemyAnimationCallbacks callbacks;

	private void HandleAttack()
	{
		callbacks.OnAttackPerformed();
	}

	private void HandleDeath()
	{
		callbacks.OnDeathPerformed();
	}
}
