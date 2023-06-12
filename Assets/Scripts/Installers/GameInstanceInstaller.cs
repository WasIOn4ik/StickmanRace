using SR.Core;
using UnityEngine;
using Zenject;

namespace SR.Extras
{
	public class GameInstanceInstaller : MonoInstaller
	{
		[SerializeField] private GameInstance gameInstancePrefab;

		public override void InstallBindings()
		{
			Container.Bind<GameInstance>().FromComponentInNewPrefab(gameInstancePrefab).AsSingle().NonLazy();
		}
	}
}