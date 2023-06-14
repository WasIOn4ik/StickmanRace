using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Customization
{
	[CreateAssetMenu(menuName = "SR/BackDoorDetail", fileName = "BackDoor")]
	public class BackDoorSO : CarDetailSO
	{
		public BackDoorSO() : base()
		{
			base.type = CarDetailType.BackDoor;
		}
	}
}

