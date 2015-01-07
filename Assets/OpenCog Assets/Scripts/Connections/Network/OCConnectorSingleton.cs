
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
using System;


#region Usings, Namespaces, and Pragmas

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using OpenCog.Actions;
using OpenCog.Attributes;
using OpenCog.Entities;
using OpenCog.Embodiment;
using OpenCog.Extensions;
using OpenCog.Network;
using OpenCog.Utility;
using ImplicitFields = ProtoBuf.ImplicitFields;
using OCEmbodimentXMLTags = OpenCog.OCEmbodimentXMLTags;
using OCEmotionalExpression = OpenCog.OCEmotionalExpression;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using ScriptableObject = UnityEngine.ScriptableObject;
using OCID = System.Guid;
using UnityEngine;
using OpenCog.Utilities.Logging;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

//namespace OpenCog.Embodiment
//{

/// <summary>
/// The OpenCog OCConnector.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[@Serializable]
	
#endregion
public sealed class OCConnectorSingleton  :OCNetworkElement
{
	//---------------------------------------------------------------------------

	#region 							Basic Initialization Variables

	//---------------------------------------------------------------------------

	private bool _isInitialized = false; // Flag to check if the OAC to this avatar is alive.
	private bool _isLoaded = false;
	private bool _firstRun = true;
	private System.DateTime _lastUpdate = System.DateTime.Now;

	#endregion
	#region 							Variables recording Avatar

	// Basic attributes of this avatar.
	private string _baseID;    /** For example "NPC" */
	private string _brainID;   /** For example "OAC_NPC" */
	private string _name;		/** Not yet been used now */
	private string _type;		/** "pet" type by default. */
	private string _traits;	/** "princess" by default. */

	private string _settingsFilename = "embodiment";

	private string _masterID; // Define master's(owner's) information.
	private string _masterName;

	#endregion
	#region 							Variables recording  Map
	private string _mapName; 	// the name of the current map
	private OpenCog.Map.OCMap _map;
	private int _blockCountX; // The size of map.
	private int _blockCountY; // The size of map.
	private int _blockCountZ; // The size of map.
	private int _globalStartPositionX; // Map global X beginning position.
	private int _globalStartPositionY; // Map global Y beginning position.
	private int _globalStartPositionZ; // Map global Z beginning position.
	private int _globalFloorHeight; // Floor height in the minecraft-like world.
	private bool _isFirstSentMapInfo; // Flag to check if a valid map info(which should contain the info of the avatar itself) has been sent to OAC as a "handshake".

	#endregion 
	#region 							 Action Crontroller Variables
	private LinkedList<OCAction> _actionsList; // The list of actions that I am going to perform. The action plans are received from OAC.

	private string _currentPlanId; // The action plan id that is being performed currently.
	private string _currentDemandName; // Currently selected demand name
	private Dictionary<string, float> _feelingValueMap; // Store the feeling values of this avatar.
	private Dictionary<string, float> _demandValueMap; // Store the demand values of this avatar.
	private Dictionary<int, string> _perceptedAgents; // Other OC agents percepted. Record the pairs of their unity object id and brain id.
	
	private int _stateChangeActionCount = 0;
	private int _disappearActionCount = 0;
	private int _appearActionCount = 0;
	private int _moveActionCount = 0;
	private bool _actionStatusesUpdated = false;

	[SerializeField]
	private OCActionController
		_actionController;

	#endregion
	#region 								Connection Variables

	private List<OCMessage> _messagesToSend = new List<OCMessage>(); // The queue used to store the messages to be sent to OAC.
	private System.Object _messageSendingLock = new System.Object(); // The lock object used to sync the atomic sequence - get a timestamp, build and enqueue a message
	// Timer to send message in a given interval. We can implement a timer by using unity API - FixedUpdate().
	private float _messageSendingTimer = 0.0f;		/**< The timer used to control message sending. */
	private float _messageSendingInterval = 0.1f;	/**< The interval to send messages in the queue. */
	private int _msgCount = 0;

	private HashSet<string> _unavailableElements = new HashSet<string>();

	#endregion
	#region                                  Dispatch Flags For Listeners

	private const int dispatchNum = 12;

	private long[] dispatchTimes = new long[dispatchNum];
	public long[] DispatchTimes {get {return dispatchTimes;}}
	public void DispatchTimesClear(int which){if(which >=0 && which < dispatchNum)dispatchTimes[which] = 0;}
	public bool[] dispatchFlags = new bool[dispatchNum];

	//This list is not exhaustive. Feel free to add onto it as/if additional functionality is needed
	///<summary>A list of types of messages we may have dispatched to the Opencog side, whose firing times are stored in dispatchTimes/DispatchTimes</summary>
	public enum DispatchTypes:int //11
	{
		terrain,
		mapInfo,
		moveActionDone,
		actionAvailability,
		existingStates,
		speechContent,
		avatarSignalsAndTick,
		actionStatus,
		actionPlanSucceeded,
		actionPlanFailed,
		finishTerrain
	};
	#endregion
	#region                                  Reception Flags For Listeners

	private const int receptNum = 1;
	private long[] receptTimes = new long[receptNum];
	public long[] ReceptTimes {get {return receptTimes;}}
	public void ReceptTimesClear(int which){if(which >=0 && which < receptNum)receptTimes[which] = 0;}	
	public bool[] receptFlags = new bool[receptNum];

	//This list is not exhaustive. Feel free to add onto it as/if additional functionality is needed
	///<summary>A list of types of messages we may have recieved, whose firing times are stored in receptTimes/ReceptTimes</summary>
	public enum ReceptTypes:int //1
	{
		robotHasPlan
		
	};

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------

	/**
     * Accessor to this avatar's brain id. (a.k.a OAC_xxx)
     */
	public string BrainID
	{
		get { return _brainID; }
	}

	public bool IsInitialized // Old property: IsInit(), old member var: isOacAlive
	{
		get { return _isInitialized; }
	}

	public bool IsLoaded
	{
		get { return _isLoaded; }
	}

//		/**
//     * Accessor to this avatar's id. (a.k.a AVATAR_xxx)
//     */
//    public string ID
//    {
//        get { return _ID; }
//    }

	/**
     * Accessor to this avatar's name. 
     * Not yet been used.
     */
	public string Name
	{
		get { return _name; }
	}

	private OpenCog.Map.OCMap Map
	{
		get
		{
			if(!_map)
			{
				_map = OpenCog.Map.OCMap.Instance;//UnityEngine.GameObject.Find("Map").GetComponent<OpenCog.Map.OCMap>() as OpenCog.Map.OCMap;
			}

			return _map;
		}

	}
    
	/**
     * Accessor to this avatar master's id.
     */
	public string MasterID
	{
		get { return _masterID; }
	}
    
	/**
     * Accessor to feelingValueMap
     */
	public Dictionary<string, float> FeelingValueMap
	{
		get { return _feelingValueMap; }
	}
    
	/**
     * Accessor to demandValueMap
     */
	public Dictionary<string, float> DemandValueMap
	{
		get { return _demandValueMap; }
	}

	/**
     * Accessor to currentDemandName
     */
	public string CurrentDemandName
	{
		get { return _currentDemandName; }
	}

