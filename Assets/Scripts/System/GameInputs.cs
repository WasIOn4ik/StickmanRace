using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SR.Core
{
	public class GameInputs : MonoBehaviour
	{
		#region Variables

		private float movementInput;

		#endregion

		#region UnityMessages

		private void Update()
		{
			movementInput = Input.GetAxis("Horizontal");
		}

		#endregion

		#region Functions

		public float GetMovement()
		{
			return movementInput;
		}

		public void SetMovement(float movement)
		{
			movementInput = movement;
		}

		#endregion
	}
}
