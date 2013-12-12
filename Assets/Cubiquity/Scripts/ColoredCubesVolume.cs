using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Substrate;
using Substrate.Core;
using Vector3 = UnityEngine.Vector3;
using System.Collections.Generic;
using System.Linq;
namespace Cubiquity
{
	public struct CubiquityVertex 
	{
		// Disable 'Field ... is never assigned to'
		// warnings as this structure is just for interop
		#pragma warning disable 0649
		public float x;
		public float y;
		public float z;
		public QuantizedColor color;
		#pragma warning restore 0649
	}
	
	[ExecuteInEditMode]
	public class ColoredCubesVolume : MonoBehaviour
	{	
		// The name of the dataset to load from disk.
		[SerializeField]
		private ColoredCubesVolumeData mData = null;

        #region private data types

        private static int rows = 0;
        private static int columns = 0;

        #endregion

        #region public properties

        public static int NumberOfRows
        {
            get
            {
                return rows;
            }
        }

        public static int NumberOfColumns
        {
            get
            {
                return columns;
            }
        }

        #endregion 

		public ColoredCubesVolumeData data
	    {
	        get { return this.mData; }
	    }
		
		// The name of the dataset to load from disk.
		//public string datasetName = null;
		
		// The side length of an extracted mesh for the most detailed LOD.
		// Bigger values mean fewer batches but slower surface extraction.
		// For some reason Unity won't serialize uints so it's stored as int.
		//public int baseNodeSize = 0;
		
		// Determines whether collision data is generated as well as a
		// renderable mesh. This does not apply when in the Unity editor.
		public bool UseCollisionMesh = true;
		
		// The extents (dimensions in voxels) of the volume.
		//public Region region = null;
		
		// If set, this identifies the volume to the Cubiquity DLL. It can
		// be tested against null to find if the volume is currently valid.
		//[System.NonSerialized]
		//internal uint? volumeHandle = null;
		
		// This corresponds to the root OctreeNode in Cubiquity.
		private GameObject rootGameObject;
		
		private int maxNodeSyncsPerFrame = 4;
		private int nodeSyncsThisFrame = 0;
		
		public static GameObject CreateGameObject(ColoredCubesVolumeData data)
		{
			// Warn about license restrictions.			
			Debug.LogWarning("This version of Cubiquity is for non-commercial and evaluation use only. Please see LICENSE.txt for further details.");
			
			// Make sure the Cubiquity library is installed.
			Installation.ValidateAndFix();
			
			GameObject VoxelTerrainRoot = new GameObject("Colored Cubes Volume");
			VoxelTerrainRoot.AddComponent<ColoredCubesVolume>();
			
			ColoredCubesVolume coloredCubesVolume = VoxelTerrainRoot.GetComponent<ColoredCubesVolume>();
			//terrainVolume.baseNodeSize = DefaultBaseNodeSize;
			
			coloredCubesVolume.mData = data;
			
			//terrainVolume.data.Initialize();
			
			return VoxelTerrainRoot;
		}
		
		// It seems that we need to implement this function in order to make the volume pickable in the editor.
		// It's actually the gizmo which get's picked which is often bigger than than the volume (unless all
		// voxels are solid). So somtimes the volume will be selected by clicking on apparently empty space.
		// We shold try and fix this by using raycasting to check if a voxel is under the mouse cursor?
		void OnDrawGizmos()
		{
			// Compute the size of the volume.
			int width = (data.region.upperCorner.x - data.region.lowerCorner.x) + 1;
			int height = (data.region.upperCorner.y - data.region.lowerCorner.y) + 1;
			int depth = (data.region.upperCorner.z - data.region.lowerCorner.z) + 1;
			float offsetX = width / 2;
			float offsetY = height / 2;
			float offsetZ = depth / 2;
			
			// The origin is at the centre of a voxel, but we want this box to start at the corner of the voxel.
			Vector3 halfVoxelOffset = new Vector3(0.5f, 0.5f, 0.5f);
			
			// Draw an invisible box surrounding the olume. This is what actually gets picked.
	        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
			Gizmos.DrawCube (transform.position - halfVoxelOffset + new Vector3(offsetX, offsetY, offsetZ), new Vector3 (width, height, depth));
	    }

