using SR.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Customization
{
	[CreateAssetMenu(menuName = "SR/Shop/Detail/WheelsDetail", fileName = "Wheels")]
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
