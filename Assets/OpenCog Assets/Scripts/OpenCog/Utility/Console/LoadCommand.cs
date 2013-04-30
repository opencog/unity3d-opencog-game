
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

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog
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
		
		/// <summary>
		/// An example variable.  Don't fall into the trap of making all variables
		/// public (I know Unity encourages you to do this).  Instead, make use of
		/// public properties whenever possible.
		/// </summary>

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Accessors and Mutators

		//---------------------------------------------------------------------------
		
		/// <summary>
		/// Gets or sets the example variable.  Includes attribute examples.
		/// </summary>
		/// <value>
		/// The example variable.
		/// </value>
			
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
		public void Start ()
		{
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
	        
			StartCoroutine (LoadAvatar (avatarName));
			
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
			return m_commandName;
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
		}
	
		/// <summary>
		/// Uninitializes this instance.  Cleanup refernces here.
		/// </summary>
		private void Uninitialize ()
		{
		}
		
		private string CreateRandomAvatarName ()
		{
			int randomID = UnityEngine.Random.Range (1, 100);
			string[] baseNames = { "Hazuki", "Bender", "Bozwollocks", "Wolverine", "Bumblebee", "OompaLoompa" };
			int baseNameIndex = UnityEngine.Random.Range (0, baseNames.Length);
			return baseNames [baseNameIndex] + randomID.ToString ();
		}
		
		private IEnumerator LoadAvatar (string avatarName)
		{
			UnityEngine.GameObject avatarClone;
		
			UnityEngine.GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");
			if (playerObject == null) {
				yield return "No object tagged with player.";
			}

			// Record the player's position and make the OCAvatar spawn near it.
			UnityEngine.Vector3 playerPos = playerObject.transform.position;

			// Calculate the player's forward direction
			UnityEngine.Vector3 eulerAngle = playerObject.transform.rotation.eulerAngles;

			float zFront = 3.0f * (float)Math.Cos ((eulerAngle.y / 180) * Math.PI);
			float xFront = 3.0f * (float)Math.Sin ((eulerAngle.y / 180) * Math.PI);

			// Instantiate an OCAvatar in front of the player.
			avatarClone = (GameObject)UnityEngine.Object.Instantiate (NPCAvatar,
                new Vector3 (playerPos.x + xFront,
		                    playerPos.y + 2,
                            playerPos.z + zFront),
                Quaternion.identity);

			OCConnector connector = avatarClone.GetComponent ("OCConnector") as OCConnector;
        
			if (avatarName == "")
				avatarName = createRandomAvatarName ();
        
			avatarClone.name = avatarName;
        
			if (avatarClone != null) {
				if (!OCARepository.AddOCA (avatarClone)) {
					// An avatar with given name is already there.
					yield break;
				}
				Debug.Log ("Add avatar[" + avatarName + "] to avatar map.");
			}
			// Get the player id as the master id of the avatar.
			// TODO Currently we use the tag "player". However, when there are multiple 
			// players in the world, we need to figure out a way to identify.
			string masterId = playerObject.GetInstanceID ().ToString ();
			string masterName = playerObject.name;
		
			// TODO Set agentType and agentTraits in the future.
			// leave agentType and agentTraits to null just for test.
			connector.Init (avatarName, null, null, masterId, masterName);

			yield return StartCoroutine(connector.connectOAC());
		
			if (!connector.IsInit ()) {
				// OAC is not loaded normally, destroy the avatar instance.
				Debug.LogError ("Cannot connect to the OAC, avatar loading failed.");
				connector.saveAndExit ();
				Destroy (avatarClone);
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
			m_commandName = "load";
		}

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	}// class LoadCommand

}// namespace OpenCog




