using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Customization
{
	[CreateAssetMenu(fileName = "Weapon", menuName = "SR/WeaponDetail")]
	public class WeaponSO : CarDetailSO
	{
		public WeaponStats weaponStats;

		public WeaponSO() : base()
		{
			base.type = CarDetailType.Weapon;
		}

	}
}
