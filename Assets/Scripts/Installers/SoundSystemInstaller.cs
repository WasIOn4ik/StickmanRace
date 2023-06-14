using SR.Core;
using UnityEngine;
using Zenject;

namespace SR.Core
{
	public class SoundSystemInstaller : MonoInstaller
	{
		[SerializeField] private SoundSystem soundSystemPrefab;
		public override void InstallBindings()
		{
			Container.Bind<SoundSystem>().FromComponentInNewPrefab(soundSystemPrefab).AsSingle().NonLazy();
		}
	}
}