using SR.Extras;
using SR.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

namespace SR.Core
{
	public class TerrainGenerator : MonoBehaviour
	{
		#region Variables

		[Header("Components")]
		[SerializeField] private SpriteShapeController spriteShapeController;
		[SerializeField] private Renderer spriteRenderer;

		[Header("Spawn area")]
		[SerializeField] private float platformOnStartRightLength = 5f;
		[SerializeField] private float platformExitSmoothness = 2f;
		[SerializeField] private float platformLeftSideOffset = 10f;
		[Header("Outposts")]
		[SerializeField] private int outpostDelayPoints = 3;
		[SerializeField] private int outpostLength = 1;
		[SerializeField] private int outpostStartPoint = 10;
		[Header("Terain")]
		[SerializeField] private int terainControlPointsCount = 50;
		[SerializeField] private float terrainLengthMultiplier = 2f;
		[SerializeField] private float terrainHeightMultiplier = 2f;
		[SerializeField] private float terrainSmoothness = 0.5f;
		[SerializeField] private float terrainNoiseStep = 0.5f;
		[SerializeField] private float terrainBottomHeight = 10f;

		private Vector3 lastPosition;

		private List<Vector3> outpostPoints = new List<Vector3>();
		private List<Outpost> spawnedOutposts = new List<Outpost>();

		private LocationDescriptor location;

		private float difficulty = 1f;

		private List<Enemy> spawnedEnemies = new List<Enemy>();

		#endregion

		#region UnityMessages

		private void Awake()
		{
			var proto = spriteShapeController.spriteShape;
			spriteShapeController.spriteShape = Instantiate(spriteShapeController.spriteShape);
			spriteShapeController.spriteShape.name = "Spawnded_" + spriteShapeController.name;
		}

		#endregion

		#region Functions

		public LocationDescriptor GetLocation()
		{
			return location;
		}

		public float GetWorldRightBorderX()
		{
			return transform.TransformPoint(lastPosition).x;
		}

		public Vector3 GetEndpoint()
		{
			return transform.TransformPoint(lastPosition);
		}

		public void Regenerate(float difficulty, bool bRandom, LocationDescriptor location, MapGenerator generator)
		{
			StartCoroutine(HandleGeneration(difficulty, bRandom, location, generator));
		}

		public Vector3 GetCenter()
		{
			return spriteRenderer.bounds.center;
		}

		private void Clear()
		{
			spriteShapeController.spline.Clear();
			outpostPoints.Clear();
			foreach (var outpost in spawnedOutposts)
			{
				outpost.DestroyOutpost();
			}
			foreach (var enemy in spawnedEnemies)
			{
				if (enemy)
					Destroy(enemy.gameObject);
			}
			spawnedEnemies.Clear();
			spawnedOutposts.Clear();
		}

		private void UpdateVisual(LocationDescriptor location)
		{
			//EditorUtility.SetDirty(spriteShapeController.spriteShape);
			spriteShapeController.spriteShape.fillTexture = location.fillTexture;
			spriteShapeController.spriteShape.angleRanges[0].sprites[0] = location.cornerSprite;
			spriteShapeController.UpdateSpriteShapeParameters();
			//EditorUtility.SetDirty(spriteShapeController.spriteShape);
		}

		private IEnumerator HandleGeneration(float difficulty, bool bRandom, LocationDescriptor location, MapGenerator generator)
		{
			Clear();
			this.difficulty = difficulty;
			this.location = location;
			GenerateTerrain();
			yield return null;

			UpdateVisual(location);
			yield return null;

			SpawnAllOutposts(difficulty, location);
			yield return null;

			var enemiesToSpawn = location.bPixel ? generator.pixelEnemies : generator.casualEnemies;
			if (enemiesToSpawn != null)
			{
				SpawnEnemies(enemiesToSpawn);
			}
		}

