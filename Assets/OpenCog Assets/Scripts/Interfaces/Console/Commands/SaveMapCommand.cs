
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
using OpenCog.Extensions;
using GameObject = UnityEngine.GameObject;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using OpenCog.Map;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Utility.Console
{

/// <summary>
/// The OpenCog SaveMapCommand.
/// </summary>
#region Class Attributes
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class SaveMapCommand : Console.ConsoleCommand
	{

		//---------------------------------------------------------------------------

	#region Private Member Data

		//---------------------------------------------------------------------------
		
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
		/// Raises the enable event when SaveMapCommand is loaded.
		/// </summary>
		public void OnEnable ()
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is enabled.");
		}

		/// <summary>
		/// Raises the disable event when SaveMapCommand goes out of scope.
		/// </summary>
		public void OnDisable ()
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is disabled.");
		}

		/// <summary>
		/// Raises the destroy event when SaveMapCommand is about to be destroyed.
		/// </summary>
		public void OnDestroy ()
		{
			Uninitialize ();
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is about to be destroyed.");
		}
		
		public override string Run (ArrayList arguments)
		{
			string mapName = "tempmap.blocks";
			// Check whether we were given a name to call the avatar
			if (arguments.Count > 0)
				mapName = (string)arguments [0];
		
			if (saveWorld (mapName))
				return "Saved map to " + mapName;
			else
				return "Saving map to " + mapName + " failed";
		
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
		}
	
		/// <summary>
		/// Uninitializes this instance.  Cleanup refernces here.
		/// </summary>
		private void Uninitialize ()
		{
		}
		
		private bool saveWorld (string filename)
		{
			// get the world game object
			// get the world data
			// and start the save process
			//return GameObject.Find("Map").GetComponent<OCMap>().;
			//return GameObject.Find ("World").GetComponent<WorldGameObject> ().WorldData.saveData (filename);
			return false;
		}
			
		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Other Members

		//---------------------------------------------------------------------------		

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenCog.SaveMapCommand"/> class.  
		/// Generally, intitialization should occur in the Start or Awake
		/// functions, not here.
		/// </summary>
		public SaveMapCommand ()
		{
			_commandName = "savemap";
		}

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	}// class SaveMapCommand

}// namespace OpenCog




