using SR.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SR.Customization
{
	public class GarageStickmanCustomizer : MonoBehaviour
	{
		#region Variables

		[SerializeField] private Image head;
		[SerializeField] private Image body;
		[SerializeField] private Image arm;
		[SerializeField] private Image wrist;
		[SerializeField] private Image hand;

		#endregion

		#region Functions

		public void SetStickman(StickmanSO stickman)
		{
			head.sprite = stickman.headSprite;
			body.sprite = stickman.bodySprite;
			arm.sprite = stickman.armSprite;
			wrist.sprite = stickman.wristSprite;
			hand.sprite = stickman.handSprite;
		}

		#endregion
	}
}
