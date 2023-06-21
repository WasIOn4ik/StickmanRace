using SR.Customization;
using SR.Extras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class InGameCarCustomizer : MonoBehaviour
	{
		#region Variables

		[Header("Components")]
		[SerializeField] private PlayerBumperVisual bumperVisual;
		[SerializeField] private PlayerWheelVisual frontWheel;
		[SerializeField] private PlayerWheelVisual backWheel;

		[Header("Car")]
		[SerializeField] private SpriteRenderer backDoor;
		[SerializeField] private SpriteRenderer weapon;

		[Header("Stickman")]
		[SerializeField] private SpriteRenderer head;
		[SerializeField] private SpriteRenderer body;
		[SerializeField] private SpriteRenderer arm;
		[SerializeField] private SpriteRenderer wrist;
		[SerializeField] private SpriteRenderer hand;

		#endregion

		#region Functions

		public void SetDetail(CarDetailSO detail)
		{
			switch (detail.type)
			{
				case CarDetailType.Wheels:
					var wheels = detail as WheelsSO;
					frontWheel.InitWheel(wheels);
					backWheel.InitWheel(wheels);
					break;
				case CarDetailType.Bumper:
					var bumper = detail as BumperSO;
					bumperVisual.InitBumper(bumper);
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
