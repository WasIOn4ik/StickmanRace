using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SR.Core
{
	public class SoundSystem : MonoBehaviour
	{
		#region Variables

		[SerializeField] private SoundsLibrary library;
		[SerializeField] private AudioSource musicAudio;
		[SerializeField] private AudioSource carAudio;
		[SerializeField] private float carSoundChangeCoef;
		[SerializeField, Range(0.1f, 1f)] private float volumeMultiplier;
		private bool isSoundEnabled;

		private float maxCarAudio;

		Coroutine carSoundCoroutine;

		#endregion

		#region Functions

		public void SetMaxCarSound(float sound)
		{
			maxCarAudio = sound;
		}

		public bool IsSoundEnabled()
		{
			return isSoundEnabled;
		}

		public void EnableSound()
		{
			isSoundEnabled = true;
		}

		public void DisableSound()
		{
			isSoundEnabled = false;
		}

		public void PlayButton1()
		{
			PlaySound(library.buttonSound1);
		}

		public void PlayButton2()
		{
			PlaySound(library.buttonSound2);
		}

		public void PlayCarMovement(bool active)
		{
			if (!isSoundEnabled)
				return;

			if (active)
			{
				if (carSoundCoroutine != null)
					StopCoroutine(carSoundCoroutine);
				carSoundCoroutine = StartCoroutine(HandleCarSound(true));
				carAudio.Play();
			}
			else
			{
				if (carSoundCoroutine != null)
					StopCoroutine(carSoundCoroutine);
				carSoundCoroutine = StartCoroutine(HandleCarSound(false));
				carAudio.Stop();
			}
		}

		private IEnumerator HandleCarSound(bool increase)
		{
			float target = maxCarAudio * volumeMultiplier;
			if (increase)
			{
				while (true)
				{
					carAudio.volume = Mathf.MoveTowards(carAudio.volume, target, carSoundChangeCoef);
					yield return null;
				}
			}
			else
			{
				while (carAudio.volume > 0)
				{
					carAudio.volume = Mathf.MoveTowards(carAudio.volume, 0, carSoundChangeCoef);
					yield return null;
				}
			}
		}

		public void PlayDeath()
		{
			PlaySound(library.StickmanDeath);
		}

		public void PlayBuildingDamage()
		{
			PlaySound(library.BuildingDamage);
		}

		public void PlayObstacleDestruction()
		{
			PlaySound(library.ObstacleDestruction);
		}

		public void PlayHighVelocityDamage()
		{
			PlaySound(library.HighVelocityDamage);
		}

		private void PlaySound(AudioClip clip)
		{
			if (!isSoundEnabled)
				return;

			AudioSource audio = (Instantiate(carAudio, transform.position, Quaternion.identity)).GetComponent<AudioSource>();

			audio.clip = clip;
			audio.volume = volumeMultiplier;

			audio.Play();

			Destroy(audio.gameObject, clip.length);
		}

		#endregion
	}
}
