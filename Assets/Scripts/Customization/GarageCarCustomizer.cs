using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SR.Customization
{
	public class GarageCarCustomizer : MonoBehaviour
	{
		#region Variables

		[SerializeField] private Image bumper;
		[SerializeField] private Image frontWheel;
		[SerializeField] private Image rearWheel;
		[SerializeField] private Image backDoor;
		[SerializeField] private Image weapon;
		[SerializeField] private GarageStickmanCustomizer stickman;

		#endregion

		#region Functions

		public void SetDetail(CarDetailSO detail)
		{
			switch (detail.type)
			{
				case CarDetailType.Wheels:
					rearWheel.sprite = detail.sprite;
					rearWheel.color = detail.color;
					SetPivotAndResetLocation(rearWheel);

					frontWheel.sprite = detail.sprite;
					frontWheel.color = detail.color;
					SetPivotAndResetLocation(frontWheel);
					break;
				case CarDetailType.Bumper:
					bumper.sprite = detail.sprite;
					bumper.color = detail.color;
					SetPivotAndResetLocation(bumper);
					break;
				case CarDetailType.BackDoor:
					backDoor.sprite = detail.sprite;
					backDoor.color = detail.color;
					SetPivotAndResetLocation(backDoor);
					break;
				case CarDetailType.Weapon:
					weapon.sprite = detail.sprite;
					weapon.color = detail.color;
					SetPivotAndResetLocation(weapon);
					break;
				case CarDetailType.Stickman:
					stickman.SetStickman((StickmanSO)detail);
					break;
			}
		}

		private void SetPivotAndResetLocation(Image imgObj)
		{
			if (imgObj.sprite == null)
				return;

			Vector2 size = imgObj.GetComponent<RectTransform>().sizeDelta;
			size *= imgObj.GetComponent<Image>().pixelsPerUnit;
			Vector2 pixelPivot = imgObj.GetComponent<Image>().sprite.pivot;
			Vector2 percentPivot = new Vector2(pixelPivot.x / imgObj.sprite.rect.width, pixelPivot.y / imgObj.sprite.rect.height);
			imgObj.GetComponent<RectTransform>().pivot = percentPivot;
			imgObj.transform.localPosition = Vector3.zero;
		}

		#endregion
	}
}

