using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	public static class SRUtils
	{
		public static bool IsInLayerMask(int layer, LayerMask layerMask)
		{
			return layerMask == (layerMask | (1 << layer));
		}
	}
}
