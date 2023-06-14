using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class Parallax : MonoBehaviour
	{
		#region Variables

		[SerializeField] private Transform cameraComp;
		[SerializeField] private SpriteRenderer spriteRenderer;
		[SerializeField] private float parallaxEffect;
		[SerializeField] private List<SpriteRenderer> sprites;

		private float length;
		private float startPos;

		#endregion

		#region UnityMessages

		private void Start()
		{
			startPos = transform.position.x;
			length = spriteRenderer.bounds.size.x;
		}

		private void LateUpdate()
		{
			float temp = cameraComp.transform.position.x * (1 - parallaxEffect);
			float distance = cameraComp.transform.position.x * parallaxEffect;

			transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

			if (temp > startPos + length)
			{
				startPos += length;
			}
			else if (temp < startPos - length)
			{
				startPos -= length;
			}
		}

		#endregion

		#region Functions

		public void SetBackground(Sprite sprite)
		{
			foreach(var el in sprites)
			{
				el.sprite = sprite;
			}
		}

		#endregion
	}
}