        //==================================================================
        #region getting dynamic path of MC map
        /// <summary>
        /// gets MC map path
        /// </summary>
        public static string Dir
        {
            get
            {
                string MapName = "MCRegion";
                string path = Path.Combine(Application.streamingAssetsPath, MapName);
                return path;
            }
        }
        #endregion 


        #region AnvilWorld 

        public static AnvilWorld World
        {
            get
            {
                AnvilWorld world = AnvilWorld.Open(Dir);
                return world;
            }
        }

        #endregion 

        /// <summary>
        /// loades MC map using Substrate 
        /// </summary>
        /// <param name="data"></param>
        public static void MCSubstrate(ColoredCubesVolumeData data)
        {
            if (Dir == null)
            {
                return;
            }
            AnvilWorld world = AnvilWorld.Open(Dir);
            IChunkManager icm = world.GetChunkManager();
           // List<ChunkRef> chunks = getChunks(icm);
            CreateCubiquityRegion(rows, columns, data, icm);


        }

        #region gets existing chunks from a region

        public static List<ChunkRef> getChunks(IChunkManager icm)
        {
            List<ChunkRef> chunks = new List<ChunkRef>();

            foreach (ChunkRef chunk in icm)
            {
                chunks.Add(chunk);
            }
            return chunks;
        }

        # endregion

        #region retrives rows and columns of a given region using Substrate from MC map
        /// <summary>
        /// to get the regions row and columns
        /// </summary>
        /// <param name="world"></param>
        public static void SetRowColomuns(AnvilWorld world)
        {
            IRegionManager irm = world.GetRegionManager();
            List<int> matrices = new List<int>();
            ///iterate the regions within a Substrate world(level.dat)
            ///and it detects the number of regins. 
            foreach (IRegion ir in irm)
            {

                for (int x = 0; x < 32; x++)
                {
                    int col = 0;
                    for (int z = 0; z < 32; z++)
                    {
                        ChunkRef chunkref = ir.GetChunkRef(x, z);
                        if (chunkref == null)
                        {
                            continue;
                        }
                        //if (chunkref != null)
                        //{
                        //    Mychunk.Enqueue(chunkref);
                        //}
                        col++;

                    }
                    if (col != 0)
                        matrices.Add(col);
                }

            }
            rows = matrices.Count;
            columns = matrices.Max();
        }

        #endregion 



        /// <summary>
        /// Creates a Substrate region using Cubiquity
        /// </summary>
        /// <param name="i">gets width of the Substrate region</param>
        /// <param name="j">gets height of the Substrate region</param>
        /// <param name="data">instance of ColoredCubesVolumeData uses to call SetVoxel </param>

        public static void CreateCubiquityRegion(int rows, int Columns, ColoredCubesVolumeData data, IChunkManager icm)
        {
            List<ChunkRef> chunks = getChunks(icm);
            getChunks(icm);


            for (int j = 0; j < rows; j++)
            {
                for (int i = 0; i < columns; i++)
                {
                    ChunkRef chunk = icm.GetChunkRef(j, i);
                    DrawAchunk(data, (i + 1), (j + 1), chunk);
                }
            }
        }


        #region draw Substrate chunk to Unity using Cubiquity

