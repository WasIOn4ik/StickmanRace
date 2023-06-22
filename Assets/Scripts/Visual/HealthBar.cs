using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SR.Core.BuildingOutpost;

namespace SR.UI
{
	public class HealthBar : MonoBehaviour
	{
		#region Variables

		[SerializeField] private RectTransform fill;

		#endregion

		#region Functions

		public void OnHPChanged(object sender, HPEventArgs hp)
		{
			fill.localScale = new Vector3(hp.hpRatio, 1f, 1f);
		}

		#endregion
	}
}
