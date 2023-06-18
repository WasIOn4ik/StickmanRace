using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexEnemyDeathHandler : MonoBehaviour
{
	[SerializeField] private Enemy enemy;
	[SerializeField] private GameObject enemyVisual;
	[SerializeField] private Animator animatorToPlayDeath;

	private void Awake()
	{
		enemy.onDeathStarted += Enemy_onDeathStarted;
	}

	private void Enemy_onDeathStarted(object sender, System.EventArgs e)
	{
		enemyVisual.SetActive(false);
		animatorToPlayDeath.gameObject.SetActive(true);
		animatorToPlayDeath.Play("Death");
	}
}
