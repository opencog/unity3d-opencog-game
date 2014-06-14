using UnityEngine;

using System;
using System.IO;
using System.Collections;

namespace Cubiquity
{
	[System.Serializable]
	public sealed class ColoredCubesVolumeData : VolumeData
	{		
		public static ColoredCubesVolumeData CreateFromVoxelDatabase(Paths basePath, string relativePathToVoxelDatabase)
		{
			return CreateFromVoxelDatabase<ColoredCubesVolumeData>(basePath, relativePathToVoxelDatabase);
		}
		
		public static ColoredCubesVolumeData CreateEmptyVolumeData(Region region)
		{
			return CreateEmptyVolumeData<ColoredCubesVolumeData>(region);
		}
		
		public static ColoredCubesVolumeData CreateEmptyVolumeData(Region region, Paths basePath, string relativePathToVoxelDatabase)
		{
			return CreateEmptyVolumeData<ColoredCubesVolumeData>(region, basePath, relativePathToVoxelDatabase);
		}
		
		public QuantizedColor GetVoxel(int x, int y, int z)
		{
			QuantizedColor result;
			if(volumeHandle.HasValue)
			{
				CubiquityDLL.GetVoxel(volumeHandle.Value, x, y, z, out result);
			}
			else
			{
				//Should maybe throw instead.
				result = new QuantizedColor();
			}
			return result;
		}
		
		public void SetVoxel(int x, int y, int z, QuantizedColor quantizedColor)
		{
			if(volumeHandle.HasValue)
			{
				if(x >= enclosingRegion.lowerCorner.x && y >= enclosingRegion.lowerCorner.y && z >= enclosingRegion.lowerCorner.z
					&& x <= enclosingRegion.upperCorner.x && y <= enclosingRegion.upperCorner.y && z <= enclosingRegion.upperCorner.z)
				{						
					CubiquityDLL.SetVoxel(volumeHandle.Value, x, y, z, quantizedColor);
				}
			}
		}
		
		protected override void InitializeEmptyCubiquityVolume(Region region)
		{				
			// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
			// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.
			if(volumeHandle == null)
			{
				// Create an empty region of the desired size.
				volumeHandle = CubiquityDLL.NewEmptyColoredCubesVolume(region.lowerCorner.x, region.lowerCorner.y, region.lowerCorner.z,
					region.upperCorner.x, region.upperCorner.y, region.upperCorner.z, fullPathToVoxelDatabase, DefaultBaseNodeSize);
			}
		}

		protected override void InitializeExistingCubiquityVolume()
		{				
			// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
			// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.
			if(volumeHandle == null)
			{
				// Create an empty region of the desired size.
				volumeHandle = CubiquityDLL.NewColoredCubesVolumeFromVDB(fullPathToVoxelDatabase, DefaultBaseNodeSize);
			}
		}
		
		protected override void ShutdownCubiquityVolume()
		{
			if(volumeHandle.HasValue)
			{
				// We only save if we are in editor mode, not if we are playing.
				bool saveChanges = !Application.isPlaying;
				
				if(saveChanges)
				{
					CubiquityDLL.AcceptOverrideBlocks(volumeHandle.Value);
				}
				CubiquityDLL.DiscardOverrideBlocks(volumeHandle.Value);
				
				CubiquityDLL.DeleteColoredCubesVolume(volumeHandle.Value);
				volumeHandle = null;
			}
		}
	}
}