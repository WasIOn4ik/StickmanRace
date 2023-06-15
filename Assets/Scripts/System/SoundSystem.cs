using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class SoundSystem : MonoBehaviour
	{
		#region Variables

		private bool isSoundEnabled;

		#endregion

		#region Functions

		public bool IsSoundEnabled()
		{
			return isSoundEnabled;
		}

		public void EnableSound()
		{
			isSoundEnabled = true;
		}

		public void DisableSound()
		{
			isSoundEnabled = false;
		}

		#endregion
	}
}