	/// <summary>
	/// Gets the singleton instance.
	/// </summary>
	/// <value>
	/// The instance of this singleton.
	/// </value>
	public new static OCConnectorSingleton Instance
	{
		get
		{
			return GetInstance<OCConnectorSingleton>();
		}
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	new void Update()
	{
		// Invoke base network element function to do networking stuffs in 
		// every frame.
		if(System.DateTime.Now.Subtract(_lastUpdate).TotalMilliseconds > 1000)
		{
			base.Update();	

			//this will only ever happen once. 
			if(_isInitialized && _actionStatusesUpdated == false)
			{
				UpdateActionStatuses();
				
				_actionStatusesUpdated = true;	
			}
			
			// Code below SHOULD be handled by physiologicalmodel in the future...it will also send the tickmessages, so it should be ok...
			// If not, uncomment the two lines below.
//			Dictionary<string, double> basicFactorMap = new Dictionary<string, double>();
//			SendAvatarSignalsAndTick(basicFactorMap);
			
			_lastUpdate = System.DateTime.Now;
		}
        

		//isOacAlive = isElementAvailable(myBrainId);
	}
    
	/**
     * This method is called by unity system in a fixed frequency,
     * so we can make a timer to do things we want.
     */
	void FixedUpdate()
	{
		
		_messageSendingTimer += UnityEngine.Time.fixedDeltaTime;
        
		if(_messageSendingTimer >= _messageSendingInterval)// && _isInitialized)
		{
			//UnityEngine.Debug.Log ("OCConnectorSingleton::FixedUpdate sending messages at " + System.DateTime.Now.ToString ("HH:mm:ss.fff"));
			SendMessages();
			_messageSendingTimer = 0.0f;
		}
	}
    
	/// <summary>
	/// A unity API function inherited from MonoBehavior.
	/// The deconstruction method executed when application quit.
	/// </summary>
	void OnApplicationQuit()
	{
		SaveAndExit();
	}

	override protected void Initialize()
	{
		dispatchFlags = new bool[dispatchNum];
	}


	public void SendMessages()
	{
		if(!_isInitialized)
		{
			//UnityEngine.Debug.Log ("OCConnectorSingleton::SendMessages: not initialized!!");
			return;
		} else
		{
			//UnityEngine.Debug.Log ("OCConnectorSingletong::SendMessages: initialized!");
		}
		
		if(_messagesToSend.Count > 0)
		{
			//UnityEngine.Debug.Log ("OCConnectorSingleton::SendMessages: " + _messagesToSend.Count + " to send.");
			List<OCMessage> localMessagesToSend;
			// copy messages to a local queue and clear the global sending queue.
			lock(_messagesToSend)
			{
				localMessagesToSend = new List<OCMessage>(_messagesToSend);
				_messagesToSend.Clear();
			} // lock
			
			int iMessageIndex = 1;

			foreach(OCMessage message in localMessagesToSend)
			{
				//UnityEngine.Debug.Log ("Processing message " + iMessageIndex + " of " + localMessagesToSend.Count + "...");
				
				if(message != null)
				{
					// Check if router and destination is available. If so, send the message. 
					// otherwise just ignore the message
					string routerId = OCConfig.Instance.get("ROUTER_ID", "ROUTER");
					if(!IsElementAvailable(routerId))
					{
						UnityEngine.Debug.Log(OCLogSymbol.DESTROY + "Router not available. Discarding message to '" +
							message.TargetID + "' of type '" + message.Type + "': " + message.ToString());
						continue;
					}
					if(!IsElementAvailable(message.TargetID))
					{
						System.Console.WriteLine(OCLogSymbol.DESTROY + "Destination not available. Discarding message to '" +
						//UnityEngine.Debug.Log(OCLogSymbol.DESTROY + "Destination not available. Discarding message to '" +
							message.TargetID + "' of type '" + message.Type + "': " + message.ToString());
						continue;
					}

					bool sendResult = SendMessage(message);

					if(message.Type != OCMessage.MessageType.TICK && !message.MessageContent.Contains("physiology"))
					{
						if(sendResult)
						{
							System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "Message sent from '" + message.SourceID + "' to '" + message.TargetID + "' of type '" + message.Type + "': " + message.ToString());
						} else
						{
							UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "Error sending message from '" + message.SourceID + "' to '" +
								message.TargetID + "' type '" + message.Type + "': " + message.ToString());
						}
					}
				} else
				{
					UnityEngine.Debug.LogError(OCLogSymbol.IMPOSSIBLE_ERROR + "A null message has been reported.");
				}
				
				iMessageIndex += 1;
				
			} // foreach
		}
//		else
//			UnityEngine.Debug.Log ("OCConnectorSingleton::SendMessages: Nothing to send.");
        
	}

	/**
	* Set the value of given demand
	* 
	* Abstract demands like certainty, competence and affiliation are updated by 
	* OpenCog (PsiDemandUpdaterAgents), which are received by OCConnector::parsePsiDemandElement. 
	* 
	* While other physiological demands like energy, integrity are updated by 
	* OCPhysiologicalModel in unity, which calls this function to update these demand values
	*/
	public void SetDemandValue(string demandName, float demandValue)
	{
		if(_demandValueMap != null)
		{
			_demandValueMap[demandName] = demandValue;
		}
	}

	//DEPRECATED: This function has been commented out to show it is never called
	/*public void SendBlockStructure(OpenCog.Map.OCBlockData startBlock, bool isToRecognize)
	{
		XmlDocument doc = new XmlDocument();
		XmlElement root = MakeXMLElementRoot(doc);
		string timestamp = GetCurrentTimestamp();
 
		XmlElement signal = (XmlElement)root.AppendChild(doc.CreateElement("block-structure-signal"));
		if(isToRecognize)
		{
			signal.SetAttribute("recognize-structure", "true");
	        
			signal.SetAttribute("startblock-x", startBlock.GlobalX.ToString());
			signal.SetAttribute("startblock-y", startBlock.GlobalY.ToString());
			signal.SetAttribute("startblock-z", startBlock.GlobalZ.ToString());
		}
		signal.SetAttribute("timestamp", timestamp);
	
		OCMessage message = OCMessage.CreateMessage(_ID, _brainID, OCMessage.MessageType.STRING, BeautifyXmlText(doc));
        
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"sending block structure signal: \n" + BeautifyXmlText(doc));
        
		lock(_messageSendingLock)
		{
			_messagesToSend.Add(message);
		}
	}
	*/

	/**
   * To be called when instantiating a new OCAvatar.
   * 
   * @param agentId The id of OCAvatar
   * @param agentName The name of OCAvatar
   * @param agentTraits The traits of OCAvatar
   * @param agentType The type of OCAvatar, "pet" by default
   * @param masterId The human player id who creates the OCAvatar
   * @param masterName The human player name who creates the OCAvatar
   * @param agentPort The local listening port of this OCConnector instance in order to communicate with OpenCog.
   * 
   * @return Result of the initialization action.
   */
	public bool InitAvatar(string agentName, string agentTraits, string agentType, string masterId, string masterName)
	{
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"OCConnectorSingleton::InitAvatar() has been called on behalf of " + agentName);
		// Initialize basic attributes.
		_baseID = agentName;//gameObject.GetInstanceID().ToString();
		_ID = "AVATAR_" + _baseID;
		_brainID = "OAC_" + _baseID;
		_name = agentName;
		_type = OCEmbodimentXMLTags.PET_OBJECT_TYPE;
		_traits = "Princess";
		_currentDemandName = "";

		//In general, messages are perceived through the router and sent to the OAC.
		//messages are of the form "Hey router, I'm _ID, tell  me corrspondant (_brainID) that I have this information for him;
    
		// Load settings from file.
		if(_settingsFilename.Length > 0)
		{
			TextAsset configFile = (TextAsset)Resources.Load(_settingsFilename);
			if(configFile != null)
			{
				OCConfig.Instance.LoadFromTextAsset(configFile);
			}

			//Load any important command line arguments *over and on top of* the text asset (which in this case, should command line arguments be present, will serve
			//as something of a default. 
			OCConfig.Instance.LoadFromCommandLine();
		}
    
		// Initialize NetworkElement
		base.InitializeNetworkElement(_ID);
    
		// Config master's settings.
		_masterID = masterId;
		_masterName = masterName;

		// Create action list.
		_actionsList = new LinkedList<OCAction>();

		_isFirstSentMapInfo = true;

		_feelingValueMap = new Dictionary<string, float>();
		_demandValueMap = new Dictionary<string, float>();
		_perceptedAgents = new Dictionary<int, string>();

		if(Map != null)
		{
			_mapName = Map.MapName;
			
			// If there are chuncks auto generated around the bounday, we should minus this boundary
			if(OCPerceptionCollector.HasBoundaryChuncks)
			{
				// Calculate the offset of the terrain.
				_blockCountX = OpenCog.Map.OCChunk.SIZE_X * Map.ChunkCountX;
				_blockCountY = OpenCog.Map.OCChunk.SIZE_Y * Map.ChunkCountY;

				// There is an invisible chunk at the edge of the terrain, so we should take count of it.
				_globalStartPositionX = (int)OpenCog.Map.OCChunk.SIZE_X;
				_globalStartPositionY = (int)OpenCog.Map.OCChunk.SIZE_Y;
			} else
			{
				// Calculate the offset of the terrain.
				_blockCountX = OpenCog.Map.OCChunk.SIZE_X * Map.ChunkCountX;
				_blockCountY = OpenCog.Map.OCChunk.SIZE_Y * Map.ChunkCountY;
	
				// There is an invisible chunk at the edge of the terrain, so we should take count of it.
				// I don't agree with this. Our map loads into 0,8,0 and 0,9,0. Though I don't see 0,9,0 here :(
				// Anyway...the lowest coords should be the 0's of the lowest chunks. So here goes:
				_globalStartPositionX = Map.Chunks.GetMinX() * OpenCog.Map.OCChunk.SIZE_X;
				_globalStartPositionY = Map.Chunks.GetMinY() * OpenCog.Map.OCChunk.SIZE_Y;
//		        _globalStartPositionX = 0;
//		        _globalStartPositionY = 0;
			}
			
			_blockCountZ = OpenCog.Map.OCChunk.SIZE_Z * Map.ChunkCountZ;
			_globalStartPositionZ = Map.Chunks.GetMinZ() * OpenCog.Map.OCChunk.SIZE_Z;
			// The floor height should be 1 unit larger than the block's z index.
			_globalFloorHeight = Map.FloorHeight;
		} else
		{
			/// TODO [UNTESTED]: I have no idea if the below code makes sense...if Map==null....
			_mapName = "unknown_map";
			_blockCountX = 128;
			_blockCountY = 128;
			_blockCountZ = 128;

			_globalStartPositionX = 0;
			_globalStartPositionY = 0;
			_globalStartPositionZ = 0;
			_globalFloorHeight = 0;
		}
		
		UnityEngine.Debug.Log(OCLogSymbol.CONNECTION + "OCConnectorSingleton reports: OpenCog will receive a world with globalStartPosition [" + _globalStartPositionX + ", " + _globalStartPositionY + ", " + _globalStartPositionZ + "] and blockCounts [" + _blockCountX + ", " + _blockCountY + ", " + _blockCountZ + "].");
    
		// Get action scheduler component.
		// TODO [LAKE]: old classes here. Lake needs to fix this.
		//_actionController = gameObject.GetComponent<OCActionController>() as OCActionController;
		// TODO [LAKE]: Removed due to new call structure for updating action statuses. Nothing to do really..just needs remembering.
		//OCActionController.globalActionCompleteEvent += HandleOtherAgentActionResult;

		//_isInitialized = true;

		return true;
	}

	public int getFloorHeight()
	{
		return _globalFloorHeight;
	}

	public IEnumerator ConnectOAC()
	{
		//only do this once. 
		if(!_firstRun) yield break;

		Debug.Log(OCLogSymbol.CONNECTION + "Attempting to connect OAC...");
		_firstRun = false;

		//FIRST STEP
		// First step, attempting connecting to the router 5 times which, at ~18 seconds per attempt, should take roughly 100 seconds.
		int timeout = 60;
		while(!base._isEstablished && timeout > 0)
		{
			yield return base.Connect().MoveNext();
			timeout--;
		}

		if(timeout <= 0)
		{
			Debug.LogError(OCLogSymbol.ERROR + "Connection attempt timed out.");
			yield break;
		} else
		{
			Debug.Log(OCLogSymbol.CLEARED + "Connection established.");
			yield return 0; //can be used by the caller of this coroutine to sense 'no errors'
		}

		//SECOND STEP
		// Second step, check if spawner is available to spawn an OAC instance.
		Debug.Log(OCLogSymbol.CONNECTION + "Waiting for spawner...");
		timeout = 60;
		do
		{
			if(IsElementAvailable(OCConfig.Instance.get("SPAWNER_ID")))
			{
				break;
			}
			yield return new UnityEngine.WaitForSeconds(1f);
			timeout--;

		} while(timeout > 0);

		if(timeout <= 0)
		{
			Debug.LogError(OCLogSymbol.ERROR + "Spawner is not available, OAC can not be launched.");
			yield break;
		} else
		{
			Debug.Log(OCLogSymbol.CLEARED + "Spawner obtained, OAC can be launched.");
			yield return 0;  //can be used by the caller of this coroutine to sense 'no errors'
		}

		//THIRD STEP
		// Third step, check if we've initialized (this is new code, and is based off of LoadMethods checking for _connector.IsInitialized)
		Debug.Log(OCLogSymbol.CONNECTION + "Waiting for OAC...");

		// Load the OAC by sending "load agent" command to spawner (this is not a coroutine)
		LoadOAC();

		timeout = 60;
		do
		{
			if(_isInitialized)
			{
				break;
			}

			yield return new UnityEngine.WaitForSeconds(1f);
			timeout--;
			
		} while(timeout > 0);

		if(timeout <= 0)
		{
			Debug.LogError(OCLogSymbol.ERROR + "OAC is not available, dying gracefully.");
			yield break;
		} else
		{
			Debug.Log(OCLogSymbol.CLEARED + "OAC initialized.");
			yield return 0;  //can be used by the caller of this coroutine to sense 'no errors'
		}
		
	}

	/**
   * Process messages that are delivered by router, overriding the method in
   * NetworkElement.
   *
   * @param message A message to be handled.
   *
   * @return false if not an exit command. (this is obsolete in this unity game, but
   * it is OK to keep it)
   */
	public override bool ProcessNextMessage(OCMessage message)
	{
//		UnityEngine.Debug.Log("OCConnectorSingleton::ProcessNextMessage: " + message.ToString());
    
		if(message.Type == OCMessage.MessageType.FEEDBACK)
		{
			// e.g. we can append the information in the console.
			Debug.LogError("Feedback " + message.ToString());
		} 
		else if(message.ToString().StartsWith(SUCCESS_LOAD))
		{
			// Format: SUCCESS LOAD NetworkElement_id avatar_id
			char[] separator = { ' ' };
			string[] tokens = message.ToString().Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
            
			string neId = tokens[2];
			Debug.Log(OCLogSymbol.CLEARED + "Successfully loaded '" + neId + "'.");
			_isInitialized = true;//_isLoaded = true;
		} 
		else if(message.ToString().StartsWith(SUCCESS_UNLOAD))
		{
			char[] separator = {' '};
			string[] tokens = message.ToString().Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

			// Format: SUCCESS UNLOAD NetworkElement_id avatar_id
			string neId = tokens[2];
			UnityEngine.Debug.Log(OCLogSymbol.CLEARED + "!!! Successfully unloaded '" + neId + "'.");
			_isInitialized = false;//_isLoaded = false;
		} 
		else
		{
//			UnityEngine.Debug.Log ("Processing an interesting message type!");
			// Get the plain text of this message(in XML format) and parse it.
			if(_isInitialized)//if(_isLoaded)
			{
				// Parse the message only when oac is ready.
				ParseXML(message.ToString());
			}
		}
		return false;
	}