		private void GenerateTerrain()
		{
			Clear();

			Vector3 startOfGeneratedLevel = Vector3.right * platformOnStartRightLength;

			int offset = 2;

			if (platformLeftSideOffset != 0)
			{
				spriteShapeController.spline.InsertPointAt(0, Vector3.left * platformLeftSideOffset);
				spriteShapeController.spline.InsertPointAt(1, startOfGeneratedLevel);
				lastPosition = startOfGeneratedLevel;
			}
			else
			{
				lastPosition = Vector3.right * terrainLengthMultiplier;
				spriteShapeController.spline.InsertPointAt(0, Vector3.zero);
				spriteShapeController.spline.InsertPointAt(1, lastPosition);
			}

			startOfGeneratedLevel = lastPosition + (Vector3.right * platformExitSmoothness) + Vector3.right * terrainLengthMultiplier;

			for (int i = 0; i < terainControlPointsCount; i++)
			{
				int outpostPoint = i % outpostDelayPoints;
				//Standart random generation
				if (outpostDelayPoints == 0 || i < outpostStartPoint || outpostPoint >= outpostLength)
				{
					lastPosition = startOfGeneratedLevel + new Vector3(i * terrainLengthMultiplier, Mathf.PerlinNoise(0, i * terrainNoiseStep) * terrainHeightMultiplier);
					spriteShapeController.spline.InsertPointAt(i + offset, lastPosition);
				}
				else // Outpost generation
				{
					float y = lastPosition.y;
					lastPosition = startOfGeneratedLevel + new Vector3(i * terrainLengthMultiplier, 0);
					lastPosition.y = y;
					if (outpostPoint == outpostLength / 2)
					{
						outpostPoints.Add(lastPosition);
					}
					spriteShapeController.spline.InsertPointAt(i + offset, lastPosition);
				}

				if (i != 0 && i != terainControlPointsCount - 1)
				{
					spriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
					spriteShapeController.spline.SetLeftTangent(i, Vector3.left * terrainLengthMultiplier * terrainSmoothness);
					spriteShapeController.spline.SetRightTangent(i, Vector3.right * terrainLengthMultiplier * terrainSmoothness);
				}
			}

			//Closing spline
			spriteShapeController.spline.InsertPointAt(terainControlPointsCount + offset, new Vector3(lastPosition.x, -terrainBottomHeight));
			spriteShapeController.spline.InsertPointAt(terainControlPointsCount + offset + 1, new Vector3(-platformLeftSideOffset, -terrainBottomHeight));
		}

		private void SpawnAllOutposts(float difficulty, LocationDescriptor location)
		{
			foreach (var point in outpostPoints)
			{
				spawnedOutposts.Add(SpawnOutpost(transform.TransformPoint(point), difficulty, location));
			}
		}

		private void SpawnEnemies(List<Enemy> enemies)
		{
			int maxPoints = spriteShapeController.spline.GetPointCount() - 2;
			int spawnDelay = Mathf.Max((int)(maxPoints / difficulty / 3), 1);
			for (int i = 0; i < maxPoints; i++)
			{
				if (i % spawnDelay == 0)
				{
					var position = transform.TransformPoint(spriteShapeController.spline.GetPosition(i));

					if (IsOutpost(position))
						continue;

					var enemy = Instantiate(enemies[Random.Range(0, enemies.Count)], position, Quaternion.identity, transform);
					enemy.SetDifficulty(difficulty);
					spawnedEnemies.Add(enemy);
				}
			}
		}

		private bool IsOutpost(Vector3 position)
		{
			var result = outpostPoints.Find(x => { return x == position; });
			return result != Vector3.zero;
		}

		private Outpost SpawnOutpost(Vector3 position, float difficulty, LocationDescriptor location)
		{
			int outpostIndex = Random.Range(0, location.availableOutposts.Count);
			var outpost = Instantiate(location.availableOutposts[outpostIndex], position, Quaternion.identity, transform);
			outpost.SetDifficulty(difficulty);
			return outpost;
		}

		#endregion
	}
}
