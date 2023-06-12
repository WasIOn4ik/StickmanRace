using SR.Core;
using UnityEngine;
using Zenject;

namespace SR.Extras
{
	public class PlayerInstaller : MonoInstaller
	{
		[SerializeField] private PlayerVehicle playerVehicleInstance;

		public override void InstallBindings()
		{
			Container.Bind<PlayerVehicle>().FromInstance(playerVehicleInstance);
		}
	}
}