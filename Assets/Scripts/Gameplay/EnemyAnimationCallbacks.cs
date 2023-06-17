using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class EnemyAnimationCallbacks : MonoBehaviour
	{
		#region Variables

		private const string DEATH_ANIM = "Death";
		private const string ATTACK_ANIM = "Attack";

		public Enemy enemy;
		public Animator animator;

		#endregion

		#region UnityMessages

		private void Start()
		{
			enemy.onAttackStarted += Enemy_onAttackStarted;
			enemy.onDeathStarted += Enemy_onDeathStarted;
		}

		#endregion

		#region Callbacks

		private void Enemy_onDeathStarted(object sender, System.EventArgs e)
		{
			animator.Play(DEATH_ANIM);
		}

		private void Enemy_onAttackStarted(object sender, System.EventArgs e)
		{
			animator.Play(ATTACK_ANIM);
		}

		#endregion

		#region AnimationEvents

		private void OnAttackPerformed()
		{
			enemy.Attack();
		}

		private void OnDeathPerformed()
		{
			enemy.HandleDestroy();
		}

		#endregion
	}
}
