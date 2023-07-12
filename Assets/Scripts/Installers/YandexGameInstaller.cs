using SR.Core;
using UnityEngine;
using YG;
using Zenject;

public class YandexGameInstaller : MonoInstaller
{
	[SerializeField] private YandexGame soundSystemPrefab;
	public override void InstallBindings()
	{
		Container.Bind<YandexGame>().FromInstance(soundSystemPrefab);
	}
}
