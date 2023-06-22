using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SR.Core
{
	public class GameInputs : MonoBehaviour
	{
		#region Variables

		public static event EventHandler onMovementStarted;
		public static event EventHandler onMovementEnded;

		private float movementInput;

		private float previousMovementInput;

		#endregion

		#region Functions

		public float GetMovement()
		{
			return movementInput;
		}

		public void SetMovement(float movement)
		{
			previousMovementInput = movementInput;
			movementInput = movement;

			if (previousMovementInput < 0.05f && movementInput > 0.05f)
				onMovementStarted?.Invoke(this, EventArgs.Empty);

			if(previousMovementInput > 0.05f && movementInput < 0.05f)
				onMovementEnded?.Invoke(this, EventArgs.Empty);
		}

		#endregion
	}
}
