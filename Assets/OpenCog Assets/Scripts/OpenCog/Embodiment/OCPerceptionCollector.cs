
/// Unity3D OpenCog World Embodiment Program
/// Copyright (C) 2013  Novamente			
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as
/// published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#region Usings, Namespaces, and Pragmas

using System.Collections;
using System.Collections.Generic;
using OpenCog.Attributes;
using OpenCog.Extensions;
using OpenCog.Network;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Embodiment
{

/// <summary>
/// The OpenCog OCPerceptionCollector.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
	public class OCPerceptionCollector : OCMonoBehaviour
{
	//---------------------------------------------------------------------------
	
		#region Private Member Data
	
	//---------------------------------------------------------------------------
		
	private float m_updatePerceptionInterval = 0.5f; // Percept 5 times per second.
	private float m_timer = 0.0f; // Reset timer at the end of every interval.
	private OCConnector m_connector; // The OCConnector instance used to send map-info.
	private int m_id; // A local copy of the game object id that this component attached to.
	private Dictionary<int, OCObjectMapInfo> m_mapInfoCache = new Dictionary<int, OCObjectMapInfo>();
	
	private Dictionary<int, bool> m_mapInfoCacheStatus = new Dictionary<int, bool>(); // A flag map indicates if a cached map info has been percepted in latest cycle.
	private Dictionary<StateChangesRegister.StateInfo, System.Object> m_stateInfoCache = new Dictionary<StateChangesRegister.StateInfo, System.Object>();
	
	private ArrayList m_statesToDelete = new ArrayList();
	
	private System.Object m_cacheLock = new System.Object();
	
	private List<OCObjectMapInfo> m_removedObjects = new List<OCObjectMapInfo>(); // A list of objects recently removed. This is a temporary data structure, cleared whenever it is processed.
	private bool m_perceptWorldFirstTime = true;

	//private WorldData m_worldData; // Reference to the world data.
	private Map m_map;
	
	private Dictionary<string, bool> m_chunkStatusMap = new Dictionary<string, bool>(); // A map to mark if current chunk needs to be percepted. True means perception in need.
	private int m_floorHeight; // Currently, just percept the block above the horizon.
	private bool m_hasPerceivedTerrainForFirstTime = false;
	
	private bool m_perceptStateChangesFirstTime = true;
			
	//---------------------------------------------------------------------------
	
		#endregion
	
	//---------------------------------------------------------------------------
	
		#region Accessors and Mutators
	
	//---------------------------------------------------------------------------
				
	//---------------------------------------------------------------------------
	
		#endregion
	
	//---------------------------------------------------------------------------
	
		#region Public Member Functions
	
	//---------------------------------------------------------------------------
			
	public static bool hasBoundaryChuncks = true; // if has auto generated boundary chunks, like the stairs around the map
	
	/// <summary>
	/// Called when the script instance is being loaded.
	/// </summary>
	public void Awake()
	{
		Initialize();
		OCLogger.Fine(gameObject.name + " is awake.");
	}
	
	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start()
	{
		OCLogger.Fine(gameObject.name + " is started.");
	}
	
	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	public void Update()
	{
		// Check if OCConnector has been initialized (a.k.a connecting to the router).
		if(!this.connector.IsInit())
		{
			return;
		}
			
		this.timer += Time.deltaTime;
			
		// Percept the world once in each interval.
		if(timer >= UpdatePerceptionInterval)
		{
			this.perceptWorld();
			this.PerceiveTerrain();
			perceptStateChanges();
			timer = 0.0f;
		}
				
		OCLogger.Fine(gameObject.name + " is updated.");	
	}
			
	/// <summary>
	/// Reset this instance to its default values.
	/// </summary>
	public void Reset()
	{
		Uninitialize();
		Initialize();
		OCLogger.Fine(gameObject.name + " is reset.");	
	}
	
	/// <summary>
	/// Raises the enable event when OCPerceptionCollector is loaded.
	/// </summary>
	public void OnEnable()
	{
		OCLogger.Fine(gameObject.name + " is enabled.");
	}
	
	/// <summary>
	/// Raises the disable event when OCPerceptionCollector goes out of scope.
	/// </summary>
	public void OnDisable()
	{
		OCLogger.Fine(gameObject.name + " is disabled.");
	}
	
	/// <summary>
	/// Raises the destroy event when OCPerceptionCollector is about to be destroyed.
	/// </summary>
	public void OnDestroy()
	{
		Uninitialize();
		OCLogger.Fine(gameObject.name + " is about to be destroyed.");
	}
			
	public OCObjectMapInfo GetOCObjectMapInfo(int objId)
	{
		OCObjectMapInfo result = null;
		if(mapInfoCache.ContainsKey(objId))
		{
			result = mapInfoCache[objId];
		}
		return result;
	}
			
	/// <summary>
	/// Percept and check map info of all available objects.
	/// </summary>
	public void perceptWorld()
	{
		HashSet<int> updatedObjects = null;
		HashSet<int> disappearedObjects = null;
	
		List<int> cacheIdList = new List<int>();
		cacheIdList.AddRange(mapInfoCache.Keys);
		// Before performing the perception task, set the all map info caches' updated status to false.
		// An object's updated status will be changed while building its map info.
		foreach(int oid in cacheIdList)
		{
			mapInfoCacheStatus[oid] = false;
		}
	
		// Update the map info of all avatar in the repository(including player avatar).
		foreach(GameObject oca in OCARepository.GetAllOCA())
		{
			if(this.buildMapInfo(oca))
			{
				if(updatedObjects == null)
				{
					updatedObjects = new HashSet<int>();
				}
				updatedObjects.Add(oca.GetInstanceID());
			}
		}
			
		// Update the map info of all OCObjects in repository.
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("OCObject"))
		{
			if(this.buildMapInfo(go))
			{
				if(updatedObjects == null)
				{
					updatedObjects = new HashSet<int>();
				}
				updatedObjects.Add(go.GetInstanceID());
			}
		}
	
		// Handle all objects that disappeared in this cycle.
		foreach(int oid in cacheIdList)
		{
			if(!mapInfoCacheStatus[oid])
			{
				if(disappearedObjects == null)
				{
					disappearedObjects = new HashSet<int>();
				}
				// The updated flag of the map info cache is false, meaning it has not been updated in last cycle.
				mapInfoCache[oid].RemoveProperty("visibility-status");
				mapInfoCache[oid].AddProperty("remove", "true", PropertyType.BOOL);
				disappearedObjects.Add(oid);
			}
		}
	
		List<OCObjectMapInfo> latestMapInfoSeq = new List<OCObjectMapInfo>();
		if(updatedObjects != null)
		{
			log.Info("PerceptionCollector: global map info has been updated");		
			foreach(int oid in updatedObjects)
			{
				latestMapInfoSeq.Add(this.mapInfoCache[oid]);
			}
		}
	
		if(disappearedObjects != null)
		{
			foreach(int oid in disappearedObjects)
			{
				connector.handleObjectAppearOrDisappear(mapInfoCache[oid].Id, mapInfoCache[oid].Type, false);
				latestMapInfoSeq.Add(this.mapInfoCache[oid]);
				// Remove disappeared object from cache.
				this.mapInfoCache.Remove(oid);
				this.mapInfoCacheStatus.Remove(oid);
			}
		}
	
		if(latestMapInfoSeq.Count > 0)
		{
			// Append latest map info sequence to OC connector's sending queue.
			connector.sendMapInfoMessage(latestMapInfoSeq, perceptWorldFirstTime);
		}
			
		perceptWorldFirstTime = false;
	}
			
	/// <summary>
	/// Mark the chunk status as changed.
	/// </summary>
	/// <param name="chunkId"></param>
	public void changeChunkStatus(string chunkId)
	{
		Console.print("In changeChunkStatus...");
		if(chunkStatusMap.ContainsKey(chunkId))
		{
			chunkStatusMap[chunkId] = true;
		}
	}
		
	public void addNewState(StateChangesRegister.StateInfo ainfo)
	{
		System.Reflection.FieldInfo stateValInfo = ainfo.behaviour.GetType().GetField(ainfo.stateName);
		System.Object valObj = stateValInfo.GetValue(ainfo.behaviour);
		stateInfoCache.Add(ainfo, valObj);		
	}
		
	public static void NotifyBlockRemoved(Vector3i blockBuildPoint)
	{
		Transform allAvatars = GameObject.Find("Avatars").transform;
		foreach(Transform child in allAvatars)
		{
			if(child.gameObject.tag != "OCA")
			{
				continue;
			}
			OCPerceptionCollector con = child.gameObject.GetComponent<OCPerceptionCollector>() as OCPerceptionCollector;
			if(con != null)
			{
				con._notifyBlockRemoved(blockBuildPoint);
			}
		}
	}
		
	public static void NotifyBlockAdded(Vector3i blockBuildPoint)
	{
		Transform allAvatars = GameObject.Find("Avatars").transform;
		foreach(Transform child in allAvatars)
		{
			if(child.gameObject.tag != "OCA")
			{
				continue;
			}
			OCPerceptionCollector con = child.gameObject.GetComponent<OCPerceptionCollector>() as OCPerceptionCollector;
			if(con != null)
			{
				con._notifyBlockAdded(blockBuildPoint);
			}
		}
	}
		
	
		
	
	//---------------------------------------------------------------------------
	
		#endregion
	
	//---------------------------------------------------------------------------
	
		#region Private Member Functions
	
	//---------------------------------------------------------------------------
		
	/// <summary>
	/// Initializes this instance.  Set default values here.
	/// </summary>
	private void Initialize()
	{
		// Obtain components of this OCAvatar.
		this.connector = gameObject.GetComponent("OCConnector") as OCConnector;
		this.id = gameObject.GetInstanceID();
			
		foreach(StateInfo ainfo in StateChangesRegister.StateList)
		{			
			System.Reflection.FieldInfo stateValInfo = ainfo.behaviour.GetType().GetField(ainfo.stateName);
			System.Object valObj = stateValInfo.GetValue(ainfo.behaviour);
			stateInfoCache.Add(ainfo, valObj);
		}
	}
		
	/// <summary>
	/// Uninitializes this instance.  Cleanup refernces here.
	/// </summary>
	private void Uninitialize()
	{
	}
			
	/// <summary>
	/// Build meta map-info and cache it. If the map-info has been cached already,
	/// then check if it is up-to-date.
	/// </summary>
	/// <param name="go">The gameobject instance to be percepted</param>
	/// <returns>return true if a object is detected for the first time or it has a different status
	/// from the cached one
	/// </returns>
	private bool BuildMapInfo(UnityEngine.GameObject go)
	{
		// The flag to mark if the map info of a game object has been updated.
		bool isUpdated = false;
		OCObjectMapInfo mapInfo;
		int goId = go.GetInstanceID();
	
		this.mapInfoCacheStatus[goId] = true;
	
		if(this.mapInfoCache.ContainsKey(goId))
		{
			// Read cache
			mapInfo = this.mapInfoCache[goId];
		}
		else
		{
			// Create a new map info and cache it.
			mapInfo = new OCObjectMapInfo(go);
			lock(this.cacheLock)
			{
				this.mapInfoCache[goId] = mapInfo;
			}
				
			// We don't send all the existing objects as appear actions to the opencog at the time the robot is loaded.
			if(! perceptWorldFirstTime)
			{
				connector.handleObjectAppearOrDisappear(mapInfo.Id, mapInfo.Type, true);
			}
				
			// When constructing the new map info instance, 
			// dynamical data of this game object has been obtained.
			return true;
		}
			
		// Position
		Vector3 currentPos = VectorUtil.ConvertToOpenCogCoord(go.transform.position);
	
		if(go.tag == "OCA")
		{
			// Fix the model center point problem.
			// The altitude of oc avatar is underneath the floor because of the 
			// 3D model problem, correct it by adding half of the avatar height.
			//currentPos.z += mapInfo.Height * 0.5f;
		}
	
		Vector3 cachedPos = mapInfo.Position;
		Vector3 cachedVelocity = mapInfo.Velocity;
		bool hasMoved = false;
			
		if(!currentPos.Equals(cachedPos) && 
	            Vector3.Distance(cachedPos, currentPos) > OCObjectMapInfo.POSITION_DISTANCE_THRESHOLD)
		{
			hasMoved = true;
			isUpdated = true;
			mapInfo.Position = currentPos;
			// Update the velocity
			mapInfo.Velocity = calculateVelocity(cachedPos, currentPos);
		}
			
		// if start to move
		if(cachedVelocity == Vector3.zero && hasMoved)
		{
			mapInfo.startMovePos = cachedPos;
			connector.handleObjectStateChange(go, "is_moving", "System.String", "false", "true");
		}
		else
		if(cachedVelocity != Vector3.zero && ! hasMoved)
		{// if stop moving
			connector.handleObjectStateChange(go, "is_moving", "System.String", "true", "false");			
			connector.sendMoveActionDone(go, mapInfo.startMovePos, currentPos);
			mapInfo.Velocity = Vector3.zero;
		}
	
		// Rotation
		Rotation currentRot = new Rotation(go.transform.rotation);
		Rotation cachedRot = mapInfo.Rotation;
	
		if(!currentRot.Equals(cachedRot))
		{
			isUpdated = true;
			mapInfo.Rotation = currentRot;
		}
	
		return isUpdated;
	}
			
	/// <summary>
	/// Notify OAC that certain block has been removed.
	/// </summary>
	/// <param name="hitPoint"></param>
	private void _notifyBlockRemoved(Vector3i hitPoint)
	{
		uint chunkX = (uint)hitPoint.X / worldData.ChunkBlockWidth;
		uint chunkY = (uint)hitPoint.Y / worldData.ChunkBlockHeight;
		uint chunkZ = (uint)hitPoint.Z / worldData.ChunkBlockDepth;
		uint blockX = (uint)hitPoint.X % worldData.ChunkBlockWidth;
		uint blockY = (uint)hitPoint.Y % worldData.ChunkBlockHeight;
		uint blockZ = (uint)hitPoint.Z % worldData.ChunkBlockDepth;
	
		Chunk currentChunk = worldData.Chunks[chunkX, chunkY, chunkZ];
	
		/*		// check if this block is contained in a block conjunction.
			// If so, find out the base z index of this conjunction.
			int z = blockZ;
			for (; z > floorHeight; z--)
			{
				if (currentChunk.Blocks[blockX, blockY, z].Type != BlockType.Air ||
					!CheckSurfaceBlock(currentChunk, blockX, blockY, z) ||
					z == floorHeight + 1)
					break;
			}
			 */
		OCObjectMapInfo mapinfo = OCObjectMapInfo.CreateTerrainMapInfo(currentChunk, blockX, blockY, blockZ, 1, currentChunk.Blocks[blockX, blockY, blockZ].Type);
		//mapinfo.Visibility = OCObjectMapInfo.VISIBLE_STATUS.UNKNOWN;
		mapinfo.RemoveProperty("visibility-status");
		mapinfo.AddProperty("remove", "true", PropertyType.BOOL);
	
		List<OCObjectMapInfo> removedBlockList = new List<OCObjectMapInfo>();
		removedBlockList.Add(mapinfo);
		connector.handleObjectAppearOrDisappear(mapinfo.Id, mapinfo.Type, false);
		connector.sendTerrainInfoMessage(removedBlockList);
		
	}
			
	private void _notifyBlockAdded(Vector3i hitPoint)
	{
		uint chunkX = (uint)(hitPoint.X / worldData.ChunkBlockWidth);
		uint chunkY = (uint)(hitPoint.Y / worldData.ChunkBlockHeight);
		uint chunkZ = (uint)(hitPoint.Z / worldData.ChunkBlockDepth);
		uint blockX = (uint)(hitPoint.X % worldData.ChunkBlockWidth);
		uint blockY = (uint)(hitPoint.Y % worldData.ChunkBlockHeight);
		uint blockZ = (uint)(hitPoint.Z % worldData.ChunkBlockDepth);
	
		Chunk currentChunk = worldData.Chunks[chunkX, chunkY, chunkZ];
		OCObjectMapInfo mapinfo = OCObjectMapInfo.CreateTerrainMapInfo(currentChunk, blockX, blockY, blockZ, 1, currentChunk.Blocks[blockX, blockY, blockZ].Type);
			
			
		List<OCObjectMapInfo> addedBlockList = new List<OCObjectMapInfo>();
		addedBlockList.Add(mapinfo);
		connector.handleObjectAppearOrDisappear(mapinfo.Id, mapinfo.Type, true);
		connector.sendTerrainInfoMessage(addedBlockList);
		
	}
	
	private void PerceiveTerrain()
	{
		if(m_hasPerceivedTerrainForFirstTime)
		{
			return;
		}

		List<OCObjectMapInfo> terrainMapinfoList = new List<OCObjectMapInfo>();
		Map map = UnityEngine.GameObject.Find("Map");


		foreach(Chunk chunk in map.GetChunks())
		{
			Vector3i viChunkPosition = chunk.GetPosition();

			Debug.Log("Perceiving Chunk at position [" + viChunkPosition.x + ", " + viChunkPosition.y + ", " + viChunkPosition.z + "].");

			// Maybe do some empty check here...there will be many empty chunks. But it might be
			// equally expensive without setting new empty flags while creating chunks.

			Vector3i viChunkStartingCorner = new Vector3i(viChunkPosition.x * chunk.SIZE_X, viChunkPosition.y * chunk.SIZE_Y & viChunkPosition.z * chunk.SIZE_Z);
			Vector3i viChunkEndingCorner = new Vector3i((viChunkPosition.x + 1) * chunk.SIZE_X - 1, (viChunkPosition.y + 1) * chunk.SIZE_Y - 1, (viChunkPosition.z + 1) * chunk.SIZE_Z - 1);

			Debug.Log("   Processing blocks from [" + viChunkStartingCorner.x + ", " + viChunkStartingCorner.y + ", " + viChunkStartingCorner.z + "].");
			Debug.Log("   to [" + viChunkEndingCorner.x + ", " + viChunkEndingCorner.y + ", " + viChunkEndingCorner.z + "].");

			for(int iGlobalX = viChunkStartingCorner.x; iGlobalX <= viChunkEndingCorner.x; iGlobalX++)
			{
				for(int iGlobalY = viChunkStartingCorner.y; iGlobalY <= viChunkEndingCorner.y; iGlobalY++)
				{
					for(int iGlobalZ = viChunkStartingCorner.z; iGlobalZ <= viChunkEndingCorner.z; iGlobalZ++)
					{
						// Ok...now we have some globalz....

						BlockData globalBlock = map.GetBlock(iGlobalX, iGlobalY, iGlobalZ);

						if(!globalBlock.IsEmpty)
						{
							OCObjectMapInfo globalMapInfo = OCObjectMapInfo.CreateObjectMapInfo(viChunkPosition.x, viChunkPosition.y, viChunkPosition.z, iGlobalX, iGlobalY, iGlobalZ, globalBlock);

							terrainMapinfoList.Add(globalMapInfo);

							// in case there are too many blocks, we send every 5000 blocks per message
							if(terrainMapinfoList.Count >= 5)
							{
								m_connector.sendTerrainInfoMessage(terrainMapinfoList, true);
								terrainMapinfoList.Clear();
							}
						}

					}

				}

			}
		}
	}
			
	/// <summary>
	/// currently, this function only runs for one time, when a robot is loaded
	/// Percept the minecraft-like terrain.
	/// TODO: what part of terrain should we percept and send the map info? Let's just send 
	/// the information of chunks in flat area first.
	/// </summary>
	private void PerceptTerrain()
	{
		if(hasPerceivedTerrainForFirstTime)
		{
			return;
		}
		// Get the world game object.
		WorldGameObject world = GameObject.Find("World").GetComponent<WorldGameObject>() as WorldGameObject;
	
	
		// Get the chunks data.
		worldData = world.WorldData;
		floorHeight = worldData.floor;
		List<OCObjectMapInfo> terrainMapinfoList = new List<OCObjectMapInfo>();

		for(int chunk_x = 0; chunk_x < worldData.ChunksWide; chunk_x++)
		{
			for(int chunk_y = 0; chunk_y < worldData.ChunksHigh; chunk_y++)
			{
				// check if it is out of our logic boundary
				if(hasBoundaryChuncks && (!isFlatChunk(worldData, chunk_x, chunk_y)))
				{
					continue;
				}
					
				Chunk currentChunk = worldData.Chunks[chunk_x, chunk_y, 0];
				if(!m_chunkStatusMap.ContainsKey(currentChunk.ToString()))
				{
					m_chunkStatusMap[currentChunk.ToString()] = true;
				}
	
				if(chunkStatusMap[currentChunk.ToString()])
				{
					// Mark as percepted.
					chunkStatusMap[currentChunk.ToString()] = false;
	
					bool conjunctionBreak = true;
					for(uint x = 0; x < currentChunk.Width; x++)
					{
						for(uint y = 0; y < currentChunk.Height; y++)
						{
							for(uint z = (uint)(currentChunk.Depth - 1); z > floorHeight; z--)
							{
								if(currentChunk.Blocks[x, y, z].Type != BlockType.Air /*&& CheckSurfaceBlock(currentChunk, x, y, z)*/)
								{
									conjunctionBreak = false;
									OCObjectMapInfo mapinfo = OCObjectMapInfo.CreateTerrainMapInfo(currentChunk, x, y, z, 1, currentChunk.Blocks[x, y, z].Type);
									
									terrainMapinfoList.Add(mapinfo);

									// in case there are too many blocks, we send every 5000 blocks per message
									if(terrainMapinfoList.Count >= 5000)
									{
										connector.sendTerrainInfoMessage(terrainMapinfoList, true);
										terrainMapinfoList.Clear();
									}
	
								}   
							}
						}
					}
	
				}
					
	
	
			}
				
		}
					
		if(terrainMapinfoList.Count > 0)
		{
			connector.sendTerrainInfoMessage(terrainMapinfoList, ! havePerceptTerrainForFirstTime);
			terrainMapinfoList.Clear();
		}
			
		if(! havePerceptTerrainForFirstTime)
		{
			connector.sendFinishPerceptTerrian();
		}
		havePerceptTerrainForFirstTime = true;
	}

		private void PerceiveStateChanges()
		{

		}

			
	private void PerceptStateChanges()
	{
		foreach(StateInfo stateInfo in StateChangesRegister.StateList)
		{
			if(stateInfo.gameObject == null || stateInfo.behaviour == null)
			{
				// the state doesn't exist anymore,prepare to delete this state
				StatesToDelete.Add(stateInfo);
				continue;
			}
	
			System.Reflection.FieldInfo stateValInfo = stateInfo.behaviour.GetType().GetField(stateInfo.stateName);
			System.Object valObj = stateValInfo.GetValue(stateInfo.behaviour);
				
			String type = stateValInfo.FieldType.ToString();
	
			if(stateValInfo.FieldType.IsEnum)
			{
				type = "Enum";
			}
						
	
			System.Object old = stateInfoCache[stateInfo];
			if(!System.Object.Equals(stateInfoCache[stateInfo], valObj))
			{
				// send state changes as a "stateChange" action
				if(type == "Enum")
				{
					// the opencog does not process enum type, we change it into string
					connector.handleObjectStateChange(stateInfo.gameObject, stateInfo.stateName, "System.String", 
						                                  stateInfoCache[stateInfo].ToString(), valObj.ToString());
				}
				else
				{
					connector.handleObjectStateChange(stateInfo.gameObject, stateInfo.stateName, type, stateInfoCache[stateInfo], valObj);
				}
					
					
				stateInfoCache[stateInfo] = valObj;
			}
			else
			if(perceptStateChangesFirstTime)
			{
				// send this existing state to the opencog first time
				if(type == "Enum")
				{
					// the opencog does not process enum type, we change it into string
					connector.sendExistingStates(stateInfo.gameObject, stateInfo.stateName, "System.String", valObj.ToString());
				}
				else
				{
					connector.sendExistingStates(stateInfo.gameObject, stateInfo.stateName, type, valObj);
				}
			
			}
					
			
		}
			
		foreach(StateInfo stateInfo in StatesToDelete)
		{
			// the state doesn't exist any more, remove it;
			stateInfoCache.Remove(stateInfo);
			StateChangesRegister.UnregisterState(stateInfo);
			
		}
		StatesToDelete.Clear();
			
		perceptStateChangesFirstTime = false;
	}
			
	/// <summary>
	/// Check if a block is on the surface of a chunk, which means it is
	/// a neighbor of an "Air" type block.
	/// TODO: This should be a function of minepackage, move it later.
	/// </summary>
	/// <param name="chunk"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	/// <returns></returns>
	private bool CheckSurfaceBlock(Chunk chunk, uint x, uint y, uint z)
	{
		// Above
		if(z + 1 <= chunk.Depth && chunk.Blocks[x, y, z + 1].Type == BlockType.Air)
		{
			return true;
		}
	
		// Below
		if(z - 1 >= 0 && chunk.Blocks[x, y, z - 1].Type == BlockType.Air)
		{
			return true;
		}
	
		// East
		if(x + 1 < chunk.Width && chunk.Blocks[x + 1, y, z].Type == BlockType.Air)
		{
			return true;
		}
	
		// West
		if(x - 1 >= 0 && chunk.Blocks[x - 1, y, z].Type == BlockType.Air)
		{
			return true;
		}
	
		// North
		if(y - 1 >= 0 && chunk.Blocks[x, y - 1, z].Type == BlockType.Air)
		{
			return true;
		}
	
		// South
		if(y + 1 < chunk.Height && chunk.Blocks[x, y + 1, z].Type == BlockType.Air)
		{
			return true;
		}
	
		return false;
	}
			
	/// <summary>
	/// Learn from the Basin Terrain Generator, just for debugging purpose.
	/// </summary>
	/// <param name="worldData"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	private bool IsFlatChunk(Map map, int x, int y)
	{
		if(map.ChunkBlockWidth < 4)
		{
			return false;
		}
		if(map.ChunkBlockHeight < 4)
		{
			return false;
		}
	
		if(x <= 1 || y <= 1)
		{
			return false;
		}
		if(x >= map.ChunksWide - 2 || y >= map.ChunksHigh - 2)
		{
			return false;
		}
	
		return true;
	}
			
	protected UnityEngine.Vector3 calculateVelocity(UnityEngine.Vector3 oldPos, UnityEngine.Vector3 newPos)
	{
		UnityEngine.Vector3 deltaVector = newPos - oldPos;
		// We suppose the object is performing uniform motion.
		UnityEngine.Vector3 velocity = deltaVector / this.UpdatePerceptionInterval;
				
		return velocity;
	}
				
	//---------------------------------------------------------------------------
	
		#endregion
	
	//---------------------------------------------------------------------------
	
		#region Other Members
	
	//---------------------------------------------------------------------------		
	
	/// <summary>
	/// Initializes a new instance of the <see cref="OpenCog.OCPerceptionCollector"/> class.  
	/// Generally, intitialization should occur in the Start or Awake
	/// functions, not here.
	/// </summary>
	public OCPerceptionCollector()
	{
	}
	
	//---------------------------------------------------------------------------
	
		#endregion
	
	//---------------------------------------------------------------------------
	
}// class OCPerceptionCollector

}// namespace OpenCog




