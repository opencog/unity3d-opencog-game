using UnityEngine;
using System.Collections;
using System.IO;

namespace Cubiquity
{
	public static class TerrainVolumeEditor
	{
		public static void SculptTerrainVolume(TerrainVolume volume, float centerX, float centerY, float centerZ, float brushInnerRadius, float brushOuterRadius, float amount)
		{
			CubiquityDLL.SculptTerrainVolume((uint)volume.data.volumeHandle, centerX, centerY, centerZ, brushInnerRadius, brushOuterRadius, amount);
		}
		
		public static void BlurTerrainVolume(TerrainVolume volume, float centerX, float centerY, float centerZ, float brushInnerRadius, float brushOuterRadius, float amount)
		{
			CubiquityDLL.BlurTerrainVolume((uint)volume.data.volumeHandle, centerX, centerY, centerZ, brushInnerRadius, brushOuterRadius, amount);
		}
		
		public static void BlurTerrainVolume(TerrainVolume volume, Region region)
		{
			CubiquityDLL.BlurTerrainVolumeRegion((uint)volume.data.volumeHandle, region.lowerCorner.x, region.lowerCorner.y, region.lowerCorner.z, region.upperCorner.x, region.upperCorner.y, region.upperCorner.z);
		}
		
		public static void PaintTerrainVolume(TerrainVolume volume, float centerX, float centerY, float centerZ, float brushInnerRadius, float brushOuterRadius, float amount, uint materialIndex)
		{
			CubiquityDLL.PaintTerrainVolume((uint)volume.data.volumeHandle, centerX, centerY, centerZ, brushInnerRadius, brushOuterRadius, amount, materialIndex);
		}
		
		/*public static void CreateCuboid(TerrainVolume volume, Region region, MaterialSet materialSet)
		{
			for(int z = region.lowerCorner.z; z <= region.upperCorner.z; z++)
			{
				for(int y = region.lowerCorner.y; y <= region.upperCorner.y; y++)
				{
					for(int x = region.lowerCorner.x; x <= region.upperCorner.x; x++)
					{
						volume.data.SetVoxel(x, y, z, materialSet);
					}
				}
			}
		}*/
	}
}
