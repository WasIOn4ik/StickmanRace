using SR.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace SR.Core
{
	[ExecuteInEditMode]
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
		[SerializeField] private List<Outpost> outpostPrefabs = new List<Outpost>();
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

		#endregion

		#region UnityMessages

		private void OnValidate()
		{
			GenerateTerrain();
		}

		#endregion

		#region Functions

		public float GetWorldRightBorderX()
		{
			return transform.TransformPoint(lastPosition).x;
		}

		public Vector3 GetEndpoint()
		{
			return transform.TransformPoint(lastPosition);
		}

		public void Regenerate()
		{
			Debug.Log("Generating " + gameObject.name);
			GenerateTerrain();
			SpawnAllOutposts();
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
			spawnedOutposts.Clear();
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

					if (i != 0 && i != terainControlPointsCount - 1)
					{
						spriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
						spriteShapeController.spline.SetLeftTangent(i, Vector3.left * terrainLengthMultiplier * terrainSmoothness);
						spriteShapeController.spline.SetRightTangent(i, Vector3.right * terrainLengthMultiplier * terrainSmoothness);
					}
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
			}

			//Closing spline
			spriteShapeController.spline.InsertPointAt(terainControlPointsCount + offset, new Vector3(lastPosition.x, -terrainBottomHeight));
			spriteShapeController.spline.InsertPointAt(terainControlPointsCount + offset + 1, new Vector3(-platformLeftSideOffset, -terrainBottomHeight));
		}

		private void SpawnAllOutposts()
		{
			foreach (var point in outpostPoints)
			{
				spawnedOutposts.Add(SpawnOutpost(transform.TransformPoint(point)));
			}
		}

		private Outpost SpawnOutpost(Vector3 position)
		{
			int outpostIndex = Random.Range(0, outpostPrefabs.Count);
			return Instantiate(outpostPrefabs[outpostIndex], position, Quaternion.identity, transform);
		}

		#endregion
	}
}
