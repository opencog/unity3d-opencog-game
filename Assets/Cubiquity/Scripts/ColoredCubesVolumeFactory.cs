using UnityEngine;
using System.Collections;
using System.IO;

/*namespace Cubiquity
{
	public class ColoredCubesVolumeFactory
	{
		private static uint DefaultBaseNodeSize = 32;
		
		public static GameObject CreateVolume(string name, Region region, string datasetName)
		{
			return CreateVolume (name, region, datasetName, DefaultBaseNodeSize);
		}
		
		public static GameObject CreateVolume(string name, Region region, string datasetName, uint baseNodeSize)
		{		
			// Make sure the Cubiquity library is installed.
			Installation.ValidateAndFix();
			
			GameObject VoxelTerrainRoot = new GameObject(name);
			VoxelTerrainRoot.AddComponent<ColoredCubesVolume>();
			
			ColoredCubesVolume coloredCubesVolume = VoxelTerrainRoot.GetComponent<ColoredCubesVolume>();
			coloredCubesVolume.region = region;
			coloredCubesVolume.baseNodeSize = (int)baseNodeSize;
			coloredCubesVolume.datasetName = datasetName;
			
			coloredCubesVolume.Initialize();
			
			return VoxelTerrainRoot;
		}
		
		public static GameObject CreateVolumeFromVolDat(string name, string voldatFolder, string datasetName)
		{
			return CreateVolumeFromVolDat(name, voldatFolder, datasetName, DefaultBaseNodeSize);
		}
		
		public static GameObject CreateVolumeFromVolDat(string name, string voldatFolder, string datasetName, uint baseNodeSize)
		{		
			// Make sure the Cubiquity library is installed.
			Installation.ValidateAndFix();
			
			GameObject VoxelTerrainRoot = new GameObject(name);
			VoxelTerrainRoot.AddComponent<ColoredCubesVolume>();
			
			ColoredCubesVolume coloredCubesVolume = VoxelTerrainRoot.GetComponent<ColoredCubesVolume>();
			coloredCubesVolume.baseNodeSize = (int)baseNodeSize;
			coloredCubesVolume.datasetName = datasetName;
			
			coloredCubesVolume.InitializeFromVoldat(voldatFolder);
			
			return VoxelTerrainRoot;
		}
		
		public static GameObject CreateVolumeFromHeightmap(string name, string heightmapFileName, string colormapFileName, string datasetName)
		{
			return CreateVolumeFromHeightmap(name, heightmapFileName, colormapFileName, datasetName, DefaultBaseNodeSize);
		}
		
		public static GameObject CreateVolumeFromHeightmap(string name, string heightmapFileName, string colormapFileName, string datasetName, uint baseNodeSize)
		{		
			// Make sure the Cubiquity library is installed.
			Installation.ValidateAndFix();
			
			GameObject VoxelTerrainRoot = new GameObject(name);
			VoxelTerrainRoot.AddComponent<ColoredCubesVolume>();
			
			ColoredCubesVolume coloredCubesVolume = VoxelTerrainRoot.GetComponent<ColoredCubesVolume>();
			coloredCubesVolume.baseNodeSize = (int)baseNodeSize;
			coloredCubesVolume.datasetName = datasetName;
			
			coloredCubesVolume.InitializeFromHeightmap(heightmapFileName, colormapFileName);
			
			return VoxelTerrainRoot;
		}
	}
}*/