// //TODO: Make sure the action status updating process initiated by OCConnector also covers the function below.
//	public void handleActionResult(OCAction action)
//  {
//      bool result = (action.Status == OCAction.ActionStatus.SUCCESS);
//
//      // Send action status to my brain.
//      SendActionStatus(_currentPlanId, action, result);
//      //Debug.LogWarning("Action plan " + this.currentPlanId + " sequence " + action.Sequence + " status sent.");
//      lock (_actionsList)
//      {
//      	_actionsList.Remove(action);
//      }
//  }

	public void UpdateActionStatuses()
	{
		// GetComponents of type ActionController

		// Call GetAndClearFinishedActions on ActionControllers to match the old situation

		// Call GetAndClearAllActions on ActionControlles in the new situation

		// Loop through Actions...or clones of them...or data structures for them...
	
		System.Console.WriteLine(OCLogSymbol.CONNECTION + "OCConnectorSingleton.UpdateActionStatuses()");
		string timestamp = GetCurrentTimestamp();
		// Create a xml document
		XmlDocument doc = new XmlDocument();
		XmlElement root = MakeXMLElementRoot(doc);

		XmlElement avatarSignal = (XmlElement)root.AppendChild(doc.CreateElement("avatar-signal"));
		avatarSignal.SetAttribute("id", this.BrainID);
		avatarSignal.SetAttribute("timestamp", timestamp);
        
		// Extract actions from action list
//        foreach (ActionSummary action in actionList)
//		{
//			// Set the action name
//	        string ocActionName = ActionManager.getOCActionNameFromMap(action.actionName);
//	        
//			// check if the method name has a mapping to opencog action name.
//			if (ocActionName == null) continue;
//			
//	        XmlElement actionElement = (XmlElement)avatarSignal.AppendChild(doc.CreateElement(EmbodimentXMLTags.ACTION_AVAILABILITY_ELEMENT));
//	        
//	        actionElement.SetAttribute("name", ocActionName);
//	        
//	        // Actions like "walk", "jump" are built-in naturally, they don't need an external target to act on.
//	        if (action.objectID != gameObject.GetInstanceID())
//			{
//		        // Set the action target
//		        actionElement.SetAttribute("target", action.objectID.ToString());
//		        
//		        // Set the action target type
//				// currently we only process the avatar and ocobject type, other types in EmbodimentXMLTages can is to be added when needed.
//				// if you add other types such as BLOCK_OBJECT_TYPE, you should also modify PAI::processAgentAvailability in opencog
//				string targetType = action.actionObject.tag;
//				if (targetType == "OCA" || targetType == "Player")// it's an avatar
//					actionElement.SetAttribute("target-type", EmbodimentXMLTags.AVATAR_OBJECT_TYPE);
//				else if (targetType == "OCObject") // it's an object
//					actionElement.SetAttribute("target-type", EmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
//				else
//					Debug.LogError("Error target type: " + targetType + " in action: " + action.actionName);
//			}
//							
//	        actionElement.SetAttribute("available", available ? "true" : "false");
//		}

		OCMessage message = OCMessage.CreateMessage(_ID, _brainID, OCMessage.MessageType.STRING, BeautifyXmlText(doc));

		lock(_messagesToSend)
		{
			_messagesToSend.Add(message);
		}

	}
  
	public void HandleOtherAgentActionResult(OCActionPlanStep step, bool status)
	{
		// don't report actions that came from us.
		// don't report actions without an action summary (these are from trying
		// to do non-existant actions).
		///FIXME [ERROR CHECKING]: Find a different way to check for this...
//    if (ar.avatar == gameObject.GetComponent<Avatar>() || ar.action == null) {
//        //Debug.LogWarning("skipping action result from " + ar.avatar);
//        return;
//    }

		// the corresponding process within OpenCog's embodiment system is in PAI::processAgentActionWithParameters

		string timestamp = GetCurrentTimestamp();
		XmlDocument doc = new XmlDocument();
		XmlElement root = MakeXMLElementRoot(doc);

		XmlElement agentSignal = (XmlElement)root.AppendChild(doc.CreateElement("agent-signal"));
		agentSignal.SetAttribute("id", gameObject.GetInstanceID().ToString());
		agentSignal.SetAttribute("type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
		agentSignal.SetAttribute("timestamp", timestamp);
		XmlElement actionElement = (XmlElement)agentSignal.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ACTION_ELEMENT));

		// note that the name and the action-instance-name are different
		// ie: name = kick , while action-instance-name = kick2342
		actionElement.SetAttribute("name", step.Arguments.ActionName);
		actionElement.SetAttribute("action-instance-name", step.Arguments.ActionName + step.ID);
		
		//bool result = (status == ActionResult.Status.SUCCESS ? true : false);
		actionElement.SetAttribute("result-state", status.ToString().ToLower()); //successful or failed
		if(step.Arguments.Source.GetInstanceID() == step.Arguments.StartTarget.GetInstanceID())
		{
			actionElement.SetAttribute("target", _brainID);
		} else
		{
			actionElement.SetAttribute("target", step.Arguments.StartTarget.GetInstanceID().ToString());
		}

		// currently we only process the avatar and ocobject type, other types in EmbodimentXMLTages can is to be added when needed.
		// if you add other types such as BLOCK_OBJECT_TYPE, you should also modify PAI::processAgentActionWithParameters in opencog
		string targetType = step.Arguments.StartTarget.tag;
		if(targetType == "OCA" || targetType == "Player" || targetType == "OCAGI")
		{// it's an avatar
			actionElement.SetAttribute("target-type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
		} else if(targetType == "OCObject")
		{ // it's an object
			actionElement.SetAttribute("target-type", OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
		} else
		{
			Debug.LogWarning("Error target type: " + targetType + " in action: " + step.Arguments.ActionName);
		}
				 
		XmlElement param = (XmlElement)actionElement.AppendChild(doc.CreateElement("param"));

		if(step.Arguments.EndTarget != null)
		{
			if(step.Arguments.EndTarget.tag == "OCObject" || step.Arguments.EndTarget.tag == "Player")
			{
				param.SetAttribute("type", "entity");
				param.SetAttribute("name", step.Arguments.EndTarget.name);
				XmlElement entityElement = (XmlElement)param.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ENTITY_ELEMENT));
				entityElement.SetAttribute(OCEmbodimentXMLTags.ID_ATTRIBUTE, step.Arguments.EndTarget.GetInstanceID().ToString());

				if(step.Arguments.EndTarget.tag == "OCObject")
				{
					entityElement.SetAttribute(OCEmbodimentXMLTags.TYPE_ATTRIBUTE, OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
				} else
				{
					entityElement.SetAttribute(OCEmbodimentXMLTags.TYPE_ATTRIBUTE, OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
				}
			} else
			{
				UnityEngine.Vector3 vec = step.Arguments.EndTarget.transform.position;
				param.SetAttribute("type", "vector");
				param.SetAttribute("name", "position");
				XmlElement vectorElement = (XmlElement)param.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.VECTOR_ELEMENT));
				vectorElement.SetAttribute(OCEmbodimentXMLTags.X_ATTRIBUTE, vec.x.ToString());
				vectorElement.SetAttribute(OCEmbodimentXMLTags.Y_ATTRIBUTE, vec.y.ToString());
				vectorElement.SetAttribute(OCEmbodimentXMLTags.Z_ATTRIBUTE, vec.z.ToString());
			}
		}





