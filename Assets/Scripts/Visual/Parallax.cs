using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class Parallax : MonoBehaviour
	{
		#region Variables

		[SerializeField] private Transform cameraComp;
		[SerializeField] private float parallaxEffect;
		[SerializeField] private List<SpriteRenderer> sprites;

		private float length;
		private float startPos;

		#endregion

		#region UnityMessages

		private void Start()
		{
			startPos = transform.position.x;
			length = sprites[0].bounds.size.x;
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

		public void SetAlpha(float alpha)
		{
			foreach (var sprite in sprites)
			{
				Color color = sprite.color;
				color.a = alpha;
				sprite.color = color;
			}
		}

		public void SetBackground(Sprite sprite)
		{
			sprites[0].sprite = sprite;
			sprites[1].sprite = sprite;
			sprites[1].transform.localPosition = new Vector3(-sprites[1].bounds.size.x, 0, 0);
			sprites[2].sprite = sprite;
			sprites[2].transform.localPosition = new Vector3(sprites[2].bounds.size.x, 0, 0);
			length = sprites[0].bounds.size.x;
		}

		#endregion
	}
}
