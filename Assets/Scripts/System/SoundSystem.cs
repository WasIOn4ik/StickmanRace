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
		[SerializeField] private float musicCoef = 0.5f;
		[SerializeField] private float volumeChange = 0.05f;
		private bool isSoundEnabled;

		private float maxCarAudio;

		private Coroutine carSoundCoroutine;
		private int currentMusicIndex;
		private Coroutine musicVolumeCoroutine;

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

		public void PlayButton1(bool dontDestroyOnLoad = false)
		{
			PlaySound(library.buttonSound1, dontDestroyOnLoad);
		}

		public void PlayButton2(bool dontDestroyOnLoad = false)
		{
			PlaySound(library.buttonSound2, dontDestroyOnLoad);
		}

		public void StartBackgroundMusic()
		{
			currentMusicIndex = Random.Range(0, library.backgroundMusic.Count);
			var clip = library.backgroundMusic[currentMusicIndex];

			if (musicVolumeCoroutine != null)
				StopCoroutine(musicVolumeCoroutine);

			musicVolumeCoroutine = StartCoroutine(HandleMusic(clip));
			StartCoroutine(HandleMusicChange());
		}

		private IEnumerator HandleMusic(AudioClip clip)
		{
			while (musicAudio.volume > 0)
			{
				musicAudio.volume -= 0.05f;
				yield return null;
			}

			musicAudio.clip = clip;
			musicAudio.Play();

			while (musicAudio.volume < volumeMultiplier * musicCoef)
			{
				musicAudio.volume = Mathf.MoveTowards(musicAudio.volume, volumeMultiplier * musicCoef, volumeChange);
				yield return null;
			}
		}

		private IEnumerator HandleMusicChange()
		{
			while (true)
			{
				yield return new WaitForSeconds(musicAudio.clip.length);
				currentMusicIndex++;
				if (currentMusicIndex >= library.backgroundMusic.Count)
					currentMusicIndex = 0;

				var clip = library.backgroundMusic[currentMusicIndex];
				musicAudio.clip = clip;
				musicAudio.Play();
			}
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
			float target = maxCarAudio * volumeMultiplier * 2;
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

		public void PlayMenuMusic()
		{
			if (musicVolumeCoroutine != null)
				StopCoroutine(musicVolumeCoroutine);

			musicVolumeCoroutine = StartCoroutine(HandleMusic(library.menuMusic));
		}

		public void PlayGarageMusic()
		{
			if (musicVolumeCoroutine != null)
				StopCoroutine(musicVolumeCoroutine);

			musicVolumeCoroutine = StartCoroutine(HandleMusic(library.garageMusic));
		}

		private void PlaySound(AudioClip clip, bool dontDestroyOnLoad = false)
		{
			if (!isSoundEnabled)
				return;

			AudioSource audio = (Instantiate(carAudio, transform.position, Quaternion.identity)).GetComponent<AudioSource>();

			audio.clip = clip;
			audio.volume = volumeMultiplier * 0.6f;
			audio.priority = 254;

			audio.Play();
			if (dontDestroyOnLoad)
				DontDestroyOnLoad(audio.gameObject);
			Destroy(audio.gameObject, clip.length);
		}

		#endregion
	}
}
