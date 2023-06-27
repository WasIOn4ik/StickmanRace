using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

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

		[Header("Volumes")]
		[SerializeField] private float enemyShootVolume = 1f;
		[SerializeField] private float weaponShootVolume = 1f;
		[SerializeField] private float buttonsVolume = 0.8f;
		[SerializeField] private float carSoundMultiplier = 1.5f;
		[SerializeField] private float deathVolume = 1f;
		[SerializeField] private float obstacleDamage = 0.5f;
		[SerializeField] private float buildingDamage = 0.5f;

		private float maxCarAudio;

		private Coroutine carSoundCoroutine;
		private int currentMusicIndex;
		private Coroutine musicVolumeCoroutine;
		private bool muted = false;

		#endregion

		#region Functions

		public void Mute()
		{
			muted = true;
			musicAudio.volume = 0;
		}

		public void Unmute()
		{
			muted = false;
			musicAudio.volume = volumeMultiplier * musicCoef;
		}

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
			musicAudio.enabled = true;
			carAudio.enabled = true;
		}

		public void DisableSound()
		{
			isSoundEnabled = false;
			musicAudio.enabled = false;
			carAudio.enabled = false;
		}

		public void PlayEnemyShoot()
		{
			PlaySound(library.enemyShootSound, 50, enemyShootVolume);
		}

		public void PlayWeapon(int bullets)
		{
			if (bullets > 1)
			{
				if (!isSoundEnabled || muted)
					return;

				AudioSource audio = (Instantiate(carAudio, transform.position, Quaternion.identity)).GetComponent<AudioSource>();

				audio.clip = library.weaponMultipleSound;
				audio.volume = weaponShootVolume;
				audio.priority = 30;

				audio.Play();
				Destroy(audio.gameObject, 0.1f * bullets);
			}
			else
			{
				PlaySound(library.weaponSingleSound, 30, weaponShootVolume);
			}
		}

		public void PlayRocketLauncher()
		{
			PlaySound(library.rocketLaucherSound, 30, weaponShootVolume);
		}

		public void PlayButton1(bool dontDestroyOnLoad = false)
		{
			PlaySound(library.buttonSound1, 1, buttonsVolume, dontDestroyOnLoad);
		}

		public void PlayButton2(bool dontDestroyOnLoad = false)
		{
			PlaySound(library.buttonSound2, 1, buttonsVolume, dontDestroyOnLoad);
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
			if (!isSoundEnabled || muted)
			{
				musicAudio.volume = 0;
				yield break;
			}

			while (musicAudio.volume > 0)
			{
				musicAudio.volume -= 0.05f;
				yield return null;
			}

			if (!isSoundEnabled || muted)
			{
				musicAudio.volume = 0;
				yield break;
			}

			musicAudio.clip = clip;
			musicAudio.Play();

			while (musicAudio.volume < volumeMultiplier * musicCoef)
			{
				if (!isSoundEnabled || muted)
				{
					musicAudio.volume = 0;
					yield break;
				}
				musicAudio.volume = Mathf.MoveTowards(musicAudio.volume, volumeMultiplier * musicCoef, volumeChange);
				yield return null;
			}
		}

		private IEnumerator HandleMusicChange()
		{
			while (true)
			{
				if (musicAudio.clip != null)
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
			float target = maxCarAudio * volumeMultiplier * carSoundMultiplier;
			if (increase)
			{
				while (carAudio.volume < target)
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
			PlaySound(library.StickmanDeath, 1, deathVolume);
		}

		public void PlayBuildingDamage()
		{
			PlaySound(library.BuildingDamage, 254, buildingDamage);
		}

		public void PlayObstacleDestruction()
		{
			PlaySound(library.ObstacleDestruction, 200, obstacleDamage);
		}

		public void PlayHighVelocityDamage()
		{
			PlaySound(library.HighVelocityDamage, 253, buildingDamage);
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

		private void PlaySound(AudioClip clip, int priority, float volumeCoef = 1f, bool dontDestroyOnLoad = false)
		{
			if (!isSoundEnabled || muted)
				return;

			AudioSource audio = (Instantiate(carAudio, transform.position, Quaternion.identity)).GetComponent<AudioSource>();

			audio.clip = clip;
			audio.volume = volumeMultiplier * 0.6f * volumeCoef;
			audio.priority = priority;

			audio.Play();
			if (dontDestroyOnLoad)
				DontDestroyOnLoad(audio.gameObject);
			Destroy(audio.gameObject, clip.length);
		}

		#endregion
	}
}
