using SR.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	public class PlayerWheelVisual : MonoBehaviour
	{
		#region Variables

		[SerializeField] private SpriteRenderer movementPart;
		[SerializeField] private SpriteRenderer nonMovementPart;

		#endregion

		#region Functions

		public void InitWheel(WheelsSO wheel)
		{
			movementPart.sprite = wheel.sprite;
			movementPart.transform.localScale = Vector3.one * wheel.scaleOverride;
			nonMovementPart.sprite = wheel.nonMovementPart;
		}

		#endregion
	}
}
