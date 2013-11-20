using UnityEngine;

using System;
using System.IO;
using System.Collections;

namespace Cubiquity
{
	[System.Serializable]
	public sealed class ColoredCubesVolumeData : VolumeData
	{		
		public static ColoredCubesVolumeData CreateFromVoxelDatabase(Region region, string pathToVoxelDatabase)
		{
			return CreateFromVoxelDatabase<ColoredCubesVolumeData>(region, pathToVoxelDatabase);
		}
		
		public static ColoredCubesVolumeData CreateEmptyVolumeData(Region region)
		{
			return CreateEmptyVolumeData<ColoredCubesVolumeData>(region);
		}
		
		public static ColoredCubesVolumeData CreateEmptyVolumeData(Region region, string pathToCreateVoxelDatabase)
		{
			return CreateEmptyVolumeData<ColoredCubesVolumeData>(region, pathToCreateVoxelDatabase);
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
				if(x >= region.lowerCorner.x && y >= region.lowerCorner.y && z >= region.lowerCorner.z
					&& x <= region.upperCorner.x && y <= region.upperCorner.y && z <= region.upperCorner.z)
				{						
					CubiquityDLL.SetVoxel(volumeHandle.Value, x, y, z, quantizedColor);
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
				volumeHandle = CubiquityDLL.NewColoredCubesVolume(region.lowerCorner.x, region.lowerCorner.y, region.lowerCorner.z,
					region.upperCorner.x, region.upperCorner.y, region.upperCorner.z, pathToVoxelDatabase, DefaultBaseNodeSize);
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