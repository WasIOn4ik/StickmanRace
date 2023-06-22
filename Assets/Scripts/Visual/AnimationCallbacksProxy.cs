using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to call animation callbacks, when using morph and animator not on root object
/// </summary>
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
