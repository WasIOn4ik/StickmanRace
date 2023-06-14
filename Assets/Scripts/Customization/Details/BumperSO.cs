using SR.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Customization
{
	[CreateAssetMenu(menuName = "SR/BumperDetail", fileName = "Bumper")]
	public class BumperSO : CarDetailSO
	{
		public BumperSO() : base()
		{
			base.type = CarDetailType.Bumper;
		}
	}
}

