using SR.Extras;
using SR.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using Zenject;

namespace SR.Core
{
	public enum TerrainCreationType
	{
		All,
		OnlyTerrain,
		OnlyTerrainAndOutposts,
		OnlyTerrainAndSeparated
	}
	public class TerrainGenerator : MonoBehaviour
	{
		#region Variables

		[Header("Components")]
		[SerializeField] private SpriteShapeController spriteShapeController;
		[SerializeField] private Renderer spriteRenderer;
		[SerializeField] private Transform rampPrefab;

		[Header("Spawn area")]
		[SerializeField] private float platformOnStartRightLength = 5f;
		[SerializeField] private float platformExitSmoothness = 2f;
		[SerializeField] private float platformLeftSideOffset = 10f;
		[Header("Outposts")]
		[SerializeField] private int outpostDelayPoints = 3;
		[SerializeField] private int outpostLength = 1;
		[SerializeField] private int outpostStartPoint = 10;
		[SerializeField] private DestructibleOutpost destructibleOutpostPrefab;
		[Header("Terain")]
		[SerializeField] private TerrainCreationType creationType;
		[SerializeField] private int terainControlPointsCount = 50;
		[SerializeField] private float terrainLengthMultiplier = 2f;
		[SerializeField] private float terrainHeightMultiplier = 2f;
		[SerializeField] private float terrainSmoothness = 0.5f;
		[SerializeField] private float terrainNoiseStep = 0.5f;
		[SerializeField] private float terrainBottomHeight = 10f;

		private Vector3 lastPosition;

		private List<int> outpostPoints = new List<int>();
		private List<int> emptyPoints = new List<int>();

		private LocationDescriptorSO location;

		private float difficulty = 1f;

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

		public void Deactivate()
		{
			gameObject.SetActive(false);
		}

		public void Activate()
		{
			gameObject.SetActive(true);
		}

		public LocationDescriptorSO GetLocation()
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

		public void Regenerate(float difficulty, bool bRandom, LocationDescriptorSO location)
		{
			StartCoroutine(HandleGeneration(difficulty, bRandom, location));
		}

		public Vector3 GetCenter()
		{
			return spriteRenderer.bounds.center;
		}

		private void Clear()
		{
			spriteShapeController.spline.Clear();
			outpostPoints.Clear();
			outpostPoints.Clear();
			emptyPoints.Clear();
			transform.Clear();
		}

		private void UpdateVisual(LocationDescriptorSO location)
		{
			spriteShapeController.spriteShape.fillTexture = location.fillTexture;
			spriteShapeController.spriteShape.angleRanges[0].sprites[0] = location.cornerSprite;
			spriteShapeController.UpdateSpriteShapeParameters();
		}

		private IEnumerator HandleGeneration(float difficulty, bool bRandom, LocationDescriptorSO location)
		{
			Clear();
			this.difficulty = difficulty;
			this.location = location;
			GenerateTerrain();
			yield return null;

			UpdateVisual(location);
			yield return null;

			if (creationType == TerrainCreationType.All || creationType == TerrainCreationType.OnlyTerrainAndOutposts)
			{
				yield return SpawnAllOutposts(difficulty, location);
				yield return null;
			}

			if (creationType == TerrainCreationType.All || creationType == TerrainCreationType.OnlyTerrainAndSeparated)
			{
				yield return SpawnEnemies();
				yield return null;

				yield return SpawnObstacles();
			}
		}

		private float GetLength()
		{
			return terainControlPointsCount * terrainLengthMultiplier;
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
					emptyPoints.Add(i);
				}
				else // Outpost generation
				{
					float y = lastPosition.y;
					lastPosition = startOfGeneratedLevel + new Vector3(i * terrainLengthMultiplier, 0);
					lastPosition.y = y;
					if (outpostPoint == outpostLength / 2 + 1)
					{
						outpostPoints.Add(i);
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

		private IEnumerator SpawnAllOutposts(float difficulty, LocationDescriptorSO location)
		{
			int count = outpostPoints.Count;
			for (int i = 0; i < count; i++)
			{
				SpawnOutpost(transform.TransformPoint(spriteShapeController.spline.GetPosition(outpostPoints[i])), difficulty, location);
				yield return null;
			}
		}

		private IEnumerator SpawnEnemies()
		{
			int numOfSegment = (int)(transform.position.x / GetLength());
			int rampPos = Random.Range(2, emptyPoints.Count);
			int spawnDelay = Mathf.Max((int)(emptyPoints.Count / difficulty / 8), 1);
			for (int i = 2; i < emptyPoints.Count; i++)
			{
				if (numOfSegment % 2 == 1 && rampPos == i)
				{
					Instantiate(rampPrefab, transform.TransformPoint(
						spriteShapeController.spline.GetPosition(emptyPoints[i])), Quaternion.identity, transform);
					continue;
				}
				if (i % spawnDelay != 0)
				{
					continue;
				}

				bool bDestructibleOutpost = Random.Range(0, 4) < 2;

				if (bDestructibleOutpost)
				{
					i++;
					if (i < emptyPoints.Count)
					{
						var dOutpost = Instantiate(destructibleOutpostPrefab,
							transform.TransformPoint(spriteShapeController.spline.GetPosition(emptyPoints[i])), Quaternion.identity, transform);
						dOutpost.Init(location);
					}
					i++;
				}
				else
				{
					var enemy = Instantiate(location.availableEnemies.GetRandomEnemy(), transform.TransformPoint
						(spriteShapeController.spline.GetPosition(emptyPoints[i])), Quaternion.identity, transform);
					enemy.SetDifficulty(difficulty);
					if (i % 5 == 0)
						yield return null;
				}
			}
		}

		private IEnumerator SpawnObstacles()
		{
			for (int i = 0; i < 10; i++)
			{
				int rand = Random.Range(0, 1);
				Vector3 offset = Vector3.left * 0.5f + Vector3.right * rand;
				int randomPoint = Random.Range(1, spriteShapeController.spline.GetPointCount() - 2);
				var obstacle = Instantiate(location.availableObstacles.GetRandomObstacle(), transform.TransformPoint(spriteShapeController.spline.GetPosition(randomPoint) + Vector3.up + offset), Quaternion.identity, transform);
				if (i % 5 == 0)
					yield return null;
			}
		}

		private Outpost SpawnOutpost(Vector3 position, float difficulty, LocationDescriptorSO location)
		{
			var outpost = Instantiate(location.availableOutposts.GetRandomOutpost(), position, Quaternion.identity, transform);
			ProjectContext.Instance.Container.Inject(outpost);
			outpost.SetDifficulty(difficulty);
			return outpost;
		}

		#endregion
	}
}
