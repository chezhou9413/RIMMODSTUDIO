using RimWorld;
using RimWorld.Planet;
using UnityEngine;

internal static class _0024BurstDirectCallInitializer
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void Initialize()
	{
		MapGenUtility.ComputeLargestRects_0000B6C2_0024BurstDirectCall.Initialize();
		MapGenUtility.RectsComputeSpaces_0000B6C3_0024BurstDirectCall.Initialize();
		FastTileFinder.Initialize_0024ComputeQueryJob_SphericalDistance_00014E0B_0024BurstDirectCall();
		PlanetLayer.CalculateAverageTileSize_000152CA_0024BurstDirectCall.Initialize();
		PlanetLayer.IntGetTileSize_000152CC_0024BurstDirectCall.Initialize();
		PlanetLayer.IntGetTileCenter_000152CF_0024BurstDirectCall.Initialize();
	}
}
