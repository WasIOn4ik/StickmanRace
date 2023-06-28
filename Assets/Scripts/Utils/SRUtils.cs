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

		public static float GetRotationTo(Vector3 from, Vector3 to)
		{
			Vector3 diff = to - from;
			diff.Normalize();

			return Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
		}
	}
	public static class TransformEx
	{
		public static Transform Clear(this Transform transform)
		{
			foreach (Transform child in transform)
			{
				GameObject.Destroy(child.gameObject);
			}
			return transform;
		}
	}
}
