using UnityEngine;
using System.Collections;
using System.IO;

namespace Cubiquity
{
	public static class TerrainVolumePicking
	{
		public static bool PickTerrainSurface(TerrainVolume volume, float rayStartX, float rayStartY, float rayStartZ, float rayDirX, float rayDirY, float rayDirZ, out float resultX, out float resultY, out float resultZ)
		{
			uint hit = CubiquityDLL.PickTerrainSurface((uint)volume.data.volumeHandle, rayStartX, rayStartY, rayStartZ, rayDirX, rayDirY, rayDirZ, out resultX, out resultY, out resultZ);
			
			return hit == 1;
		}
	}
}
