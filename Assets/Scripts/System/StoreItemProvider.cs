using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Purchasing;

public class StoreItemProvider
{
	public static Dictionary<string, Texture2D> Icons { get; private set; } = new();

	private static int iconsCount;

	public delegate void onLoadCompleteDelegate();
	public static event onLoadCompleteDelegate onLoadComplete;

	public static Texture2D GetIcon(string id)
	{
		if (Icons.Count == 0)
		{
			Debug.LogError("StoreItemProvider not initialized");
			throw new InvalidOperationException("StoreItemProvider.Initialize() must be called first");
		}
		else
		{
			Icons.TryGetValue(id, out var icon);
			return icon;
		}
	}

	public static void Initialize(ProductCollection products)
	{
		if (Icons.Count == 0)
		{
			iconsCount = products.all.Length;
			foreach (var product in products.all)
			{
				ResourceRequest request = Resources.LoadAsync<Texture2D>($"StoreIcons/{product.definition.id}");
				request.completed += HandleLoadedIcon;
			}
		}
		else
		{
			Debug.LogError("StoreItemProvider already initialized");
		}
	}

	public static void Initialize(string[] items)
	{
		if (Icons.Count == 0)
		{
			iconsCount = items.Length;
			foreach (var product in items)
			{
				ResourceRequest request = Resources.LoadAsync<Texture2D>($"StoreIcons/{product}");
				request.completed += HandleLoadedIcon;
			}
		}
		else
		{
			Debug.LogError("StoreItemProvider already initialized");
		}
	}

	public static bool IsInitialized()
	{
		return Icons.Count != 0;
	}

	private static void HandleLoadedIcon(AsyncOperation operation)
	{
		ResourceRequest request = operation as ResourceRequest;

		if (request.asset != null)
		{
			Icons.Add(request.asset.name, request.asset as Texture2D);

			if (Icons.Count == iconsCount)
			{
				onLoadComplete?.Invoke();
			}
		}
		Debug.Log("Shop icons loaded");
	}
}
