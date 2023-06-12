using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerVehicle : MonoBehaviour
{
	#region Variables

	[Header("Components")]
	[SerializeField] private Rigidbody2D frontTireRB;
	[SerializeField] private Rigidbody2D backTireRB;
	[SerializeField] private Rigidbody2D carRB;

	[Header("Properties")]
	[SerializeField] private float velocity;
	[SerializeField] private float rotationSpeed;

	#endregion

	#region UnityMessages

	private void FixedUpdate()
	{
		float input = Input.GetAxisRaw("Horizontal");
		frontTireRB.AddTorque(-input * velocity * Time.fixedDeltaTime);
		backTireRB.AddTorque(-input * velocity * Time.fixedDeltaTime);
		carRB.AddTorque(-input * rotationSpeed * Time.fixedDeltaTime);
	}

	#endregion

	#region Functions

	#endregion

}
