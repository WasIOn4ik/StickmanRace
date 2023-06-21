using SR.Core;
using SR.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	[CreateAssetMenu(menuName = "SR/Location", fileName = "Location")]
	public class LocationDescriptorSO : ScriptableObject
	{
		public bool bPixel;
		public bool bUseDefault = true;

		public OutpostsListSO availableOutposts;
		public EnemiesListSO availableEnemies;

		public List<Sprite> backgrounds;

		public Texture2D fillTexture;
		public Sprite cornerSprite;
	}
}
