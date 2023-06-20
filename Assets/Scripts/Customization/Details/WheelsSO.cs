using SR.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Customization
{
	[CreateAssetMenu(fileName = "Wheels", menuName = "SR/WheelsDetail")]
	public class WheelsSO : CarDetailSO
	{
		public Sprite nonMovementPart;
		public float scaleOverride = 1f;

		public WheelsSO() : base()
		{
			base.type = CarDetailType.Wheels;
		}
	}
}
