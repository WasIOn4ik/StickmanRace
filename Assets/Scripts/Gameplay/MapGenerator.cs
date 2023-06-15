using SR.Core;
using SR.Extras;
using SR.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MapGenerator : MonoBehaviour
{
	#region HelperClasses

	public class TerrainChangedEventArgs : EventArgs
	{
		public LocationDescriptor location;
	}

	#endregion

	#region Variables

	public event EventHandler<TerrainChangedEventArgs> onTerrainChanged;

	[Header("Components")]
	[SerializeField] private TerrainGenerator terrainGeneratorPrefab;
	[SerializeField] private TerrainGenerator startTerrain;

	[Header("Properties")]
	[SerializeField] private bool bRandom = false;
	[SerializeField] private List<LocationDescriptor> locations;
	public List<Outpost> pixelOutposts = new List<Outpost>();
	public List<Outpost> casualOutposts = new List<Outpost>();

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
			activeTerrain = value;
			onTerrainChanged?.Invoke(this, new TerrainChangedEventArgs() { location = activeTerrain.GetLocation() });
		}
	}

	#endregion

	#region UnityMessages

	private void Start()
	{
		startTerrain.Regenerate(gameplayBase.GetDifficulty(), bRandom, GetRandomLocation());
		var tempTerrain = Instantiate(terrainGeneratorPrefab, startTerrain.GetEndpoint(), Quaternion.identity, transform);
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
				var tempTerrain = Instantiate(terrainGeneratorPrefab, terrainsCache.Peek().GetEndpoint(), Quaternion.identity, transform);
				tempTerrain.Regenerate(gameplayBase.GetDifficulty(), bRandom, GetRandomLocation());
				terrainsCache.Enqueue(tempTerrain);

				var tempTerrain2 = Instantiate(terrainGeneratorPrefab, tempTerrain.GetEndpoint(), Quaternion.identity, transform);
				tempTerrain.Regenerate(gameplayBase.GetDifficulty(), bRandom, GetRandomLocation());
				terrainsCache.Enqueue(tempTerrain2);

				ActiveTerrain = terrainsCache.Dequeue();
				Destroy(startTerrain);
				Debug.Log("Switch start");
			}
		}
		else if (playerVehicle.transform.position.x > ActiveTerrain.GetWorldRightBorderX())
		{
			Debug.LogWarning($"{ActiveTerrain.name}");
			terrainsCache.Enqueue(ActiveTerrain);

			var tempTerrain = terrainsCache.Dequeue();
			tempTerrain.transform.position = ActiveTerrain.GetEndpoint();
			ActiveTerrain = tempTerrain;

			var tempTerrain2 = terrainsCache.Peek();
			tempTerrain2.transform.position = ActiveTerrain.GetEndpoint();
			tempTerrain2.Regenerate(gameplayBase.GetDifficulty(), bRandom, GetRandomLocation());

			Debug.Log($"Switch endless: {playerVehicle.transform.position.x} > {ActiveTerrain.GetWorldRightBorderX()}");
			Debug.Log($"Camera: {ActiveTerrain.GetCenter()}");
		}

		playerVehicle.UpdateCameraFollow(ActiveTerrain.GetCenter());
	}

	#endregion

	#region Functions

	private LocationDescriptor GetRandomLocation()
	{

		int index = UnityEngine.Random.Range(0, locations.Count);
		//Background repeat peotection
		while (index == currentLocationId)
		{
			index = UnityEngine.Random.Range(0, locations.Count);
		}

		currentLocationId = index;
		Debug.Log("Generated:" + currentLocationId);

		var location = locations[index];
		if (location.bUseDefault)
		{
			location.availableOutposts = location.bPixel ? pixelOutposts : casualOutposts;
		}
		return location;
	}

	#endregion
}
