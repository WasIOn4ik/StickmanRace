using SR.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class InGameCarCustomizer : MonoBehaviour
	{
		#region Variables

		[Header("Car")]
		[SerializeField] private SpriteRenderer bumper;
		[SerializeField] private SpriteRenderer frontWheel;
		[SerializeField] private SpriteRenderer rearWheel;
		[SerializeField] private SpriteRenderer backDoor;
		[SerializeField] private SpriteRenderer weapon;

		[Header("Stickman")]
		[SerializeField] private SpriteRenderer head;
		[SerializeField] private SpriteRenderer body;
		[SerializeField] private SpriteRenderer arm;
		[SerializeField] private SpriteRenderer wrist;
		[SerializeField] private SpriteRenderer hand;

		#endregion

		private void Awake()
		{
			
		}

		#region Functions

		public void SetDetail(CarDetailSO detail)
		{
			switch (detail.type)
			{
				case CarDetailType.Wheels:
					rearWheel.sprite = detail.sprite;
					rearWheel.color = detail.color;

					frontWheel.sprite = detail.sprite;
					frontWheel.color = detail.color;
					break;
				case CarDetailType.Bumper:
					bumper.sprite = detail.sprite;
					bumper.color = detail.color;
					break;
				case CarDetailType.BackDoor:
					backDoor.sprite = detail.sprite;
					backDoor.color = detail.color;
					break;
				case CarDetailType.Weapon:
					weapon.sprite = detail.sprite;
					weapon.color = detail.color;
					break;
				case CarDetailType.Stickman:
					var stickmanDetail = (StickmanSO)detail;
					head.sprite = stickmanDetail.headSprite;
					arm.sprite = stickmanDetail.armSprite;
					wrist.sprite = stickmanDetail.wristSprite;
					hand.sprite = stickmanDetail.handSprite;
					body.sprite = stickmanDetail.bodySprite;
					break;
			}
		}

		#endregion

	}
}
