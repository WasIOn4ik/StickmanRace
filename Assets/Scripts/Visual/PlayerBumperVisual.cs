using SR.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	public class PlayerBumperVisual : MonoBehaviour
	{
		#region Variables

		private const string ANIMATION = "BumperAnimation";

		[SerializeField] private SpriteRenderer staticBumper;
		[SerializeField] private SpriteRenderer part1;
		[SerializeField] private SpriteRenderer part2;
		[SerializeField] private SpriteRenderer part3;
		[SerializeField] private SpriteRenderer part4;
		[SerializeField] private SpriteRenderer part5;
		[SerializeField] private Animator animator;

		#endregion

		#region Functions

		public void InitBumper(BumperSO bumper)
		{
			if (bumper.animatorOverride != null)
			{
				staticBumper.gameObject.SetActive(false);
				animator.runtimeAnimatorController = bumper.animatorOverride;
				animator.Play(ANIMATION);
			}
			else
			{
				staticBumper.gameObject.SetActive(true);
				staticBumper.sprite = bumper.sprite;
				animator.StopPlayback();
				animator.runtimeAnimatorController = null;
			}
		}

		#endregion

	}
}
