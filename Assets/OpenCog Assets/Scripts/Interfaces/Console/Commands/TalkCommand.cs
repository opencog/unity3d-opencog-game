
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

#region Usings, Namespaces, and Pragmas
using System.Collections;
using System.Collections.Generic;
using OpenCog.Attributes;
using OpenCog.Embodiment;
using OpenCog.Extensions;
using GameObject = UnityEngine.GameObject;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using Type = System.Type;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Utility.Console
{

/// <summary>
/// The OpenCog TalkCommand.
/// </summary>
#region Class Attributes
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class TalkCommand : Console.ConsoleCommand
	{

		//---------------------------------------------------------------------------

	#region Private Member Data

		//---------------------------------------------------------------------------
		
		private UnityEngine.GameObject player;

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
		/// Raises the enable event when TalkCommand is loaded.
		/// </summary>
		public void OnEnable ()
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is enabled.");
		}

		/// <summary>
		/// Raises the disable event when TalkCommand goes out of scope.
		/// </summary>
		public void OnDisable ()
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is disabled.");
		}

		/// <summary>
		/// Raises the destroy event when TalkCommand is about to be destroyed.
		/// </summary>
		public void OnDestroy ()
		{
			Uninitialize ();
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is about to be destroyed.");
		}
		
		public override string Run(ArrayList arguments) {
       // string text = string.Join(" ", arguments.ToArray(typeof(string)) as string[]);
        // Send the message to all avatars.
				//@TODO: Reimplement the talk command...
//        foreach (GameObject go in OCARepository.GetAllOCA()) 
//				{
//            // Don't send message to the player
//            if (go.tag == "Player") continue;
//            // Send message to OpenCog avatars
//						OCConnector connection = go.GetComponent<OCConnector>();
//						if (connection != null)
//                connection.SendSpeechContent(text,player);
//            // TODO: send the message to other human players using Unity RPC
//        }
        // return Null because sendPredavese updates the log somehow..
        return null;
    }

    public override ArrayList GetSignature() {
        // Unlimited strings allowed...
        KeyValuePair<Type,int> kt = new KeyValuePair<Type,int>(Type.GetType("string"),0);
        ArrayList sig = new ArrayList();
        sig.Add(kt);
        return sig;
    }
    
    public override string GetName() {
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
			player = GameObject.FindGameObjectWithTag("Player");
		}
	
		/// <summary>
		/// Uninitializes this instance.  Cleanup refernces here.
		/// </summary>
		private void Uninitialize ()
		{
		}
			
		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Other Members

		//---------------------------------------------------------------------------		

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenCog.TalkCommand"/> class.  
		/// Generally, intitialization should occur in the Start or Awake
		/// functions, not here.
		/// </summary>
		public TalkCommand ()
		{
			_commandName = "say";
		}

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	}// class TalkCommand

}// namespace OpenCog




