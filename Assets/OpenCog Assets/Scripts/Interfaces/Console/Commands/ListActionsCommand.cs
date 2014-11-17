
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
using UnityEngine;
using OpenCog.Utilities.Logging;

#region Usings, Namespaces, and Pragmas
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenCog.Actions;
using OpenCog.Attributes;
using OpenCog.Entities;
using OpenCog.Extensions;
using GameObject = UnityEngine.GameObject;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Utility.Console
{

/// <summary>
/// The OpenCog ListActionsCommand.
/// </summary>
#region Class Attributes
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class ListActionsCommand : Console.ConsoleCommand
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
		/// Raises the enable event when ListActionsCommand is loaded.
		/// </summary>
		public void OnEnable ()
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is enabled.");
		}

		/// <summary>
		/// Raises the disable event when ListActionsCommand goes out of scope.
		/// </summary>
		public void OnDisable ()
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is disabled.");
		}

		/// <summary>
		/// Raises the destroy event when ListActionsCommand is about to be destroyed.
		/// </summary>
		public void OnDestroy ()
		{
			Uninitialize ();
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is about to be destroyed.");
		}
		
		public override string Run (ArrayList arguments)
		{
			//OCObjects = GameObject.Find ("Objects") as GameObject;
			if (arguments.Count != 1)
				return "Wrong number of arguments";

			string agentName = (string)arguments [0];

			// Get the appropriate agent's actionController
			OCActionController actionController = null;
			OCActionController[] actionControllers = Object.FindObjectsOfType(typeof(OCActionController)) as OCActionController[];

	
			foreach(OCActionController ac in actionControllers)
			{
				if(ac.gameObject.name == agentName)
					actionController = ac;
			}

			if(actionController == null)
			{
				//If there is not currently an agent, then...
				return "There are no agent by this name with an OCActionController component. Use /load to create one.";
			}
			
			string result = actionController.gameObject.name + "'s actions: ";
			bool first = true;
			
			List<OCAction> actions = actionController.GetComponentsInChildren<OCAction>().ToList();
			
			foreach (OCAction action in actions)
			{
				if (!first) {
					result += "\n";
				}
				first = false;
				result += action.ToString();
			}
        
			return result;
		}

		public override ArrayList GetSignature ()
		{
			// Accepts one string as the NPC name
			KeyValuePair<System.Type,int> args = new KeyValuePair<System.Type,int> (System.Type.GetType ("string"), 2);
			ArrayList sig = new ArrayList ();
			sig.Add (args);
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
			
		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Other Members

		//---------------------------------------------------------------------------		

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenCog.ListActionsCommand"/> class.  
		/// Generally, intitialization should occur in the Start or Awake
		/// functions, not here.
		/// </summary>
		public ListActionsCommand ()
		{
			_commandName = "list";
		}

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	}// class ListActionsCommand

}// namespace OpenCog




