using SR.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Customization
{
	[CreateAssetMenu(menuName = "SR/Shop/Detail/BumperDetail", fileName = "Bumper")]
	public class BumperSO : CarDetailSO
	{
		public AnimatorOverrideController animatorOverride;

		public BumperSO() : base()
		{
			base.type = CarDetailType.Bumper;
		}
	}
}

