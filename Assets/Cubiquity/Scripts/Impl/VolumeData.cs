using UnityEngine;

using System;
using System.IO;
using System.Collections;

namespace Cubiquity
{
	[System.Serializable]
	public abstract class VolumeData : ScriptableObject
	{		
		// We need to explicitly serialize the private field because
		// Unity3D doesn't automatically serialize the public property
		[SerializeField]
		protected Region _region;
	    public Region region
	    {
	        get { return this._region; }
	    }
		
		// If set, this identifies the volume to the Cubiquity DLL. It can
		// be tested against null to find if the volume is currently valid.
		[System.NonSerialized] // Internal variables aren't serialized anyway?
		internal uint? volumeHandle = null;
		
		[SerializeField]
		protected string pathToVoxelDatabase;
		
		// Don't really like having this defined here. The base node size should be a rendering property rather than a
		// property of the actual volume data. Need to make this change in the underlying Cubiquity library as well though.
		protected static uint DefaultBaseNodeSize = 32;
		
		// We use a static Random for making filenames, as Randoms are seeded by timestamp
		// and client code could potentially create a number of volumes on quick sucession.  
		protected static System.Random randomIntGenerator = new System.Random();
		
		protected static VolumeDataType CreateFromVoxelDatabase<VolumeDataType>(Region region, string pathToVoxelDatabase) where VolumeDataType : VolumeData
		{
			if(!File.Exists(pathToVoxelDatabase))
			{
				throw new FileNotFoundException("Voxel database '" + pathToVoxelDatabase + "' does not exist (or you do not have the required permissions)");
			}
			
			VolumeDataType volumeData = ScriptableObject.CreateInstance<VolumeDataType>();
			volumeData._region = region;
			volumeData.pathToVoxelDatabase = pathToVoxelDatabase;
			
			volumeData.InitializeCubiquityVolume();
			
			return volumeData;
		}
		
		protected static VolumeDataType CreateEmptyVolumeData<VolumeDataType>(Region region) where VolumeDataType : VolumeData
		{
			string pathToCreateVoxelDatabase = GeneratePathToVoxelDatabase();
			return CreateEmptyVolumeData<VolumeDataType>(region, pathToCreateVoxelDatabase);
		}
		
		protected static VolumeDataType CreateEmptyVolumeData<VolumeDataType>(Region region, string pathToCreateVoxelDatabase) where VolumeDataType : VolumeData
		{
			if(File.Exists(pathToCreateVoxelDatabase))
			{
				throw new FileNotFoundException("Voxel database '" + pathToCreateVoxelDatabase + "' already exists. Please choose a different filename.");
			}
			
			VolumeDataType volumeData = ScriptableObject.CreateInstance<VolumeDataType>();
			volumeData._region = region;
			volumeData.pathToVoxelDatabase = pathToCreateVoxelDatabase;
			
			volumeData.InitializeCubiquityVolume();
			
			return volumeData;
		}
		
		private void OnEnable()
		{			
			InitializeCubiquityVolume();
		}
		
		private void OnDisable()
		{
			ShutdownCubiquityVolume();
		}
		
		private void OnDestroy()
		{
			ShutdownCubiquityVolume();
		}
		
		protected abstract void InitializeCubiquityVolume();
		protected abstract void ShutdownCubiquityVolume();
		
		protected static string GeneratePathToVoxelDatabase()
		{
			// Generate a random filename from an integer
			string filename = randomIntGenerator.Next().ToString("X8") + ".vol";
			return Application.streamingAssetsPath + Path.DirectorySeparatorChar + filename;
		}
	}
}
