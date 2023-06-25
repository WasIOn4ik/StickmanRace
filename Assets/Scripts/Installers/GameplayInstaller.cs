using SR.UI;
using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
	[SerializeField] private GameplayBase gameplayInstance;

	public override void InstallBindings()
	{
		Container.Bind<GameplayBase>().FromInstance(gameplayInstance);
	}
}