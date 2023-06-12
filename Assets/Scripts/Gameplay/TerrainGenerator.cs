using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
	#region Variables

	[Header("Components")]
	[SerializeField] private SpriteShapeController spriteShapeController;

	[Header("Properties")]
	[SerializeField] private float platformOnStartLength = 5f;
	[SerializeField] private float platformExitSmoothness = 2f;
	[SerializeField] private int controlPointsCount = 50;
	[SerializeField] private float lengthMultiplier = 2f;
	[SerializeField] private float heightMultiplier = 2f;
	[SerializeField] private float smoothness = 0.5f;
	[SerializeField] private float noiseStep = 0.5f;
	[SerializeField] private float bottom = 10f;

	private Vector3 lastPosition;

	#endregion

	#region UnityMessages

	private void OnValidate()
	{
		spriteShapeController.spline.Clear();

		Vector3 startOfGeneratedLevel = transform.position + Vector3.right * platformOnStartLength;

		spriteShapeController.spline.InsertPointAt(0, transform.position);
		spriteShapeController.spline.InsertPointAt(1, startOfGeneratedLevel);

		startOfGeneratedLevel += Vector3.right * platformExitSmoothness;

		for (int i = 0; i < controlPointsCount; i++)
		{
			lastPosition = startOfGeneratedLevel + new Vector3(i * lengthMultiplier, Mathf.PerlinNoise(0, i * noiseStep) * heightMultiplier);
			spriteShapeController.spline.InsertPointAt(i + 2, lastPosition);

			if (i != 0 && i != controlPointsCount - 1)
			{
				spriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
				spriteShapeController.spline.SetLeftTangent(i, Vector3.left * lengthMultiplier * smoothness);
				spriteShapeController.spline.SetRightTangent(i, Vector3.right * lengthMultiplier * smoothness);
			}
		}

		spriteShapeController.spline.InsertPointAt(controlPointsCount + 2, new Vector3(lastPosition.x, transform.position.y - bottom));
		spriteShapeController.spline.InsertPointAt(controlPointsCount + 3, new Vector3(transform.position.x, transform.position.y - bottom));
	}

	#endregion

	#region Functions


	#endregion
}
