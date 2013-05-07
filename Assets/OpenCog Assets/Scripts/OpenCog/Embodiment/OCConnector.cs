
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
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using System.Xml;
using OpenCog.Actions;
using OpenCog.Character;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Embodiment
{

/// <summary>
/// The OpenCog OCConnector.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCConnector : Network.OCNetworkElement
{
		//TOFIX: delete these fake classes.
//		public class MetaAction
//		{}
//
//		public class ActionResult
//		{}
//
//		public class ActionSummary
//		{}

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------

	private string _myBrainID;   /** For example "OAC_NPC" */
	private bool _isInitialized = false; // Flag to check if the OAC to this avatar is alive.

	// Basic attributes of this avatar.
	private string _baseID;    /** For example "NPC" */
	private string _brainID;   /** For example "OAC_NPC" */
	private string _name;		/** Not yet been used now */
	private string _type;		/** "pet" type by default. */
	private string _traits;	/** "princess" by default. */

	private string _settingsFilename = "Assets/embodiment.config";

	private string _masterID; // Define master's(owner's) information.
	private string _masterName;
	private List<Network.OCMessage> _messagesToSend = new List<Network.OCMessage>(); // The queue used to store the messages to be sent to OAC.
	private System.Object _messageSendingLock = new System.Object(); // The lock object used to sync the atomic sequence - get a timestamp, build and enqueue a message

		// Timer to send message in a given interval. We can implement a timer by using unity API - FixedUpdate().
	private float _messageSendingTimer = 0.0f;		/**< The timer used to control message sending. */
	private float _messageSendingInterval = 0.1f;	/**< The interval to send messages in the queue. */
	private string _mapName; 	// the name of the current map
	private int _blockCountX; // The size of map.
	private int _blockCountY; // The size of map.
	private int _blockCountZ; // The size of map.
	private int _globalStartPositionX; // Map global X beginning position.
	private int _globalStartPositionY; // Map global Y beginning position.
	private int _globalStartPositionZ; // Map global Z beginning position.
	private int _globalFloorHeight; // Floor height in the minecraft-like world.
	private bool _isFirstSentMapInfo; // Flag to check if a valid map info(which should contain the info of the avatar itself) has been sent to OAC as a "handshake".
	private LinkedList<OCAction> _actionsList; // The list of actions that I am going to perform. The action plans are received from OAC.

	private int _msgCount = 0;
	private string _currentPlanId; // The action plan id that is being performed currently.
	private string _currentDemandName; // Currently selected demand name
	private Dictionary<string, float> _feelingValueMap; // Store the feeling values of this avatar.
	private Dictionary<string, float> _demandValueMap; // Store the demand values of this avatar.
	private Dictionary<int, string> _perceptedAgents; // Other OC agents percepted. Record the pairs of their unity object id and brain id.
	private int _stateChangeActionCount = 0;
	private int _disappearActionCount = 0;
	private int _appearActionCount = 0;
	private int _moveActionCount = 0;
	private HashSet<string> _unavailableElements = new HashSet<string>();
	private OpenCog.Map.OCMap _map;


	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------

	/**
     * Accessor to this avatar's brain id. (a.k.a OAC_xxx)
     */
	public string BrainID
	{
		get { return _myBrainID; }
	}



	public bool IsInitialized // Old property: IsInit(), old member var: isOacAlive
	{
		get { return _isInitialized; }
	}

	/**
     * Accessor to this avatar's id. (a.k.a AVATAR_xxx)
     */
    public string ID
    {
        get { return _ID; }
    }

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
			if (!_map)
				_map = UnityEngine.GameObject.Find("OCMap").GetComponent<OpenCog.Map.OCMap>() as OpenCog.Map.OCMap;

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
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

    /// <summary>
    /// A unity API function inherited from MonoBehavior.
    /// Initialization work would be done in Init() for we need some parameters
    /// for this instance.
    /// </summary>
    void Awake()
    {
    }
    
    void Update()
    {
        // Invoke base network element function to do networking stuffs in 
        // every frame.
        Pulse();

        //isOacAlive = isElementAvailable(myBrainId);
    }
    
    /**
     * This method is called by unity system in a fixed frequency,
     * so we can make a timer to do things we want.
     */
    void FixedUpdate()
    {
        _messageSendingTimer += UnityEngine.Time.fixedDeltaTime;
        
        if (_messageSendingTimer >= _messageSendingInterval) {
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

	public void SendMessages()
	{
		if(!_isInitialized)
		{
			return;
		}

		if(_messagesToSend.Count > 0)
		{
			List<Network.OCMessage> localMessagesToSend;
			// copy messages to a local queue and clear the global sending queue.
			lock(_messageSendingLock)
			{
				localMessagesToSend = new List<Network.OCMessage>(_messagesToSend);
				_messagesToSend.Clear();
			} // lock

			foreach(Network.OCMessage message in localMessagesToSend)
			{
				// Check if router and destination is available. If so, send the message. 
				// otherwise just ignore the message
				string routerId = new OCConfig().get("ROUTER_ID", "ROUTER");
				if(!IsElementAvailable(routerId))
				{
					OCLogger.Warn("Router not available. Discarding message to '" +
                                 message.TargetID + "' of type '" + message.Type + "'.");
					continue;
				}
				if(!IsElementAvailable(message.TargetID))
				{
					OCLogger.Warn("Destination not available. Discarding message to '" +
                                 message.TargetID + "' of type '" + message.Type + "'.");
					continue;
				}
				if(SendMessage(message))
				{
					OCLogger.Debugging("Message from '" + message.SourceID + "' to '" +
                                 message.TargetID + "' of type '" + message.Type + "'.");
				}
				else
				{
					OCLogger.Warn("Error sending message from '" + message.SourceID + "' to '" +
                                 message.TargetID + "' type '" + message.Type + "'.");
				}
			} // foreach
		}
        
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
	  _demandValueMap[demandName] = demandValue;
	}

	public void sendBlockStructure(OpenCog.Map.OCBlockData startBlock, bool isToRecognize)
	{
	    XmlDocument doc = new XmlDocument();
        XmlElement root = MakeXMLElementRoot(doc);
		string timestamp = GetCurrentTimestamp();
 
        XmlElement signal = (XmlElement) root.AppendChild(doc.CreateElement("block-structure-signal"));
		if (isToRecognize)
		{
	        signal.SetAttribute("recognize-structure","true" );
	        
	        signal.SetAttribute("startblock-x", startBlock.GlobalX.ToString());
			signal.SetAttribute("startblock-y", startBlock.GlobalY.ToString());
			signal.SetAttribute("startblock-z", startBlock.GlobalZ.ToString());
		}
		signal.SetAttribute("timestamp",timestamp );
	
		Network.OCMessage message = Network.OCMessage.CreateMessage(_ID, _brainID, OpenCog.Network.OCMessage.MessageType.STRING, BeautifyXmlText(doc));
        
        OCLogger.Debugging("sending block structure signal: \n" + BeautifyXmlText(doc));
        
        lock (_messageSendingLock)
        {
            _messagesToSend.Add(message);
        }
	}

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
  public bool Init(string agentName, string agentTraits, string agentType,
                  string masterId, string masterName)
  {
    // Initialize basic attributes.
    _baseID = agentName;//gameObject.GetInstanceID().ToString();
    _ID = "AVATAR_" + _baseID;
    _brainID = "OAC_" + _baseID;
    _name = agentName;
    _type = OCEmbodimentXMLTags.PET_OBJECT_TYPE;
    _traits = "Princess";
    _currentDemandName = "";
    
    // Load settings from file.
    if (_settingsFilename.Length > 0) {
        new OCConfig().LoadFromFile(_settingsFilename);
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

    if (Map != null)
    {
			_mapName = Map.MapName;
			
			// If there are chuncks auto generated around the bounday, we should minus this boundary
			if (OCPerceptionCollector.HasBoundaryChuncks)
			{
				// Calculate the offset of the terrain.
				_blockCountX = OpenCog.Map.OCChunk.SIZE_X * Map.ChunkCountX;
				_blockCountY = OpenCog.Map.OCChunk.SIZE_Y * Map.ChunkCountY;

	            // There is an invisible chunk at the edge of the terrain, so we should take count of it.
	            _globalStartPositionX = (int)OpenCog.Map.OCChunk.SIZE_X;
	            _globalStartPositionY = (int)OpenCog.Map.OCChunk.SIZE_Y;
			}
			else
			{
	      // Calculate the offset of the terrain.
				_blockCountX = OpenCog.Map.OCChunk.SIZE_X * Map.ChunkCountX;
				_blockCountY = OpenCog.Map.OCChunk.SIZE_Y * Map.ChunkCountY;
	
        // There is an invisible chunk at the edge of the terrain, so we should take count of it.
        _globalStartPositionX = 0;
        _globalStartPositionY = 0;
			}
			_blockCountZ = OpenCog.Map.OCChunk.SIZE_Z * Map.ChunkCountZ;
			_globalStartPositionZ = 0;
            // The floor height should be 1 unit larger than the block's z index.
            _globalFloorHeight = Map.FloorHeight;
        }
        else
        {
			/// TOFIX: I have no idea if the below code makes sense...if Map==null....
			_mapName = "unknown_map";
	    _blockCountX = 128;
			_blockCountY = 128;
			_blockCountZ = 128;

			_globalStartPositionX = 0;
			_globalStartPositionY = 0;
			_globalStartPositionZ = 0;
			_globalFloorHeight = 0;
		}
    
    // Get action scheduler component.
		// TOFIX: old classes here.
    //_actionController = gameObject.GetComponent<OCActionController>() as OCActionController;
    OCActionController.globalActionCompleteEvent += HandleOtherAgentActionResult;

    return true;
  }

	public IEnumerator ConnectOAC()
	{
        // First step, connect to the router.
        int timeout = 100;
        while (!base._isEstablished && timeout > 0)
        {
            StartCoroutine(base.Connect());

            yield return new UnityEngine.WaitForSeconds(1.0f);
            timeout--;
        }

        if (timeout == 0)
        {
            OCLogger.Error("Breaking");
            yield break;
        }

        // Second step, check if spawner is available to spawn an OAC instance.
        bool isSpawnerAlive = IsElementAvailable(new OCConfig().get("SPAWNER_ID"));
        timeout = 60;
        while (!isSpawnerAlive && timeout > 0)
        {
            OCLogger.Info("Waiting for spawner...");
            yield return new UnityEngine.WaitForSeconds(1f);
            isSpawnerAlive = IsElementAvailable(new OCConfig().get("SPAWNER_ID"));
            timeout--;
        }

        if (!isSpawnerAlive)
        {
            OCLogger.Error("Spawner is not available, OAC can not be launched.");
            yield break;
        }

        // Finally, load the OAC by sending "load agent" command to spawner.
        LoadOAC();
        timeout = 100;
        // Wait some time for OAC to be ready.
        while (!_isInitialized && timeout > 0)
        {
            yield return new UnityEngine.WaitForSeconds(1f);
            timeout--;
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
	public override bool ProcessNextMessage(Network.OCMessage message)
	{
		OCLogger.Debugging(message.ToString());
    
		if(message.Type == Network.OCMessage.MessageType.FEEDBACK)
		{
			// e.g. we can append the information in the console.
			OCLogger.Error("Feedback " + message.ToString());
		}
		else
		if(message.ToString().StartsWith(SUCCESS_LOAD))
		{
			// Format: SUCCESS LOAD NetworkElement_id avatar_id
			char[] separator = { ' ' };
			string[] tokens = message.ToString().Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
            
			string neId = tokens[2];
			OCLogger.Info("Successfully loaded '" + neId + "'.");
			_isInitialized = true;
		}
		else
		if(message.ToString().StartsWith(SUCCESS_UNLOAD))
		{
			char[] separator = {' '};
			string[] tokens = message.ToString().Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

			// Format: SUCCESS UNLOAD NetworkElement_id avatar_id
			string neId = tokens[2];
			OCLogger.Info("Successfully unloaded '" + neId + "'.");
			_isInitialized = false;
		}
		else
		{
			// Get the plain text of this message(in XML format) and parse it.
			if(_isInitialized)
			{
				// Parse the message only when oac is ready.
				ParseXML(message.ToString());
			}
		}
		return false;
	}

	public void handleActionResult(OCAction action)
  {
      bool result = action.status == OCAction.Status.SUCCESS;
      
      // Send action status to my brain.
      SendActionStatus(_currentPlanId, action, result);
      //Debug.LogWarning("Action plan " + this.currentPlanId + " sequence " + action.Sequence + " status sent.");
      lock (_actionsList)
      {
      	_actionsList.Remove(action);
      }
  }
  
  public void HandleOtherAgentActionResult(OCAction action)
  {
    // don't report actions that game from us.
    // don't report actions without an action summary (these are from trying
    // to do non-existant actions).
		//@TODO: Find a different way to check for this...
//    if (ar.avatar == gameObject.GetComponent<Avatar>() || ar.action == null) {
//        //Debug.LogWarning("skipping action result from " + ar.avatar);
//        return;
//    }

    // the corresponding process within OpenCog's embodiment system is in PAI::processAgentActionWithParameters

    string timestamp = getCurrentTimestamp();
    XmlDocument doc = new XmlDocument();
    XmlElement root = makeXMLElementRoot(doc);

    XmlElement agentSignal = (XmlElement) root.AppendChild(doc.CreateElement("agent-signal"));
    agentSignal.SetAttribute("id", ar.avatar.gameObject.GetInstanceID().ToString());
    agentSignal.SetAttribute("type", ar.avatar.agentType);
    agentSignal.SetAttribute("timestamp", timestamp);
    XmlElement actionElement = (XmlElement)agentSignal.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ACTION_ELEMENT));
      
		// note that the name and the action-instance-name are different
		// ie: name = kick , while action-instance-name = kick2342
		actionElement.SetAttribute("name", ar.action.actionName);
		actionElement.SetAttribute("action-instance-name", ar.actionInstanceName);
		
		bool result = (ar.status == ActionResult.Status.SUCCESS ? true : false);
		actionElement.SetAttribute("result-state", "true"); //successful or failed
		if (ar.action.objectID == gameObject.GetInstanceID()) {
			actionElement.SetAttribute("target", _brainID);
		} else {
			actionElement.SetAttribute("target", ar.action.objectID.ToString());
		}
		
		// currently we only process the avatar and ocobject type, other types in EmbodimentXMLTages can is to be added when needed.
		// if you add other types such as BLOCK_OBJECT_TYPE, you should also modify PAI::processAgentActionWithParameters in opencog
		string targetType = ar.action.actionObject.tag;
		if (targetType == "OCA" || targetType == "Player")// it's an avatar
			actionElement.SetAttribute("target-type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
		else if (targetType == "OCObject") // it's an object
			actionElement.SetAttribute("target-type",OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
		else
			Debug.LogError("Error target type: " + targetType + " in action: " + ar.action.actionName);
				 
		// we can only process the parameter type defined in class ActionParamType both in opencog and unity
		// currently they are : boolean, int, float, string, vector, rotation, entity
		// also see opencog/opencog/embodiment/control/perceptionActionInterface/BrainProxyAxon.xsd
    ArrayList paramList = ar.parameters;
      
    if (paramList != null) 
		{
		
			int i;
			if (targetType == "OCA" || targetType == "Player")
			{
				if (ar.action.objectID == ar.avatar.gameObject.GetInstanceID())
					i = -1;
				else 
					i = 0;
			}
	    else
			{
				i = 0;
			}
				
     foreach (System.Object obj in paramList)
    	{
      	XmlElement param = (XmlElement)actionElement.AppendChild(doc.CreateElement("param"));
			
				// the first param in pinfo is usually the avator does this action, so we just skip it
				string paratype = obj.GetType().ToString();
				if (paratype == "System.Int32") // it's a int
				{
					param.SetAttribute("type", "int");
					param.SetAttribute("name", ar.action.pinfo[i+1].Name);
					param.SetAttribute("value", obj.ToString());
				}
				else if (paratype == "System.Single") // it's a float
				{
					param.SetAttribute("type", "float");
					param.SetAttribute("name", ar.action.pinfo[i+1].Name);
					param.SetAttribute("value", obj.ToString());
				}
				else if (paratype == "System.Boolean") // it's a bool
				{
					param.SetAttribute("type", "boolean");
					param.SetAttribute("name", ar.action.pinfo[i+1].Name);
				
					param.SetAttribute("value", obj.ToString().ToLower());
				}
				else if (paratype == "System.String")// it's a string
				{
					param.SetAttribute("type", "string");
					param.SetAttribute("name", ar.action.pinfo[i+1].Name);
					param.SetAttribute("value", obj.ToString());
				}
				// it's an entity, we only process the ActionTarget, 
				// if your parameter is an Entiy, please change it into ActionTarget type first
				else if (paratype == "ActionTarget") 
				{
					param.SetAttribute("type", "entity");
					param.SetAttribute("name", ar.action.pinfo[i+1].Name);
					XmlElement entityElement = (XmlElement)param.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ENTITY_ELEMENT));
					OCAction.Target entity = obj as OCAction.Target;
					entityElement.SetAttribute(OCEmbodimentXMLTags.ID_ATTRIBUTE, entity.id.ToString());
					entityElement.SetAttribute(OCEmbodimentXMLTags.TYPE_ATTRIBUTE, entity.type);
					
					// currently it seems not use of OWNER_ID_ATTRIBUTE and OWNER_NAME_ATTRIBUTE, we just skip them
				}
				else if ( paratype == "UnityEngine.Vector3") // it's an vector
				{   
					UnityEngine.Vector3 vec = (Vector3)obj ;
					param.SetAttribute("type", "vector");
					param.SetAttribute("name", ar.action.pinfo[i+1].Name);
					XmlElement vectorElement = (XmlElement)param.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.VECTOR_ELEMENT));
					vectorElement.SetAttribute(OCEmbodimentXMLTags.X_ATTRIBUTE, vec.x.ToString());
					vectorElement.SetAttribute(OCEmbodimentXMLTags.Y_ATTRIBUTE, vec.y.ToString());
					vectorElement.SetAttribute(OCEmbodimentXMLTags.Z_ATTRIBUTE, vec.z.ToString());
					
				}
				else if ( paratype ==  "IntVect") // it's an vector
				{   
					Vector3i vec = (Vector3i)obj ;
					param.SetAttribute("type", "vector");
					param.SetAttribute("name", ar.action.pinfo[i+1].Name);
					XmlElement vectorElement = (XmlElement)param.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.VECTOR_ELEMENT));
					vectorElement.SetAttribute(OCEmbodimentXMLTags.X_ATTRIBUTE, (vec.X + 0.5f).ToString());
					vectorElement.SetAttribute(OCEmbodimentXMLTags.Y_ATTRIBUTE, (vec.Y + 0.5f).ToString());
					vectorElement.SetAttribute(OCEmbodimentXMLTags.Z_ATTRIBUTE, (vec.Z + 0.5f).ToString());
					
				}
				// todo: we don't have a rotation type
				else 
				{
					// we can only process the type define in ActionParamType
					continue;
				}
				              
	      i++;                
	    }
    }

    Network.OCStringMessage message = new Network.OCStringMessage(_ID, _brainID, BeautifyXmlText(doc));

    OCLogger.Warn("sending action result from " + ar.avatar + "\n" + BeautifyXmlText(doc));

    lock (_messageSendingLock)
    {
        _messagesToSend.Add(message);
    }
  }
	
	
	// When isAppear is true, it's an appear action, if false, it's a disappear action 
	public void HandleObjectAppearOrDisappear(string objectID, string objectType, bool isAppear)
	{
		if (objectID == gameObject.GetInstanceID().ToString())
			return;
		
	  string timestamp = GetCurrentTimestamp();
    XmlDocument doc = new XmlDocument();
    XmlElement root = MakeXMLElementRoot(doc);
		
    XmlElement agentSignal = (XmlElement) root.AppendChild(doc.CreateElement("agent-signal"));
    agentSignal.SetAttribute("id", objectID);
		
		string targetType;

		if (objectType == "OCA" || objectType == "Player")// it's an avatar
			targetType = OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE;
		else // it's an object
			targetType = OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE;
		
    agentSignal.SetAttribute("type", "object");
    agentSignal.SetAttribute("timestamp", timestamp);
    XmlElement actionElement = (XmlElement)agentSignal.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ACTION_ELEMENT));
        
		// note that the name and the action-instance-name are different
		// ie: name = kick , while action-instance-name = kick2342
		if (isAppear)
		{
			actionElement.SetAttribute("name", "appear");
			actionElement.SetAttribute("action-instance-name", "appear"+ (++_appearActionCount).ToString());
		}
		else
		{
			actionElement.SetAttribute("name", "disappear");
			actionElement.SetAttribute("action-instance-name", "disappear"+ (++_disappearActionCount).ToString());
		}

		actionElement.SetAttribute("result-state", "true"); 
		
		actionElement.SetAttribute("target", objectID);
		actionElement.SetAttribute("target-type",targetType);		
		
   	Network.OCStringMessage message = new Network.OCStringMessage(_ID, _brainID, BeautifyXmlText(doc));
   	
   	OCLogger.Debugging("sending state change of " + objectID + "\n" + BeautifyXmlText(doc));
   	
   	lock (_messageSendingLock)
   	{
   	    _messagesToSend.Add(message);
   	}		
		
	}
	
	public void SendMoveActionDone (UnityEngine.GameObject obj,UnityEngine.Vector3 startPos, UnityEngine.Vector3 endPos)
	{
		if (obj == gameObject)
			return;
		
    string timestamp = GetCurrentTimestamp();
    XmlDocument doc = new XmlDocument();
    XmlElement root = MakeXMLElementRoot(doc);

    XmlElement agentSignal = (XmlElement) root.AppendChild(doc.CreateElement("agent-signal"));
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
		if (targetType == "OCA" || targetType == "Player")// it's an avatar
		{
			agentSignal.SetAttribute("type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
			actionElement.SetAttribute("target-type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
		}
		else// it's an object
		{
			agentSignal.SetAttribute("type", OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
			actionElement.SetAttribute("target-type",OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
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
		
		Network.OCStringMessage message = new Network.OCStringMessage(_ID, _brainID, BeautifyXmlText(doc));
        
    OCLogger.Debugging("sending move action result: \n" + BeautifyXmlText(doc));
        
    lock (_messageSendingLock)
    {
        _messagesToSend.Add(message);
    }
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
				
    XmlElement StateSignal = (XmlElement) root.AppendChild(doc.CreateElement("state-info"));
    StateSignal.SetAttribute("object-id", id);

		if (obj.tag == "OCA" || obj.tag == "Player")// it's an avatar
			StateSignal.SetAttribute("object-type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
		else // it's an object
			StateSignal.SetAttribute("object-type",OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);

    StateSignal.SetAttribute("state-name", stateName);
		StateSignal.SetAttribute("timestamp", timestamp);
		
		XmlElement valueElement = (XmlElement)StateSignal.AppendChild(doc.CreateElement("state-value"));
						
		if (valueType == "System.Int32") // it's a int
		{
			valueElement.SetAttribute("type", "int");
			valueElement.SetAttribute("value",stateValue.ToString());
		}
		else if (valueType == "System.Single") // it's a float
		{
			valueElement.SetAttribute("type", "float");
			valueElement.SetAttribute("value",stateValue.ToString());			
		}
		else if (valueType == "System.Boolean") // it's a bool
		{
			valueElement.SetAttribute("type", "boolean");
			valueElement.SetAttribute("value",stateValue.ToString().ToLower());
		}
		else if (valueType == "System.String")// it's a string
		{
			valueElement.SetAttribute("type", "string");
			valueElement.SetAttribute("value", stateValue as string);
			
		}
		else if (valueType == "UnityEngine.GameObject") 
		{
			valueElement.SetAttribute("type", "entity");
			XmlElement entityElement = (XmlElement)valueElement.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ENTITY_ELEMENT));
			MakeEntityElement(stateValue as UnityEngine.GameObject, entityElement);
		}
		else if ( valueType == "UnityEngine.Vector3") // it's an vector
		{   
			valueElement.SetAttribute("type", "vector");
			UnityEngine.Vector3 vec = (UnityEngine.Vector3)stateValue ;
			XmlElement vectorElement = (XmlElement)valueElement.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.VECTOR_ELEMENT));
			vectorElement.SetAttribute(OCEmbodimentXMLTags.X_ATTRIBUTE, vec.x.ToString());
			vectorElement.SetAttribute(OCEmbodimentXMLTags.Y_ATTRIBUTE, vec.y.ToString());
			vectorElement.SetAttribute(OCEmbodimentXMLTags.Z_ATTRIBUTE, vec.z.ToString());
		}
		
		// todo: we don't have a rotation type
		else 
		{
			// we can only process the type define in ActionParamType
			OCLogger.Warn("Unexcepted type: " + valueType + " in OCConnector::handleObjectStateChange!" );
			return;
		}

    Network.OCMessage message = Network.OCMessage.CreateMessage(_ID, _brainID, OpenCog.Network.OCMessage.MessageType.STRING, BeautifyXmlText(doc));
    
    OCLogger.Debugging("sending state change of " + obj + "\n" + BeautifyXmlText(doc));
    
    lock (_messageSendingLock)
    {
        _messagesToSend.Add(message);
    }		
	}	
	
	// we handle object state change as an "stateChange" action, and send it to the opencog via "agent-signal"
	// it will be processed in opencog the same as handleOtherAgentActionResult
	public void HandleObjectStateChange(UnityEngine.GameObject obj, string stateName, string valueType, System.Object oldValue, System.Object newValue, string blockId = "")
	{
		if (obj == gameObject)
			return;
		
    string timestamp = GetCurrentTimestamp();
    XmlDocument doc = new XmlDocument();
    XmlElement root = MakeXMLElementRoot(doc);
		
		string id;
		if (blockId != "")
			id = blockId;
		else
			id = obj.GetInstanceID().ToString();
			
		string targetType;
		if (blockId != "")
			targetType = OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE;
		else
		    targetType = obj.tag;
		
    XmlElement agentSignal = (XmlElement) root.AppendChild(doc.CreateElement("agent-signal"));
    agentSignal.SetAttribute("id", id);
    
    agentSignal.SetAttribute("timestamp", timestamp);
    XmlElement actionElement = (XmlElement)agentSignal.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ACTION_ELEMENT));
        
		// note that the name and the action-instance-name are different
		// ie: name = kick , while action-instance-name = kick2342
		actionElement.SetAttribute("name", "stateChange");
		actionElement.SetAttribute("action-instance-name", "stateChange"+ (++_stateChangeActionCount).ToString());

		actionElement.SetAttribute("result-state", "true"); 
		
		actionElement.SetAttribute("target", id);
		
		
		if (targetType == "OCA" || targetType == "Player")// it's an avatar
		{
			agentSignal.SetAttribute("type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
			actionElement.SetAttribute("target-type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
		}
		else // it's an object
		{
			agentSignal.SetAttribute("type", OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
			actionElement.SetAttribute("target-type",OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
		}
			
   	XmlElement paramStateName = (XmlElement)actionElement.AppendChild(doc.CreateElement("param"));
		paramStateName.SetAttribute("name", "stateName");
		paramStateName.SetAttribute("type", "string");
		paramStateName.SetAttribute("value",stateName );

    XmlElement paramOld = (XmlElement)actionElement.AppendChild(doc.CreateElement("param"));
		paramOld.SetAttribute("name", "OldValue");
    XmlElement paramNew = (XmlElement)actionElement.AppendChild(doc.CreateElement("param"));
		paramNew.SetAttribute("name", "NewValue");
		
				
		if (valueType == "System.Int32") // it's a int
		{
			paramOld.SetAttribute("type", "int");
			paramOld.SetAttribute("value",oldValue.ToString());
			
			paramNew.SetAttribute("type", "int");
			paramNew.SetAttribute("value",newValue.ToString());
		}
		else if (valueType == "System.Single") // it's a float
		{
			paramOld.SetAttribute("type", "float");
			paramOld.SetAttribute("value",oldValue.ToString());
			
			paramNew.SetAttribute("type", "float");
			paramNew.SetAttribute("value",newValue.ToString());
		}
		else if (valueType == "System.Boolean") // it's a bool
		{
			paramOld.SetAttribute("type", "boolean");
			paramOld.SetAttribute("value",oldValue.ToString().ToLower());
			
			paramNew.SetAttribute("type", "boolean");
			paramNew.SetAttribute("value",newValue.ToString().ToLower());
		}
		else if (valueType == "System.String")// it's a string
		{
			paramOld.SetAttribute("type", "string");
			paramOld.SetAttribute("value", oldValue as string);
			
			paramNew.SetAttribute("type", "string");
			paramNew.SetAttribute("value", newValue as string);
		}
		else if ((valueType == "UnityEngine.GameObject" ) || (valueType == "Avatar") )
		{
			paramOld.SetAttribute("type", "entity");
			XmlElement oldEntityElement = (XmlElement)paramOld.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ENTITY_ELEMENT));
			MakeEntityElement(oldValue as UnityEngine.GameObject, oldEntityElement);
			
			paramNew.SetAttribute("type", "entity");
			XmlElement newEntityElement = (XmlElement)paramNew.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ENTITY_ELEMENT));
			MakeEntityElement(newValue as UnityEngine.GameObject, newEntityElement);
		}
		else if ( valueType == "UnityEngine.Vector3") // it's an vector
		{   
			paramOld.SetAttribute("type", "vector");
			UnityEngine.Vector3 vec = (UnityEngine.Vector3)oldValue ;
			XmlElement oldVectorElement = (XmlElement)paramOld.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.VECTOR_ELEMENT));
			oldVectorElement.SetAttribute(OCEmbodimentXMLTags.X_ATTRIBUTE, vec.x.ToString());
			oldVectorElement.SetAttribute(OCEmbodimentXMLTags.Y_ATTRIBUTE, vec.y.ToString());
			oldVectorElement.SetAttribute(OCEmbodimentXMLTags.Z_ATTRIBUTE, vec.z.ToString());
			
			paramNew.SetAttribute("type", "vector");
			vec = (UnityEngine.Vector3)newValue ;
			XmlElement newVectorElement = (XmlElement)paramNew.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.VECTOR_ELEMENT));
			newVectorElement.SetAttribute(OCEmbodimentXMLTags.X_ATTRIBUTE, vec.x.ToString());
			newVectorElement.SetAttribute(OCEmbodimentXMLTags.Y_ATTRIBUTE, vec.y.ToString());
			newVectorElement.SetAttribute(OCEmbodimentXMLTags.Z_ATTRIBUTE, vec.z.ToString());			

		}
		// todo: we don't have a rotation type
		else 
		{
			// we can only process the type define in ActionParamType
			OCLogger.Warn("Unexcepted type: " + valueType + " in OCConnector::handleObjectStateChange!" );
		}


    Network.OCStringMessage message = new Network.OCStringMessage(_ID, _brainID, BeautifyXmlText(doc));
    
    OCLogger.Debugging("sending state change of " + obj + "\n" + BeautifyXmlText(doc));
    
    lock (_messageSendingLock)
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
    foreach (OCAction action in actionList)
		{
			// Set the action name
	    string ocActionName = OCActionController.getOCActionNameFromMap(action.actionName);
	        
			// check if the method name has a mapping to opencog action name.
			if (ocActionName == null) continue;
			
	    System.Xml.XmlElement actionElement = (XmlElement)avatarSignal.AppendChild(doc.CreateElement(OCEmbodimentXMLTags.ACTION_AVAILABILITY_ELEMENT));
	        
	    actionElement.SetAttribute("name", ocActionName);
	        
	    // Actions like "walk", "jump" are built-in naturally, they don't need an external target to act on.
	    if (action.objectID != gameObject.GetInstanceID())
			{
		    // Set the action target
		    actionElement.SetAttribute("target", action.objectID.ToString());
		        
		    // Set the action target type
				// currently we only process the avatar and ocobject type, other types in EmbodimentXMLTages can is to be added when needed.
				// if you add other types such as BLOCK_OBJECT_TYPE, you should also modify PAI::processAgentAvailability in opencog
				string targetType = action.actionObject.tag;
				if (targetType == "OCA" || targetType == "Player")// it's an avatar
					actionElement.SetAttribute("target-type", OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
				else if (targetType == "OCObject") // it's an object
					actionElement.SetAttribute("target-type", OCEmbodimentXMLTags.ORDINARY_OBJECT_TYPE);
				else
					OCLogger.Error("Error target type: " + targetType + " in action: " + action.actionName);
			}
							
	    actionElement.SetAttribute("available", available ? "true" : "false");
		}

    Network.OCStringMessage message = new Network.OCStringMessage(_ID, _brainID, BeautifyXmlText(doc));

    lock (_messageSendingLock)
    {
        _messagesToSend.Add(message);
    }
	}

  /**
   *  Add the map-info to message sending queue, yet we use the verb "send" here,
   *  because that's what we intend to do.
   */
  public void SendMapInfoMessage(List<OCObjectMapInfo> newMapInfoSeq, bool isFirstTimePerceptMapObjects= false)
  {
      // No new map info to send.
      if (newMapInfoSeq.Count == 0) return;

      LinkedList<OCObjectMapInfo> localMapInfo = new LinkedList<OCObjectMapInfo>(newMapInfoSeq);

      // If information of avatar itself is in the list, then put it at the first position.
      bool foundAvatarId = false;
      foreach (OCObjectMapInfo objMapInfo in localMapInfo)
      {
          if (objMapInfo.ID.Equals(_brainID))
          {
              localMapInfo.Remove(objMapInfo);
              localMapInfo.AddFirst(objMapInfo);
              foundAvatarId = true;
              break;
          }
      } // foreach
      
      if (!foundAvatarId && _isFirstSentMapInfo)
      {
          // First <map-info> message should contain information about the OCAvatar itself
          // If it is not there, skip this.
          OCLogger.Warn("Skipping first map-info message because it " +
                   "does not contain info about the avatar itself!");
          return;
      }
          
      Network.OCStringMessage message =
          (Network.OCStringMessage)SerializeMapInfo(new List<OCObjectMapInfo>(localMapInfo), "map-info", "map-data",isFirstTimePerceptMapObjects);

      lock (_messageSendingLock)
      {
          _messagesToSend.Add(message);
      } // lock

      // First map info message has been sent.
      _isFirstSentMapInfo = false;
  }

  /**
   * Add the terrain info message to message sending queue.
   */
  public void SendTerrainInfoMessage(List<OCObjectMapInfo> terrainInfoSeq, bool isFirstTimePerceptTerrain= false)
  {
      // No new map info to send.
      if (terrainInfoSeq.Count == 0) return;

      Network.OCStringMessage message =
          (Network.OCStringMessage)SerializeMapInfo(terrainInfoSeq, "terrain-info", "terrain-data",isFirstTimePerceptTerrain);

      lock (_messageSendingLock)
      {
          _messagesToSend.Add(message);
		SendMessages();
      } // lock
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
  private Network.OCMessage SerializeMapInfo(List<OCObjectMapInfo> mapinfoSeq, string messageTag, string payloadTag, bool isFirstTimePerceptWorld = false)
  {
      string timestamp = GetCurrentTimestamp();
      // Create a xml document
      XmlDocument doc = new XmlDocument();
      XmlElement root = MakeXMLElementRoot(doc);

      // Create a terrain-info element and append to root element.
      XmlElement mapInfo = (XmlElement)root.AppendChild(doc.CreateElement(messageTag));
	mapInfo.SetAttribute("map-name", _mapName);
      mapInfo.SetAttribute("global-position-x", _globalStartPositionX.ToString());
      mapInfo.SetAttribute("global-position-y", _globalStartPositionY.ToString());
	mapInfo.SetAttribute("global-position-z", _globalStartPositionZ.ToString());
      mapInfo.SetAttribute("global-position-offset-x", _blockCountX.ToString());
	mapInfo.SetAttribute("global-position-offset-y", _blockCountY.ToString());
	mapInfo.SetAttribute("global-position-offset-z", _blockCountZ.ToString());
      mapInfo.SetAttribute("global-floor-height", (_globalFloorHeight).ToString());
	mapInfo.SetAttribute("is-first-time-percept-world", isFirstTimePerceptWorld.ToString().ToLower());
	mapInfo.SetAttribute("timestamp", timestamp);

      string encodedPlainText;
      using (var stream = new System.IO.MemoryStream())
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

      return new Network.OCStringMessage(_ID, _brainID, BeautifyXmlText(doc));
  }
	
	public void SendFinishPerceptTerrian()
	{
		XmlDocument doc = new XmlDocument();
        XmlElement root = MakeXMLElementRoot(doc);
		string timestamp = GetCurrentTimestamp();
 
        XmlElement signal = (XmlElement) root.AppendChild(doc.CreateElement("finished-first-time-percept-terrian-signal"));
		signal.SetAttribute("timestamp", timestamp );
	
		Network.OCStringMessage message = new Network.OCStringMessage(_ID, _brainID, BeautifyXmlText(doc));
        
        OCLogger.Debugging("sending finished-first-time-percept-terrian-signal: \n" + BeautifyXmlText(doc));
        
        lock (_messageSendingLock)
        {
            _messagesToSend.Add(message);
        }

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
      if (!_isInitialized) {
          OCLogger.Warn("Avatar[" + _ID + "]: Received '" + text +
                  "' from player but I am not connected to an OAC.");
          return;
      }
      OCLogger.Debugging("Avatar[" + _ID + "]: Received '" + text + "' from player.");

      // Avoid creating messages if the destination (avatar brain) isn't available 
      if (!IsElementAvailable(_brainID))
          return;
      
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
      
      Network.OCMessage message = Network.OCMessage.CreateMessage(_ID, _brainID, Network.OCMessage.MessageType.RAW, speechMsg.ToString());
      
      // Add the message to the sending queue.
      lock (_messageSendingLock) {
          _messagesToSend.Add(message);
      }
  }
    
  /**
   * Save data and exit from embodiment system normally 
   * when finalizing the OCAvatar. 
   */
  public void SaveAndExit()
  {
      if (_isInitialized) {
          // TODO save local data of this avatar.
          UnloadOAC();
      }
	// TOFIX
	// Finalize is nog Uninitialize, which I believe is called on raising the OnDestroy event...?
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
        }
        catch (System.Exception e)
        {
            OCLogger.Error(e.ToString());
        }
        ParseDOMDocument(document);
    }

		private void ParsePsiDemandElement(XmlElement element)
    {
        string avatarId = element.GetAttribute(OCEmbodimentXMLTags.ENTITY_ID_ATTRIBUTE);

        // Parse all demands and add them to a map.
        XmlNodeList list = element.GetElementsByTagName(OCEmbodimentXMLTags.DEMAND_ELEMENT);
        for (int i = 0; i < list.Count; i++)
        {
            XmlElement demandElement = (XmlElement)list.Item(i);
            string demand = demandElement.GetAttribute(OCEmbodimentXMLTags.NAME_ATTRIBUTE);
            float value = float.Parse(demandElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));

            // group all demands to be updated only once
            _demandValueMap[demand] = value;

            OCLogger.Debugging("Avatar[" + _ID + "] -> parsePsiDemandElement: Demand '" + demand + "' value '" + value + "'.");
        }
    }

		private void ParseEmotionalFeelingElement(XmlElement element)
    {
        string avatarId = element.GetAttribute(OCEmbodimentXMLTags.ENTITY_ID_ATTRIBUTE);

        // Parse all feelings and add them to a map.
        XmlNodeList list = element.GetElementsByTagName(OCEmbodimentXMLTags.FEELING_ELEMENT);
        for (int i = 0; i < list.Count; i++)
        {
            XmlElement feelingElement = (XmlElement)list.Item(i);
            string feeling = feelingElement.GetAttribute(OCEmbodimentXMLTags.NAME_ATTRIBUTE);
            float value = float.Parse(feelingElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));

            // group all feelings to be updates only once
            _feelingValueMap[feeling] = value;

            OCLogger.Debugging("Avatar[" + _ID + "] -> parseEmotionalFeelingElement: Feeling '" + feeling + "' value '" + value + "'.");
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
        OCEmotionalExpression emotionalExpression = gameObject.GetComponent<OCEmotionalExpression>() as OCEmotionalExpression;
        emotionalExpression.showEmotionExpression(this.FeelingValueMap);

    }

	private void ParseDOMDocument(XmlDocument document)
    {
        // Handles action-plans
        XmlNodeList list = document.GetElementsByTagName(OCEmbodimentXMLTags.ACTION_PLAN_ELEMENT);
        for (int i = 0; i < list.Count; i++)
        {
            ParseActionPlanElement((XmlElement)list.Item(i));
        }

        // Handles emotional-feelings       
        XmlNodeList feelingsList = document.GetElementsByTagName(OCEmbodimentXMLTags.EMOTIONAL_FEELING_ELEMENT);
        for (int i = 0; i < feelingsList.Count; i++)
        {
            ParseEmotionalFeelingElement((XmlElement)feelingsList.Item(i));
        }
        
        // Handle psi-demand
        XmlNodeList demandsList = document.GetElementsByTagName(OCEmbodimentXMLTags.PSI_DEMAND_ELEMENT);
        for (int i = 0; i< demandsList.Count; i++)
        {
            ParsePsiDemandElement((XmlElement)demandsList.Item(i));
        }
		
		// Handle single-action-command
	    XmlNodeList singleActionList = document.GetElementsByTagName(OCEmbodimentXMLTags.SINGLE_ACTION_COMMAND_ELEMENT);
        for (int i = 0; i< singleActionList.Count; i++)
        {
            ParseSingleActionElement((XmlElement)singleActionList.Item(i));
        }
    }

	// this kind of message is from the opencog, and it's not a completed plan,
	// but a single action that current the robot want to do
	private void ParseSingleActionElement(XmlElement element)
	{
		OCActionController oca = gameObject.GetComponent<OCActionController>() as OCActionController;
		if (oca == null)
			return;
		
	  string actionName = element.GetAttribute(OCEmbodimentXMLTags.NAME_ATTRIBUTE);
			
    if (actionName == "BuildBlockAtPosition")
		{
			int x = 0,y = 0,z = 0;
			XmlNodeList list = element.GetElementsByTagName(OCEmbodimentXMLTags.PARAMETER_ELEMENT);
	        for (int i = 0; i < list.Count; i++)
	        {
	            XmlElement paraElement = (XmlElement)list.Item(i);
				string paraName = paraElement.GetAttribute(OCEmbodimentXMLTags.NAME_ATTRIBUTE);
				if (paraName == "x")
					x = int.Parse(paraElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));
				else if (paraName == "y")
					y = int.Parse(paraElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));
				else if (paraName == "z")
					z = int.Parse(paraElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));
				
	        }
			Vector3i blockBuildPoint = new Vector3i(x, y, z);
			
			oca.BuildBlockAtPosition(blockBuildPoint);
		}
		else if (actionName == "MoveToCoordinate")
		{
			int x = 0,y = 0,z = 0;
			XmlNodeList list = element.GetElementsByTagName(OCEmbodimentXMLTags.PARAMETER_ELEMENT);
	        for (int i = 0; i < list.Count; i++)
	        {
	            XmlElement paraElement = (XmlElement)list.Item(i);
				string paraName = paraElement.GetAttribute(OCEmbodimentXMLTags.NAME_ATTRIBUTE);
				if (paraName == "x")
					x = int.Parse(paraElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));
				else if (paraName == "y")
					y = int.Parse(paraElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));
				else if (paraName == "z")
					z = int.Parse(paraElement.GetAttribute(OCEmbodimentXMLTags.VALUE_ATTRIBUTE));
				
	        }
			UnityEngine.Vector3 vec = new UnityEngine.Vector3(x,z,y);
			oca.MoveToCoordinate(vec);
		}
 
	}

		 /**
     * Parse action plan and append the result into action list.
     *
     * @param element meta action in xml format
     */
    private void ParseActionPlanElement(XmlElement element)
    {
        // Get the action performer id.
        string avatarId = element.GetAttribute(OCEmbodimentXMLTags.ENTITY_ID_ATTRIBUTE);
        if (avatarId != _brainID)
        {
            // Usually this would not happen.
            OCLogger.Warn("Avatar[" + _ID + "]: This action plan is not for me.");
            return;
        }
 
        // Cancel current action and clear old action plan in the list.
        if (_actionsList.Count > 0)
        {
            OCLogger.Warn("Stop all current actions");
            CancelAvatarActions();
        }
        
        // Update current plan id and selected demand name.
        _currentPlanId = element.GetAttribute(OCEmbodimentXMLTags.ID_ATTRIBUTE);
        _currentDemandName = element.GetAttribute(OCEmbodimentXMLTags.DEMAND_ATTRIBUTE);
        
        XmlNodeList list = element.GetElementsByTagName(OCEmbodimentXMLTags.ACTION_ELEMENT);
        LinkedList<OCAction> actionPlan = new LinkedList<OCAction>();
        for (int i = 0; i < list.Count; i++)
        {
            OCAction avatarAction = OCAction.CreateAction((XmlElement)list.Item(i), true);

            actionPlan.AddLast(avatarAction);
        }

        lock (_actionsList)
        {
            _actionsList = actionPlan;
        }
        // Start to perform an action in front of the action list.
        _actionController.SendMessage("receiveActionPlan", actionPlan);
        //processNextAvatarAction();
    }
	/**
     * Cancel current action and clear actions from previous action plan.
     */
	private void CancelAvatarActions()
    {
        if(_actionsList.Count > 0)
            lock (_actionsList)
            {
                _actionsList.Clear();
            }
        
        // Ask action scheduler to stop all current actions.
        _actionController.SendMessage("cancelCurrentActionPlan");
        SendActionStatus(_currentPlanId, false);
    }

		  /// <summary>
    /// Load an OAC instance as the brain of this OCAvatar.
    /// </summary>
    private void LoadOAC()
    {
        System.Text.StringBuilder msgCmd = new System.Text.StringBuilder("LOAD_AGENT ");
        msgCmd.Append(_brainID + WHITESPACE + _masterID + WHITESPACE);
        msgCmd.Append(_type + WHITESPACE + _traits + "\n");

        Network.OCMessage msg = Network.OCMessage.CreateMessage(_ID,
                                      new OCConfig().get("SPAWNER_ID"),
                                      Network.OCMessage.MessageType.STRING,
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

        Network.OCMessage msg = Network.OCMessage.CreateMessage(_ID,
                                      new OCConfig().get("SPAWNER_ID"),
                                      Network.OCMessage.MessageType.STRING,
                                      msgCmd.ToString());
        if (!SendMessage(msg))
        {
            OCLogger.Warn("Could not send unload message to spawner.");
        }

        // Wait some time for the message to be sent.
        try {
            System.Threading.Thread.Sleep(500);
        } catch (System.Threading.ThreadInterruptedException e) {
            OCLogger.Error("Error putting NetworkElement main thread to sleep. " +
                           e.Message);
        }
    }


		 /**
     * Send the physiological information of this avatar to OAC with a tick message
     * for OAC to handle it.
     * This method would be invoked by physiological model.
     */
    private void SendAvatarSignalsAndTick(Dictionary<string, double> physiologicalInfo)
    {
        string timestamp = GetCurrentTimestamp();
        XmlDocument doc = new XmlDocument();
        XmlElement root = MakeXMLElementRoot(doc);
        
        // (currently this is avatar-signal, but should be changed...)
        XmlElement avatarSignal = (XmlElement)root.AppendChild(doc.CreateElement("avatar-signal"));
        avatarSignal.SetAttribute("id", _brainID);
        
        avatarSignal.SetAttribute("timestamp", timestamp);
        
        // Append all physiological factors onto the message content.
        foreach (string factor in physiologicalInfo.Keys)
        {
            // <physiology-level name="hunger" value="0.3"/>   
            XmlElement p = (XmlElement)avatarSignal.AppendChild(doc.CreateElement("physiology-level"));
            p.SetAttribute("name", factor);
            p.SetAttribute("value", physiologicalInfo[factor].ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
        }
        
        string xmlText = BeautifyXmlText(doc);
        //OCLogger.Debugging("OCConnector - sendAvatarSignalsAndTick: " + xmlText);
            
        // Construct a string message.
        Network.OCStringMessage message = new Network.OCStringMessage(_ID, _brainID, xmlText);

        lock (_messageSendingLock)
        {
            // Add physiological information to message sending queue.
            _messagesToSend.Add(message);

            // Send a tick message to make OAC start next cycle.
            if (bool.Parse(new OCConfig().get("GENERATE_TICK_MESSAGE")))
            {
                Network.OCMessage tickMessage = Network.OCMessage.CreateMessage(_ID, _brainID, OpenCog.Network.OCMessage.MessageType.TICK, "");
                _messagesToSend.Add(tickMessage);
            }
        }
    }

	private void MakeEntityElement(UnityEngine.GameObject obj, XmlElement entityElement)
	{

		if (obj == null)
		{
			entityElement.SetAttribute(OCEmbodimentXMLTags.ID_ATTRIBUTE, "null");
			entityElement.SetAttribute(OCEmbodimentXMLTags.TYPE_ATTRIBUTE, OCEmbodimentXMLTags.UNKNOWN_OBJECT_TYPE);
		}
		else
		{
			string targetType = obj.tag;
			if (targetType == "OCA" || targetType == "Player")// it's an avatar
			{
				entityElement.SetAttribute(OCEmbodimentXMLTags.ID_ATTRIBUTE, obj.GetInstanceID().ToString());
				entityElement.SetAttribute(OCEmbodimentXMLTags.TYPE_ATTRIBUTE, OCEmbodimentXMLTags.AVATAR_OBJECT_TYPE);
			}
			else // it's an object
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
  private void SendActionStatus(string planId, OCAction action, bool success)
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
      actionElement.SetAttribute(OCEmbodimentXMLTags.SEQUENCE_ATTRIBUTE, action.Sequence.ToString());
      actionElement.SetAttribute("name", action.Name);
      actionElement.SetAttribute("status", success ? "done" : "error");

      Network.OCStringMessage message = new Network.OCStringMessage(_ID, _brainID, BeautifyXmlText(doc));

      lock (_messageSendingLock)
      {
          _messagesToSend.Add(message);
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
   * @param success action result
   */
  private void SendActionStatus(string planId, bool success)
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

      Network.OCMessage message = Network.OCMessage.CreateMessage(_ID, _brainID, OpenCog.Network.OCMessage.MessageType.STRING, BeautifyXmlText(doc));

      lock (_messageSendingLock)
      {
          _messagesToSend.Add(message);
      }
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

}// namespace OpenCog.Network




