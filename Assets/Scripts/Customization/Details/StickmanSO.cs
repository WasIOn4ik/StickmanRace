using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Customization
{
	[CreateAssetMenu(menuName = "SR/Shop/Detail/Stickman", fileName = "Stickman")]
	public class StickmanSO : CarDetailSO
	{
		public Sprite headSprite;
		public Sprite bodySprite;
		public Sprite armSprite;
		public Sprite wristSprite;
		public Sprite handSprite;
		public Sprite thighSprite;
		public Sprite calfSprite;
		public Sprite footSprite;
		public Sprite bodyDecorSprite;
		public Sprite legsDecorSprite;
		public AudioClip deathSound;

		public StickmanSO() : base()
		{
			base.type = CarDetailType.Stickman;
		}
	}
}