        /// <summary>
        /// Draw Substrate chunk on Unity using Cubiquity
        /// </summary>
        /// <param name="data">a reference to invoke SetVexel</param>
        /// <param name="col">determines a chunk columns, for instance , col=1 first column witch start loopint at z=(16*(1-1)=0 to z=16*1=16  and 
        /// col=2 second column that starts loopint at z=(16*(2-1))=16 to z=(16*2)=32 and so on </param>
        /// <param name="row">determines a chunk rows,for instance, row=1 first row that starting looping at x=(16(1-1))=0 to x=(16*1)=16
        /// </param>
        /// <param name="chunk">a chunk reference for Substrate blocks</param>
        /// <remarks> the first row will be changed until all the columns are drawn fully in Unity </remarks>
        public static void DrawAchunk(ColoredCubesVolumeData data, int col, int row, ChunkRef chunk)
        {
            int StartColumns = 16 * (col - 1);
            int StartRows = 16 * (row - 1);
            for (int x = StartRows; x < 16 * row; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    for (int z = StartColumns; z < 16 * col; z++)
                    {
                        int color = chunk.Blocks.GetID(x % 16, y, z % 16);
                        data.SetVoxel(z, y, x, (QuantizedColor)setColor(color));
                    }

                }

            }
        }

        #endregion 



        #region color mapping
        /// <summary>
        /// Substrate to Cubiquity color mapping
        /// </summary>
        /// <param name="id">Substrate color id to be mapped</param>
        /// <returns></returns>

        public static Color32 setColor(int id)
        {
            Dictionary<int, Color32> ColorType = new Dictionary<int, Color32>();
            ColorType.Add(80, new Color32(255, 255, 255, 255));
            ColorType.Add(41, new Color32(251, 216, 79, 255));
            ColorType.Add(90, new Color32(44, 0, 107, 255));
            ColorType.Add(22, new Color32(0, 59, 175, 255));
            ColorType.Add(133, new Color32(4, 253, 0, 255));
            if (ColorType.ContainsKey(id))
            {
                return ColorType[id];
            }
            return new Color32(0, 0, 0, 0);
        }

        #endregion
	//=======================================	
		/*internal void Initialize()
		{	
			// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
			// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.
			if(volumeHandle == null)
			{				
				if(region != null)
				{
					// Create an empty region of the desired size.
					volumeHandle = CubiquityDLL.NewColoredCubesVolume(region.lowerCorner.x, region.lowerCorner.y, region.lowerCorner.z,
						region.upperCorner.x, region.upperCorner.y, region.upperCorner.z,  datasetName, (uint)baseNodeSize);
				}
			}
		}
		
		internal void InitializeFromHeightmap(string heightmapFileName, string colormapFileName)
		{
			// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
			// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.
			if(volumeHandle == null)
			{
				// Ask Cubiquity to create a volume from the VolDat data.
				volumeHandle = CubiquityDLL.NewColoredCubesVolumeFromHeightmap(heightmapFileName, colormapFileName, datasetName, (uint)baseNodeSize);
				
				// The user didn't specify a region as this is determined by the size of
				// the VolDat data, so we have to pull this information back from Cubiquity.
				int lowerX, lowerY, lowerZ, upperX, upperY, upperZ;
				CubiquityDLL.GetEnclosingRegion(volumeHandle.Value, out lowerX, out lowerY, out lowerZ, out upperX, out upperY, out upperZ);
				region = new Region(lowerX, lowerY, lowerZ, upperX, upperY, upperZ);
			}
		}
		
		internal void InitializeFromVoldat(string voldatFolder)
		{
			// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
			// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.
			if(volumeHandle == null)
			{
				// Ask Cubiquity to create a volume from the VolDat data.
				volumeHandle = CubiquityDLL.NewColoredCubesVolumeFromVolDat(voldatFolder, datasetName, (uint)baseNodeSize);
				
				// The user didn't specify a region as this is determined by the size of
				// the VolDat data, so we have to pull this information back from Cubiquity.
				int lowerX, lowerY, lowerZ, upperX, upperY, upperZ;
				CubiquityDLL.GetEnclosingRegion(volumeHandle.Value, out lowerX, out lowerY, out lowerZ, out upperX, out upperY, out upperZ);
				region = new Region(lowerX, lowerY, lowerZ, upperX, upperY, upperZ);
			}
		}*/
		
