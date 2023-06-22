using UnityEngine;
using UnityEngine.UI;

namespace SR.Customization
{
	public class GarageCarCustomizer : MonoBehaviour
	{
		#region Variables

		[SerializeField] private Image bumper;
		[SerializeField] private Image frontWheel;
		[SerializeField] private Image frontWheelNonMovement;
		[SerializeField] private Image rearWheel;
		[SerializeField] private Image rearWheelNonMovement;
		[SerializeField] private Image backDoor;
		[SerializeField] private Image weapon;
		[SerializeField] private GarageStickmanCustomizer stickman;
		[SerializeField] private RectTransform weaponHolder;
		[SerializeField] private Sprite defaultWeaponSprite;

		Vector2 defaultWeaponSize;

		#endregion

		private void Awake()
		{
			defaultWeaponSize = weapon.rectTransform.sizeDelta;
		}

		#region Functions

		public void SetDetail(CarDetailSO detail)
		{
			switch (detail.type)
			{
				case CarDetailType.Wheels:
					var wheels = detail as WheelsSO;
					rearWheelNonMovement.sprite = wheels.nonMovementPart;
					rearWheelNonMovement.color = wheels.color;
					rearWheel.sprite = wheels.sprite;
					rearWheel.color = wheels.color;
					SetPivotAndResetLocation(rearWheel);
					SetPivotAndResetLocation(rearWheelNonMovement);

					frontWheelNonMovement.sprite = wheels.nonMovementPart;
					frontWheelNonMovement.color = wheels.color;
					frontWheel.sprite = detail.sprite;
					frontWheel.color = detail.color;
					SetPivotAndResetLocation(frontWheel);
					SetPivotAndResetLocation(frontWheelNonMovement);
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
					if (weapon.sprite != null)
					{
						var delta = defaultWeaponSize;
						delta.x *= detail.sprite.rect.width / defaultWeaponSprite.rect.width;
						delta.y *= detail.sprite.rect.height / defaultWeaponSprite.rect.height;
						weapon.rectTransform.sizeDelta = delta;
					}
					weapon.rectTransform.anchoredPosition = weaponHolder.anchoredPosition;
					break;
				case CarDetailType.Stickman:
					stickman.SetStickman((StickmanSO)detail);
					break;
			}
		}
		void CopyRectTransformSize(RectTransform copyFrom, RectTransform copyTo)
		{
			copyTo.anchorMin = copyFrom.anchorMin;
			copyTo.anchorMax = copyFrom.anchorMax;
			copyTo.anchoredPosition = copyFrom.anchoredPosition;
			copyTo.sizeDelta = copyFrom.sizeDelta;
		}

		public static void SetPivotAndResetLocation(Image imgObj)
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

