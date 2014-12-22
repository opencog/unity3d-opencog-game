
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

using OpenCog.Utilities.Logging;
using OpenCog.Map;

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
using System.Linq;
using UnityEngine;

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
public class OCPerceptionCollector : OCSingletonMonoBehaviour<OCPerceptionCollector>
{
	//---------------------------------------------------------------------------
		
			#region Private Member Data
		
	//---------------------------------------------------------------------------
			
	private float _updatePerceptionInterval = 0.5f; // Percept 5 times per second.
	private float _timer = 0.0f; // Reset timer at the end of every interval.
	private OCConnectorSingleton _connector; // The OCConnector instance used to send map-info.
	private int _id; // A local copy of the game object id that this component attached to.
	private Dictionary<int, OCObjectMapInfo> _mapInfoCache = new Dictionary<int, OCObjectMapInfo>();

	[SerializeField]
	private List<int>
		_mapInfoCacheKeys = new List<int>();

	[SerializeField]
	private List<string>
		_mapInfoCacheNames = new List<string>();

	private Dictionary<int, bool> _mapInfoCacheStatus = new Dictionary<int, bool>(); // A flag map indicates if a cached map info has been percepted in latest cycle.
	private Dictionary<OCStateChangesRegister.StateInfo, System.Object> _stateInfoCache = new Dictionary<OCStateChangesRegister.StateInfo, System.Object>();

	private ArrayList _statesToDelete = new ArrayList();

	private System.Object _cacheLock = new System.Object();

	private List<OCObjectMapInfo> _removedObjects = new List<OCObjectMapInfo>(); // A list of objects recently removed. This is a temporary data structure, cleared whenever it is processed.
	private bool _isPerceivingWorldForTheFirstTime = true;

	//private WorldData _worldData; // Reference to the world data.
	private OpenCog.Map.OCMap _map;

	private Dictionary<string, bool> _chunkStatusMap = new Dictionary<string, bool>(); // A map to mark if current chunk needs to be percepted. True means perception in need.
	private int _floorHeight; // Currently, just percept the block above the horizon.

	private bool _hasPerceivedTerrainForFirstTime = false;

	private bool _hasStartedPerceivingTerrainForTheFirstTime = false;

	private bool _isPerceivingStateChangesForTheFirstTime = true;

	private bool _hasPerceivedWorldForTheFirstTime = false;

	private bool _perceptionInProgress = false;


	//work with iteration to compare how long it takes blocks to process.
	System.DateTime dtStartProcessing;
				
	//---------------------------------------------------------------------------
		
			#endregion
		
	//---------------------------------------------------------------------------
		
			#region Accessors and Mutators
		
	//---------------------------------------------------------------------------
			
	public static OCPerceptionCollector Instance
	{
		get
		{
			GameObject agi = GameObject.FindGameObjectWithTag("OCAGI");
			if(agi)
			{
				return agi.GetComponent<OCPerceptionCollector>();
			} else
			{
				return null;
			}
		}
	}
			
	//---------------------------------------------------------------------------
		
			#endregion
		
	//---------------------------------------------------------------------------
		
			#region Public Member Functions
		
	//---------------------------------------------------------------------------
				
	public static bool HasBoundaryChuncks = false; // if has auto generated boundary chunks, like the stairs around the map

		
	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start()
	{
		_connector = OCConnectorSingleton.Instance;

		_floorHeight = _connector.getFloorHeight();
				
		UnityEngine.Debug.Log(OCLogSymbol.CREATE + gameObject.name + " is started.");
	}
		
	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	public void Update()
	{
		_mapInfoCacheKeys = _mapInfoCache.Keys.ToList();
		_mapInfoCacheNames = _mapInfoCache.Values.Select(o => o.name).ToList();
		// Check if OCConnector has been initialized (a.k.a connecting to the router).
		if(!_connector.IsInitialized)
		{
			// || !_connector.IsLoaded) {
			return;
		}
				
		_timer += UnityEngine.Time.deltaTime;

		//we do not want to update if PerceiveTerrain is running (that will keep changes from being sensed) or if the timer has not elapsed
		if(_perceptionInProgress)
		{
			return;
		}
		if(_timer < _updatePerceptionInterval)
		{
			return;
		}

		// I'm not sure what this does...
				
		// I'm not sure about the right order...when I look at the old world on the OpenCog side, it receives edible objects first and then the terrain.
		// So I moved the calls below to the same order.
				
		if(!_hasStartedPerceivingTerrainForTheFirstTime)
		{
			// We only want this coroutine to launch once, so:
			_hasStartedPerceivingTerrainForTheFirstTime = true;
					
			StartCoroutine(this.PerceiveTerrain());
					
			// Once PerceiveTerrain (blocks) has finished, _hasPerceivedTerrainForFirstTime -> true;
		}
				
				
		if(_hasPerceivedTerrainForFirstTime && !_hasPerceivedWorldForTheFirstTime)
		{
			// Once _hasPerceivedTerrainForFirstTime is true, we start perceiving the world (characters, batteries, other non-block entities)
					
			this.PerceiveWorld();
					
			// This sets _hasPerceivedWorldForTheFirstTime to true, so after this it only transmits updates (disappeared / created batteries, etc)
		}
				
		if(_hasPerceivedWorldForTheFirstTime)
		{
			PerceiveWorld();	
			PerceiveStateChanges();
		}
				
		_timer = 0.0f;

					
		//System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is updated.");	
	}
				
