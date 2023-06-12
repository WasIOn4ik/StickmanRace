using UnityEngine;
using Cinemachine;

namespace SR.Extras
{
	/// <summary>
	/// An add-on module for Cinemachine Virtual Camera that locks the camera's Z co-ordinate
	/// </summary>
	[ExecuteInEditMode]
	[SaveDuringPlay]
	[AddComponentMenu("")] // Hide in menu
	public class LockCameraAxis : CinemachineExtension
	{
		[SerializeField] private float YPosition;

		protected override void PostPipelineStageCallback(
			CinemachineVirtualCameraBase vcam,
			CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage == CinemachineCore.Stage.Body)
			{
				var pos = state.RawPosition;
				pos.y = Mathf.Min(YPosition, pos.y);
				state.RawPosition = pos;
			}
		}
	}
}