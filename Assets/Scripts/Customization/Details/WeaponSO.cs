using SR.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Customization
{
	[CreateAssetMenu(menuName = "SR/Shop/Detail/WeaponDetail", fileName = "Weapon")]
	public class WeaponSO : CarDetailSO
	{
		public WeaponStats weaponStats;
		public Bullet bulletPrefab;

		public WeaponSO() : base()
		{
			base.type = CarDetailType.Weapon;
		}

	}
}
