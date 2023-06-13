using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public class StickmanHead : MonoBehaviour
	{
		#region Variables

		[SerializeField] private PlayerVehicle playerVehicle;
		[SerializeField] private LayerMask terrainLayerMask;

		#endregion

		#region UnityMessages

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (SRUtils.IsInLayerMask(collision.gameObject.layer, terrainLayerMask))
			{
				playerVehicle.Death();
			}
		}

		#endregion
	}
}
