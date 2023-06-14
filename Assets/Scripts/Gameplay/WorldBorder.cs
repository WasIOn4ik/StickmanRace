using SR.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SR.Core
{
	public class WorldBorder : MonoBehaviour
	{
		#region Variables

		[SerializeField] private float baseVelocity;
		[SerializeField] private LayerMask playerMask;

		[Inject] GameplayBase gameplayBase;

		private bool bMove = false;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			gameplayBase.onGameStarted += GameplayBase_onGameStarted;
		}

		private void Update()
		{
			if (bMove)
			{
				transform.position += Vector3.right * (baseVelocity * gameplayBase.GetDifficulty() * Time.deltaTime);
			}
		}

		private void OnTriggerEnter2D(Collider2D collider)
		{
			Debug.Log("Collision");
			if (SRUtils.IsInLayerMask(collider.gameObject.layer, playerMask))
			{
				var player = collider.gameObject.GetComponent<PlayerVehicle>();
				player.Death();
			}
		}

		#endregion

		#region Callbacks

		private void GameplayBase_onGameStarted(object sender, System.EventArgs e)
		{
			bMove = true;
		}

		#endregion

	}
}
