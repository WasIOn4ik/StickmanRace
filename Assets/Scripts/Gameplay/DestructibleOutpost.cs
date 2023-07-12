using SR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SR.Extras
{
	public class DestructibleOutpost : Outpost
	{
		#region Variables

		[Header("DestructibleOutpost")]
		[SerializeField] private int minHeight = 1;
		[SerializeField] private int maxHeight = 3;
		[SerializeField] private int minLength = 1;
		[SerializeField] private int maxLength = 5;
		[SerializeField] private LayerMask terrainMask;
		[SerializeField] private SleepableObstacle boxToSpawn;

		private List<SleepableObstacle> components = new List<SleepableObstacle>();
		private List<Vector3> spawnPositions = new List<Vector3>();
		private float length;
		private float height;

		#endregion

		#region Functions

		public void Init(LocationDescriptorSO location)
		{
			height = Random.Range(minHeight, maxHeight);
			length = Random.Range(minLength, maxLength);

			if (length != 1)
			{
				Vector3 leftPos = transform.position + Vector3.up * (height + 0.5f) + Vector3.left * length / 2;
				Vector3 rightPos = transform.position + Vector3.up * (height + 0.5f) + Vector3.right * length / 2;

				spawnPositions.Add(leftPos + Vector3.up);
				spawnPositions.Add(rightPos + Vector3.up);

				for (int i = 0; i < height + 0.99f; i++)
				{
					var box = Instantiate(boxToSpawn, transform);
					box.Freeze();
					box.onUnsleep = Unsleep;
					components.Add(box);
					box.transform.position = leftPos + Vector3.down * i;
				}

				for (int i = 0; i < height + 0.99f; i++)
				{
					var box = Instantiate(boxToSpawn, transform);
					box.Freeze();
					box.onUnsleep = Unsleep;
					components.Add(box);
					box.transform.position = rightPos + Vector3.down * i;
				}

				for (int i = 1; i < length; i++)
				{
					var box = Instantiate(boxToSpawn, transform);
					box.Freeze();
					box.onUnsleep = Unsleep;
					components.Add(box);
					box.transform.position = leftPos + Vector3.right * i;
				}
			}
			else
			{
				var start = transform.position + Vector3.up * height;
				spawnPositions.Add(start + Vector3.up * 1.5f);

				for (int i = 0; i < height + 0.99f; i++)
				{
					var box = Instantiate(boxToSpawn, transform);
					box.Freeze();
					box.onUnsleep = Unsleep;
					components.Add(box);
					box.transform.position = start + Vector3.down * i;
				}
			}

			foreach (var sp in spawnPositions)
			{
				var enemy = Instantiate(location.availableEnemies.GetRandomEnemy(), sp, Quaternion.identity, transform);
				enemy.UnFreeze();
			}
		}

		private void Unsleep()
		{
			foreach (var so in components)
			{
				if (so)
					so.UnFreeze();
			}
		}

		public override void DestroyOutpost()
		{
			foreach (var so in components)
			{
				if (so)
					Destroy(so.gameObject);
			}
			base.DestroyOutpost();
		}

		#endregion
	}
}
