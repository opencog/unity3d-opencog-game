using UnityEngine;

using System;
using System.IO;
using System.Collections;

namespace Cubiquity
{
	[System.Serializable]
	public sealed class TerrainVolumeData : VolumeData
	{
		public static TerrainVolumeData CreateFromVoxelDatabase(Region region, string pathToVoxelDatabase)
		{
			return CreateFromVoxelDatabase<TerrainVolumeData>(region, pathToVoxelDatabase);
		}
		
		public static TerrainVolumeData CreateEmptyVolumeData(Region region)
		{
			return CreateEmptyVolumeData<TerrainVolumeData>(region);
		}
		
		public static TerrainVolumeData CreateEmptyVolumeData(Region region, string pathToCreateVoxelDatabase)
		{
			return CreateEmptyVolumeData<TerrainVolumeData>(region, pathToCreateVoxelDatabase);
		}
		
		public MaterialSet GetVoxel(int x, int y, int z)
		{
			MaterialSet materialSet;
			if(volumeHandle.HasValue)
			{
				CubiquityDLL.GetVoxelMC(volumeHandle.Value, x, y, z, out materialSet);
			}
			else
			{
				// Should maybe throw instead?
				materialSet = new MaterialSet();
			}
			return materialSet;
		}
		
		public void SetVoxel(int x, int y, int z, MaterialSet materialSet)
		{
			if(volumeHandle.HasValue)
			{
				if(x >= region.lowerCorner.x && y >= region.lowerCorner.y && z >= region.lowerCorner.z
					&& x <= region.upperCorner.x && y <= region.upperCorner.y && z <= region.upperCorner.z)
				{						
					CubiquityDLL.SetVoxelMC(volumeHandle.Value, x, y, z, materialSet);
				}
			}
		}

		protected override void InitializeCubiquityVolume()
		{	
			// Make sure the Cubiquity library is installed.
			Installation.ValidateAndFix();
			
			// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
			// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.
			if((volumeHandle == null) && (_region != null))
			{
				// Create an empty region of the desired size.
				volumeHandle = CubiquityDLL.NewTerrainVolume(region.lowerCorner.x, region.lowerCorner.y, region.lowerCorner.z,
					region.upperCorner.x, region.upperCorner.y, region.upperCorner.z, pathToVoxelDatabase, DefaultBaseNodeSize, 0, 0);
			}
		}
		
		protected override void ShutdownCubiquityVolume()
		{
			// Shutdown could get called multiple times. E.g by OnDisable() and then by OnDestroy().
			if(volumeHandle.HasValue)
			{
				// We only save if we are in editor mode, not if we are playing.
				bool saveChanges = !Application.isPlaying;
				
				if(saveChanges)
				{
					CubiquityDLL.AcceptOverrideBlocksMC(volumeHandle.Value);
				}
				CubiquityDLL.DiscardOverrideBlocksMC(volumeHandle.Value);
				
				CubiquityDLL.DeleteTerrainVolume(volumeHandle.Value);
				volumeHandle = null;
			}
		}
	}
}