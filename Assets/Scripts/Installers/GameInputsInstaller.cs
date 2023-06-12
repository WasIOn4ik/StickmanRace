using SR.Core;
using UnityEngine;
using Zenject;

namespace SR.Extras
{
	public class GameInputsInstaller : MonoInstaller
	{
		[SerializeField] private GameInputs gameInputsPrefab;

		public override void InstallBindings()
		{
			Container.Bind<GameInputs>().FromComponentInNewPrefab(gameInputsPrefab).AsSingle().NonLazy();
		}
	}
}