		public void Synchronize()
		{
			nodeSyncsThisFrame = 0;
			
			if(data.volumeHandle.HasValue)
			{
				CubiquityDLL.UpdateVolume(data.volumeHandle.Value);
				
				if(CubiquityDLL.HasRootOctreeNode(data.volumeHandle.Value) == 1)
				{		
					uint rootNodeHandle = CubiquityDLL.GetRootOctreeNode(data.volumeHandle.Value);
					if(rootGameObject == null)
					{					
						rootGameObject = BuildGameObjectFromNodeHandle(rootNodeHandle, null);	
					}
					syncNode(rootNodeHandle, rootGameObject);
				}
			}
		}
		
		/*public void Shutdown(bool saveChanges)
		{
			Debug.Log("In ColoredCubesVolume.Shutdown()");
			
			if(volumeHandle.HasValue)
			{			
				if(saveChanges)
				{
					CubiquityDLL.AcceptOverrideBlocks(volumeHandle.Value);
				}
				CubiquityDLL.DiscardOverrideBlocks(volumeHandle.Value);
				
				CubiquityDLL.DeleteColoredCubesVolume(volumeHandle.Value);
				volumeHandle = null;
				
				// Game objects in our tree are created with the 'DontSave' flag set, and according to the Unity docs this means
				// we have to destroy them manually. In the case of 'Destroy' the Unity docs explicitally say that it will destroy
				// transform children as well, so I'm assuming DestroyImmediate has the same behaviour.
				DestroyImmediate(rootGameObject);
			}
		}*/
		
		void OnEnable()
		{
			Debug.Log ("ColoredCubesVolume.OnEnable()");
			//Initialize();
		}
		
		// Use this for initialization
		/*void Start()
		{		
			
		}*/
		
		// Update is called once per frame
		void Update()
		{
			Synchronize();
		}
		
		public void OnDisable()
		{
			Debug.Log ("ColoredCubesVolume.OnDisable()");
			
			// Game objects in our tree are created with the 'DontSave' flag set, and according to the Unity docs this means
			// we have to destroy them manually. In the case of 'Destroy' the Unity docs explicitally say that it will destroy
			// transform children as well, so I'm assuming DestroyImmediate has the same behaviour.
			DestroyImmediate(rootGameObject);
			
			// We only save if we are in editor mode, not if we are playing.
			//bool saveChanges = !Application.isPlaying;
			
			//Shutdown(saveChanges);
		}
		
