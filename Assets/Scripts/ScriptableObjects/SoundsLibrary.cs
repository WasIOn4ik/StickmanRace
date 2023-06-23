using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Core
{
	[CreateAssetMenu(menuName = "SR/SoundsLibrary", fileName = "SoundsLibrary")]
	public class SoundsLibrary : ScriptableObject
	{
		[SerializeField] public AudioClip buttonSound1;
		[SerializeField] public AudioClip buttonSound2;
		[SerializeField] public AudioClip CarMovement;
		[SerializeField] public AudioClip BuildingDamage;
		[SerializeField] public AudioClip HighVelocityDamage;
		[SerializeField] public AudioClip ObstacleDestruction;
		[SerializeField] public AudioClip StickmanDeath;
		[SerializeField] public List<AudioClip> backgroundMusic;
		[SerializeField] public AudioClip menuMusic;
		[SerializeField] public AudioClip garageMusic;
	}
}
