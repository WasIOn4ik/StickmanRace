using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	public class MorphVisibilityHandler : MonoBehaviour
	{
		[SerializeField] private int offset = 10;

		private void Awake()
		{
			var srs = GetComponentsInChildren<SpriteRenderer>();

		}
	}
}
