
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
using OpenCog.Master;
using OpenCog.Utilities.Logging;

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
/// The OpenCog Console LoadCommand.
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

		// TODO [MULTI-AVATAR]: Replace code below once the (real) property exposition stuff works.
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
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is awake.");
		}

		/// <summary>
		/// Use this for initialization
		/// </summary>
		public new void Start ()
		{
			base.Start();
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is started.");
		}

		/// <summary>
		/// Update is called once per frame.
		/// </summary>
		public void Update ()
		{
			//System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is updated.");	
		}
		
		/// <summary>
		/// Reset this instance to its default values.
		/// </summary>
		public void Reset ()
		{
			Uninitialize ();
			Initialize ();
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is reset.");	
		}

		/// <summary>
		/// Raises the enable event when LoadCommand is loaded.
		/// </summary>
		public void OnEnable ()
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is enabled.");
		}

		/// <summary>
		/// Raises the disable event when LoadCommand goes out of scope.
		/// </summary>
		public void OnDisable ()
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is disabled.");
		}

		/// <summary>
		/// Raises the destroy event when LoadCommand is about to be destroyed.
		/// </summary>
		public void OnDestroy ()
		{
			Uninitialize ();
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is about to be destroyed.");
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
			// TODO [MULTI-AVATAR]: Remove stuff below once (real) property exposition components are working.
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
		


		/// <summary>
		/// It's purpose is to wrap LoadAgent(agentName, pos)
		/// </summary>
		/// <returns>Failure, success, or the continuation of a coroutine which attempts to establish a connection with the OpenCog embodiment.</returns>
		/// <param name="agentName">The name of the agent to instantiate.</param>
		private IEnumerator LoadAgent(string agentName)
		{
			//get the player object, if it exists
			UnityEngine.GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");

			//if it does not exist, we cannot run this wrapper
			if (playerObject == null) {
				Debug.LogError("No object tagged with player.");
				yield break;
			}

			//we want to place it ahead of the player
			UnityEngine.Vector3 eulerAngle = playerObject.transform.rotation.eulerAngles;
			UnityEngine.Vector3 position = playerObject.transform.position;

			//find a position slightly in front of us
			float zFront = 3.0f * (float)Math.Cos ((eulerAngle.y / 180) * Math.PI);
			float xFront = 3.0f * (float)Math.Sin ((eulerAngle.y / 180) * Math.PI);

			//add the position data together. 
			UnityEngine.Vector3 newPos = new Vector3(position.x + xFront, position.y + 2, position.z + zFront);

			// TODO [MULTI-AVATAR] Currently we use the tag "player". However, when there are multiple 
			// players in the world, we need to figure out a way to identify.
			// Set agentType and agentTraits in the future.
			// leave agentType and agentTraits to null just for test.
			//get data about the player
			string masterId = playerObject.GetInstanceID ().ToString ();
			string masterName = playerObject.name;

			//Attack of the top level management decisions!
			yield return StartCoroutine(GameManager.entity.load.AtRunTime(newPos, _NPCAgent,agentName, masterName, masterId));

			//this is unnecessary
			//yield break;
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




