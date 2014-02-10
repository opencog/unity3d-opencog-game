using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Text;

namespace Cubiquity
{
	public class OctreeNode : MonoBehaviour
	{
		[System.NonSerialized]
		public uint meshLastSyncronised;
		[System.NonSerialized]
		public Vector3 lowerCorner;
		[System.NonSerialized]
		public GameObject[,,] children;
		
		[System.NonSerialized]
		public uint nodeHandle;
		
		public static GameObject CreateOctreeNode(uint nodeHandle, GameObject parentGameObject)
		{			
			int xPos, yPos, zPos;
			//Debug.Log("Getting position for node handle = " + nodeHandle);
			CubiquityDLL.GetNodePosition(nodeHandle, out xPos, out yPos, out zPos);
			
			StringBuilder name = new StringBuilder("(" + xPos + ", " + yPos + ", " + zPos + ")");
			
			GameObject newGameObject = new GameObject(name.ToString ());
			newGameObject.AddComponent<OctreeNode>();
			
			OctreeNode octreeNode = newGameObject.GetComponent<OctreeNode>();
			octreeNode.lowerCorner = new Vector3(xPos, yPos, zPos);
			octreeNode.nodeHandle = nodeHandle;
			
			if(parentGameObject)
			{
				newGameObject.layer = parentGameObject.layer;
					
				newGameObject.transform.parent = parentGameObject.transform;
				newGameObject.transform.localPosition = new Vector3();
				newGameObject.transform.localRotation = new Quaternion();
				newGameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				
				OctreeNode parentOctreeNode = parentGameObject.GetComponent<OctreeNode>();
				
				if(parentOctreeNode != null)
				{
					Vector3 parentLowerCorner = parentOctreeNode.lowerCorner;
					newGameObject.transform.localPosition = octreeNode.lowerCorner - parentLowerCorner;
				}
				else
				{
					newGameObject.transform.localPosition = octreeNode.lowerCorner;
				}
			}
			else
			{
				newGameObject.transform.localPosition = octreeNode.lowerCorner;
			}
			
			newGameObject.hideFlags = HideFlags.HideInHierarchy;
			
			return newGameObject;
		}
		
		public void syncNode(ref int availableNodeSyncs, GameObject voxelTerrainGameObject)
		{
			if(availableNodeSyncs <= 0)
			{
				return;
			}
			
			uint meshLastUpdated = CubiquityDLL.GetMeshLastUpdated(nodeHandle);		
			
			if(meshLastSyncronised < meshLastUpdated)
			{			
				if(CubiquityDLL.NodeHasMesh(nodeHandle) == 1)
				{					
					// Set up the rendering mesh
					VolumeRenderer volumeRenderer = voxelTerrainGameObject.GetComponent<VolumeRenderer>();
					if(volumeRenderer != null)
					{						
						Mesh renderingMesh = volumeRenderer.BuildMeshFromNodeHandle(nodeHandle);
				
				        MeshFilter meshFilter = gameObject.GetOrAddComponent<MeshFilter>() as MeshFilter;
				        MeshRenderer meshRenderer = gameObject.GetOrAddComponent<MeshRenderer>() as MeshRenderer;
						
						if(meshFilter.sharedMesh != null)
						{
							DestroyImmediate(meshFilter.sharedMesh);
						}
						
				        meshFilter.sharedMesh = renderingMesh;				
						
						meshRenderer.sharedMaterial = volumeRenderer.material;
						
						#if UNITY_EDITOR
						EditorUtility.SetSelectedWireframeHidden(meshRenderer, true);
						#endif
					}
					
					// Set up the collision mesh
					VolumeCollider volumeCollider = voxelTerrainGameObject.GetComponent<VolumeCollider>();					
					if((volumeCollider != null) && (Application.isPlaying))
					{
						Mesh collisionMesh = volumeCollider.BuildMeshFromNodeHandle(nodeHandle);
						MeshCollider meshCollider = gameObject.GetOrAddComponent<MeshCollider>() as MeshCollider;
						meshCollider.sharedMesh = collisionMesh;
					}
				}
				// If there is no mesh in Cubiquity then we make sure there isn't on in Unity.
				else
				{
					MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>() as MeshCollider;
					if(meshCollider)
					{
						DestroyImmediate(meshCollider);
					}
					
					MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>() as MeshRenderer;
					if(meshRenderer)
					{
						DestroyImmediate(meshRenderer);
					}
					
					MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>() as MeshFilter;
					if(meshFilter)
					{
						DestroyImmediate(meshFilter);
					}
				}
				
				meshLastSyncronised = CubiquityDLL.GetCurrentTime();
				availableNodeSyncs--;
			}		
			
			//Now syncronise any children
			for(uint z = 0; z < 2; z++)
			{
				for(uint y = 0; y < 2; y++)
				{
					for(uint x = 0; x < 2; x++)
					{
						if(CubiquityDLL.HasChildNode(nodeHandle, x, y, z) == 1)
						{					
						
							uint childNodeHandle = CubiquityDLL.GetChildNode(nodeHandle, x, y, z);					
							
							GameObject childGameObject = GetChild(x,y,z);
							
							if(childGameObject == null)
							{							
								childGameObject = OctreeNode.CreateOctreeNode(childNodeHandle, gameObject);
								
								SetChild(x, y, z, childGameObject);
							}
							
							//syncNode(childNodeHandle, childGameObject);
							
							OctreeNode childOctreeNode = childGameObject.GetComponent<OctreeNode>();
							childOctreeNode.syncNode(ref availableNodeSyncs, voxelTerrainGameObject);
						}
					}
				}
			}
		}
		
		public GameObject GetChild(uint x, uint y, uint z)
		{
			if(children != null)
			{
				return children[x, y, z];
			}
			else
			{
				return null;
			}
		}
		
		public void SetChild(uint x, uint y, uint z, GameObject gameObject)
		{
			if(children == null)
			{
				children = new GameObject[2, 2, 2];
			}
			
			children[x, y, z] = gameObject;
		}
	}
}
