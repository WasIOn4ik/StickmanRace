using SR.Core;
using SR.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MapGenerator : MonoBehaviour
{
	#region Variables

	[Header("Components")]
	[SerializeField] private TerrainGenerator terrainGeneratorPrefab;
	[SerializeField] private TerrainGenerator startTerrain;

	[Header("Properties")]
	[SerializeField] private bool bRandom = false;

	[Inject] private PlayerVehicle playerVehicle;
	[Inject] private GameplayBase gameplayBase;

	private Queue<TerrainGenerator> terrainsCache = new Queue<TerrainGenerator>();

	private TerrainGenerator activeTerrain;

	#endregion

	#region UnityMessages

	private void Awake()
	{
		startTerrain.Regenerate(gameplayBase.GetDifficulty(), bRandom);
		var tempTerrain = Instantiate(terrainGeneratorPrefab, startTerrain.GetEndpoint(), Quaternion.identity, transform);
		tempTerrain.Regenerate(gameplayBase.GetDifficulty(), bRandom);
		terrainsCache.Enqueue(tempTerrain);
		activeTerrain = startTerrain;
	}

	private void Update()
	{
		if (startTerrain != null)
		{
			if (playerVehicle.transform.position.x > startTerrain.GetWorldRightBorderX() && activeTerrain == startTerrain)
			{
				var tempTerrain = Instantiate(terrainGeneratorPrefab, terrainsCache.Peek().GetEndpoint(), Quaternion.identity, transform);
				tempTerrain.Regenerate(gameplayBase.GetDifficulty(), bRandom);
				terrainsCache.Enqueue(tempTerrain);

				var tempTerrain2 = Instantiate(terrainGeneratorPrefab, tempTerrain.GetEndpoint(), Quaternion.identity, transform);
				tempTerrain.Regenerate(gameplayBase.GetDifficulty(), bRandom);
				terrainsCache.Enqueue(tempTerrain2);

				activeTerrain = terrainsCache.Dequeue();
				Destroy(startTerrain);
				Debug.Log("Switch start");
			}
		}
		else if (playerVehicle.transform.position.x > activeTerrain.GetWorldRightBorderX())
		{
			Debug.LogWarning($"{activeTerrain.name}");
			terrainsCache.Enqueue(activeTerrain);

			var tempTerrain = terrainsCache.Dequeue();
			tempTerrain.transform.position = activeTerrain.GetEndpoint();
			activeTerrain = tempTerrain;

			var tempTerrain2 = terrainsCache.Peek();
			tempTerrain2.transform.position = activeTerrain.GetEndpoint();
			tempTerrain2.Regenerate(gameplayBase.GetDifficulty(), bRandom);

			Debug.Log($"Switch endless: {playerVehicle.transform.position.x} > {activeTerrain.GetWorldRightBorderX()}");
			Debug.Log($"Camera: {activeTerrain.GetCenter()}");
		}

		playerVehicle.UpdateCameraFollow(activeTerrain.GetCenter());
	}

	#endregion

	#region Functions



	#endregion
}
