using UnityEngine;
using System.Collections;
using System.IO;

namespace Cubiquity
{
	public static class TerrainVolumeGenerator
	{
		public static void GenerateFloor(TerrainVolumeData volumeData, int lowerLayerHeight, uint lowerLayerMaterial, int upperLayerHeight, uint upperLayerMaterial)
		{
			CubiquityDLL.GenerateFloor(volumeData.volumeHandle.Value, lowerLayerHeight, lowerLayerMaterial, upperLayerHeight, upperLayerMaterial);
		}
	}
}