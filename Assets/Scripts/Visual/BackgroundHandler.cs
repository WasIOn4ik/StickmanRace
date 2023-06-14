using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	public class BackgroundHandler : MonoBehaviour
	{
		#region Variables

		[SerializeField] private MapGenerator generator;
		[SerializeField] private Parallax background0;
		[SerializeField] private Parallax background1;
		[SerializeField] private Parallax background2;
		[SerializeField] private Parallax background3;
		[SerializeField] private Parallax background4;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			generator.onTerrainChanged += Generator_onTerrainChanged;
		}

		#endregion

		#region Functions

		public void UpdateLocation(LocationDescriptor location)
		{
			background0.SetBackground(location.background0);
			background1.SetBackground(location.background1);
			background2.SetBackground(location.background2);
			background3.SetBackground(location.background3);
			background4.SetBackground(location.background4);
		}

		#endregion

		#region Callbacks

		private void Generator_onTerrainChanged(object sender, MapGenerator.TerrainChangedEventArgs e)
		{
			UpdateLocation(e.location);
			Debug.Log($"Location changed to: {e.location.bPixel} {e.location.background0}");
		}

		#endregion
	}
}
