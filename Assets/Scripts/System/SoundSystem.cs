using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class SoundSystem : MonoBehaviour
	{
		#region Variables

		private static bool isSoundEnabled;

		#endregion

		#region StaticVariables

		public static SoundSystem instance;

		#endregion

		public static void EnableSound()
		{
			isSoundEnabled = true;
		}

		public static void DisableSound()
		{
			isSoundEnabled = false;
		}
	}
}