//		// we can only process the parameter type defined in class ActionParamType both in opencog and unity
//		// currently they are : boolean, int, float, string, vector, rotation, entity
//		// also see opencog/opencog/embodiment/control/perceptionActionInterface/BrainProxyAxon.xsd
//    ArrayList paramList = ar.parameters;
//      
//    if (paramList != null) 
//		{
//		
//			int i;
//			if (targetType == "OCA" || targetType == "Player")
//			{
//				if (ar.action.objectID == ar.avatar.gameObject.GetInstanceID())
//					i = -1;
//				else 
//					i = 0;
//			}
//	    else
//			{
//				i = 0;
//			}
//				
//     foreach (System.Object obj in paramList)
//    	{
//      	XmlElement param = (XmlElement)actionElement.AppendChild(doc.CreateElement("param"));
//			
//				// the first param in pinfo is usually the avator does this action, so we just skip it
//				string paratype = obj.GetType().ToString();
//				if (paratype == "System.Int32") // it's a int
//				{
//					param.SetAttribute("type", "int");
//					param.SetAttribute("name", ar.action.pinfo[i+1].Name);
//					param.SetAttribute("value", obj.ToString());
//				}
//				else if (paratype == "System.Single") // it's a float
//				{
//					param.SetAttribute("type", "float");
//					param.SetAttribute("name", ar.action.pinfo[i+1].Name);
//					param.SetAttribute("value", obj.ToString());
//				}
//				else if (paratype == "System.Boolean") // it's a bool
//				{
//					param.SetAttribute("type", "boolean");
//					param.SetAttribute("name", ar.action.pinfo[i+1].Name);
//				
//					param.SetAttribute("value", obj.ToString().ToLower());
//				}
//				else if (paratype == "System.String")// it's a string
//				{
//					param.SetAttribute("type", "string");
//					param.SetAttribute("name", ar.action.pinfo[i+1].Name);
//					param.SetAttribute("value", obj.ToString());
//				}
//				// it's an entity, we only process the ActionTarget, 
//				// if your parameter is an Entiy, please change it into ActionTarget type first
//				else if (paratype == "ActionTarget") 
//				{
//					param.SetAttribute("type", "entity");
//					param.SetAttribute("name", ar.action.pinfo[i+1].Name);
//					XmlElement entityElement = (XmlElement)param.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ENTITY_ELEMENT));
//					OCAction.Target entity = obj as OCAction.Target;
//					entityElement.SetAttribute(OCEmbodimentXMLTags.ID_ATTRIBUTE, entity.id.ToString());
//					entityElement.SetAttribute(OCEmbodimentXMLTags.TYPE_ATTRIBUTE, entity.type);
//					
//					// currently it seems not use of OWNER_ID_ATTRIBUTE and OWNER_NAME_ATTRIBUTE, we just skip them
//				}
//				else if ( paratype == "UnityEngine.Vector3") // it's an vector
//				{   
//					UnityEngine.Vector3 vec = (Vector3)obj ;
//					param.SetAttribute("type", "vector");
//					param.SetAttribute("name", ar.action.pinfo[i+1].Name);
//					XmlElement vectorElement = (XmlElement)param.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.VECTOR_ELEMENT));
//					vectorElement.SetAttribute(OCEmbodimentXMLTags.X_ATTRIBUTE, vec.x.ToString());
//					vectorElement.SetAttribute(OCEmbodimentXMLTags.Y_ATTRIBUTE, vec.y.ToString());
//					vectorElement.SetAttribute(OCEmbodimentXMLTags.Z_ATTRIBUTE, vec.z.ToString());
//					
//				}
//				else if ( paratype ==  "IntVect") // it's an vector
//				{   
//					Vector3i vec = (Vector3i)obj ;
//					param.SetAttribute("type", "vector");
//					param.SetAttribute("name", ar.action.pinfo[i+1].Name);
//					XmlElement vectorElement = (XmlElement)param.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.VECTOR_ELEMENT));
//					vectorElement.SetAttribute(OCEmbodimentXMLTags.X_ATTRIBUTE, (vec.X + 0.5f).ToString());
//					vectorElement.SetAttribute(OCEmbodimentXMLTags.Y_ATTRIBUTE, (vec.Y + 0.5f).ToString());
//					vectorElement.SetAttribute(OCEmbodimentXMLTags.Z_ATTRIBUTE, (vec.Z + 0.5f).ToString());
//					
//				}
//				// todo: we don't have a rotation type
//				else 
//				{
//					// we can only process the type define in ActionParamType
//					continue;
//				}
//				              
//	      i++;                
//	    }
//    }

		OCMessage message = new OCMessage(_ID, _brainID, OCMessage.MessageType.STRING, BeautifyXmlText(doc));

		Debug.LogWarning(OCLogSymbol.WARN +"sending action result from " + _ID + "\n" + BeautifyXmlText(doc));

		lock(_messagesToSend)
		{
			_messagesToSend.Add(message);
		}
	}
	
	
	// When isAppear is true, it's an appear action, if false, it's a disappear action 
	public void HandleObjectAppearOrDisappear(string objectID, string objectType, bool isAppear)
	{
		// TODO [TASK][MINOR]: Add other things we shouldn't report.
		if(objectID == ID.ToString())
		{
			return;
		}

		if(isAppear)
		{
			UnityEngine.Debug.Log(OCLogSymbol.RUNNING + "Reporting appearance of object with ID '" + objectID + "' of type '" + objectType + "'.");
		} else
		{
			UnityEngine.Debug.Log(OCLogSymbol.RUNNING + "Reporting disappearance of object with ID '" + objectID + "' of type '" + objectType + "'.");
		}
		
		string timestamp = GetCurrentTimestamp();
		XmlDocument doc = new XmlDocument();
		XmlElement root = MakeXMLElementRoot(doc);
			
		XmlElement agentSignal = (XmlElement)root.AppendChild(doc.CreateElement("agent-signal"));
		agentSignal.SetAttribute("id", objectID.ToString());
			
		string targetType;

		if(objectType == "OCA" || objectType == "Player")
		{// it's an avatar
			targetType = OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE;
		} else
		{ // it's an object
			targetType = OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE;
		}
			
		agentSignal.SetAttribute("type", "object");
		agentSignal.SetAttribute("timestamp", timestamp);
		XmlElement actionElement = (XmlElement)agentSignal.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ACTION_ELEMENT));
	        
		// note that the name and the action-instance-name are different
		// ie: name = kick , while action-instance-name = kick2342
		if(isAppear)
		{
			actionElement.SetAttribute("name", "appear");
			actionElement.SetAttribute("action-instance-name", "appear" + (++_appearActionCount).ToString());
		} else
		{
			actionElement.SetAttribute("name", "disappear");
			actionElement.SetAttribute("action-instance-name", "disappear" + (++_disappearActionCount).ToString());


			// 'remove' seems to only get processed for blocks under mapinfo changes;
			//right now we're pushing an object change owed to battery's dual block-object citizenship. 
			//if(targetType == OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE)
				//actionElement.SetAttribute("remove", "true");
		}
	
		actionElement.SetAttribute("result-state", "true"); 
		
		actionElement.SetAttribute("target", objectID.ToString());
		actionElement.SetAttribute("target-type", targetType);		
		
		OCMessage message = OCMessage.CreateMessage(_ID, _brainID, OCMessage.MessageType.STRING, BeautifyXmlText(doc));
	   	
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"sending state change of " + objectID + "\n" + BeautifyXmlText(doc));
	   	
		lock(_messagesToSend)
		{
			_messagesToSend.Add(message);
		}		
		
	}
	
	public void SendMoveActionDone(UnityEngine.GameObject obj, UnityEngine.Vector3 startPos, UnityEngine.Vector3 endPos)
	{
		if(obj == GameObject.FindGameObjectWithTag("OCAGI"))
		{
			return;
		}
		
		string timestamp = GetCurrentTimestamp();
		XmlDocument doc = new XmlDocument();
		XmlElement root = MakeXMLElementRoot(doc);

		XmlElement agentSignal = (XmlElement)root.AppendChild(doc.CreateElement("agent-signal"));
		agentSignal.SetAttribute("id", obj.GetInstanceID().ToString());
    
		agentSignal.SetAttribute("timestamp", timestamp);
		XmlElement actionElement = (XmlElement)agentSignal.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ACTION_ELEMENT));
        
		// note that the name and the action-instance-name are different
		// ie: name = kick , while action-instance-name = kick2342
		actionElement.SetAttribute("name", "move");
		actionElement.SetAttribute("action-instance-name", "move" + (++_moveActionCount));
		actionElement.SetAttribute("result-state", "true"); //successful or failed
		
		actionElement.SetAttribute("target", obj.GetInstanceID().ToString());

		string targetType = obj.tag;
		if(targetType == "OCA" || targetType == "Player")// it's an avatar
		{
			agentSignal.SetAttribute("type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
			actionElement.SetAttribute("target-type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
		} else// it's an object
		{
			agentSignal.SetAttribute("type", OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
			actionElement.SetAttribute("target-type", OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
		}
	
		XmlElement paramOld = (XmlElement)actionElement.AppendChild(doc.CreateElement("param"));
		paramOld.SetAttribute("name", "startPosition");
		XmlElement paramNew = (XmlElement)actionElement.AppendChild(doc.CreateElement("param"));
		paramNew.SetAttribute("name", "endPosition");
		
		paramOld.SetAttribute("type", "vector");
		XmlElement oldVectorElement = (XmlElement)paramOld.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.VECTOR_ELEMENT));
		oldVectorElement.SetAttribute(OCEmbodimentXMLTags.X_ATTRIBUTE, startPos.x.ToString());
		oldVectorElement.SetAttribute(OCEmbodimentXMLTags.Y_ATTRIBUTE, startPos.y.ToString());
		oldVectorElement.SetAttribute(OCEmbodimentXMLTags.Z_ATTRIBUTE, startPos.z.ToString());
		
		paramNew.SetAttribute("type", "vector");
		XmlElement newVectorElement = (XmlElement)paramNew.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.VECTOR_ELEMENT));
		newVectorElement.SetAttribute(OCEmbodimentXMLTags.X_ATTRIBUTE, endPos.x.ToString());
		newVectorElement.SetAttribute(OCEmbodimentXMLTags.Y_ATTRIBUTE, endPos.y.ToString());
		newVectorElement.SetAttribute(OCEmbodimentXMLTags.Z_ATTRIBUTE, endPos.z.ToString());
		
		OCMessage message = OCMessage.CreateMessage(_ID, _brainID, OCMessage.MessageType.STRING, BeautifyXmlText(doc));
        
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"sending move action result: \n" + BeautifyXmlText(doc));
        
		lock(_messagesToSend)
		{
			_messagesToSend.Add(message);
		}

		//set a time so we can record it laterz!
		if(dispatchFlags[(int)DispatchTypes.moveActionDone])
			dispatchTimes[(int)DispatchTypes.moveActionDone] = System.DateTime.Now.Ticks;
		

	}
	
	// Send all the existing states of object to opencog when the robot is loaded 
	// it will be processed in opencog the same as opencog::pai::InitStateInfo()
	public void SendExistingStates(UnityEngine.GameObject obj, string stateName, string valueType, System.Object stateValue)
	{
		string timestamp = GetCurrentTimestamp();
		System.Diagnostics.Debug.Assert(obj != null && stateName != "" && valueType != "");
		XmlDocument doc = new XmlDocument();
		XmlElement root = MakeXMLElementRoot(doc);
		
		string id = obj.GetInstanceID().ToString();
				
		XmlElement StateSignal = (XmlElement)root.AppendChild(doc.CreateElement("state-info"));
		StateSignal.SetAttribute("object-id", id);

		if(obj.tag == "OCA" || obj.tag == "Player")
		{// it's an avatar
			StateSignal.SetAttribute("object-type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
		} else
		{ // it's an object
			StateSignal.SetAttribute("object-type", OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
		}

		StateSignal.SetAttribute("state-name", stateName);
		StateSignal.SetAttribute("timestamp", timestamp);
		
		XmlElement valueElement = (XmlElement)StateSignal.AppendChild(doc.CreateElement("state-value"));
						
		if(valueType == "System.Int32") // it's a int
		{
			valueElement.SetAttribute("type", "int");
			valueElement.SetAttribute("value", stateValue.ToString());
		} else if(valueType == "System.Single") // it's a float
		{
			valueElement.SetAttribute("type", "float");
			valueElement.SetAttribute("value", stateValue.ToString());			
		} else if(valueType == "System.Boolean") // it's a bool
		{
			valueElement.SetAttribute("type", "boolean");
			valueElement.SetAttribute("value", stateValue.ToString().ToLower());
		} else if(valueType == "System.String")// it's a string
		{
			valueElement.SetAttribute("type", "string");
			valueElement.SetAttribute("value", stateValue as string);
			
		} else if(valueType == "UnityEngine.GameObject")
		{
			valueElement.SetAttribute("type", "entity");
			XmlElement entityElement = (XmlElement)valueElement.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ENTITY_ELEMENT));
			MakeEntityElement(stateValue as UnityEngine.GameObject, entityElement);
		} else if(valueType == "UnityEngine.Vector3") // it's an vector
		{   
			valueElement.SetAttribute("type", "vector");
			UnityEngine.Vector3 vec = (UnityEngine.Vector3)stateValue;
			XmlElement vectorElement = (XmlElement)valueElement.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.VECTOR_ELEMENT));
			vectorElement.SetAttribute(OCEmbodimentXMLTags.X_ATTRIBUTE, vec.x.ToString());
			vectorElement.SetAttribute(OCEmbodimentXMLTags.Y_ATTRIBUTE, vec.y.ToString());
			vectorElement.SetAttribute(OCEmbodimentXMLTags.Z_ATTRIBUTE, vec.z.ToString());
		}
		
		// todo: we don't have a rotation type
		else
		{
			// we can only process the type define in ActionParamType
			Debug.LogWarning(OCLogSymbol.WARN +"Unexcepted type: " + valueType + " in OCConnector::handleObjectStateChange!");
			return;
		}

		OCMessage message = OCMessage.CreateMessage(_ID, _brainID, OCMessage.MessageType.STRING, BeautifyXmlText(doc));
    
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"sending state change of " + obj + "\n" + BeautifyXmlText(doc));
    
		lock(_messagesToSend)
		{
			_messagesToSend.Add(message);
		}

		//set a time so we can record it laterz!
		if(dispatchFlags[(int)DispatchTypes.existingStates])
			dispatchTimes[(int)DispatchTypes.existingStates] = System.DateTime.Now.Ticks;
	}	
	
	// we handle object state change as an "stateChange" action, and send it to the opencog via "agent-signal"
	// it will be processed in opencog the same as handleOtherAgentActionResult
	public void HandleObjectStateChange(UnityEngine.GameObject obj, string stateName, string valueType, System.Object oldValue, System.Object newValue, string blockId = "")
	{
		if(obj == GameObject.FindGameObjectWithTag("OCAGI"))
		{
			return;
		}
		
		string timestamp = GetCurrentTimestamp();
		XmlDocument doc = new XmlDocument();
		XmlElement root = MakeXMLElementRoot(doc);
		
		string id;
		if(blockId != "")
		{
			id = blockId;
		} else
		{
			id = obj.GetInstanceID().ToString();
		}
			
		string targetType;
		if(blockId != "")
		{
			targetType = OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE;
		} else
		{
			targetType = obj.tag;
		}
		
		XmlElement agentSignal = (XmlElement)root.AppendChild(doc.CreateElement("agent-signal"));
		agentSignal.SetAttribute("id", id);
    
		agentSignal.SetAttribute("timestamp", timestamp);
		XmlElement actionElement = (XmlElement)agentSignal.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ACTION_ELEMENT));
        
		// note that the name and the action-instance-name are different
		// ie: name = kick , while action-instance-name = kick2342
		actionElement.SetAttribute("name", "stateChange");
		actionElement.SetAttribute("action-instance-name", "stateChange" + (++_stateChangeActionCount).ToString());

		actionElement.SetAttribute("result-state", "true"); 
		
		actionElement.SetAttribute("target", id);
		
		
		if(targetType == "OCA" || targetType == "Player")// it's an avatar
		{
			agentSignal.SetAttribute("type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
			actionElement.SetAttribute("target-type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
		} else // it's an object
		{
			agentSignal.SetAttribute("type", OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
			actionElement.SetAttribute("target-type", OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
		}
			
		XmlElement paramStateName = (XmlElement)actionElement.AppendChild(doc.CreateElement("param"));
		paramStateName.SetAttribute("name", "stateName");
		paramStateName.SetAttribute("type", "string");
		paramStateName.SetAttribute("value", stateName);

		XmlElement paramOld = (XmlElement)actionElement.AppendChild(doc.CreateElement("param"));
		paramOld.SetAttribute("name", "OldValue");
		XmlElement paramNew = (XmlElement)actionElement.AppendChild(doc.CreateElement("param"));
		paramNew.SetAttribute("name", "NewValue");
		
				
		if(valueType == "System.Int32") // it's a int
		{
			paramOld.SetAttribute("type", "int");
			paramOld.SetAttribute("value", oldValue.ToString());
			
			paramNew.SetAttribute("type", "int");
			paramNew.SetAttribute("value", newValue.ToString());
		} else if(valueType == "System.Single") // it's a float
		{
			paramOld.SetAttribute("type", "float");
			paramOld.SetAttribute("value", oldValue.ToString());
			
			paramNew.SetAttribute("type", "float");
			paramNew.SetAttribute("value", newValue.ToString());
		} else if(valueType == "System.Boolean") // it's a bool
		{
			paramOld.SetAttribute("type", "boolean");
			paramOld.SetAttribute("value", oldValue.ToString().ToLower());
			
			paramNew.SetAttribute("type", "boolean");
			paramNew.SetAttribute("value", newValue.ToString().ToLower());
		} else if(valueType == "System.String")// it's a string
		{
			paramOld.SetAttribute("type", "string");
			paramOld.SetAttribute("value", oldValue as string);
			
			paramNew.SetAttribute("type", "string");
			paramNew.SetAttribute("value", newValue as string);
		} else if((valueType == "UnityEngine.GameObject") || (valueType == "Avatar"))
		{
			paramOld.SetAttribute("type", "entity");
			XmlElement oldEntityElement = (XmlElement)paramOld.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ENTITY_ELEMENT));
			MakeEntityElement(oldValue as UnityEngine.GameObject, oldEntityElement);
			
			paramNew.SetAttribute("type", "entity");
			XmlElement newEntityElement = (XmlElement)paramNew.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ENTITY_ELEMENT));
			MakeEntityElement(newValue as UnityEngine.GameObject, newEntityElement);
		} else if(valueType == "UnityEngine.Vector3") // it's an vector
		{   
			paramOld.SetAttribute("type", "vector");
			UnityEngine.Vector3 vec = (UnityEngine.Vector3)oldValue;
			XmlElement oldVectorElement = (XmlElement)paramOld.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.VECTOR_ELEMENT));
			oldVectorElement.SetAttribute(OCEmbodimentXMLTags.X_ATTRIBUTE, vec.x.ToString());
			oldVectorElement.SetAttribute(OCEmbodimentXMLTags.Y_ATTRIBUTE, vec.y.ToString());
			oldVectorElement.SetAttribute(OCEmbodimentXMLTags.Z_ATTRIBUTE, vec.z.ToString());
			
			paramNew.SetAttribute("type", "vector");
			vec = (UnityEngine.Vector3)newValue;
			XmlElement newVectorElement = (XmlElement)paramNew.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.VECTOR_ELEMENT));
			newVectorElement.SetAttribute(OCEmbodimentXMLTags.X_ATTRIBUTE, vec.x.ToString());
			newVectorElement.SetAttribute(OCEmbodimentXMLTags.Y_ATTRIBUTE, vec.y.ToString());
			newVectorElement.SetAttribute(OCEmbodimentXMLTags.Z_ATTRIBUTE, vec.z.ToString());			

		}
		// todo: we don't have a rotation type
		else
		{
			// we can only process the type define in ActionParamType
			Debug.LogWarning(OCLogSymbol.WARN +"Unexcepted type: " + valueType + " in OCConnector::handleObjectStateChange!");
		}

		OCMessage message = OCMessage.CreateMessage(_ID, _brainID, OCMessage.MessageType.STRING, BeautifyXmlText(doc));
    
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"sending state change of " + obj + "\n" + BeautifyXmlText(doc));
    
		lock(_messagesToSend)
		{
			_messagesToSend.Add(message);
		}		
	}

	/// <summary>
	///	Send the availability status of an action to OAC. It is a mechanism to notify
	/// OAC whether or not a specific action can be performed currently.
	/// e.g. When avatar is close enough to a food stuff, it can pick it up, otherwise,
	/// the "pick up" action is not available.
	/// </summary>
	/// <param name="action">
	/// A <see cref="ActionSummary"/>
	/// </param>
	/// <param name="available">
	/// whether or not the action is available.
	/// </param>
	public void SendActionAvailability(List<OCAction> actionList, bool available)
	{
		string timestamp = GetCurrentTimestamp();
		// Create a xml document
		XmlDocument doc = new XmlDocument();
		XmlElement root = MakeXMLElementRoot(doc);

		XmlElement avatarSignal = (XmlElement)root.AppendChild(doc.CreateElement("avatar-signal"));
		avatarSignal.SetAttribute("id", _brainID);
		avatarSignal.SetAttribute("timestamp", timestamp);
    
		// Extract actions from action list
		foreach(OCAction action in actionList)
		{
			// Set the action name
			string ocActionName = OCActionController.GetOCActionNameFromMap(action.FullName);

			// check if the method name has a mapping to opencog action name.
			if(ocActionName == null)
			{
				continue;
			}
			
			System.Xml.XmlElement actionElement = (XmlElement)avatarSignal.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ACTION_AVAILABILITY_ELEMENT));
	        
			actionElement.SetAttribute("name", ocActionName);

			// DEPRECATED: Our new actions won't have targets, so we can skip this whole section.

			// Actions like "walk", "jump" are built-in naturally, they don't need an external target to act on.
