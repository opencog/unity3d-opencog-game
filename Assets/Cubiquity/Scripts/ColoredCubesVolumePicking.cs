using UnityEngine;
using System.Collections;
using System.IO;

namespace Cubiquity
{
	public static class ColoredCubesVolumePicking
	{
		public static bool PickFirstSolidVoxel(ColoredCubesVolume volume, float rayStartX, float rayStartY, float rayStartZ, float rayDirX, float rayDirY, float rayDirZ, out int resultX, out int resultY, out int resultZ)
		{
			uint hit = CubiquityDLL.PickFirstSolidVoxel((uint)volume.data.volumeHandle, rayStartX, rayStartY, rayStartZ, rayDirX, rayDirY, rayDirZ, out resultX, out resultY, out resultZ);
			
			return hit == 1;
		}
		
		public static bool PickLastEmptyVoxel(ColoredCubesVolume volume, float rayStartX, float rayStartY, float rayStartZ, float rayDirX, float rayDirY, float rayDirZ, out int resultX, out int resultY, out int resultZ)
		{
			uint hit = CubiquityDLL.PickLastEmptyVoxel((uint)volume.data.volumeHandle, rayStartX, rayStartY, rayStartZ, rayDirX, rayDirY, rayDirZ, out resultX, out resultY, out resultZ);
			
			return hit == 1;
		}
	}
}
