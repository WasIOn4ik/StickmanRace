using CAS.AdObject;
using SR.Core;
using SR.Extras;
using SR.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;
using Zenject;

public class MapGenerator : MonoBehaviour
{
	#region HelperClasses

	public class TerrainChangedEventArgs : EventArgs
	{
		public LocationDescriptorSO location;
	}

	#endregion

	#region Variables

	public event EventHandler<TerrainChangedEventArgs> onTerrainChanged;

	[Header("Components")]
	[SerializeField] private TerrainGenerator terrainGeneratorPrefab;
	[SerializeField] private TerrainGenerator startTerrain;

	[Header("Properties")]
	[SerializeField] private bool bRandom = false;
	[SerializeField] private List<LocationDescriptorSO> locations;
	[SerializeField] private float deativationTimer = 3f;

	[Inject] private PlayerVehicle playerVehicle;
	[Inject] private GameplayBase gameplayBase;

	private Queue<TerrainGenerator> terrainsCache = new Queue<TerrainGenerator>();

	private TerrainGenerator activeTerrain;
	private int currentLocationId = -1;

	public TerrainGenerator ActiveTerrain
	{
		get => activeTerrain;
		set
		{
			if (activeTerrain != null)
				activeTerrain.Invoke("Deactivate", deativationTimer);
			activeTerrain = value;

			terrainsCache.Peek().Activate();
			onTerrainChanged?.Invoke(this, new TerrainChangedEventArgs() { location = activeTerrain.GetLocation() });
		}
	}

	#endregion

	#region UnityMessages

	private void Start()
	{
		gameplayBase.onGameStarted += GameplayBase_onGameStarted;
		//Creating start segment
		startTerrain.Regenerate(gameplayBase.GetDifficulty(), bRandom, GetRandomLocation());

		//Creating the first segment
		var tempTerrain = Instantiate(terrainGeneratorPrefab, startTerrain.GetEndpoint(), Quaternion.identity, transform);
		tempTerrain.name = "FirstSpawned";
		tempTerrain.Regenerate(gameplayBase.GetDifficulty(), bRandom, GetRandomLocation());
		terrainsCache.Enqueue(tempTerrain);

		ActiveTerrain = startTerrain;
	}

	private void Update()
	{
		if (startTerrain != null)
		{
			if (playerVehicle.transform.position.x > startTerrain.GetWorldRightBorderX() && ActiveTerrain == startTerrain)
			{
				//Creating the second segment
				var tempTerrain = Instantiate(terrainGeneratorPrefab, terrainsCache.Peek().GetEndpoint(), Quaternion.identity, transform);
				tempTerrain.name = "SecondSpawned";
				tempTerrain.Regenerate(gameplayBase.GetDifficulty(), bRandom, GetRandomLocation());
				terrainsCache.Enqueue(tempTerrain);/*

				var tempTerrain2 = Instantiate(terrainGeneratorPrefab, tempTerrain.GetEndpoint(), Quaternion.identity, transform);
				tempTerrain.name = "ThirdSpawned";
				tempTerrain2.Regenerate(gameplayBase.GetDifficulty(), bRandom, GetRandomLocation());
				terrainsCache.Enqueue(tempTerrain2);
*/
				ActiveTerrain = terrainsCache.Dequeue();
				Destroy(startTerrain.gameObject, deativationTimer);
			}
		}
		else if (playerVehicle.transform.position.x > ActiveTerrain.GetWorldRightBorderX())
		{
			if (terrainsCache.Count < 2)
			{
				var tempTerrain = Instantiate(terrainGeneratorPrefab, terrainsCache.Peek().GetEndpoint(), Quaternion.identity, transform);
				tempTerrain.name = "ThirdSpawned";
				tempTerrain.Regenerate(gameplayBase.GetDifficulty(), bRandom, GetRandomLocation());
				terrainsCache.Enqueue(tempTerrain);
				terrainsCache.Enqueue(ActiveTerrain);
				ActiveTerrain = terrainsCache.Dequeue();
			}
			else
			{
				terrainsCache.Enqueue(ActiveTerrain);

				var tempTerrain = terrainsCache.Dequeue();
				tempTerrain.transform.position = ActiveTerrain.GetEndpoint();
				ActiveTerrain = tempTerrain;

				var tempTerrain2 = terrainsCache.Peek();
				tempTerrain2.transform.position = ActiveTerrain.GetEndpoint();
				tempTerrain2.Regenerate(gameplayBase.GetDifficulty(), bRandom, GetRandomLocation());
			}
		}

		playerVehicle.UpdateCameraFollow(ActiveTerrain.GetCenter());
	}

	#endregion

	#region Functions

	private bool IsPlayerNotAliveOrADsActive()
	{
#if UNITY_ANDROID
		return GameInstance.isInterstitialActive || GameInstance.isRewardedActive || !playerVehicle.IsAlive();
#elif UNITY_WEBGL
		return YandexGame.nowFullAd || YandexGame.nowVideoAd || !playerVehicle.IsAlive();
#endif
	}

	private LocationDescriptorSO GetRandomLocation()
	{

		int index = UnityEngine.Random.Range(0, locations.Count);
		//Background repeat peotection
		while (index == currentLocationId)
		{
			index = UnityEngine.Random.Range(0, locations.Count);
		}

		currentLocationId = index;

		var location = locations[index];
		return location;
	}

#endregion

	#region Coroutines

	private IEnumerator HandleIdle()
	{
		float velocityThreshold = 5f;
		float idleTimerThreshold = 20f;
		float idleTimer = 0f;

		while (true)
		{
			if(IsPlayerNotAliveOrADsActive())
			{
				idleTimer = 0;
				yield return null;
			}
			if (playerVehicle.GetVelocity() < velocityThreshold)
			{
				idleTimer += Time.deltaTime;
			}
			else
			{
				idleTimer = 0;
			}
			if (idleTimer > idleTimerThreshold)
			{
				while (playerVehicle.GetVelocity() < velocityThreshold)
				{
					if (IsPlayerNotAliveOrADsActive())
					{
						idleTimer = 0;
						break;
					}
					yield return new WaitForSeconds(0.9f);
					Instantiate(locations[currentLocationId].availableEnemies.GetRandomEnemy(), playerVehicle.transform.position
						+ new Vector3(3f, 2f, 0f), Quaternion.identity, ActiveTerrain.transform);
				}
				idleTimer = 0;
			}
			yield return null;
		}
	}

#endregion

	#region Callbacks

	private Coroutine handleIdleCoroutine;

	private void GameplayBase_onGameStarted(object sender, EventArgs e)
	{
		handleIdleCoroutine = StartCoroutine(HandleIdle());
	}

	#endregion
}
