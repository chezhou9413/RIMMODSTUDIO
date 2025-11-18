using System.Collections.Generic;
using UnityEngine;

namespace RimWorld.Planet;

public static class WorldTerrainColliderManager
{
	private static readonly Dictionary<PlanetLayer, Dictionary<int, GameObject>> layerWorldTerrainColliders = new Dictionary<PlanetLayer, Dictionary<int, GameObject>>();

	public static void ClearCache()
	{
		PlanetLayer planetLayer = default(PlanetLayer);
		Dictionary<int, GameObject> dictionary = default(Dictionary<int, GameObject>);
		int num = default(int);
		GameObject obj = default(GameObject);
		foreach (KeyValuePair<PlanetLayer, Dictionary<int, GameObject>> layerWorldTerrainCollider in layerWorldTerrainColliders)
		{
			layerWorldTerrainCollider.Deconstruct(ref planetLayer, ref dictionary);
			foreach (KeyValuePair<int, GameObject> item in dictionary)
			{
				item.Deconstruct(ref num, ref obj);
				Object.Destroy(obj);
			}
		}
		layerWorldTerrainColliders.Clear();
	}

	public static void EnsureRaycastCollidersUpdated()
	{
		PlanetLayer planetLayer = default(PlanetLayer);
		Dictionary<int, GameObject> dictionary = default(Dictionary<int, GameObject>);
		int num = default(int);
		GameObject gameObject = default(GameObject);
		foreach (KeyValuePair<PlanetLayer, Dictionary<int, GameObject>> layerWorldTerrainCollider in layerWorldTerrainColliders)
		{
			layerWorldTerrainCollider.Deconstruct(ref planetLayer, ref dictionary);
			PlanetLayer planetLayer2 = planetLayer;
			foreach (KeyValuePair<int, GameObject> item in dictionary)
			{
				item.Deconstruct(ref num, ref gameObject);
				gameObject.SetActive(planetLayer2.Raycastable);
			}
		}
	}

	private static GameObject CreateGameObject(PlanetLayer planetLayer, int layer)
	{
		GameObject gameObject = new GameObject($"{planetLayer} WorldTerrainCollider layer {layer}");
		Object.DontDestroyOnLoad(gameObject);
		gameObject.layer = layer;
		return gameObject;
	}

	public static GameObject Get(PlanetLayer planetLayer, int layer)
	{
		if (!layerWorldTerrainColliders.TryGetValue(planetLayer, out var value))
		{
			value = (layerWorldTerrainColliders[planetLayer] = new Dictionary<int, GameObject>());
		}
		if (!value.TryGetValue(layer, out var value2))
		{
			value2 = (value[layer] = CreateGameObject(planetLayer, layer));
		}
		value2.SetActive(value: false);
		return value2;
	}
}
