using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Cubiquity
{	
	[ExecuteInEditMode]
	public class TerrainVolume : Volume
	{		
		// The name of the dataset to load from disk.
		[SerializeField]
		private TerrainVolumeData mData = null;
		public TerrainVolumeData data
	    {
	        get { return this.mData; }
			set { this.mData = value; RequestFlushInternalData(); }
	    }
		
		public static GameObject CreateGameObject(TerrainVolumeData data)
		{
			// Create our main game object representing the volume.
			GameObject terrainVolumeGameObject = new GameObject("Terrain Volume");
			
			//Add the requied components.
			TerrainVolume terrainVolume = terrainVolumeGameObject.GetOrAddComponent<TerrainVolume>();
			terrainVolumeGameObject.AddComponent<TerrainVolumeRenderer>();
			terrainVolumeGameObject.AddComponent<TerrainVolumeCollider>();
			
			// Set the provided data.
			terrainVolume.mData = data;
			
			return terrainVolumeGameObject;
		}
		
		// It seems that we need to implement this function in order to make the volume pickable in the editor.
		// It's actually the gizmo which get's picked which is often bigger than than the volume (unless all
		// voxels are solid). So somtimes the volume will be selected by clicking on apparently empty space.
		// We shold try and fix this by using raycasting to check if a voxel is under the mouse cursor?
		void OnDrawGizmos()
		{
			// If there's no data then we don't need to (and can't?) draw the gizmos
			if(data != null)
			{
				// Compute the size of the volume.
				int width = (data.enclosingRegion.upperCorner.x - data.enclosingRegion.lowerCorner.x) + 1;
				int height = (data.enclosingRegion.upperCorner.y - data.enclosingRegion.lowerCorner.y) + 1;
				int depth = (data.enclosingRegion.upperCorner.z - data.enclosingRegion.lowerCorner.z) + 1;
				float offsetX = width / 2;
				float offsetY = height / 2;
				float offsetZ = depth / 2;
				
				// The origin is at the centre of a voxel, but we want this box to start at the corner of the voxel.
				Vector3 halfVoxelOffset = new Vector3(0.5f, 0.5f, 0.5f);
				
				// Draw an invisible box surrounding the volume. This is what actually gets picked.
		        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
				Gizmos.DrawCube (transform.position - halfVoxelOffset + new Vector3(offsetX, offsetY, offsetZ), new Vector3 (width, height, depth));
			}
	    }
		
		public override void Synchronize()
		{
			base.Synchronize();
			
			// Syncronize the mesh data.
			if(data != null)
			{
				if(data.volumeHandle.HasValue)
				{
					CubiquityDLL.UpdateVolumeMC(data.volumeHandle.Value);
					
					if(CubiquityDLL.HasRootOctreeNodeMC(data.volumeHandle.Value) == 1)
					{		
						uint rootNodeHandle = CubiquityDLL.GetRootOctreeNodeMC(data.volumeHandle.Value);
						
						if(rootOctreeNodeGameObject == null)
						{
							rootOctreeNodeGameObject = OctreeNode.CreateOctreeNode(rootNodeHandle, gameObject);	
						}
						
						OctreeNode rootOctreeNode = rootOctreeNodeGameObject.GetComponent<OctreeNode>();
						int copyOfMaxNodePerSync = maxNodesPerSync;
						rootOctreeNode.syncNode(ref copyOfMaxNodePerSync, gameObject);
					}
				}
			}
		}
	}
}
