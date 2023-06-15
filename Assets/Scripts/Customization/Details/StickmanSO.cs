using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Customization
{
	[CreateAssetMenu(menuName = "SR/Stickman", fileName = "Stickman")]
	public class StickmanSO : CarDetailSO
	{
		public Sprite headSprite;
		public Sprite bodySprite;
		public Sprite armSprite;
		public Sprite wristSprite;
		public Sprite handSprite;

		public StickmanSO() : base()
		{
			base.type = CarDetailType.Stickman;
		}
	}
}