using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SR.Customization
{
	public class GarageCarCustomizer : MonoBehaviour
	{
		[SerializeField] private Image bumper;
		[SerializeField] private Image frontWheel;
		[SerializeField] private Image rearWheel;
		[SerializeField] private Image backDoor;
		[SerializeField] private Image weapon;
		[SerializeField] private GarageStickmanCustomizer stickman;

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
					stickman.SetStickman((StickmanSO)detail);
					break;
			}
		}
	}
}

