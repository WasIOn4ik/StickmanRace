using System;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace SR.UI
{
	public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		#region Variables

		public event EventHandler onClickedStarted;
		public event EventHandler onClickedFinished;

		[NonSerialized] public bool isPressed;

		[SerializeField] private Image backgroundImage;

		[SerializeField] private Color pressedColor;

		[SerializeField] private Color defaultColor;

		#endregion

		#region Functions

		public void OnPointerDown(PointerEventData eventData)
		{
			backgroundImage.color = pressedColor;

			if (!isPressed)
				onClickedStarted?.Invoke(this, EventArgs.Empty);

			isPressed = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			backgroundImage.color = defaultColor;

			if (isPressed)
				onClickedFinished?.Invoke(this, EventArgs.Empty);

			isPressed = false;
		}

		#endregion
	}
}