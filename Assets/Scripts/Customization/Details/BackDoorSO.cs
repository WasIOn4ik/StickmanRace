using SR.Extras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Customization
{
	[CreateAssetMenu(menuName = "SR/Shop/Detail/BackDoorDetail", fileName = "BackDoor")]
	public class BackDoorSO : CarDetailSO
	{
		public PlayerEffectLevel effects = PlayerEffectLevel.Nothing;

		public BackDoorSO() : base()
		{
			base.type = CarDetailType.BackDoor;
		}
	}
}

