
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
using OpenCog.Utility;

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
		
	private float _updatePerceptionInterval = 0.5f; // Percept 5 times per second.
	private float _timer = 0.0f; // Reset timer at the end of every interval.
	private OCConnector _connector; // The OCConnector instance used to send map-info.
	private int _id; // A local copy of the game object id that this component attached to.
	private Dictionary<int, OCObjectMapInfo> _mapInfoCache = new Dictionary<int, OCObjectMapInfo>();
	
	private Dictionary<int, bool> _mapInfoCacheStatus = new Dictionary<int, bool>(); // A flag map indicates if a cached map info has been percepted in latest cycle.
	private Dictionary<OCStateChangesRegister.StateInfo, System.Object> _stateInfoCache = new Dictionary<OCStateChangesRegister.StateInfo, System.Object>();
	
	private ArrayList _statesToDelete = new ArrayList();
	
	private System.Object _cacheLock = new System.Object();
	
	private List<OCObjectMapInfo> _removedObjects = new List<OCObjectMapInfo>(); // A list of objects recently removed. This is a temporary data structure, cleared whenever it is processed.
	private bool _perceptWorldFirstTime = true;

	//private WorldData _worldData; // Reference to the world data.
	private OpenCog.Map.OCMap _map;
	
	private Dictionary<string, bool> _chunkStatusMap = new Dictionary<string, bool>(); // A map to mark if current chunk needs to be percepted. True means perception in need.
	private int _floorHeight; // Currently, just percept the block above the horizon.
	private bool _hasPerceivedTerrainForFirstTime = false;
	
	private bool _perceptStateChangesFirstTime = true;
			
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
			
	public static bool HasBoundaryChuncks = true; // if has auto generated boundary chunks, like the stairs around the map
	
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
		if(!this._connector.IsInitialized)
		{
			return;
		}
			
		_timer += UnityEngine.Time.deltaTime;
			
		// Percept the world once in each interval.
		if(_timer >= _updatePerceptionInterval)
		{
			this.PerceptWorld();
			this.PerceiveTerrain();
			PerceiveStateChanges();
			_timer = 0.0f;
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
		if(_mapInfoCache.ContainsKey(objId))
		{
			result = _mapInfoCache[objId];
		}
		return result;
	}
			
	/// <summary>
	/// Percept and check map info of all available objects.
	/// </summary>
	public void PerceptWorld()
	{
		HashSet<int> updatedObjects = null;
		HashSet<int> disappearedObjects = null;
	
		List<int> cacheIdList = new List<int>();
		cacheIdList.AddRange(_mapInfoCache.Keys);
		// Before performing the perception task, set the all map info caches' updated status to false.
		// An object's updated status will be changed while building its map info.
		foreach(int oid in cacheIdList)
		{
			_mapInfoCacheStatus[oid] = false;
		}
	
		// Update the map info of all avatar in the repository(including player avatar).
		// TODO: This should loop through a certain set of game objects, but I'm not sure which. So I'm disabling it for now.
//		foreach(UnityEngine.GameObject oca in OCARepository.GetAllOCA())
//		{
//			if(this.buildMapInfo(oca))
//			{
//				if(updatedObjects == null)
//				{
//					updatedObjects = new HashSet<int>();
//				}
//				updatedObjects.Add(oca.GetInstanceID());
//			}
//		}
			
		// Update the map info of all OCObjects in repository.
		foreach(UnityEngine.GameObject go in UnityEngine.GameObject.FindGameObjectsWithTag("OCObject"))
		{
			if(this.BuildMapInfo(go))
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
			if(!_mapInfoCacheStatus[oid])
			{
				if(disappearedObjects == null)
				{
					disappearedObjects = new HashSet<int>();
				}
				// The updated flag of the map info cache is false, meaning it has not been updated in last cycle.
				_mapInfoCache[oid].RemoveTag("visibility-status");
				_mapInfoCache[oid].AddTag("remove", "true", System.Type.GetType("System.Boolean"));
				disappearedObjects.Add(oid);
			}
		}
	
		List<OCObjectMapInfo> latestMapInfoSeq = new List<OCObjectMapInfo>();
		if(updatedObjects != null)
		{
			OCLogger.Info("PerceptionCollector: global map info has been updated");
			foreach(int oid in updatedObjects)
			{
				latestMapInfoSeq.Add(this._mapInfoCache[oid]);
			}
		}
	
		if(disappearedObjects != null)
		{
			foreach(int oid in disappearedObjects)
			{
				_connector.HandleObjectAppearOrDisappear(_mapInfoCache[oid].ID, _mapInfoCache[oid].Type, false);
				latestMapInfoSeq.Add(this._mapInfoCache[oid]);
				// Remove disappeared object from cache.
				this._mapInfoCache.Remove(oid);
				this._mapInfoCacheStatus.Remove(oid);
			}
		}
	
		if(latestMapInfoSeq.Count > 0)
		{
			// Append latest map info sequence to OC connector's sending queue.
			_connector.SendMapInfoMessage(latestMapInfoSeq, _perceptWorldFirstTime);
		}
			
		_perceptWorldFirstTime = false;
	}
			
	/// <summary>
	/// Mark the chunk status as changed.
	/// </summary>
	/// <param name="chunkId"></param>
	public void ChangeChunkStatus(string chunkId)
	{
		OCLogger.Info("In changeChunkStatus...");
		if(_chunkStatusMap.ContainsKey(chunkId))
		{
			_chunkStatusMap[chunkId] = true;
		}
	}
		
	public void AddNewState(OCStateChangesRegister.StateInfo ainfo)
	{
		System.Reflection.FieldInfo stateValInfo = ainfo.behaviour.GetType().GetField(ainfo.stateName);
		System.Object valObj = stateValInfo.GetValue(ainfo.behaviour);
		_stateInfoCache.Add(ainfo, valObj);
	}
		
	public static void NotifyBlockRemoved(Vector3i blockBuildPoint)
	{
		UnityEngine.Transform allAvatars = UnityEngine.GameObject.Find("Avatars").transform;
		foreach(UnityEngine.Transform child in allAvatars)
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
		UnityEngine.Transform allAvatars = UnityEngine.GameObject.Find("Avatars").transform;
		foreach(UnityEngine.Transform child in allAvatars)
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
		_connector = gameObject.GetComponent("OCConnector") as OCConnector;
		_id = gameObject.GetInstanceID();
			
		foreach(OCStateChangesRegister.StateInfo ainfo in OCStateChangesRegister.StateList)
		{			
			System.Reflection.FieldInfo stateValInfo = ainfo.behaviour.GetType().GetField(ainfo.stateName);
			System.Object valObj = stateValInfo.GetValue(ainfo.behaviour);
			_stateInfoCache.Add(ainfo, valObj);
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
	
		this._mapInfoCacheStatus[goId] = true;
	
		if(this._mapInfoCache.ContainsKey(goId))
		{
			// Read cache
			mapInfo = _mapInfoCache[goId];
		}
		else
		{
			// Create a new map info and cache it.
			mapInfo = new OCObjectMapInfo(go);
			lock(_cacheLock)
			{
				_mapInfoCache[goId] = mapInfo;
			}
				
			// We don't send all the existing objects as appear actions to the opencog at the time the robot is loaded.
			if(! _perceptWorldFirstTime)
			{
				_connector.HandleObjectAppearOrDisappear(mapInfo.ID, mapInfo.Type, true);
			}
				
			// When constructing the new map info instance, 
			// dynamical data of this game object has been obtained.
			return true;
		}
			
		// Position
		UnityEngine.Vector3 currentPos = Utility.VectorUtil.ConvertToOpenCogCoord(go.transform.position);
	
		if(go.tag == "OCA")
		{
			// Fix the model center point problem.
			// The altitude of oc avatar is underneath the floor because of the 
			// 3D model problem, correct it by adding half of the avatar height.
			//currentPos.z += mapInfo.Height * 0.5f;
		}
	
		UnityEngine.Vector3 cachedPos = mapInfo.Position;
		UnityEngine.Vector3 cachedVelocity = mapInfo.Velocity;
		bool hasMoved = false;
			
		if(!currentPos.Equals(cachedPos) && 
	            UnityEngine.Vector3.Distance(cachedPos, currentPos) > OCObjectMapInfo.POSITION_DISTANCE_THRESHOLD)
		{
			hasMoved = true;
			isUpdated = true;
			mapInfo.Position = currentPos;
			// Update the velocity
			mapInfo.Velocity = CalculateVelocity(cachedPos, currentPos);
		}
			
		// if start to move
		if(cachedVelocity == UnityEngine.Vector3.zero && hasMoved)
		{
			mapInfo.StartMovePos = cachedPos;
			_connector.HandleObjectStateChange(go, "is_moving", "System.String", "false", "true");
		}
		else
		if(cachedVelocity != UnityEngine.Vector3.zero && ! hasMoved)
		{// if stop moving
			_connector.HandleObjectStateChange(go, "is_moving", "System.String", "true", "false");
			_connector.SendMoveActionDone(go, mapInfo.StartMovePos, currentPos);
			mapInfo.Velocity = UnityEngine.Vector3.zero;
		}
	
		// Rotation
		Utility.Rotation currentRot = new Utility.Rotation(go.transform.rotation);
		Utility.Rotation cachedRot = mapInfo.Rotation;
	
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
		int chunkX = (int)hitPoint.x / OpenCog.Map.OCChunk.SIZE_X;
		int chunkY = (int)hitPoint.y / OpenCog.Map.OCChunk.SIZE_Y;
		int chunkZ = (int)hitPoint.z / OpenCog.Map.OCChunk.SIZE_Z;
		int blockX = (int)hitPoint.x % OpenCog.Map.OCChunk.SIZE_X;
		int blockY = (int)hitPoint.y % OpenCog.Map.OCChunk.SIZE_Y;
		int blockZ = (int)hitPoint.z % OpenCog.Map.OCChunk.SIZE_Z;

		int globalBlockX = (int)hitPoint.x;
		int globalBlockY = (int)hitPoint.y;
		int globalBlockZ = (int)hitPoint.z;

		OpenCog.Map.OCChunk currentChunk = _map.Chunks.Get(chunkX, chunkY, chunkZ);
	
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

		// TODO: Get the right value from BlockData to put into mapinfo
		OCObjectMapInfo mapinfo = OCObjectMapInfo.CreateObjectMapInfo(chunkX, chunkY, chunkZ, globalBlockX, globalBlockY, globalBlockZ, _map.GetBlock(globalBlockX, globalBlockY, globalBlockZ));
		mapinfo.RemoveTag("visibility-status");
		mapinfo.AddTag("remove", "true", System.Type.GetType("System.Boolean"));
		//mapinfo.Visibility = OCObjectMapInfo.VISIBLE_STATUS.UNKNOWN;
	
		List<OCObjectMapInfo> removedBlockList = new List<OCObjectMapInfo>();
		removedBlockList.Add(mapinfo);
		_connector.HandleObjectAppearOrDisappear(mapinfo.ID, mapinfo.Type, false);
		_connector.SendTerrainInfoMessage(removedBlockList);
		
	}
			
	private void _notifyBlockAdded(Vector3i hitPoint)
	{
		//TODO MAYBE, XYZ XZY ETC.
		int chunkX = (int)(hitPoint.x / OpenCog.Map.OCChunk.SIZE_X);
		int chunkY = (int)(hitPoint.y / OpenCog.Map.OCChunk.SIZE_Y);
		int chunkZ = (int)(hitPoint.z / OpenCog.Map.OCChunk.SIZE_Z);
		int blockX = (int)(hitPoint.x % OpenCog.Map.OCChunk.SIZE_X);
		int blockY = (int)(hitPoint.y % OpenCog.Map.OCChunk.SIZE_Y);
		int blockZ = (int)(hitPoint.z % OpenCog.Map.OCChunk.SIZE_Z);
	
		OpenCog.Map.OCChunk currentChunk = _map.Chunks.Get(chunkX, chunkY, chunkZ);
		OCObjectMapInfo mapinfo = OCObjectMapInfo.CreateObjectMapInfo(chunkX, chunkY, chunkZ, blockX, blockY, blockZ, currentChunk.GetBlock(blockX, blockY, blockZ));
			
			
		List<OCObjectMapInfo> addedBlockList = new List<OCObjectMapInfo>();
		addedBlockList.Add(mapinfo);
		_connector.HandleObjectAppearOrDisappear(mapinfo.ID, mapinfo.Type, true);
		_connector.SendTerrainInfoMessage(addedBlockList);
		
	}
	
	private void PerceiveTerrain()
	{
		if(_hasPerceivedTerrainForFirstTime)
		{
			return;
		}

		List<OCObjectMapInfo> terrainMapinfoList = new List<OCObjectMapInfo>();
		OpenCog.Map.OCMap map = UnityEngine.GameObject.Find("Map").GetComponent<OpenCog.Map.OCMap>() as OpenCog.Map.OCMap;
			
		for(int x = map.Chunks.GetMinX(); x < map.Chunks.GetMaxX(); ++x)
		{
			for(int y = map.Chunks.GetMinY(); y < map.Chunks.GetMaxY(); ++y)
			{
				for(int z = map.Chunks.GetMinZ(); z < map.Chunks.GetMaxZ(); ++z)
				{
					OpenCog.Map.OCChunk chunk = map.Chunks.Get(x, y, z);
						
					Vector3i viChunkPosition = chunk.GetPosition();
			
					OCLogger.Info("Perceiving Chunk at position [" + viChunkPosition.x + ", " + viChunkPosition.y + ", " + viChunkPosition.z + "].");
			
					// Maybe do some empty check here...there will be many empty chunks. But it might be
					// equally expensive without setting new empty flags while creating chunks.
			
					Vector3i viChunkStartingCorner = new Vector3i(viChunkPosition.x * Map.OCChunk.SIZE_X, viChunkPosition.y * Map.OCChunk.SIZE_Y & viChunkPosition.z * Map.OCChunk.SIZE_Z);
					Vector3i viChunkEndingCorner = new Vector3i((viChunkPosition.x + 1) * Map.OCChunk.SIZE_X - 1, (viChunkPosition.y + 1) * Map.OCChunk.SIZE_Y - 1, (viChunkPosition.z + 1) * Map.OCChunk.SIZE_Z - 1);
			
					OCLogger.Info("   Processing blocks from [" + viChunkStartingCorner.x + ", " + viChunkStartingCorner.y + ", " + viChunkStartingCorner.z + "].");
					OCLogger.Info("   to [" + viChunkEndingCorner.x + ", " + viChunkEndingCorner.y + ", " + viChunkEndingCorner.z + "].");
			
					for(int iGlobalX = viChunkStartingCorner.x; iGlobalX <= viChunkEndingCorner.x; iGlobalX++)
					{
						for(int iGlobalY = viChunkStartingCorner.y; iGlobalY <= viChunkEndingCorner.y; iGlobalY++)
						{
							for(int iGlobalZ = viChunkStartingCorner.z; iGlobalZ <= viChunkEndingCorner.z; iGlobalZ++)
							{
								// Ok...now we have some globalz....
			
								OpenCog.Map.OCBlockData globalBlock = map.GetBlock(iGlobalX, iGlobalY, iGlobalZ);
			
								if(!globalBlock.IsEmpty())
								{
									OCObjectMapInfo globalMapInfo = OCObjectMapInfo.CreateObjectMapInfo(viChunkPosition.x, viChunkPosition.y, viChunkPosition.z, iGlobalX, iGlobalY, iGlobalZ, globalBlock);
			
									terrainMapinfoList.Add(globalMapInfo);
			
									// in case there are too many blocks, we send every 5000 blocks per message
									if(terrainMapinfoList.Count >= 5)
									{
										_connector.SendTerrainInfoMessage(terrainMapinfoList, true);
										terrainMapinfoList.Clear();
									}
								} // end if (!globalBlock.IsEmpty())
			
							} // End for(int iGlobalZ = viChunkStartingCorner.z; iGlobalZ <= viChunkEndingCorner.z; iGlobalZ++)
			
						} // End for(int iGlobalY = viChunkStartingCorner.y; iGlobalY <= viChunkEndingCorner.y; iGlobalY++)
			
					} // End for(int iGlobalX = viChunkStartingCorner.x; iGlobalX <= viChunkEndingCorner.x; iGlobalX++)
						
				}
			}
		}

		// Check for remaining blocks to report to OpenCog
		if(terrainMapinfoList.Count > 0)
		{
			_connector.SendTerrainInfoMessage(terrainMapinfoList, ! _hasPerceivedTerrainForFirstTime);
			terrainMapinfoList.Clear();
		}

		// Communicate completion of initial terrain perception
		if(! _hasPerceivedTerrainForFirstTime)
		{
			_connector.SendFinishPerceptTerrain();
			_hasPerceivedTerrainForFirstTime = true;
		}
	}

	private void PerceiveStateChanges()
	{
		foreach(OpenCog.Embodiment.OCStateChangesRegister.StateInfo stateInfo in OpenCog.Embodiment.OCStateChangesRegister.StateList)
		{
			if(stateInfo.gameObject == null || stateInfo.behaviour == null)
			{
				// The state doesn't exist anymore, add it to _statesToDelete for deletion later and continue to the next iteration through the loop.
				_statesToDelete.Add(stateInfo);
				continue;
			}

			System.Reflection.FieldInfo stateValInfo = stateInfo.behaviour.GetType().GetField(stateInfo.stateName);
			System.Object valObj = stateValInfo.GetValue(stateInfo.behaviour);
				
			string type = stateValInfo.FieldType.ToString();
	
			if(stateValInfo.FieldType.IsEnum)
			{
				type = "Enum";
			}
						

			System.Object old = _stateInfoCache[stateInfo];
			if(!System.Object.Equals(_stateInfoCache[stateInfo], valObj))
			{
				// send state changes as a "stateChange" action
				if(type == "Enum")
				{
					// the opencog does not process enum type, we change it into string
					_connector.HandleObjectStateChange(stateInfo.gameObject, stateInfo.stateName, "System.String",
						                                  _stateInfoCache[stateInfo].ToString(), valObj.ToString());
				}
				else
				{
					_connector.HandleObjectStateChange(stateInfo.gameObject, stateInfo.stateName, type, _stateInfoCache[stateInfo], valObj);
				}
					
					
				_stateInfoCache[stateInfo] = valObj;
			}
			else
			if(_perceptStateChangesFirstTime)
			{
				// send this existing state to the opencog first time
				if(type == "Enum")
				{
					// the opencog does not process enum type, we change it into string
					_connector.SendExistingStates(stateInfo.gameObject, stateInfo.stateName, "System.String", valObj.ToString());
				}
				else
				{
					_connector.SendExistingStates(stateInfo.gameObject, stateInfo.stateName, type, valObj);
				}
			
			}
		}
		foreach(OCStateChangesRegister.StateInfo stateInfo in _statesToDelete)
		{
			// the state doesn't exist any more, remove it;
			_stateInfoCache.Remove(stateInfo);
			OCStateChangesRegister.UnregisterState(stateInfo);

		}
		_statesToDelete.Clear();

		_perceptStateChangesFirstTime = false;
	}

	protected UnityEngine.Vector3 CalculateVelocity(UnityEngine.Vector3 oldPos, UnityEngine.Vector3 newPos)
	{
		UnityEngine.Vector3 deltaVector = newPos - oldPos;
		// We suppose the object is performing uniform motion.
		UnityEngine.Vector3 velocity = deltaVector / this._updatePerceptionInterval;
				
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




