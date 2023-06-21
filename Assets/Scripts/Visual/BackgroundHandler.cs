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
		[SerializeField] private ParallaxController firstParallax;
		[SerializeField] private ParallaxController secondParallax;

		private ParallaxController activeParallaxController;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			generator.onTerrainChanged += Generator_onTerrainChanged;
			activeParallaxController = firstParallax;
			secondParallax.gameObject.SetActive(false);
		}

		#endregion

		#region Functions

		public void UpdateLocation(LocationDescriptorSO location)
		{
			activeParallaxController.Fade();
			activeParallaxController = activeParallaxController == firstParallax ? secondParallax : firstParallax;
			activeParallaxController.SetLocation(location);
			activeParallaxController.Unfade();
		}

		#endregion

		#region Callbacks

		private void Generator_onTerrainChanged(object sender, MapGenerator.TerrainChangedEventArgs e)
		{
			UpdateLocation(e.location);
		}

		#endregion
	}
}
