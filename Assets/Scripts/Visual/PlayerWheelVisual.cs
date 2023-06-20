using SR.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	public class PlayerWheelVisual : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer movementPart;
		[SerializeField] private SpriteRenderer nonMovementPart;

		public void InitWheel(WheelsSO wheel)
		{
			movementPart.sprite = wheel.sprite;
			movementPart.transform.localScale = Vector3.one * wheel.scaleOverride;
			nonMovementPart.sprite = wheel.nonMovementPart;
		}
	}
}