//	    if (action.Target != gameObject.GetInstanceID())
//			{
//		    // Set the action target
//		    actionElement.SetAttribute("target", action.objectID.ToString());
//		        
//		    // Set the action target type
//				// currently we only process the avatar and ocobject type, other types in EmbodimentXMLTages can is to be added when needed.
//				// if you add other types such as BLOCK_OBJECT_TYPE, you should also modify PAI::processAgentAvailability in opencog
//				string targetType = action.actionObject.tag;
//				if (targetType == "OCA" || targetType == "Player")// it's an avatar
//					actionElement.SetAttribute("target-type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
//				else if (targetType == "OCObject") // it's an object
//					actionElement.SetAttribute("target-type", OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
//				else
//					Debug.LogError("Error target type: " + targetType + " in action: " + action.actionName);
//			}
							
			actionElement.SetAttribute("available", available ? "true" : "false");
		}
		
		OCMessage message = OCMessage.CreateMessage(_ID, _brainID, OCMessage.MessageType.STRING, BeautifyXmlText(doc));

		lock(_messagesToSend)
		{
			_messagesToSend.Add(message);
		}

		//set a time so we can record it laterz!
		if(dispatchFlags[(int)DispatchTypes.actionAvailability])
			dispatchTimes[(int)DispatchTypes.actionAvailability] = System.DateTime.Now.Ticks;
	}

	/**
   *  Add the map-info to message sending queue, yet we use the verb "send" here,
   *  because that's what we intend to do.
   */
	public void SendMapInfoMessage(List<OCObjectMapInfo> newMapInfoSeq, bool isFirstTimePerceptMapObjects= false)
	{
		// No new map info to send.
		if(newMapInfoSeq.Count == 0)
		{
			return;
		}

		LinkedList<OCObjectMapInfo> localMapInfo = new LinkedList<OCObjectMapInfo>(newMapInfoSeq);

		// If information of avatar itself is in the list, then put it at the first position.
		bool foundAvatarId = false;
		foreach(OCObjectMapInfo objMapInfo in localMapInfo)
		{
			if(objMapInfo.ID == null)
			{
				UnityEngine.Debug.Log("Damn, objectMapInfo with ID == null....");
			} else
			{
				if(objMapInfo.ID.Equals(_brainID))
				{
					//UnityEngine.Debug.Log ("OCConnectorSingleton::SendMapInfoMessage: objMapInfo.ID.Equals(_brainID), moving its objectmapinfo to first position...");
					localMapInfo.Remove(objMapInfo);
					localMapInfo.AddFirst(objMapInfo);
					foundAvatarId = true;
					break;
				}		
//			else
//				UnityEngine.Debug.Log ("OCConnectorSingleton::SendMapInfoMessage: objMapInfo.ID.Equals(_brainID) == false");
			}
		} // foreach
      
		if(!foundAvatarId && _isFirstSentMapInfo)
		{
			// First <map-info> message should contain information about the OCAvatar itself
			// If it is not there, skip this.
			Debug.LogWarning(OCLogSymbol.WARN +"Skipping first map-info message because it " +
				"does not contain info about the avatar itself!");
			return;
		}
         
		OCMessage message = SerializeMapInfo(new List<OCObjectMapInfo>(localMapInfo), "map-info", "map-data", isFirstTimePerceptMapObjects);

		lock(_messagesToSend)
		{
			_messagesToSend.Add(message);
		} // lock

		// First map info message has been sent.
		_isFirstSentMapInfo = false;

		//set a time so we can record it laterz!
		if(dispatchFlags[(int)DispatchTypes.mapInfo])
		{
			dispatchTimes[(int)DispatchTypes.mapInfo] = System.DateTime.Now.Ticks;
		}


	}

	/**
   * Add the terrain info message to message sending queue.
   */
	public void SendTerrainInfoMessage(List<OCObjectMapInfo> terrainInfoSeq, bool isFirstTimePerceptTerrain= false)
	{
		// No new map info to send.
		if(terrainInfoSeq.Count == 0)
		{
			return;
		}
		
		OCMessage message = SerializeMapInfo(terrainInfoSeq, "terrain-info", "terrain-data", isFirstTimePerceptTerrain);

		lock(_messagesToSend)
		{
			_messagesToSend.Add(message);
			SendMessages();
		} // lock

		//set a time so we can record it laterz!
		if(dispatchFlags[(int)DispatchTypes.terrain])
			dispatchTimes[(int)DispatchTypes.terrain] = System.DateTime.Now.Ticks;
	}

	/**
   * Function to serialize map info(and terrain info). The map info instance are serialized 
   * by using protobuf-net, which is a fast message compiling tool. Eventually, all
   * map infos will be packed instead of XML. But since the message processing in PAI(server side)
   * distinguish message type by xml tags, we need to construct a simple XML with following format:
   * 
   *      <?xml version="1.0" encoding="UTF-8"?>
   *      <oc:embodiment-msg xsi:schemaLocation="..." xmlns:xsi="..." xmlns:pet="...">
   *          <map(terrain)-info global-position-x="24" global-position-y="24" \
   *              global-position-offset="96" global-floor-height="99" is-first-time-percept-world = "false"(or "true") >
   *              <map(terrain)-data timestamp="...">packed message stream</map(terrain)-data>
   *          </map(terrain)-info>
   *      </oc:embodiment-msg>
   * 
   * @param mapinfoSeq the map-info instance sequence to be serialized.
   * @param messageTag the XML tag of message, currently there are "map-info" and "terrain-info".
   * @param payloadTag the XML tag for wrapping the payload of message, currently there are "map-data"
   * and "terrain-data".
   */
	private OCMessage SerializeMapInfo(List<OCObjectMapInfo> mapinfoSeq, string messageTag, string payloadTag, bool isFirstTimePerceptWorld = false)
	{
		string timestamp = GetCurrentTimestamp();
		// Create a xml document
		XmlDocument doc = new XmlDocument();
		XmlElement root = MakeXMLElementRoot(doc);

		// Create a terrain-info element and append to root element.
		XmlElement mapInfo = (XmlElement)root.AppendChild(doc.CreateElement(messageTag));
		mapInfo.SetAttribute("map-name", _mapName);
		//mapInfo.SetAttribute("name", _mapName);
		
		
		// ORIGINAL:
//      mapInfo.SetAttribute("global-position-x", _globalStartPositionX.ToString());
//      mapInfo.SetAttribute("global-position-y", _globalStartPositionY.ToString());
//		mapInfo.SetAttribute("global-position-z", _globalStartPositionZ.ToString());
//      mapInfo.SetAttribute("global-position-offset-x", _blockCountX.ToString());
//		mapInfo.SetAttribute("global-position-offset-y", _blockCountY.ToString());
//		mapInfo.SetAttribute("global-position-offset-z", _blockCountZ.ToString());
		
		// Y Z SWAPPED:
		mapInfo.SetAttribute("global-position-x", _globalStartPositionX.ToString());
		mapInfo.SetAttribute("global-position-y", _globalStartPositionZ.ToString());
		mapInfo.SetAttribute("global-position-z", _globalStartPositionY.ToString());
		mapInfo.SetAttribute("global-position-offset-x", _blockCountX.ToString());
		mapInfo.SetAttribute("global-position-offset-y", _blockCountZ.ToString());
		mapInfo.SetAttribute("global-position-offset-z", _blockCountY.ToString());
		mapInfo.SetAttribute("global-floor-height", (_globalFloorHeight).ToString());
		
		
		mapInfo.SetAttribute("is-first-time-percept-world", isFirstTimePerceptWorld.ToString().ToLower());
		mapInfo.SetAttribute("timestamp", timestamp);

		string encodedPlainText;
		using(var stream = new System.IO.MemoryStream())
		{
			// Serialize the instances into memory stream by protobuf-net
			ProtoBuf.Serializer.Serialize<List<OCObjectMapInfo>>(stream, mapinfoSeq);
			byte[] binary = stream.ToArray();
			// Encoding the binary in base64 string format in order to transport
			// via NetworkElement.
			encodedPlainText = System.Convert.ToBase64String(binary);
		}

		XmlElement data = (XmlElement)mapInfo.AppendChild(doc.CreateElement(payloadTag));

		data.InnerText = encodedPlainText;
		
		return OCMessage.CreateMessage(_ID, _brainID, OCMessage.MessageType.STRING, BeautifyXmlText(doc));
		
		//return new OCStringMessage(_ID, _brainID, BeautifyXmlText(doc));
	}
	
	public void SendFinishPerceptTerrain()
	{
		XmlDocument doc = new XmlDocument();
		XmlElement root = MakeXMLElementRoot(doc);
		string timestamp = GetCurrentTimestamp();
 
		XmlElement signal = (XmlElement)root.AppendChild(doc.CreateElement("finished-first-time-percept-terrian-signal"));
		signal.SetAttribute("timestamp", timestamp);
		//signal.SetAttribute("name", "finished-first-time-percept-terrian-signal");
	
		//OCStringMessage message = new OCStringMessage(_ID, _brainID, BeautifyXmlText(doc));
		
		OCMessage message = OCMessage.CreateMessage(_ID, _brainID, OCMessage.MessageType.STRING, BeautifyXmlText(doc));
        
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"sending finished-first-time-percept-terrian-signal: \n" + BeautifyXmlText(doc));
        
		lock(_messagesToSend)
		{
			_messagesToSend.Add(message);
		}

		//set a time so we can record it laterz!
		if(dispatchFlags[(int)DispatchTypes.finishTerrain])
			dispatchTimes[(int)DispatchTypes.finishTerrain] = System.DateTime.Now.Ticks;

	}

	/**
   * Return a current time stamp with a specific format.
   */
	public static string GetCurrentTimestamp()
	{
		return System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
	}

	/**
   * The interface of dialog system in the game.
   * A player chats with avatar by typing text in the console,
   * then the text would be sent to OAC by this method.
   * 
   * @param text the chat text to be sent.
   * @param source GameObject that sent the communication.
   */
	public void SendSpeechContent(string text, UnityEngine.GameObject source)
	{
		text = text.TrimEnd('\n');
		// Don't send a message unless we are initialized to connect to
		// a server
		if(!_isInitialized)
		{
			Debug.LogWarning(OCLogSymbol.WARN +"Avatar[" + _ID + "]: Received '" + text +
				"' from player but I am not connected to an OAC.");
			return;
		}
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"Avatar[" + _ID + "]: Received '" + text + "' from player.");

		// Avoid creating messages if the destination (avatar brain) isn't available 
		if(!IsElementAvailable(_brainID))
		{
			return;
		}
      
		System.Text.StringBuilder speechMsg = new System.Text.StringBuilder();
		speechMsg.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");

		speechMsg.Append("<oc:embodiment-msg xmlns:oc=\"http://www.opencog.org/brain\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.opencog.org/brain BrainProxyAxon.xsd\">\n");
		speechMsg.Append("<communication source-id=\"");
		speechMsg.Append(source.GetInstanceID());
		speechMsg.Append("\" timestamp=\"");
		speechMsg.Append(GetCurrentTimestamp());
		speechMsg.Append("\">");
		speechMsg.Append(text);
		speechMsg.Append("</communication>");
		speechMsg.Append("</oc:embodiment-msg>");
      
		OCMessage message = OCMessage.CreateMessage(_ID, _brainID, OCMessage.MessageType.RAW, speechMsg.ToString());
      
		// Add the message to the sending queue.
		lock(_messagesToSend)
		{
			_messagesToSend.Add(message);
		}

		//set a time so we can record it laterz!
		if(dispatchFlags[(int)DispatchTypes.speechContent])
			dispatchTimes[(int)DispatchTypes.speechContent] = System.DateTime.Now.Ticks;
	}
    
	/**
   * Save data and exit from embodiment system normally 
   * when finalizing the OCAvatar. 
   */
	public void SaveAndExit()
	{
		if(_isInitialized)
		{
			// TODO [LEGACY]: save local data of this avatar.
			UnloadOAC();
		}
		// TODO [LEGACY]: Finalize is not Uninitialize, which I believe is called on raising the OnDestroy event...?
		//finalize();
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------



	/**
     * Parse the xml information contained in a message.
     *
     * @param xmlText xml text to be parsed
     */
	private void ParseXML(string xmlText)
	{
		XmlDocument document = new XmlDocument();
		try
		{
			document.Load(new System.IO.StringReader(xmlText));
		} catch(System.Exception e)
		{
			Debug.LogError(e.ToString());
		}
		ParseDOMDocument(document);
	}

	private void ParsePsiDemandElement(XmlElement element)
	{
		//string avatarId = element.GetAttribute(OCEmbodimentXMLTags.ENTITY_ID_ATTRIBUTE);

		// Parse all demands and add them to a map.
		XmlNodeList list = element.GetElementsByTagName(OCEmbodimentXMLTags.DEMAND_ELEMENT);
		for(int i = 0; i < list.Count; i++)
		{
			XmlElement demandElement = (XmlElement)list.Item(i);
			string demand = demandElement.GetAttribute(OCEmbodimentXMLTags.NAME_ATTRIBUTE);
			float value = float.Parse(demandElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));

			// group all demands to be updated only once
			_demandValueMap[demand] = value;

			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"Avatar[" + _ID + "] -> parsePsiDemandElement: Demand '" + demand + "' value '" + value + "'.");
		}
	}

	private void ParseEmotionalFeelingElement(XmlElement element)
	{
//		UnityEngine.Debug.Log ("OCConnectorSingleton::ParseEmotionalFeelingElement");
		//string avatarId = element.GetAttribute(OCEmbodimentXMLTags.ENTITY_ID_ATTRIBUTE);

		// Parse all feelings and add them to a map.
		XmlNodeList list = element.GetElementsByTagName(OCEmbodimentXMLTags.FEELING_ELEMENT);
		for(int i = 0; i < list.Count; i++)
		{
			XmlElement feelingElement = (XmlElement)list.Item(i);
			string feeling = feelingElement.GetAttribute(OCEmbodimentXMLTags.NAME_ATTRIBUTE);
			float value = float.Parse(feelingElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));

			// group all feelings to be updates only once
			_feelingValueMap[feeling] = value;

			//System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"Avatar[" + _ID + "] -> parseEmotionalFeelingElement: Feeling '" + feeling + "' value '" + value + "'.");
		}

		// Update feelings of this avatar.
		UpdateEmotionFeelings();
	}

	/**
     * TODO This function is supposed to receive emotion information from OpenPsi
     * and update local avatar's emotion.(e.g. facial expression?)
     */
	private void UpdateEmotionFeelings()
	{
		// DEPRECATED: Update this, I don't know if we still want to use these components?
		
//        OCEmotionalExpression emotionalExpression = gameObject.GetComponent<OCEmotionalExpression>() as OCEmotionalExpression;
//        emotionalExpression.showEmotionExpression(this.FeelingValueMap);
	}

	private void ParseDOMDocument(XmlDocument document)
	{
		//UnityEngine.Debug.Log ("OCConnectorSingleton::ParseDOMDocument");
		// Handles action-plans


		System.IO.StringWriter stringWriter = new System.IO.StringWriter();
		XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);

		document.WriteTo(xmlTextWriter);
		
		if(stringWriter.ToString().IndexOf("oc:emotional-feeling") == -1)
		{
			UnityEngine.Debug.Log(OCLogSymbol.CLEARED + "The robot has a plan! " + stringWriter.ToString());	

			//set the flag, if it is requested. 
			if(this.receptFlags[(int)ReceptTypes.robotHasPlan])
			{
				this.receptTimes[(int)ReceptTypes.robotHasPlan] = System.DateTime.Now.Ticks;
			}
		}
		else
		{
			//Debug.LogWarning(OCLogSymbol.WARN + "The robot formed a strange plan. Attempting to interprit it... " + stringWriter.ToString());	
		}

		
		XmlNodeList list = document.GetElementsByTagName(OCEmbodimentXMLTags.ACTION_PLAN_ELEMENT);
		for(int i = 0; i < list.Count; i++)
		{
			System.Console.WriteLine(OCLogSymbol.RUNNING + "OCConnectorSingleton::ParseDOMDocument: ParseActionPlanElement");
			ParseActionPlanElement((XmlElement)list.Item(i));
		}

		// Handles emotional-feelings       
		XmlNodeList feelingsList = document.GetElementsByTagName(OCEmbodimentXMLTags.EMOTIONAL_FEELING_ELEMENT);
		for(int i = 0; i < feelingsList.Count; i++)
		{
			//UnityEngine.Debug.Log ("OCConnectorSingleton::ParseDOMDocument: ParseEmotionalFeelingElement");
			ParseEmotionalFeelingElement((XmlElement)feelingsList.Item(i));
		}
        
		// Handle psi-demand
		XmlNodeList demandsList = document.GetElementsByTagName(OCEmbodimentXMLTags.PSI_DEMAND_ELEMENT);
		for(int i = 0; i< demandsList.Count; i++)
		{
			UnityEngine.Debug.Log("OCConnectorSingleton::ParseDOMDocument: ParsePsiDemandElement");
			ParsePsiDemandElement((XmlElement)demandsList.Item(i));
		}
		
		// Handle single-action-command
		XmlNodeList singleActionList = document.GetElementsByTagName(OCEmbodimentXMLTags.SINGLE_ACTION_COMMAND_ELEMENT);
		for(int i = 0; i< singleActionList.Count; i++)
		{
			UnityEngine.Debug.Log("OCConnectorSingleton::ParseDOMDocument: ParseSingleActionElement");
			ParseSingleActionElement((XmlElement)singleActionList.Item(i));
		}
	}

	// this kind of message is from the opencog, and it's not a completed plan,
	// but a single action that current the robot want to do
	private void ParseSingleActionElement(XmlElement element)
	{
		OCActionController oca = GameObject.FindGameObjectWithTag("OCAGI").GetComponent<OCActionController>() as OCActionController;
		if(oca == null)
		{
			return;
		}
		
		string actionName = element.GetAttribute(OCEmbodimentXMLTags.NAME_ATTRIBUTE);
			
		if(actionName == "BuildBlockAtPosition")
		{
			int x = 0, y = 0, z = 0;
			XmlNodeList list = element.GetElementsByTagName(OCEmbodimentXMLTags.PARAMETER_ELEMENT);
			for(int i = 0; i < list.Count; i++)
			{
				XmlElement paraElement = (XmlElement)list.Item(i);
				string paraName = paraElement.GetAttribute(OCEmbodimentXMLTags.NAME_ATTRIBUTE);
				if(paraName == "x")
				{
					x = int.Parse(paraElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));
				} else if(paraName == "y")
				{
					y = int.Parse(paraElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));
				} else if(paraName == "z")
				{
					z = int.Parse(paraElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));
				}
				
			}
			Vector3i blockBuildPoint = new Vector3i(x, y, z);
			
			oca.BuildBlockAtPosition(blockBuildPoint);
		} else if(actionName == "MoveToCoordinate")
		{
			int x = 0, y = 0, z = 0;
			XmlNodeList list = element.GetElementsByTagName(OCEmbodimentXMLTags.PARAMETER_ELEMENT);
			for(int i = 0; i < list.Count; i++)
			{
				XmlElement paraElement = (XmlElement)list.Item(i);
				string paraName = paraElement.GetAttribute(OCEmbodimentXMLTags.NAME_ATTRIBUTE);
				if(paraName == "x")
				{
					x = int.Parse(paraElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));
				} else if(paraName == "y")
				{
					y = int.Parse(paraElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));
				} else if(paraName == "z")
				{
					z = int.Parse(paraElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));
				}
				
			}
			UnityEngine.Vector3 vec = new UnityEngine.Vector3(x, z, y);
			oca.MoveToCoordinate(vec);
		}
 
	}
	
	/**
	 * Parse action plan and append the result into action list.
	 *
	 * @param element meta action in xml format
	 */
	public void ParseActionPlanElement(XmlElement actionPlan)
	{
		// DEPRECATED: Determine if we need this:
		//bool adjustCoordinate = false;
		
		OpenCog.Utility.Console.Console console = OpenCog.Utility.Console.Console.Instance;
		
		// Get the action performer id.
		string avatarId = actionPlan.GetAttribute(OCEmbodimentXMLTags.ENTITY_ID_ATTRIBUTE);
		if(avatarId != _brainID)
		{
			// Usually this would not happen.
			Debug.LogWarning(OCLogSymbol.WARN +"Avatar[" + _ID + "]: This action plan is not for me.");
			return;
		}
		
		// Cancel current action and clear old action plan in the list.
		if(_actionsList.Count > 0)
		{
			Debug.LogWarning(OCLogSymbol.WARN +"Stop all current actions");
			CancelAvatarActions();
		}
		
		GameObject endPointStub = GameObject.Find("EndPointStub");

		// Update current plan id and selected demand name.
		_currentPlanId = actionPlan.GetAttribute(OCEmbodimentXMLTags.ID_ATTRIBUTE);
		_currentDemandName = actionPlan.GetAttribute(OCEmbodimentXMLTags.DEMAND_ATTRIBUTE);
        
		// Get the action elements from the actionPlan
		XmlNodeList list = actionPlan.GetElementsByTagName(OCEmbodimentXMLTags.ACTION_ELEMENT);

		if(_actionController == null)
		{
			_actionController = GameObject.FindGameObjectWithTag("OCAGI").GetComponent<OCActionController>();
		}

		// list contains 'action' elements.
		foreach(XmlNode actionNode in list)
		{
			// Cast actionNode to actionElement (XmlElement)
			XmlElement actionElement = (XmlElement)actionNode;
			
			// Get attributes from actionElement (name, sequence)
			string actionName = actionElement.GetAttribute(OCEmbodimentXMLTags.NAME_ATTRIBUTE);
			int sequence = System.Int32.Parse(actionElement.GetAttribute(OCEmbodimentXMLTags.SEQUENCE_ATTRIBUTE));
			
			// Get the actionParameter nodes from the actionElement
			XmlNodeList actionParameters = actionElement.GetElementsByTagName(OCEmbodimentXMLTags.PARAMETER_ELEMENT);
			
			// Prepare a new actionArgs object
			OCAction.OCActionArgs actionArguments = new OCAction.OCActionArgs();
			GameObject avatar = GameObject.FindGameObjectWithTag("OCAGI");
			
			actionArguments.StartTarget = GameObject.Find("StartPointStub");
			actionArguments.StartTarget.transform.position = avatar.transform.position;		
			actionArguments.EndTarget = null;

			// Create a Stub goal when one is not specified:
			// @TODO: Generalize this or remove the requirement that actions have goals.
			if(actionParameters.Count == 0 &&
				(actionName == "step_forward" || actionName == "rotate_left" || actionName == "rotate_right"))
			{
				actionArguments.EndTarget = GameObject.Find("EndPointStub");
				actionArguments.EndTarget.transform.position = avatar.transform.position + avatar.transform.forward;
			}
			
			// 'action' elements contain 'params'
			foreach(XmlNode actionParameterNode in actionParameters)
			{
				// Cast actionParameterNode to an actionParameterElement (XmlElement)
				XmlElement actionParameterElement = (XmlElement)actionParameterNode;
				
				// Get attributes from actionParameterElement
				string actionParameterType = actionParameterElement.GetAttribute(OCEmbodimentXMLTags.TYPE_ATTRIBUTE);
				
				switch(actionParameterType)
				{
				// If it's a vector, then it's a walk. So the target is a gameobject at the location of the vector.
				case "vector":


					XmlNodeList vectorParameterChildren = actionParameterElement.GetElementsByTagName(OCEmbodimentXMLTags.VECTOR_ELEMENT);
					XmlElement vectorElement = (XmlElement)vectorParameterChildren.Item(0);
						
					float x = float.Parse(vectorElement.GetAttribute(OCEmbodimentXMLTags.X_ATTRIBUTE), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
					float y = float.Parse(vectorElement.GetAttribute(OCEmbodimentXMLTags.Y_ATTRIBUTE), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
					float z = float.Parse(vectorElement.GetAttribute(OCEmbodimentXMLTags.Z_ATTRIBUTE), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
					
//						if (adjustCoordinate)
//						{
//							x += 0.5f;
//							y += 0.5f;
//							z += 0.5f;
//						}
					
					string gameObjectString = "PLAN" + _currentPlanId.ToString().PadLeft(3, '0') + "_SEQ" + sequence.ToString().PadLeft(3, '0') + "_VECTOR";
							
					UnityEngine.GameObject vectorGameObject = (UnityEngine.GameObject)UnityEngine.GameObject.Instantiate(_map.WaypointPrefab);
					vectorGameObject.name = gameObjectString;
					vectorGameObject.transform.parent = _map.WaypointsSceneObject.transform;
					
						// Swapping Y and Z!!
					
						// ORIGINAL:
						// vectorGameObject.transform.position = new Vector3(x, y, z);
						// UnityEngine.Debug.Log ("A '" + actionName + "' command told me to go to [" + x + ", " + y + ", " + z + "]");
					
						// SWAPPED:
					vectorGameObject.transform.position = new Vector3(x, z, y);
					TextMesh textMesh = vectorGameObject.GetComponentInChildren<TextMesh>();
					textMesh.text = sequence.ToString();
						//UnityEngine.Debug.Log ("A '" + actionName + "' command (planID = " +  _currentPlanId + ", sequence = " + sequence + " told me to go to [" + x + ", " + z + ", " + y + "]");

					if(actionName == "walk" || actionName == "jump_toward")
					{
						System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"A '" + actionName + "' command (planID = " + _currentPlanId + ", sequence = " + sequence + " told me to go to [" + x + ", " + z + ", " + y + "]");
						//console.AddConsoleEntry("A '" + actionName + "' command (planID = " + _currentPlanId + ", sequence = " + sequence + " told me to go to [" + x + ", " + z + ", " + y + "]", "AGI Robot", OpenCog.Utility.Console.Console.ConsoleEntry.Type.SAY);
					
						actionArguments.EndTarget = vectorGameObject;
						//actionArguments.EndTarget = GameObject.Find("EndPointStub");
						actionArguments.EndTarget.transform.position = new Vector3(x, z, y);
						

//						OCGoalController goalController = _actionController.gameObject.GetComponent<OCGoalController>();
//						goalController.BlockType = "Hearth";
//						goalController.FindGoalBlockPositionInChunks(_map.Chunks);
					
					} else if(actionName == "destroy" || actionName == "build_block")
					{
						System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "A '" + actionName + "' command (planID = " + _currentPlanId + ", sequence = " + sequence + " told me to destroy/create block at [" + x + ", " + z + ", " + y + "]");
						//console.AddConsoleEntry("A '" + actionName + "' command (planID = " + _currentPlanId + ", sequence = " + sequence + " told me to destroy/create block at [" + x + ", " + z + ", " + y + "]", "AGI Robot", OpenCog.Utility.Console.Console.ConsoleEntry.Type.SAY);
						
						actionArguments.EndTarget = vectorGameObject;
						//actionArguments.EndTarget = GameObject.Find("EndPointStub");
						actionArguments.EndTarget.transform.position = new Vector3(x, z, y);
					}
					
					break;	
				// If it's an entity, then it's a grab or a consume. So the target is the battery.
				case "entity":
					XmlNodeList entityParameterChildren = actionParameterElement.GetElementsByTagName(OCEmbodimentXMLTags.ENTITY_ELEMENT);
					XmlElement entityElement = (XmlElement)entityParameterChildren.Item(0);
						
					int entityID = System.Int32.Parse(entityElement.GetAttribute(OCEmbodimentXMLTags.ID_ATTRIBUTE));
					//string entityType = entityElement.GetAttribute(OCEmbodimentXMLTags.TYPE_ATTRIBUTE);
					
					if(actionName == "grab" || actionName == "eat")
					{
						UnityEngine.GameObject[] batteryArray = UnityEngine.GameObject.FindGameObjectsWithTag("OCBattery");
					
						for(int iBattery = 0; iBattery < batteryArray.Length; iBattery++)
						{
							UnityEngine.GameObject batteryObject = batteryArray[iBattery];
								
							if(entityID == batteryObject.GetInstanceID())
							{
								endPointStub.transform.position = batteryObject.transform.position;
								
								// This is the one!	
								actionArguments.EndTarget = batteryObject;
									
								break;
							}
						}
						
						if(actionArguments.EndTarget != null)
						{
							// Then we can grab it, eat it, whatever...since it's a battery
							if(actionName == "grab")
							{
								System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "A 'grab' command (planID = " +  _currentPlanId + ", sequence = " + sequence + " told me to grab an object with ID " + entityID);
								
								//console.AddConsoleEntry("A 'grab' command (planID = " + _currentPlanId + ", sequence = " + sequence + " told me to grab an object with ID " + entityID, "AGI Robot", OpenCog.Utility.Console.Console.ConsoleEntry.Type.SAY);
							} else if(actionName == "eat")
							{
								System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "An 'eat' command (planID = " +  _currentPlanId + ", sequence = " + sequence + " told me to eat an object with ID " + entityID);
								
								//console.AddConsoleEntry("An 'eat' command (planID = " + _currentPlanId + ", sequence = " + sequence + " told me to eat an object with ID " + entityID, "AGI Robot", OpenCog.Utility.Console.Console.ConsoleEntry.Type.SAY);
								
							}	

							OCGoalController goalController = _actionController.gameObject.GetComponent<OCGoalController>();
							goalController.BlockType = "Battery";
							goalController.FindGoalBlockPositionInChunks(_map.Chunks);
						} else
						{
							// That's silly..we can only eat / grab batteries!
							UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "Received a grab or eat command, but couldn't find the battery in Unity!");
						}
						
							
					} 
					// if (actionName == "grab" || actionName == "eat"
//						else
//						{
//							// I'll bet the section below does nothing. Go home 
////							UnityEngine.GameObject[] hearthArray = UnityEngine.GameObject.FindGameObjectsWithTag("OCHearth");
////					
////							for (int iHearth = 0; iHearth < hearthArray.Length; iHearth++)
////							{
////								UnityEngine.GameObject hearthObject = hearthArray[iHearth];
////								
////								if (entityID == hearthObject.GetInstanceID())
////								{
////									// This is the one!	
////									actionArguments.EndTarget = hearthObject;
////									
////									break;
////								}
////							}	
//						}

					break;
				case "string":
//						XmlNodeList stringParameterChildren = actionParameterElement.GetElementsByTagName(OCEmbodimentXMLTags.VECTOR_ELEMENT);
//						XmlElement stringElement = (XmlElement)stringParameterChildren.Item (0);
					
					if(actionName == "say")
					{
						string toSay = actionParameterElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE);
						
						if(toSay != string.Empty)
						{
							UnityEngine.Debug.Log("Robot say: " + toSay + "(actionPlan = " + _currentPlanId + ", sequence = " + sequence);
												
							console.AddConsoleEntry(toSay, "AGI Robot", OpenCog.Utility.Console.Console.ConsoleEntry.Type.SAY);
						}

						// We need to set the target to the avatar I guess....
						UnityEngine.GameObject[] agiArray = UnityEngine.GameObject.FindGameObjectsWithTag("OCAGI");
						
						for(int iAGI = 0; iAGI < agiArray.Length; iAGI++)
						{
							UnityEngine.GameObject agiObject = agiArray[iAGI];
							
							endPointStub.transform.position = agiObject.transform.position;
							
							actionArguments.EndTarget = endPointStub;
						}
					}

					
					break;

				} // end switch actionParameterType
			} // end foreach actionParameterNode
			
			if(_actionController == null)
			{
				_actionController = GameObject.FindGameObjectWithTag("OCAGI").GetComponent<OCActionController>();
			}
			
			actionArguments.ActionName = actionName;
			actionArguments.ActionPlanID = _currentPlanId;
			actionArguments.SequenceID = sequence;
			actionArguments.Source = UnityEngine.GameObject.FindGameObjectWithTag("OCAGI");
			//actionArguments.Source.transform.SetY(actionArguments.Source.transform.position.y - 0.5f);
			
			// Lake's function here.
			if(actionName != "say")
			{
				_actionController.LoadActionPlanStep(actionName, actionArguments);
			}
			
			// Just for fun, I'm going to send success for everything for a while.
			
			//this.SendActionStatus(_currentPlanId, sequence, actionName, true);
			
			//actionPlan.Add((XmlElement)node);
		} // end foreach actionPlanElement 
		
		// And again for fun, send a whole action plan successful message:
		//this.SendActionPlanStatus(_currentPlanId, true);
				
		// Start to perform an action in front of the action list.
		//_actionController.ReceiveActionPlan(actionPlan);// SendMessage("receiveActionPlan", actionPlan);
		//processNextAvatarAction();
	}
	
	/**
     * Cancel current action and clear actions from previous action plan.
     */
	private void CancelAvatarActions()
	{
		// Ask action scheduler to stop all current actions.
		_actionController.CancelActionPlan();// SendMessage("cancelCurrentActionPlan");
		SendActionPlanStatus(_currentPlanId, false);
	}

	/// <summary>
	/// Load an OAC instance as the brain of this OCAvatar.
	/// </summary>
	private void LoadOAC()
	{
			System.Text.StringBuilder msgCmd = new System.Text.StringBuilder("LOAD_AGENT ");
			msgCmd.Append(_brainID + WHITESPACE + _masterID + WHITESPACE);
			msgCmd.Append(_type + WHITESPACE + _traits + "\n");

			OCMessage msg = OCMessage.CreateMessage(_ID,
	                                      OCConfig.Instance.get("SPAWNER_ID"),
	                                      OCMessage.MessageType.STRING,
	                                      msgCmd.ToString());
			SendMessage(msg);
	}


    
	/// <summary>
	/// Unload the OAC instance controlling this avatar when finalizing.
	/// </summary>
	private void UnloadOAC()
	{
		System.Text.StringBuilder msgCmd = new System.Text.StringBuilder("UNLOAD_AGENT ");
		msgCmd.Append(_brainID + "\n");

		OCMessage msg = OCMessage.CreateMessage(_ID,
                                      OCConfig.Instance.get("SPAWNER_ID"),
                                      OCMessage.MessageType.STRING,
                                      msgCmd.ToString());
		if(!SendMessage(msg))
		{
			Debug.LogWarning(OCLogSymbol.WARN +"Could not send unload message to spawner.");
		}

		// Wait some time for the message to be sent.
		try
		{
			System.Threading.Thread.Sleep(500);
		} catch(System.Threading.ThreadInterruptedException e)
		{
			Debug.LogError("Error putting NetworkElement main thread to sleep. " +
				e.Message);
		}
	}


	/**
     * Send the physiological information of this avatar to OAC with a tick message
     * for OAC to handle it.
     * This method would be invoked by physiological model.
     */
	public void SendAvatarSignalsAndTick(Dictionary<string, double> physiologicalInfo)
	{
		if(_isEstablished)
		{
			//UnityEngine.Debug.Log ("OCConnectorSingleton::SendAvatarSignalsAndTick: _isEstablished -> Sending tick message and phys info.");
			string timestamp = GetCurrentTimestamp();
			XmlDocument doc = new XmlDocument();
			XmlElement root = MakeXMLElementRoot(doc);
	        
			// (currently this is avatar-signal, but should be changed...)
			XmlElement avatarSignal = (XmlElement)root.AppendChild(doc.CreateElement("avatar-signal"));
			avatarSignal.SetAttribute("id", _brainID);
	        
			avatarSignal.SetAttribute("timestamp", timestamp);
			
			//avatarSignal.SetAttribute("type-of-message", "tick-message-really");
	        
			// Append all physiological factors onto the message content.
			if(_firstSendOfPhysiologicalFactors)
			{
				// First time it seems to want an empty message...
				_firstSendOfPhysiologicalFactors = false;
				
			} else
			{
				foreach(string factor in physiologicalInfo.Keys)
				{
					// <physiology-level name="hunger" value="0.3"/>   
					XmlElement p = (XmlElement)avatarSignal.AppendChild(doc.CreateElement("physiology-level"));
					p.SetAttribute("name", factor);
					p.SetAttribute("value", physiologicalInfo[factor].ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
				}	
			}	
	        
	        
			string xmlText = BeautifyXmlText(doc);
			//System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"OCConnector - sendAvatarSignalsAndTick: " + xmlText);
	            
			// Construct a string message.
			OCMessage message = OCMessage.CreateMessage(_ID, _brainID, OCMessage.MessageType.STRING, xmlText);
			
			lock(_messagesToSend)
			{
				// Add physiological information to message sending queue.
				_messagesToSend.Add(message);
	
				// Send a tick message to make OAC start next cycle.
				if(bool.Parse(OCConfig.Instance.get("GENERATE_TICK_MESSAGE")))
				{
					OCMessage tickMessage = OCMessage.CreateMessage(_ID, _brainID, OCMessage.MessageType.TICK, "TICK_MESSAGE");
					
//					if (tickMessage == null)
//						UnityEngine.Debug.Log ("Its the tick!");
					
					_messagesToSend.Add(tickMessage);
				}
			}
		} 
		else
		{
			//UnityEngine.Debug.Log ("OCConnectorSingleton::SendAvatarSignalsAndTick: !isEstablished -> Not sending a tick message / phys info.");	
		}
	}

	private void MakeEntityElement(UnityEngine.GameObject obj, XmlElement entityElement)
	{

		if(obj == null)
		{
			entityElement.SetAttribute(OCEmbodimentXMLTags.ID_ATTRIBUTE, "null");
			entityElement.SetAttribute(OCEmbodimentXMLTags.TYPE_ATTRIBUTE, OCEmbodimentXMLTags.UNKNOWN_OBJECT_TYPE);
		} else
		{
			string targetType = obj.tag;
			if(targetType == "OCA" || targetType == "Player")// it's an avatar
			{
				entityElement.SetAttribute(OCEmbodimentXMLTags.ID_ATTRIBUTE, obj.GetInstanceID().ToString());
				entityElement.SetAttribute(OCEmbodimentXMLTags.TYPE_ATTRIBUTE, OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
			} else // it's an object
			{
				entityElement.SetAttribute(OCEmbodimentXMLTags.ID_ATTRIBUTE, obj.GetInstanceID().ToString());
				entityElement.SetAttribute(OCEmbodimentXMLTags.TYPE_ATTRIBUTE, OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
			}
		}
	}


    
	/**
   * Creates and store an action status message to be sent to an OAC.
   * This message represents the status of all actions of the given planId.
   * This method will create a agent-signal xml message like:
   * 
   * (currently this is avatar-signal, but should be changed...)
   * <agent-signal id="..." timestamp="...">
   * <action name="..." plan-id="..." sequence="..." status="..."/>
   * </agent-signal>
   * 
   * @param planId plan id
   * @param action avatar action
   * @param success action result
   */
	public void SendActionStatus(string planId, int sequence, string actionName, bool success)
	{
		string timestamp = GetCurrentTimestamp();
		// Create a xml document
		XmlDocument doc = new XmlDocument();
		XmlElement root = MakeXMLElementRoot(doc);
		
		XmlElement avatarSignal = (XmlElement)root.AppendChild(doc.CreateElement("avatar-signal"));
		avatarSignal.SetAttribute("id", _brainID);
		avatarSignal.SetAttribute("timestamp", timestamp);
		XmlElement actionElement = (XmlElement)avatarSignal.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ACTION_ELEMENT));
		actionElement.SetAttribute(OCEmbodimentXMLTags.ACTION_PLAN_ID_ATTRIBUTE, planId);
		
		// TODO [LEGACY]: Fix the Sequence attribute on Action which is currently missing.
		actionElement.SetAttribute(OCEmbodimentXMLTags.SEQUENCE_ATTRIBUTE, sequence.ToString());
		actionElement.SetAttribute("name", actionName);
		actionElement.SetAttribute("status", success ? "done" : "error");
		
//	  	System.IO.StringWriter stringWriter = new System.IO.StringWriter();
//		XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
//		
//		doc.WriteTo(xmlTextWriter);
//		
//		UnityEngine.Debug.Log ("Send ActionStatus message: " + stringWriter.ToString());	
		
		OCMessage message = OCMessage.CreateMessage(_ID, _brainID, OCMessage.MessageType.STRING, BeautifyXmlText(doc));
		
		lock(_messagesToSend)
		{
			_messagesToSend.Add(message);
		}

		//set a time so we can record it laterz!
		if(dispatchFlags[(int)DispatchTypes.actionStatus])
			dispatchTimes[(int)DispatchTypes.actionStatus] = System.DateTime.Now.Ticks;
		
		//UnityEngine.Debug.Log ("Queued message to report '" + ((success ? "done (success)" : "error") + "' on action '" + actionName + "' (planID = " + planId + ", sequence = " + sequence.ToString () + ")"));
	}
  
	/**
   * Creates and store an action status message to be sent to an OAC.
   * This message represents the status of all actions of the given planId.
   * This method will create a agent-signal xml message like:
   * 
   * (currently this is avatar-signal, but should be changed...)
   * <agent-signal id="..." timestamp="...">
   * <action name="..." plan-id="..." sequence="..." status="..."/>
   * </agent-signal>
   * 
   * @param planId plan id
   * @param success action result
   */
	public void SendActionPlanStatus(string planId, bool success/*, long timeCompleted*/)
	{
		string timestamp = GetCurrentTimestamp();
		XmlDocument doc = new XmlDocument();
		XmlElement root = MakeXMLElementRoot(doc);
		
		XmlElement avatarSignal = (XmlElement)root.AppendChild(doc.CreateElement("avatar-signal"));
		avatarSignal.SetAttribute("id", _brainID);
		avatarSignal.SetAttribute("timestamp", timestamp);
		XmlElement actionElement = (XmlElement)avatarSignal.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ACTION_ELEMENT));
		actionElement.SetAttribute(OCEmbodimentXMLTags.ACTION_PLAN_ID_ATTRIBUTE, planId);
		actionElement.SetAttribute("status", success ? "done" : "error");
		actionElement.SetAttribute("name", planId);
		
