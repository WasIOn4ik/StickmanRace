using SR.Core;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace SR.Extras
{
	public class ParallaxController : MonoBehaviour
	{
		#region Variables

		[SerializeField] private List<Parallax> backgrounds;
		[SerializeField] private float fadeTime;

		#endregion

		#region Functions

		public void SetLocation(LocationDescriptorSO location)
		{
			for (int i = 0; i < backgrounds.Count; i++)
			{
				bool visible = i < location.backgrounds.Count;
				if (visible)
					backgrounds[i].SetBackground(location.backgrounds[i]);

				backgrounds[i].gameObject.SetActive(visible);
			}
		}

		public void Fade()
		{
			gameObject.SetActive(true);
			StartCoroutine(FadeAway(true, fadeTime));
		}

		public void Unfade()
		{
			gameObject.SetActive(true);
			StartCoroutine(FadeAway(false, fadeTime));
		}

		private IEnumerator FadeAway(bool fade, float time)
		{
			if (fade)
			{
				for (float i = 1; i >= 0; i -= Time.deltaTime / time)
				{
					foreach (var bg in backgrounds)
					{
						bg.SetAlpha(i);
					}
					yield return null;
				}
			}
			else
			{
				for (float i = 0; i <= 1; i += Time.deltaTime / time)
				{
					foreach (var bg in backgrounds)
					{
						bg.SetAlpha(i);
					}
					yield return null;
				}
			}
			gameObject.SetActive(!fade);
		}

		#endregion
	}
}
