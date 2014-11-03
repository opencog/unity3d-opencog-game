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
using OpenCog.Actions;
using UnityEngine;
using System;

#pragma warning disable 0414
#endregion

namespace OpenCog.Character
{
	
	public partial class CharacterManager:OCSingletonMonoBehaviour<CharacterManager>
	{
		///<summary><para>Access this interface's methods using GameManager.Character.load.*</para>
		///<para>This interface exposes a subcatagory of CharacterManager methods, which should be used for loading characters.</para></summary>
		public interface LoadMethods
		{
			IEnumerator AtRunTime(UnityEngine.Vector3 spawnPosition, GameObject agentPrefab, string agentName = "", string masterName = "", string masterId = "");
		}

		///<summary><para>Access this class's methods using GameManager.Character.load.*</para>
		/// <para>A subcatagory of CharacterManager methods, which should be used for loading characters. Nested in CharacterManager, its functions are exposed to the outside through the LoadMethods interface.</para></summary>
		protected class _LoadMethods:OCSingletonMonoBehaviour<_LoadMethods>, LoadMethods
		{
			protected _LoadMethods(){}

			/// <summary>This initialization function will be called automatically by the SingletonMonoBehavior's Awake().</summary>
			protected override void Initialize()
			{
				//shouldn't be destroyed with scenes. 
				DontDestroyOnLoad(this);

			}

			/// <summary>Used to instantiate this class. It should only be called once. It will supply a single instance, and then throw an error
			/// if it is called a second time.</summary>
			public static _LoadMethods New()
			{
				//THERE CAN ONLY BE ONE (and it must be created by the CharacterManager)
				if(_instance)
				{
					throw new OCException( "Two CharacterManager.LoadMethods exist and this is forbidden.");

				}
				
				//the Singleton pattern handles everything about instantiation for us, including searching
				//the game for a pre-existing object.
				return GetInstance<_LoadMethods>();

			}


			/// <summary>
			/// Gives functionality to the 'load' console command. 
			/// </summary>
			/// <returns>Failure, success, or a coroutine concerned with making a connection to the embodiment.</returns>
			/// <param name="agentName">The name of the agent to instantiate.</param>
			/// <param name="spawnPosition">The position at which to instantiate the agent.</param>
			public IEnumerator AtRunTime(UnityEngine.Vector3 spawnPosition, GameObject agentPrefab, string agentName = "", string masterName = "", string masterId = "")
			{
				//get the spawn position in terms of the grid
				spawnPosition.x = (float)((int)spawnPosition.x); 
				spawnPosition.y = (float)((int)spawnPosition.y);
				spawnPosition.z = (float)((int)spawnPosition.z); 
				
				
				
				//Debug.Log ("_NPCAgent is" + (_NPCAgent == null ? " null " : " not null"));
				//instantiate the game object, specified by NPCAgent at  point spawn position and rotated as per the identity Quaternion (that is, not at all)
				UnityEngine.GameObject agentClone = (GameObject)UnityEngine.Object.Instantiate (agentPrefab, spawnPosition, Quaternion.identity);
				
				//All agents belong to the Characters parent game object.
				agentClone.transform.parent = GameObject.Find("Characters").transform;
				
				//the _NPCAgent should have come with an OCActionController
				OCActionController agiAC = agentClone.GetComponent<OCActionController>();
				
				//
				agiAC.DefaultEndTarget = GameObject.Find("EndPointStub");
				agiAC.DefaultStartTarget = GameObject.Find("StartPointStub");
				
				//Debug.Log ("agentClone is" + (agentClone == null ? " null " : " not null"));
				//
				OCConnectorSingleton connector = OCConnectorSingleton.Instance;
				UnityEngine.Debug.Log ("The GUID of our OCC instance in LoadAgent is " + connector.VerificationGuid);
				//Debug.Log ("connector is" + (connector == null ? " null " : " not null"));
				
				
				//Ensure our agent is properly named
				if (agentName == "")
					agentName = CreateRandomAgentName ();
				agentClone.name = agentName;
				
				
				//			if (agentClone != null) {
				//				if (!OCARepository.AddOCA (agentClone)) {
				//					// An avatar with given name is already there.
				//					yield break;
				//				}
				//				Debug.Log ("Add avatar[" + agentName + "] to avatar map.");
				//			}
				
				//initialize the connector
				connector.Init (agentName, null, null, masterId, masterName);
				
				//and try to connect
				yield return StartCoroutine(connector.ConnectOAC());
				
				//if we failed to initialize the connector
				if (!connector.IsInitialized) {
					
					// OAC is not loaded normally, destroy the avatar instance.
					Debug.LogError ("Cannot connect to the OAC, avatar loading failed.");
					connector.SaveAndExit ();
					Destroy (agentClone);
				} 
				
			}
			
			private string CreateRandomAgentName ()
			{
				int randomID = UnityEngine.Random.Range (1, 100);
				string[] baseNames = { "Hazuki", "Bender", "Bozwollocks", "Wolverine", "Bumblebee", "OompaLoompa" };
				int baseNameIndex = UnityEngine.Random.Range (0, baseNames.Length);
				return baseNames [baseNameIndex] + randomID.ToString ();
			}
		}
	}
}
