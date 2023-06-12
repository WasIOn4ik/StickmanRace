using UnityEngine;
using Zenject;

public class GameInstanceInstaller : MonoInstaller
{
	[SerializeField] private GameInstance gameInstancePrefab;

	public override void InstallBindings()
	{
		Container.Bind<GameInstance>().FromComponentInNewPrefab(gameInstancePrefab).AsSingle().NonLazy();
	}
}