//	  	System.IO.StringWriter stringWriter = new System.IO.StringWriter();
//		XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
//	
//		doc.WriteTo(xmlTextWriter);
//		
//		UnityEngine.Debug.Log ("Send ActionPlanStatus message: " + stringWriter.ToString());	

		OCMessage message = OCMessage.CreateMessage(_ID, _brainID, OCMessage.MessageType.STRING, BeautifyXmlText(doc));
		
		lock(_messagesToSend)
		{
			_messagesToSend.Add(message);
		}

		//set a time so we can record it laterz! (success and fialed are seperate XD)
		if(success && dispatchFlags[(int)DispatchTypes.actionPlanSucceeded])
			dispatchTimes[(int)DispatchTypes.actionPlanSucceeded] = System.DateTime.Now.Ticks;
		if(!success && dispatchFlags[(int)DispatchTypes.actionPlanFailed])
			dispatchTimes[(int)DispatchTypes.actionPlanFailed] = System.DateTime.Now.Ticks;

		
		UnityEngine.Debug.Log(OCLogSymbol.CLEARED + "Queued message to report '" + ((success ? "done (success)" : "error") + "' on actionPlan " + planId));
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	private const string SUCCESS_LOAD = "SUCCESS LOAD";

	private const string SUCCESS_UNLOAD = "SUCCESS UNLOAD";

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCConnector

//}// namespace OpenCog.Network




