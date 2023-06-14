using SR.Customization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SR.Customization
{
	[Serializable]
	public struct PlayerDescriptor
	{
		public BumperSO bumper;
		public WheelsSO wheels;
		public BackDoorSO backDoor;
		public WeaponSO weapon;

		public StickmanSO stickman;
	}
}