		public void syncNode(uint nodeHandle, GameObject gameObjectToSync)
		{
			if(nodeSyncsThisFrame >= maxNodeSyncsPerFrame)
			{
				return;
			}
			
			uint meshLastUpdated = CubiquityDLL.GetMeshLastUpdated(nodeHandle);		
			OctreeNodeData octreeNodeData = (OctreeNodeData)(gameObjectToSync.GetComponent<OctreeNodeData>());
			
			if(octreeNodeData.meshLastSyncronised < meshLastUpdated)
			{			
				if(CubiquityDLL.NodeHasMesh(nodeHandle) == 1)
				{				
					Mesh renderingMesh;
					Mesh physicsMesh;
					
					BuildMeshFromNodeHandle(nodeHandle, out renderingMesh, out physicsMesh);
			
			        MeshFilter mf = (MeshFilter)gameObjectToSync.GetComponent(typeof(MeshFilter));
			        MeshRenderer mr = (MeshRenderer)gameObjectToSync.GetComponent(typeof(MeshRenderer));
					
					if(mf.sharedMesh != null)
					{
						DestroyImmediate(mf.sharedMesh);
					}
					
			        mf.sharedMesh = renderingMesh;				
					
					mr.material = new Material(Shader.Find("ColoredCubesVolume"));
					
					if(UseCollisionMesh)
					{
						MeshCollider mc = (MeshCollider)gameObjectToSync.GetComponent(typeof(MeshCollider));
						mc.sharedMesh = physicsMesh;
					}
				}
				
				uint currentTime = CubiquityDLL.GetCurrentTime();
				octreeNodeData.meshLastSyncronised = (int)(currentTime);
				
				nodeSyncsThisFrame++;
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
							
							GameObject childGameObject = octreeNodeData.GetChild(x,y,z);
							
							if(childGameObject == null)
							{							
								childGameObject = BuildGameObjectFromNodeHandle(childNodeHandle, gameObjectToSync);
								
								octreeNodeData.SetChild(x, y, z, childGameObject);
							}
							
							syncNode(childNodeHandle, childGameObject);
						}
					}
				}
			}
		}
		
		GameObject BuildGameObjectFromNodeHandle(uint nodeHandle, GameObject parentGameObject)
		{
			int xPos, yPos, zPos;
			//Debug.Log("Getting position for node handle = " + nodeHandle);
			CubiquityDLL.GetNodePosition(nodeHandle, out xPos, out yPos, out zPos);
			
			StringBuilder name = new StringBuilder("(" + xPos + ", " + yPos + ", " + zPos + ")");
			
			GameObject newGameObject = new GameObject(name.ToString ());
			newGameObject.AddComponent<OctreeNodeData>();
			newGameObject.AddComponent<MeshFilter>();
			newGameObject.AddComponent<MeshRenderer>();
			newGameObject.AddComponent<MeshCollider>();
			
			OctreeNodeData octreeNodeData = newGameObject.GetComponent<OctreeNodeData>();
			octreeNodeData.lowerCorner = new Vector3(xPos, yPos, zPos);
			
			if(parentGameObject)
			{
				newGameObject.transform.parent = parentGameObject.transform;				
				Vector3 parentLowerCorner = parentGameObject.GetComponent<OctreeNodeData>().lowerCorner;
				newGameObject.transform.localPosition = octreeNodeData.lowerCorner - parentLowerCorner;
			}
			else
			{
				newGameObject.transform.localPosition = octreeNodeData.lowerCorner;
			}
			
			newGameObject.hideFlags = HideFlags.HideAndDontSave;
			
			return newGameObject;
		}
		
		void BuildMeshFromNodeHandle(uint nodeHandle, out Mesh renderingMesh, out Mesh physicsMesh)
		{
			// At some point I should read this: http://forum.unity3d.com/threads/5687-C-plugin-pass-arrays-from-C
			
			// Create rendering and possible collision meshes.
			renderingMesh = new Mesh();		
			physicsMesh = UseCollisionMesh ? new Mesh() : null;
			
			// Get the data from Cubiquity.
			int[] indices = CubiquityDLL.GetIndices(nodeHandle);		
			CubiquityVertex[] cubiquityVertices = CubiquityDLL.GetVertices(nodeHandle);			
			
			// Create the arrays which we'll copy the data to.
	        Vector3[] renderingVertices = new Vector3[cubiquityVertices.Length];	
			Color32[] renderingColors = new Color32[cubiquityVertices.Length];
			Vector3[] physicsVertices = UseCollisionMesh ? new Vector3[cubiquityVertices.Length] : null;
			
			for(int ct = 0; ct < cubiquityVertices.Length; ct++)
			{
				// Get the vertex data from Cubiquity.
				Vector3 position = new Vector3(cubiquityVertices[ct].x, cubiquityVertices[ct].y, cubiquityVertices[ct].z);
				QuantizedColor color = cubiquityVertices[ct].color;
					
				// Copy it to the arrays.
				renderingVertices[ct] = position;
				renderingColors[ct] = (Color32)color;
				
				if(UseCollisionMesh)
				{
					physicsVertices[ct] = position;
				}
			}
			
			// Assign vertex data to the meshes.
			renderingMesh.vertices = renderingVertices; 
			renderingMesh.colors32 = renderingColors;
			renderingMesh.triangles = indices;
			
			// FIXME - Get proper bounds
			renderingMesh.bounds = new Bounds(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(500.0f, 500.0f, 500.0f));
			
			if(UseCollisionMesh)
			{
				physicsMesh.vertices = physicsVertices;
				physicsMesh.triangles = indices;
			}
		}
	}
}
