
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
using OpenCog.Embodiment;
using OpenCog.Extensions;
using GameObject = UnityEngine.GameObject;
using ImplicitFields = ProtoBuf.ImplicitFields;
using Math = System.Math;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Debug = UnityEngine.Debug;
using OpenCog.Actions;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Utility.Console
{

/// <summary>
/// The OpenCog LoadCommand.
/// </summary>
#region Class Attributes
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class LoadCommand : Console.ConsoleCommand
	{

		//---------------------------------------------------------------------------

	#region Private Member Data

		//---------------------------------------------------------------------------
		
		private string _CommandName = "load";
    	private GameObject _NPCAgent;

		// TODO: Replace code below once the (real) property exposition stuff works.
		public GameObject NPCAvatarMember;

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Accessors and Mutators

		//---------------------------------------------------------------------------
		
		public GameObject NPCAvatar 
		{
			get {return _NPCAgent;}
			set {_NPCAgent = value;}
		}	
		
		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Public Member Functions

		//---------------------------------------------------------------------------

		/// <summary>
		/// Called when the script instance is being loaded.
		/// </summary>
		public void Awake ()
		{
			Initialize ();
			OCLogger.Fine (gameObject.name + " is awake.");
		}

		/// <summary>
		/// Use this for initialization
		/// </summary>
		public new void Start ()
		{
			base.Start();
			OCLogger.Fine (gameObject.name + " is started.");
		}

		/// <summary>
		/// Update is called once per frame.
		/// </summary>
		public void Update ()
		{
			OCLogger.Fine (gameObject.name + " is updated.");	
		}
		
		/// <summary>
		/// Reset this instance to its default values.
		/// </summary>
		public void Reset ()
		{
			Uninitialize ();
			Initialize ();
			OCLogger.Fine (gameObject.name + " is reset.");	
		}

		/// <summary>
		/// Raises the enable event when LoadCommand is loaded.
		/// </summary>
		public void OnEnable ()
		{
			OCLogger.Fine (gameObject.name + " is enabled.");
		}

		/// <summary>
		/// Raises the disable event when LoadCommand goes out of scope.
		/// </summary>
		public void OnDisable ()
		{
			OCLogger.Fine (gameObject.name + " is disabled.");
		}

		/// <summary>
		/// Raises the destroy event when LoadCommand is about to be destroyed.
		/// </summary>
		public void OnDestroy ()
		{
			Uninitialize ();
			OCLogger.Fine (gameObject.name + " is about to be destroyed.");
		}
		
		public override string Run (ArrayList arguments)
		{
			string avatarName = "";
			// Check whether we were given a name to call the avatar
			if (arguments.Count > 0)
				avatarName = (string)arguments [0];
	        
			StartCoroutine (LoadAgent (avatarName));
			
			return "Starting OAC named " + avatarName;
		}
		
		public override ArrayList GetSignature ()
		{
			// Accepts one string as the NPC name
			KeyValuePair<System.Type,int> kt = new KeyValuePair<System.Type,int> (System.Type.GetType ("string"), 1);
			ArrayList sig = new ArrayList ();
			sig.Add (kt);
			return sig;
		}
		
		public override string GetName ()
		{
			return _commandName;
		}
		
		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Private Member Functions

		//---------------------------------------------------------------------------
	
		/// <summary>
		/// Initializes this instance.  Set default values here.
		/// </summary>
		private void Initialize ()
		{
			// TODO: Remove stuff below once (real) property exposition components are working.
			if (_NPCAgent == null)
			{
				_NPCAgent = NPCAvatarMember;
			}
		}
	
		/// <summary>
		/// Uninitializes this instance.  Cleanup refernces here.
		/// </summary>
		private void Uninitialize ()
		{
		}
		
		private string CreateRandomAgentName ()
		{
			int randomID = UnityEngine.Random.Range (1, 100);
			string[] baseNames = { "Hazuki", "Bender", "Bozwollocks", "Wolverine", "Bumblebee", "OompaLoompa" };
			int baseNameIndex = UnityEngine.Random.Range (0, baseNames.Length);
			return baseNames [baseNameIndex] + randomID.ToString ();
		}
		
		private IEnumerator LoadAgent (string agentName)
		{
			UnityEngine.GameObject agentClone;
		
			UnityEngine.GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");
			if (playerObject == null) {
				Debug.Log ("No object tagged with player.");
				yield return "No object tagged with player.";
			}

			// Record the player's position and make the OCAvatar spawn near it.
			UnityEngine.Vector3 playerPos = playerObject.transform.position;
			
			//Debug.Log ("PlayerPos = [" + playerPos.x + ", " + playerPos.y + ", " + playerPos.z + "]");

			// Calculate the player's forward direction
			UnityEngine.Vector3 eulerAngle = playerObject.transform.rotation.eulerAngles;
			
			//Debug.Log ("eulerAngle = [" + eulerAngle.x + ", " + eulerAngle.y + ", " + eulerAngle.z + "]");

			float zFront = 3.0f * (float)Math.Cos ((eulerAngle.y / 180) * Math.PI);
			float xFront = 3.0f * (float)Math.Sin ((eulerAngle.y / 180) * Math.PI);
			
			UnityEngine.Vector3 spawnPosition = new UnityEngine.Vector3(playerPos.x + xFront, playerPos.y + 2, playerPos.z + zFront);
			spawnPosition.x = (float)((int)spawnPosition.x);
			spawnPosition.y = (float)((int)spawnPosition.y);
			spawnPosition.z = (float)((int)spawnPosition.z);
			
			//Debug.Log ("spawnPosition = [" + spawnPosition.x + ", " + spawnPosition.y + ", " + spawnPosition.z + "]");

			// Instantiate an OCAvatar in front of the player.
			
			//Debug.Log ("_NPCAgent is" + (_NPCAgent == null ? " null " : " not null"));
			
			agentClone = (GameObject)UnityEngine.Object.Instantiate (_NPCAgent, spawnPosition, Quaternion.identity);
			agentClone.transform.parent = GameObject.Find("Characters").transform;
			OCActionController agiAC = agiAC = agentClone.GetComponent<OCActionController>();
			agiAC.DefaultEndTarget = GameObject.Find("EndPointStub");
			agiAC.DefaultStartTarget = GameObject.Find("StartPointStub");
			
			//Debug.Log ("agentClone is" + (agentClone == null ? " null " : " not null"));

			OCConnectorSingleton connector = OCConnectorSingleton.Instance;
			
			UnityEngine.Debug.Log ("The GUID of our OCC instance in LoadAgent is " + connector.VerificationGuid);
			
			//Debug.Log ("connector is" + (connector == null ? " null " : " not null"));

			if (agentName == "")
				agentName = CreateRandomAgentName ();
			
			//Debug.Log("We shall name him '" + agentName + "'");
        
			agentClone.name = agentName;
        
//			if (agentClone != null) {
//				if (!OCARepository.AddOCA (agentClone)) {
//					// An avatar with given name is already there.
//					yield break;
//				}
//				Debug.Log ("Add avatar[" + agentName + "] to avatar map.");
//			}
			
			// Get the player id as the master id of the avatar.
			// TODO Currently we use the tag "player". However, when there are multiple 
			// players in the world, we need to figure out a way to identify.
			string masterId = playerObject.GetInstanceID ().ToString ();
			string masterName = playerObject.name;
		
			// TODO Set agentType and agentTraits in the future.
			// leave agentType and agentTraits to null just for test.
			connector.Init (agentName, null, null, masterId, masterName);

			yield return StartCoroutine(connector.ConnectOAC());
		
			if (!connector.IsInitialized) {
				// OAC is not loaded normally, destroy the avatar instance.
				Debug.LogError ("Cannot connect to the OAC, avatar loading failed.");
				connector.SaveAndExit ();
				Destroy (agentClone);
				yield break;
			} 
		}
			
		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Other Members

		//---------------------------------------------------------------------------		

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenCog.LoadCommand"/> class.  
		/// Generally, intitialization should occur in the Start or Awake
		/// functions, not here.
		/// </summary>
		public LoadCommand ()
		{
			_commandName = "load";
		}

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	}// class LoadCommand

}// namespace OpenCog




