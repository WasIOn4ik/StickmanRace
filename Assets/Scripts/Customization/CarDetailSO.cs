using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Customization
{
	public class CarDetailSO : ScriptableObject
	{
		public string identifier;
		public Sprite sprite;
		public Color color;
		public CarDetailStats stats;
		public CarDetailType type;

		public CarDetailSO()
		{
			color = Color.white;
		}
	}
}
