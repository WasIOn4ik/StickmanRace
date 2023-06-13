using SR.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

namespace SR.Core
{
	public class PlayerVehicle : MonoBehaviour
	{
		#region Variables

		[Header("Components")]
		[SerializeField] private Rigidbody2D frontTireRB;
		[SerializeField] private Rigidbody2D backTireRB;
		[SerializeField] private Rigidbody2D carRB;
		[SerializeField] private Transform cameraFollow;

		[Header("Properties")]
		[SerializeField] private float velocity;
		[SerializeField] private float rotationSpeed;
		[SerializeField] private float cameraUpperOffset = 3f;
		[SerializeField] private float cameraBlendDelta = 0.25f;

		[Inject] GameplayBase gameplayBase;
		[Inject] GameInputs gameInputs;

		private bool bFrozen;

		#endregion

		#region UnityMessages

		private void Start()
		{
			Freeze();
			gameplayBase.onGameStarted += GameplayBase_onGameStarted;
		}

		private void FixedUpdate()
		{
			if (bFrozen)
				return;

			float input = gameInputs.GetMovement();
			frontTireRB.AddTorque(-input * velocity * Time.fixedDeltaTime);
			backTireRB.AddTorque(-input * velocity * Time.fixedDeltaTime);
			carRB.AddTorque(input * rotationSpeed * Time.fixedDeltaTime);
		}

		#endregion

		#region Functions

		public void Freeze()
		{
			bFrozen = true;
		}

		public void UnFreeze()
		{
			bFrozen = false;
		}

		public void UpdateCameraFollow(Vector3 upperLimit)
		{
			var tempPos = cameraFollow.position;
			tempPos.y = Mathf.MoveTowards(tempPos.y, upperLimit.y = cameraUpperOffset, cameraBlendDelta);
			cameraFollow.position = tempPos;
		}

		#endregion

		#region Callbacks

		private void GameplayBase_onGameStarted(object sender, System.EventArgs e)
		{
			UnFreeze();
		}

		#endregion
	}
}
