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

		[SerializeField] private Sprite standartHead;
		[SerializeField] private Sprite standartBodyDecor;
		[SerializeField] private Sprite empty;

		[SerializeField] private Image head;
		[SerializeField] private Image body;
		[SerializeField] private Image arm;
		[SerializeField] private Image wrist;
		[SerializeField] private Image hand;
		[SerializeField] private Image thigh;
		[SerializeField] private Image calf;
		[SerializeField] private Image foot;
		[SerializeField] private Image legsDecor;
		[SerializeField] private Image bodyDecor;

		private Vector2 headStartSizeDelta;
		private Vector2 bodyDecorStartSizeDelta;

		#endregion

		private void Awake()
		{
			headStartSizeDelta = head.rectTransform.sizeDelta;
			bodyDecorStartSizeDelta = bodyDecor.rectTransform.sizeDelta;
		}

		#region Functions

		public void SetStickman(StickmanSO stickman)
		{
			head.sprite = stickman.headSprite;
			body.sprite = stickman.bodySprite;
			arm.sprite = stickman.armSprite;
			wrist.sprite = stickman.wristSprite;
			hand.sprite = stickman.handSprite;
			thigh.sprite = stickman.thighSprite;
			calf.sprite = stickman.calfSprite;
			foot.sprite = stickman.footSprite;
			legsDecor.sprite = stickman.legsDecorSprite;

			var location = head.transform.localPosition;
			GarageCarCustomizer.SetPivotAndResetLocation(head);
			Vector2 delta = headStartSizeDelta;
			delta.x *= stickman.headSprite.rect.width / standartHead.rect.width;
			delta.y *= stickman.headSprite.rect.height / standartHead.rect.height;
			head.rectTransform.sizeDelta = delta;
			head.transform.localPosition = location;

			if(stickman.bodyDecorSprite != null)
			{
				bodyDecor.sprite = stickman.bodyDecorSprite;
				location = bodyDecor.transform.localPosition;
				GarageCarCustomizer.SetPivotAndResetLocation(bodyDecor);
				delta = bodyDecorStartSizeDelta;
				delta.x *= stickman.bodyDecorSprite.rect.width / standartBodyDecor.rect.width;
				delta.y *= stickman.bodyDecorSprite.rect.height / standartBodyDecor.rect.height;
				bodyDecor.rectTransform.sizeDelta = delta;
				bodyDecor.transform.localPosition = location;
			}
			else
			{
				bodyDecor.sprite = empty;
			}

			legsDecor.sprite = stickman.legsDecorSprite == null ? empty : stickman.legsDecorSprite;

		}

		#endregion
	}
}