	/// <summary>
	/// Reset this instance to its default values.
	/// </summary>
	public void Reset()
	{
		Uninitialize();
		Initialize();
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + gameObject.name + " is reset.");	
	}
		
	/// <summary>
	/// Raises the enable event when OCPerceptionCollector is loaded.
	/// </summary>
	public void OnEnable()
	{
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + gameObject.name + " is enabled.");
	}
		
	/// <summary>
	/// Raises the disable event when OCPerceptionCollector goes out of scope.
	/// </summary>
	public void OnDisable()
	{
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + gameObject.name + " is disabled.");
	}
		
	/// <summary>
	/// Raises the destroy event when OCPerceptionCollector is about to be destroyed.
	/// </summary>
	public void OnDestroy()
	{
		Uninitialize();
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + gameObject.name + " is about to be destroyed.");
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
	public void PerceiveWorld()
	{
		// Track updated and dissappeared objects in case we must send messages.
		HashSet<int> updatedObjects = new HashSet<int>();
		HashSet<int> disappearedObjects = new HashSet<int>();
		
		// I'm still worried if we're communicating the right id's on both sides...
		List<int> cacheIdList = new List<int>();
				
		// This appears to be a collection of gameObject IDs. Stuff that has been deemed relevant in previous loops?
		// The values are objectMapInfo objects; I know at least that battery IDs get passed through here.
		cacheIdList.AddRange(_mapInfoCache.Keys);
				
		// Before performing the perception task, set the all map info caches' updated status to false.
		// An object's updated status will be changed while building its map info.
		// This is another dictionary...also containing ints (probably gameobject id's again) and booleans, to indicate if the status has been updated?
		// Yep, the boolean indicates if the status has been updated (in the last cycle (?))
		foreach(int oid in cacheIdList)
		{
			_mapInfoCacheStatus[oid] = false;
		}
		
		// I'll write some code here that we need to call later...but I don't know what will replace the OCARepository
		// Looks like we need to build a mapinfo object for each one....
				
		// Old comment: Update the map info of all avatar in the repository(including player avatar).
		UnityEngine.GameObject[] npcArray = UnityEngine.GameObject.FindGameObjectsWithTag("OCNPC");
				
		OpenCog.Utility.Console.Console console = OpenCog.Utility.Console.Console.Instance;
				
				
		for(int iNPC = 0; iNPC < npcArray.Length; iNPC++)
		{
			UnityEngine.GameObject npcObject = npcArray[iNPC];
					
			if(this.BuildMapInfo(npcObject))
			{
				updatedObjects.Add(npcObject.GetInstanceID());					
				UnityEngine.Debug.Log("Added NPC with ID '" + npcObject.GetInstanceID() + "' to updatedObjects");
			} else
			{
				//UnityEngine.Debug.Log ("NPC with ID '" + npcObject.GetInstanceID() + "' has not changed, so will not be added to updatedObjects");
			}	
					
			console.AddConsoleEntry("I can see someone else! Their ID is " + npcObject.GetInstanceID(), "AGI Robot", OpenCog.Utility.Console.Console.ConsoleEntry.Type.SAY);
		}
				
		UnityEngine.GameObject[] agiArray = UnityEngine.GameObject.FindGameObjectsWithTag("OCAGI");
				
		for(int iAGI = 0; iAGI < agiArray.Length; iAGI++)
		{
			UnityEngine.GameObject agiObject = agiArray[iAGI];
					
			if(this.BuildMapInfo(agiObject))
			{
				updatedObjects.Add(agiObject.GetInstanceID());
						
				UnityEngine.Debug.Log("Added AGI with ID '" + agiObject.GetInstanceID() + "' to updatedObjects");
			} else
			{
				//UnityEngine.Debug.Log ("AGI with ID '" + agiObject.GetInstanceID() + "' has not changed, so will not be added to updatedObjects");
			}	
		}
				
		// Ok, so that was everything in the OCA (OpenCog Avatar?) Repository. Next come the OCObjects...
				
		//List<UnityEngine.GameObject> batteryObjects = UnityEngine.GameObject.FindGameObjectsWithTag("OCBattery").ToList();
		//			foreach (UnityEngine.GameObject batteryObject in batteryObjects)
		//			{
		//				if (this.BuildMapInfo(batteryObject))
		//					updatedObjects.Add (batteryObject.GetInstanceID());
		//			}
				
		UnityEngine.GameObject[] batteryArray = UnityEngine.GameObject.FindGameObjectsWithTag("OCBattery");
				
		for(int iBattery = 0; iBattery < batteryArray.Length; iBattery++)
		{
			UnityEngine.GameObject batteryObject = batteryArray[iBattery];
					
			if(this.BuildMapInfo(batteryObject))
			{
				updatedObjects.Add(batteryObject.GetInstanceID());
						
				System.Console.WriteLine(OCLogSymbol.RUNNING + "Added Battery with ID '" + batteryObject.GetInstanceID() + "' to updatedObjects");
			} else
			{
				//UnityEngine.Debug.Log ("Battery with ID '" + batteryObject.GetInstanceID() + "' has not changed, so will not be added to updatedObjects");
			}	
					
			console.AddConsoleEntry("I can see a battery! Its ID is " + batteryObject.GetInstanceID(), "AGI Robot", OpenCog.Utility.Console.Console.ConsoleEntry.Type.SAY);
		}
				
		UnityEngine.GameObject[] hearthArray = UnityEngine.GameObject.FindGameObjectsWithTag("OCHearth");
				
		for(int iHearth = 0; iHearth < hearthArray.Length; iHearth++)
		{
			UnityEngine.GameObject hearthObject = hearthArray[iHearth];
					
			if(this.BuildMapInfo(hearthObject))
			{
				updatedObjects.Add(hearthObject.GetInstanceID());
						
				UnityEngine.Debug.Log("Added Hearth with ID '" + hearthObject.GetInstanceID() + "' to updatedObjects");
			} else
			{
				//UnityEngine.Debug.Log ("Battery with ID '" + batteryObject.GetInstanceID() + "' has not changed, so will not be added to updatedObjects");
			}	
					
			console.AddConsoleEntry("I can see my home! It's where I feel safe.", "AGI Robot", OpenCog.Utility.Console.Console.ConsoleEntry.Type.SAY);
		}
				
				
		//			for (int ocObject = 0; ocObject < 0; ocObject++)
		//			{
		//				UnityEngine.GameObject someGameObject = null;
		//				
		//				// Seems this function always tries to build a mapinfo...but returns false if it wasn't updated or something? Or maybe if it was the first time too?
		//				if (this.BuildMapInfo(someGameObject))
		//					updatedObjects.Add (someGameObject.GetInstanceID());
		//			}
		
		// Ok...here we're determining which objects have disappeared. It seems that cacheIdList contains objects that existed before.
		// We then look for these objects in _mapInfoCacheStatus (by gameobjectID it seems) to see if their boolean is false...not sure why yet.
		// Oh...I just found out it means that it's status has not been updated in the last cycle...weird...why would it not complete....?
		// Handle all objects that disappeared in this cycle.
				
		string strCacheIdList = "";
				
		foreach(int cacheIDItem in cacheIdList)
		{
			strCacheIdList += cacheIDItem.ToString() + ", ";	
		}
				
		//UnityEngine.Debug.Log ("CacheIDList contains these ID's: " + strCacheIdList );
				
				
		foreach(int oid in cacheIdList)
		{
			//UnityEngine.Debug.Log ("Now we're checking _mapInfoCache for the ID " + oid);
			if(!_mapInfoCacheStatus[oid])
			{
				//UnityEngine.Debug.LogWarning(OCLogSymbol.WARN + "We didn't find the ID " + oid);
				// So...if the game object's boolean is false...
						
				// The updated flag of the map info cache is false, meaning it has not been updated in last cycle.
				// Why do we remove the visibility status tag??
				// Anyway, we look up the object based on object id...so we get an objectMapInfo...and remove the visibility tag
				_mapInfoCache[oid].RemoveProperty("visibility-status");
				// And we add a 'remove' tag...guess that means removed = true!
				_mapInfoCache [oid].AddProperty ("remove", "true", System.Type.GetType ("System.Boolean"));
				// Finally we add it's gameobject id to a hashset of disappeared objects...
				disappearedObjects.Add(oid);
			}
			//else
			//UnityEngine.Debug.Log ("We found the ID " + oid);
		}
		
				
		// Next...we make a list of ObjectMapInfo's...and pipe the updatedobjects into it...
		// but...we're just piping all OCARepository and OCObject tagged objects into it...that doesn't 
		// mean they were updated. It seems that buildMapInfo actually does some kind of 'isupdated' analysis...
		List<OCObjectMapInfo> latestMapInfoSeq = new List<OCObjectMapInfo>();
		if(updatedObjects != null)
		{
			//UnityEngine.Debug.Log ("PerceptionCollector: global map info has been updated");
			foreach(int oid in updatedObjects)
			{
				latestMapInfoSeq.Add(this._mapInfoCache[oid]);
			}
		}
		
		// Next we go through the hashset of disappeared objects...
		if(disappearedObjects != null)
		{
			// Looptie-dooptie-doo....
			foreach(int oid in disappearedObjects)
			{
				// Report disappearance to the OCConnector...
				_connector.HandleObjectAppearOrDisappear(_mapInfoCache[oid].ID, _mapInfoCache[oid].Type, false);
						
				// AND add it to latestMapInfoSeq? That's weird...seems redundant to me now...
				latestMapInfoSeq.Add(this._mapInfoCache[oid]);
				// Remove disappeared object from cache.
				_mapInfoCache.Remove(oid); // The dictionary of game object id's and objectmapinfo's
				_mapInfoCacheStatus.Remove(oid); // The dictionary of game object id's and booleans (updated yes / no)
			}
		}
		
		// So if we have updated objects we send them to OpenCog. This is different from sendTerrainInfoMessage. Oh loo, it indicates if it was
		// the first time too.
		if(latestMapInfoSeq.Count > 0)
		{
			// Append latest map info sequence to OC connector's sending queue.
			_connector.SendMapInfoMessage(latestMapInfoSeq, _isPerceivingWorldForTheFirstTime);
		}
				
		_isPerceivingWorldForTheFirstTime = false;
		_hasPerceivedWorldForTheFirstTime = true;
	}
				
	/// <summary>
	/// Mark the chunk status as changed.
	/// </summary>
	/// <param name="chunkId"></param>
	public void ChangeChunkStatus(string chunkId)
	{
		Debug.Log(OCLogSymbol.RUNNING + "Perception.ChangeChunkStatus is running.");
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
			
	public void NotifyBlockRemoved(Vector3i blockBuildPoint)
	{
		// blockBuildPoint should be a global coordinate, and they're ints...so I guess we can use them directly. Still need to figure out the chunk.
		//			uint chunkX = (uint)(hitPoint.X / worldData.ChunkBlockWidth);
		//			uint chunkY = (uint)(hitPoint.Y / worldData.ChunkBlockHeight);
		//			uint chunkZ = (uint)(hitPoint.Z / worldData.ChunkBlockDepth);
		//			uint blockX = (uint)(hitPoint.X % worldData.ChunkBlockWidth);
		//			uint blockY = (uint)(hitPoint.Y % worldData.ChunkBlockHeight);
		//			uint blockZ = (uint)(hitPoint.Z % worldData.ChunkBlockDepth);
				
		Vector3i chunkPosition = OpenCog.Map.OCChunk.ToChunkPosition(blockBuildPoint);
		Vector3i localPosition = OpenCog.Map.OCChunk.ToLocalPosition(blockBuildPoint);
				
		OpenCog.Map.OCMap map = OpenCog.Map.OCMap.Instance;//UnityEngine.GameObject.Find ("Map").GetComponent<OpenCog.Map.OCMap> () as OpenCog.Map.OCMap;
				
		OpenCog.Map.OCBlockData globalBlock = map.GetBlock(blockBuildPoint.x, blockBuildPoint.y, blockBuildPoint.z);
				
		// IMPORTANT: ANOTHER Y Z SWAP:
		// ORIGINAL:
		//OCObjectMapInfo mapInfo = new OCObjectMapInfo(chunkPosition.x, chunkPosition.y, chunkPosition.z, localPosition.x, localPosition.y, localPosition.z, globalBlock);
		// SWAPPED:
		OCObjectMapInfo mapInfo = new OCObjectMapInfo(chunkPosition.x, chunkPosition.z, chunkPosition.y, localPosition.x, localPosition.z, localPosition.y, globalBlock);
				
		List<OCObjectMapInfo> removedBlockList = new List<OCObjectMapInfo>();
				
		removedBlockList.Add(mapInfo);
				
		OCConnectorSingleton connector = OCConnectorSingleton.Instance;
				
		connector.HandleObjectAppearOrDisappear(mapInfo.ID, mapInfo.Type, false);
		//connector.SendTerrainInfoMessage(removedBlockList);
	}
			
	public void NotifyBlockAdded(Vector3i blockBuildPoint)
	{
		// blockBuildPoint should be a global coordinate, and they're ints...so I guess we can use them directly. Still need to figure out the chunk.
		//			uint chunkX = (uint)(hitPoint.X / worldData.ChunkBlockWidth);
		//			uint chunkY = (uint)(hitPoint.Y / worldData.ChunkBlockHeight);
		//			uint chunkZ = (uint)(hitPoint.Z / worldData.ChunkBlockDepth);
		//			uint blockX = (uint)(hitPoint.X % worldData.ChunkBlockWidth);
		//			uint blockY = (uint)(hitPoint.Y % worldData.ChunkBlockHeight);
		//			uint blockZ = (uint)(hitPoint.Z % worldData.ChunkBlockDepth);
				
		Vector3i chunkPosition = OpenCog.Map.OCChunk.ToChunkPosition(blockBuildPoint);
		//Vector3i localPosition = OpenCog.Map.OCChunk.ToLocalPosition(blockBuildPoint);
				
		OpenCog.Map.OCMap map = OpenCog.Map.OCMap.Instance;//UnityEngine.GameObject.Find ("Map").GetComponent<OpenCog.Map.OCMap> () as OpenCog.Map.OCMap;
				
		OpenCog.Map.OCBlockData globalBlock = map.GetBlock(blockBuildPoint.x, blockBuildPoint.y, blockBuildPoint.z);
				
		// IMPORTANT: SWAPPING OUT Y AND Z HERE AGAIN!!
		// ORIGINAL:
		//OCObjectMapInfo mapInfo = new OCObjectMapInfo(chunkPosition.x, chunkPosition.y, chunkPosition.z, localPosition.x, localPosition.y, localPosition.z, globalBlock);
		// SWAPPED:
		OCObjectMapInfo mapInfo = new OCObjectMapInfo(chunkPosition.x, chunkPosition.z, chunkPosition.y, blockBuildPoint.x, blockBuildPoint.z, blockBuildPoint.y, globalBlock);
				
				
		List<OCObjectMapInfo> addedBlockList = new List<OCObjectMapInfo>();
				
		addedBlockList.Add(mapInfo);
				
		OCConnectorSingleton connector = OCConnectorSingleton.Instance;
				
		connector.HandleObjectAppearOrDisappear(mapInfo.ID, mapInfo.Type, true);
		connector.SendTerrainInfoMessage(addedBlockList);
	}
			
	public void NotifyBatteryAdded(Vector3i batteryCreationPoint)
	{
		// blockBuildPoint should be a global coordinate, and they're ints...so I guess we can use them directly. Still need to figure out the chunk.
		//			uint chunkX = (uint)(hitPoint.X / worldData.ChunkBlockWidth);
		//			uint chunkY = (uint)(hitPoint.Y / worldData.ChunkBlockHeight);
		//			uint chunkZ = (uint)(hitPoint.Z / worldData.ChunkBlockDepth);
		//			uint blockX = (uint)(hitPoint.X % worldData.ChunkBlockWidth);
		//			uint blockY = (uint)(hitPoint.Y % worldData.ChunkBlockHeight);
		//			uint blockZ = (uint)(hitPoint.Z % worldData.ChunkBlockDepth);
				
		//Vector3i chunkPosition = OpenCog.Map.OCChunk.ToChunkPosition(batteryCreationPoint);
		//Vector3i localPosition = OpenCog.Map.OCChunk.ToLocalPosition(batteryCreationPoint);
				
		//OpenCog.Map.OCMap map = OpenCog.Map.OCMap.Instance;//UnityEngine.GameObject.Find ("Map").GetComponent<OpenCog.Map.OCMap> () as OpenCog.Map.OCMap;
				
		//OpenCog.Map.OCBlockData globalBlock = map.GetBlock(batteryCreationPoint.x, batteryCreationPoint.y, batteryCreationPoint.z);
				
		// IMPORTANT: SWAPPING OUT Y AND Z HERE AGAIN!!
		// ORIGINAL:
		//OCObjectMapInfo mapInfo = new OCObjectMapInfo(chunkPosition.x, chunkPosition.y, chunkPosition.z, localPosition.x, localPosition.y, localPosition.z, globalBlock);
		// SWAPPED:
		//OCObjectMapInfo mapInfo = new OCObjectMapInfo(chunkPosition.x, chunkPosition.z, chunkPosition.y, batteryCreationPoint.x, batteryCreationPoint.z, batteryCreationPoint.y, globalBlock);
				
		// The mapInfo created above is USELESS! We need to send the BATTERY not the BLOCK that surrounds the battery.
				
		OCObjectMapInfo mapInfo = null;
				
		// So we need to find this battery...
				
		UnityEngine.GameObject[] batteryArray = UnityEngine.GameObject.FindGameObjectsWithTag("OCBattery");
				
		for(int iBattery = 0; iBattery < batteryArray.Length; iBattery++)
		{
			UnityEngine.GameObject batteryObject = batteryArray[iBattery];
					
			Vector3i v3iBatteryPosition = new Vector3i((int)batteryObject.transform.position.x, (int)batteryObject.transform.position.y, (int)batteryObject.transform.position.z);
					
			if(v3iBatteryPosition == batteryCreationPoint)
			{
				// You're the one that I want! (the one that I want!) Ooh Ooh oooooooh!	
				if(!_mapInfoCache.ContainsKey(batteryObject.GetInstanceID()))
				{
					if(this.BuildMapInfo(batteryObject))
					{
						// THat puts into a dictionary, so we still need to retrieve it here.		
													
						mapInfo = _mapInfoCache[batteryObject.GetInstanceID()];
								
						//console.AddConsoleEntry("I can see a new battery! Its ID is " + batteryObject.GetInstanceID(), "AGI Robot", OpenCog.Utility.Console.Console.ConsoleEntry.Type.SAY);
						UnityEngine.Debug.Log(OCLogSymbol.RUNNING + "I can see a new battery! Its ID is " + batteryObject.GetInstanceID());
					}
				}
			}
		}
				
		if(mapInfo != null)
		{
			List<OCObjectMapInfo> addedBlockList = new List<OCObjectMapInfo>();
				
			addedBlockList.Add(mapInfo);

			float y = mapInfo.position.y;
			float z = mapInfo.position.z;

			mapInfo.position.Set(mapInfo.position.x, z, y);
					
			OCConnectorSingleton connector = OCConnectorSingleton.Instance;
					
			//connector.HandleObjectAppearOrDisappear(mapInfo.ID, mapInfo.Type, true);
			connector.SendTerrainInfoMessage(addedBlockList);	
		} else
		{
			UnityEngine.Debug.Log("mapInfo == null, nothing to report.");	
		}
	}
			
	public void NotifyBatteryRemoved(Vector3i batteryDestructionPoint)
	{
		// blockBuildPoint should be a global coordinate, and they're ints...so I guess we can use them directly. Still need to figure out the chunk.
		//			uint chunkX = (uint)(hitPoint.X / worldData.ChunkBlockWidth);
		//			uint chunkY = (uint)(hitPoint.Y / worldData.ChunkBlockHeight);
		//			uint chunkZ = (uint)(hitPoint.Z / worldData.ChunkBlockDepth);
		//			uint blockX = (uint)(hitPoint.X % worldData.ChunkBlockWidth);
		//			uint blockY = (uint)(hitPoint.Y % worldData.ChunkBlockHeight);
		//			uint blockZ = (uint)(hitPoint.Z % worldData.ChunkBlockDepth);
				
		//Vector3i chunkPosition = OpenCog.Map.OCChunk.ToChunkPosition(batteryDestructionPoint);
		//Vector3i localPosition = OpenCog.Map.OCChunk.ToLocalPosition(batteryDestructionPoint);
				
		//OpenCog.Map.OCMap map = OpenCog.Map.OCMap.Instance;//UnityEngine.GameObject.Find ("Map").GetComponent<OpenCog.Map.OCMap> () as OpenCog.Map.OCMap;
				
		//OpenCog.Map.OCBlockData globalBlock = map.GetBlock(batteryDestructionPoint.x, batteryDestructionPoint.y, batteryDestructionPoint.z);
				
		// IMPORTANT: SWAPPING OUT Y AND Z HERE AGAIN!!
		// ORIGINAL:
		//OCObjectMapInfo mapInfo = new OCObjectMapInfo(chunkPosition.x, chunkPosition.y, chunkPosition.z, localPosition.x, localPosition.y, localPosition.z, globalBlock);
		// SWAPPED:
		//OCObjectMapInfo mapInfo = new OCObjectMapInfo(chunkPosition.x, chunkPosition.z, chunkPosition.y, batteryCreationPoint.x, batteryCreationPoint.z, batteryCreationPoint.y, globalBlock);
				
		// The mapInfo created above is USELESS! We need to send the BATTERY not the BLOCK that surrounds the battery.
				
		OCObjectMapInfo mapInfo = null;
				
		// So we need to find this battery...
				
		UnityEngine.GameObject[] batteryArray = UnityEngine.GameObject.FindGameObjectsWithTag("OCBattery");
				
		//int batteryIDToRemove = 0;
				
		for(int iBattery = 0; iBattery < batteryArray.Length; iBattery++)
		{
			UnityEngine.GameObject batteryObject = batteryArray[iBattery];
					
			Vector3i v3iBatteryPosition = new Vector3i((int)batteryObject.transform.position.x, (int)batteryObject.transform.position.y, (int)batteryObject.transform.position.z);
					
			if(v3iBatteryPosition == batteryDestructionPoint)
			{
				//UnityEngine.Debug.Log ("We'll be searching the _mapInfoCache for an objeject with key '" + batteryObject.GetInstanceID() + "'");
						
				//					foreach (KeyValuePair<int, OpenCog.Embodiment.OCObjectMapInfo> pair in _mapInfoCache)
				//					{
				//						UnityEngine.Debug.Log ("In any case, there is a key '" + pair.Key + "' in it.");	
				//					}
						
				// You're the one that I want! (the one that I want!) Ooh Ooh oooooooh!	
				if(_mapInfoCache.ContainsKey(batteryObject.GetInstanceID()))
				{
					mapInfo = _mapInfoCache[batteryObject.GetInstanceID()];
							
					//batteryIDToRemove = batteryObject.GetInstanceID();
				}
						
				//UnityEngine.GameObject.Destroy (batteryObject);
			}
		}
				
		if(mapInfo != null)
		{
			List<OCObjectMapInfo> removedBlockList = new List<OCObjectMapInfo>();

			float y = mapInfo.position.y;
			float z = mapInfo.position.z;
					
			mapInfo.position.Set(mapInfo.position.x, z, y);

			removedBlockList.Add(mapInfo);
					
			//OCConnectorSingleton connector = OCConnectorSingleton.Instance;
					
			///connector.HandleObjectAppearOrDisappear(mapInfo.ID, mapInfo.Type, false);
			//connector.SendTerrainInfoMessage(removedBlockList);
					
			//OCConnectorSingleton connector = OCConnectorSingleton.Instance;
					
			//connector.HandleObjectAppearOrDisappear(mapInfo.ID, mapInfo.Type, false);
			//connector.SendTerrainInfoMessage(addedBlockList);	
					 
			//_mapInfoCache.Remove (batteryIDToRemove);
					
			// Now we need to destroy the actual batteryobject...terrifying!!
					
					
		}
		//			else
		//			{
		//				UnityEngine.Debug.Log ("mapInfo == null, nothing to report.");	
		//			}
	}
		
	//---------------------------------------------------------------------------
		
			#endregion
		
	//---------------------------------------------------------------------------
		
			#region Private Member Functions
		
	//---------------------------------------------------------------------------
			
	/// <summary>
	/// Initializes this instance.  Set default values here.
	/// </summary>
	override protected void Initialize()
	{
		// Obtain components of this OCAvatar.
		_connector = OCConnectorSingleton.Instance;
		_id = GameObject.FindGameObjectWithTag("OCAGI").GetInstanceID();
				
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
		if(go == null)
		{
			return false;
		}
				
		// The flag to mark if the map info of a game object has been updated.
		bool isUpdated = false;
		OCObjectMapInfo mapInfo;
		int gameObjectID = go.GetInstanceID();
		
		// So here we register the fact that this gameobject has already been updated in the previous (or current?) cycle.
		_mapInfoCacheStatus[gameObjectID] = true;
		
		// Check if it's in our list of previously existing objects...?
		if(_mapInfoCache.ContainsKey(gameObjectID))
		{
			// Read cache
			mapInfo = _mapInfoCache[gameObjectID];
		} else
		{
			/// If it's not in our list of previously existing objects...we should create a mapinfo, add it, and send it to opencog as an 'appeared' object.
			mapInfo = new OCObjectMapInfo(go);
					
			lock(_mapInfoCache)
			{
				_mapInfoCache[gameObjectID] = mapInfo;
			}
					
			// We don't send all the existing objects as appear actions to the opencog at the time the robot is loaded.
			// If it isn't perceiving the world for the first time, we inform OpenCog. Otherwise...we don't!
			if(! _isPerceivingWorldForTheFirstTime)
			{
				_connector.HandleObjectAppearOrDisappear(mapInfo.ID, mapInfo.Type, true);
			}
					
			// When constructing the new map info instance, 
			// dynamical data of this game object has been obtained.
			return true;
		}
				
		// Position
		UnityEngine.Vector3 currentPos = Utility.VectorUtil.ConvertToOpenCogCoord(go.transform.position);

		if(go.tag == "OCNPC" || go.tag == "OCAGI" || go.tag == "Player")
		{
			if(mapInfo.Height > 1.0f) // just to make sure that the center point of the character will not be in the block where the feet are
			{
				currentPos = new UnityEngine.Vector3(currentPos.x, currentPos.y, currentPos.z + 1.0f);	
			}
		}
		
		if(go.tag == "OCA")
		{
			// Fix the model center point problem.
			// The altitude of oc avatar is underneath the floor because of the 
			// 3D model problem, correct it by adding half of the avatar height.
			//currentPos.z += mapInfo.Height * 0.5f;
		}
		
		UnityEngine.Vector3 cachedPos = mapInfo.position;
		UnityEngine.Vector3 cachedVelocity = mapInfo.Velocity;
		bool hasMoved = false;
				
		if(!currentPos.Equals(cachedPos) && 
			UnityEngine.Vector3.Distance(cachedPos, currentPos) > OCObjectMapInfo.POSITION_DISTANCE_THRESHOLD)
		{
			hasMoved = true;
			isUpdated = true;
			mapInfo.position = currentPos;
			// Update the velocity
			mapInfo.Velocity = CalculateVelocity(cachedPos, currentPos);
		}
				
		// if start to move
		if(cachedVelocity == UnityEngine.Vector3.zero && hasMoved)
		{
			mapInfo.StartMovePos = cachedPos;
			_connector.HandleObjectStateChange(go, "is_moving", "System.String", "false", "true");
		} else if(cachedVelocity != UnityEngine.Vector3.zero && ! hasMoved)
		{// if stop moving
			_connector.HandleObjectStateChange(go, "is_moving", "System.String", "true", "false");
			_connector.SendMoveActionDone(go, mapInfo.StartMovePos, currentPos);
			mapInfo.Velocity = UnityEngine.Vector3.zero;
		}
		
		// Rotation
		Utility.Rotation currentRot = new Utility.Rotation(go.transform.rotation);
		Utility.Rotation cachedRot = mapInfo.rotation;
		
		if(!currentRot.Equals(cachedRot))
		{
			isUpdated = true;
			mapInfo.rotation = currentRot;
		}
		
		return isUpdated;
	}

	//DEPRECATED
	/// <summary>
	/// Notify OAC that certain block has been removed.
	/// </summary>
	/// <param name="hitPoint"></param>
	//		private void _notifyBlockRemoved (Vector3i hitPoint)
	//		{
	//			int chunkX = (int)hitPoint.x / OpenCog.Map.OCChunk.SIZE_X;
	//			int chunkY = (int)hitPoint.y / OpenCog.Map.OCChunk.SIZE_Y;
	//			int chunkZ = (int)hitPoint.z / OpenCog.Map.OCChunk.SIZE_Z;
	//			int blockX = (int)hitPoint.x % OpenCog.Map.OCChunk.SIZE_X;
	//			int blockY = (int)hitPoint.y % OpenCog.Map.OCChunk.SIZE_Y;
	//			int blockZ = (int)hitPoint.z % OpenCog.Map.OCChunk.SIZE_Z;
	//
	//			int globalBlockX = (int)hitPoint.x;
	//			int globalBlockY = (int)hitPoint.y;
	//			int globalBlockZ = (int)hitPoint.z;
	//
	//			OpenCog.Map.OCChunk currentChunk = _map.Chunks.Get (chunkX, chunkY, chunkZ);
	//	
	//			/*		// check if this block is contained in a block conjunction.
	//			// If so, find out the base z index of this conjunction.
	//			int z = blockZ;
	//			for (; z > floorHeight; z--)
	//			{
	//				if (currentChunk.Blocks[blockX, blockY, z].Type != BlockType.Air ||
	//					!CheckSurfaceBlock(currentChunk, blockX, blockY, z) ||
	//					z == floorHeight + 1)
	//					break;
	//			}
	//			 */
	//
	//			// TODO: Get the right value from BlockData to put into mapinfo
	//			OCObjectMapInfo mapinfo = new OCObjectMapInfo (chunkX, chunkY, chunkZ, globalBlockX, globalBlockY, globalBlockZ, _map.GetBlock (globalBlockX, globalBlockY, globalBlockZ));
	//			mapinfo.RemoveProperty ("visibility-status");
	//			mapinfo.AddProperty ("remove", "true", System.Type.GetType ("System.Boolean"));
	//			//mapinfo.Visibility = OCObjectMapInfo.VISIBLE_STATUS.UNKNOWN;
	//	
	//			List<OCObjectMapInfo> removedBlockList = new List<OCObjectMapInfo> ();
	//			removedBlockList.Add (mapinfo);
	//			_connector.HandleObjectAppearOrDisappear (mapinfo.ID, mapinfo.Type, false);
	//			_connector.SendTerrainInfoMessage (removedBlockList);
	//		
	//		}
	//			
	//		private void _notifyBlockAdded (Vector3i hitPoint)
	//		{
	//			//TODO MAYBE, XYZ XZY ETC.
	//			int chunkX = (int)(hitPoint.x / OpenCog.Map.OCChunk.SIZE_X);
	//			int chunkY = (int)(hitPoint.y / OpenCog.Map.OCChunk.SIZE_Y);
	//			int chunkZ = (int)(hitPoint.z / OpenCog.Map.OCChunk.SIZE_Z);
	//			int blockX = (int)(hitPoint.x % OpenCog.Map.OCChunk.SIZE_X);
	//			int blockY = (int)(hitPoint.y % OpenCog.Map.OCChunk.SIZE_Y);
	//			int blockZ = (int)(hitPoint.z % OpenCog.Map.OCChunk.SIZE_Z);
	//	
	//			OpenCog.Map.OCChunk currentChunk = _map.Chunks.Get (chunkX, chunkY, chunkZ);
	//			OCObjectMapInfo mapinfo = new OCObjectMapInfo (chunkX, chunkY, chunkZ, blockX, blockY, blockZ, currentChunk.GetBlock (blockX, blockY, blockZ));
	//			
	//			
	//			List<OCObjectMapInfo> addedBlockList = new List<OCObjectMapInfo> ();
	//			addedBlockList.Add (mapinfo);
	//			_connector.HandleObjectAppearOrDisappear (mapinfo.ID, mapinfo.Type, true);
	//			_connector.SendTerrainInfoMessage (addedBlockList);
	//		
	//		}``


	public IEnumerator PerceiveChunk(OCMap map, OCChunk chunk, List<OCObjectMapInfo> terrainMapinfoList)
	{
		_perceptionInProgress = true;
			
		if(chunk == null || chunk.IsEmpty)
		{
			yield break;
		}

		//some iteration parameters
		int blocksProcessed = 0;
		int emptyBlocksProcessed = 0;

				
		// chunk position is the coordinates of the chunk.
		Vector3i viChunkPosition = chunk.GetPosition();

		//let the user know about our hard labor!
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "Perceiving Chunk at position [" + viChunkPosition.x + ", " + viChunkPosition.y + ", " + viChunkPosition.z + "].");
			
		// Maybe do some empty check here...there will be many empty chunks. But it might be
		// equally expensive without setting new empty flags while creating chunks.
			
		int startX = viChunkPosition.x * Map.OCChunk.SIZE_X;
		int startY = viChunkPosition.y * Map.OCChunk.SIZE_Y;
		int startZ = viChunkPosition.z * Map.OCChunk.SIZE_Z;
			
		int endX = ((viChunkPosition.x + 1) * Map.OCChunk.SIZE_X) - 1;
		int endY = ((viChunkPosition.y + 1) * Map.OCChunk.SIZE_Y) - 1;
		int endZ = ((viChunkPosition.z + 1) * Map.OCChunk.SIZE_Z) - 1;
			
		Vector3i viChunkStartingCorner = new Vector3i(startX, startY, startZ);
		Vector3i viChunkEndingCorner = new Vector3i(endX, endY, endZ);
			
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "Processing blocks from chunk [" + viChunkStartingCorner.x + ", " + viChunkStartingCorner.y + ", " + viChunkStartingCorner.z + " to [" + viChunkEndingCorner.x + ", " + viChunkEndingCorner.y + ", " + viChunkEndingCorner.z + "].");
			
		for(int iGlobalX = viChunkStartingCorner.x; iGlobalX <= viChunkEndingCorner.x; iGlobalX++)
		{
			for(int iGlobalY = viChunkStartingCorner.y; iGlobalY <= viChunkEndingCorner.y; iGlobalY++)
			{
				for(int iGlobalZ = viChunkStartingCorner.z; iGlobalZ <= viChunkEndingCorner.z; iGlobalZ++)
				{

					// Ok...now we have some globalCoordinates and we can get the block out
					OpenCog.Map.OCBlockData globalBlock = map.GetBlock(iGlobalX, iGlobalY, iGlobalZ);

					//skip empty blocks, batteries, and those beneathe the floorheight
					if(globalBlock.IsEmpty() || globalBlock.block.GetName().ToLower() == "battery" || iGlobalY <= _floorHeight)
					{
						emptyBlocksProcessed += 1;	
						continue;
					}

					// WARNING: Y AND Z SWAPPED HERE FOR OPENCOG'S SAKE:
					OCObjectMapInfo globalMapInfo = new OCObjectMapInfo(viChunkPosition.x, viChunkPosition.z, viChunkPosition.y, iGlobalX, iGlobalZ, iGlobalY, globalBlock);
						
					terrainMapinfoList.Add(globalMapInfo);
					blocksProcessed += 1;
			
				} // End for(int iGlobalZ = viChunkStartingCorner.z; iGlobalZ <= viChunkEndingCorner.z; iGlobalZ++)

			} // End for(int iGlobalY = viChunkStartingCorner.y; iGlobalY <= viChunkEndingCorner.y; iGlobalY++)

			//send the terrain in chunk slices, which should never be any bigger than 256*16 = 4096 blocks (identical to 
			//the arbitrary 'max blocks per transmission' parameter we sent in.

			//FIXME [ERROR CHECKING]: We may want to make sure empty terrainMapInfoLists are never sent here. 
			_connector.SendTerrainInfoMessage(terrainMapinfoList, true);
			terrainMapinfoList.Clear();

			//Print debug messages
			System.DateTime dtProcessingTick = System.DateTime.Now;
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "Sent Terrain Info. Processed " + blocksProcessed + " blocks in " + dtProcessingTick.Subtract(dtStartProcessing).TotalMilliseconds + " milliseconds. " + emptyBlocksProcessed + " were empty.");
			emptyBlocksProcessed = 0;
			blocksProcessed = 0;
			dtStartProcessing = dtProcessingTick;

			//yield till next frame!
			yield return null;
				
		} // End for(int iGlobalX = viChunkStartingCorner.x; iGlobalX <= viChunkEndingCorner.x; iGlobalX++)	
	} 
	

		
	/// <summary>
	/// A coroutine which performs a type of initialization by reading in the map to opencog
	/// </summary>
	/// <returns>The terrain.</returns>
	public IEnumerator PerceiveTerrain()
	{
		//We want to record that this has run because it is a form of initialization
		Debug.Log(OCLogSymbol.RUNNING + "OCPerceptionCollector.PerceiveTerrain() running.");

		//we need this so we can print to the console that we've sensed the world and are going to set to percieving it
		OpenCog.Utility.Console.Console console = OpenCog.Utility.Console.Console.Instance;
		

		//We want to check to make sure we haven't run this function twice
		if(_hasPerceivedTerrainForFirstTime)
		{
			Debug.LogWarning(OCLogSymbol.WARN + "PerceiveTerrain() was called with _hasPercievedTerrainForFirstTime set equal to true. Typically, this should not happen.");
			console.AddConsoleEntry("I've seen this terrain before... Perhaps this is in error? Can you show me something new?", "AGI Robot", OpenCog.Utility.Console.Console.ConsoleEntry.Type.SAY);
			yield return null;
			yield break;
		} else
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "PerceiveTerrain() was called with _hasPercievedTerrainForFirstTime set equal to false. This is correct.");
			console.AddConsoleEntry("What a beautiful world! Please give me a few seconds to process it...", "AGI Robot", OpenCog.Utility.Console.Console.ConsoleEntry.Type.SAY);
		}

		//initialize the time at which we're starting the perception
		System.DateTime dtStartPerceptTerrain = System.DateTime.Now;

		//get an instance of the map to work with
		List<OCObjectMapInfo> terrainMapinfoList = new List<OCObjectMapInfo>();
		OpenCog.Map.OCMap map = OpenCog.Map.OCMap.Instance;//UnityEngine.GameObject.Find ("Map").GetComponent<OpenCog.Map.OCMap> () as OpenCog.Map.OCMap;
			
		if(map == null)
		{
			Debug.LogError(OCLogSymbol.IMPOSSIBLE_ERROR + "OCPerceptionCollector.PerceiveTerrain() was unable to get an instance of the map.");
			yield break;
		}
							
		for(int x = map.Chunks.GetMinX(); x < map.Chunks.GetMaxX(); ++x)
		{
			for(int y = map.Chunks.GetMinY(); y < map.Chunks.GetMaxY(); ++y)
			{
				for(int z = map.Chunks.GetMinZ(); z < map.Chunks.GetMaxZ(); ++z)
				{
					yield return StartCoroutine(PerceiveChunk(map, map.Chunks.Get(x, y, z), terrainMapinfoList));
				}
			}
		}

		// Check for remaining blocks to report to OpenCog
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "Let's check if there are any blocks left to report in our terrainMapinfoList...");
		if(terrainMapinfoList.Count > 0)
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "   Yep, looks like there are...");
			_connector.SendTerrainInfoMessage(terrainMapinfoList, ! _hasPerceivedTerrainForFirstTime);
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "   Ok, all blocks have been sent now...");
			terrainMapinfoList.Clear();
		} else
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "   Nope, looks like we already sent everything!");	
		}
					
		// Communicate completion of initial terrain perception
		if(!_hasPerceivedTerrainForFirstTime)
		{

			System.Console.WriteLine(OCLogSymbol.RUNNING + "Time to send the 'finished perceiving terrain' message!");
			_connector.SendFinishPerceptTerrain();
			_hasPerceivedTerrainForFirstTime = true;
		} else
		{
			Debug.LogError(OCLogSymbol.IMPOSSIBLE_ERROR + "That's weird...it thinks _hasPerceivedTerrainForFirstTime is true already...");	
		}
				
		Debug.Log(OCLogSymbol.CLEARED + "Finished perceiving terrain, total time taken: " + System.DateTime.Now.Subtract(dtStartPerceptTerrain).TotalSeconds);
		Debug.Log(OCLogSymbol.RUNNING + "Waiting for Embodiment... (This may take a few seconds)");
				
		console.AddConsoleEntry("Thank you for waiting! Let me figure out what to do next...", "AGI Robot", OpenCog.Utility.Console.Console.ConsoleEntry.Type.SAY);

		_perceptionInProgress = false;
	}
			
	// This function is only relevant in terms of the state changes of:
	// FoodStuff
	// Lift
	// LiftButton
	// Picker
	// PutOnAbleObject
	// StoveButton
	// So it appears we don't need it right now! Guess the interesting one is then PerceiveWorld.
	private void PerceiveStateChanges()
	{
		//UnityEngine.Debug.Log ("OCPerceptionCollector::PerceiveStateChanges");
		// Loop through the states that have previously been registered. A stateInfo contains an gameobject, a behaviour and a statename.
		foreach(OpenCog.Embodiment.OCStateChangesRegister.StateInfo stateInfo in OpenCog.Embodiment.OCStateChangesRegister.StateList)
		{
			// In some cases a stateinfo object is not relevant, _statesToDelete is a local arraylist of states that should later be deleted
			// from _stateInfoCache (a local dictionary of stateInfo's) as well as from the StateChangesRegister.
			if(stateInfo.gameObject == null || stateInfo.behaviour == null)
			{
				// The state doesn't exist anymore, add it to _statesToDelete for deletion later and continue to the next iteration through the loop.
				_statesToDelete.Add(stateInfo);
				continue;
			}
					
			// This is strange to me, bit too much reflection.
					
			// They get some fieldinfo about the stateInfo's stateName field. Which is just a string...
			System.Reflection.FieldInfo stateValInfo = stateInfo.behaviour.GetType().GetField(stateInfo.stateName);
			// Then they retrieve the value of the behaviour property of the stateInfo, which also appears to be directly accessible, and put it in an object.
			System.Object valObj = stateValInfo.GetValue(stateInfo.behaviour);
					
			// Now I'm lost...we query what kind of field type the stateName field has...which is a STRING WARGSRLBSLL!!
			string type = stateValInfo.FieldType.ToString();
		
			if(stateValInfo.FieldType.IsEnum)
			{
				type = "Enum";
				UnityEngine.Debug.LogError(OCLogSymbol.IMPOSSIBLE_ERROR + "0.00001 BitCoins says this never happens!! IT'S A STRING FFS WAARHEHFSHJKSJKSH!");
			}
					
			// Now we query the local _stateInfoCache (a dictionary with stateInfo objects as the key...and a vague object as the value...
			//System.Object old = _stateInfoCache [stateInfo];
					
			// And we compare that vague object to...the behaviour...in the form of an object...
			if(!System.Object.Equals(_stateInfoCache[stateInfo], valObj))
			{
				// If they are the same...
				// ...send state changes as a "stateChange" action
						
				// If it's an Enum (NEVAH!! NEVAH EVAH EVAH!!")
				if(type == "Enum")
				{
					// the opencog does not process enum types, so we change it into a string
					_connector.HandleObjectStateChange(stateInfo.gameObject, stateInfo.stateName, "System.String",
															  _stateInfoCache[stateInfo].ToString(), valObj.ToString());
				} else
				{
					// And here we revert to using the nicely typed objects WE COULD HAVE STARTED WITH IN THE FIRST PLACE!
					_connector.HandleObjectStateChange(stateInfo.gameObject, stateInfo.stateName, type, _stateInfoCache[stateInfo], valObj);
				}
						
				// Then we update the local stateInfoCache's item (found by searching by stateInfo object) with...the stateInfo's (new?) behaviour.
				_stateInfoCache[stateInfo] = valObj;
			} 
					// Now this is kind of strange...the first if checks if the cached (old) behaviour is the same as the new behaviour...
					// ... oh I see...if it's perceiving the states for the first time then of course they will always be different...or not exist in the cache...
					// ... or something...
					else if(_isPerceivingStateChangesForTheFirstTime)
			{
				// send this existing state to the opencog first time
				if(type == "Enum")
				{
					// WAARGAARBLL!L!L!L?!?
					// the opencog does not process enum type, we change it into string
					_connector.SendExistingStates(stateInfo.gameObject, stateInfo.stateName, "System.String", valObj.ToString());
				} else
				{
					_connector.SendExistingStates(stateInfo.gameObject, stateInfo.stateName, type, valObj);
				}
				
			}
		} // end foreach (OpenCog.Embodiment.OCStateChangesRegister.StateInfo stateInfo in OpenCog.Embodiment.OCStateChangesRegister.StateList) 
				
		// Ok...so _statesToDelete is a bunch of stateInfo's where either gameobject or behaviour is null...
		foreach(OCStateChangesRegister.StateInfo stateInfo in _statesToDelete)
		{
			// the state doesn't exist any more, remove it;
			_stateInfoCache.Remove(stateInfo);
			OCStateChangesRegister.UnregisterState(stateInfo);

		}
				
		_statesToDelete.Clear();

		_isPerceivingStateChangesForTheFirstTime = false;
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




