using SR.Core;
using SR.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	[CreateAssetMenu(menuName = "SR/Location", fileName = "Location")]
	public class LocationDescriptor : ScriptableObject
	{
		public bool bPixel;
		public bool bUseDefault = true;

		public List<Outpost> availableOutposts = new List<Outpost>();

		public Sprite background0;
		public Sprite background1;
		public Sprite background2;
		public Sprite background3;
		public Sprite background4;

		public Texture2D fillTexture;
		public Sprite cornerSprite;
	}
}
