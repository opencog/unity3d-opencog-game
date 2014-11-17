
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
using System.Linq;
using OpenCog.Attributes;
using OpenCog.Entities;
using OpenCog.Extensions;
using Component = UnityEngine.Component;
using GameObject = UnityEngine.GameObject;
using ImplicitFields = ProtoBuf.ImplicitFields;
using Object = UnityEngine.Object;
using OCID = System.Guid;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using OpenCog.Actions;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Utility.Console
{

/// <summary>
/// The OpenCog ActionCommand.
/// </summary>
#region Class Attributes
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class ActionCommand : Console.ConsoleCommand
	{

		//---------------------------------------------------------------------------

	#region Private Member Data

		//---------------------------------------------------------------------------
		
		/// <summary>
		/// An example variable.  Don't fall into the trap of making all variables
		/// public (I know Unity encourages you to do this).  Instead, make use of
		/// public properties whenever possible.
		/// </summary>
		private int _exampleVar;

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
		/// Raises the enable event when ActionCommand is loaded.
		/// </summary>
		public void OnEnable ()
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is enabled.");
		}

		/// <summary>
		/// Raises the disable event when ActionCommand goes out of scope.
		/// </summary>
		public void OnDisable ()
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is disabled.");
		}

		/// <summary>
		/// Raises the destroy event when ActionCommand is about to be destroyed.
		/// </summary>
		public void OnDestroy ()
		{
			Uninitialize ();
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is about to be destroyed.");
		}
		
		public override string Run (ArrayList arguments)
		{
			//if (arguments.Count != 2) return "Wrong number of arguments";
			string sourceName = (string)arguments [0];
			string targetName = (string)arguments [1];//also targetStartName
			string actionName = (string)arguments [2];
			string targetEndName = (string)arguments [3];


			// Get the appropriate agent and gameobject		
			OCActionController actionController = null;
			OCActionController[] actionControllers = Object.FindObjectsOfType(typeof(OCActionController)) as OCActionController[];

			foreach(OCActionController ac in actionControllers)
			{
				if(ac.gameObject.name == sourceName)
					actionController = ac;
			}
			
			if (actionController == null)
				return "No Action Controller script on agent \"" + sourceName + "\".";
			if (actionController.gameObject.tag == "Player")
				return "Agent \"" + sourceName + "\" is controlled by the player!";
			
			OCID sourceID = actionController.ID;

			// Get the object
			// if objectName is "self" then assume the script is on the agent
			OCID targetStartID;
			if (targetName != null && targetName != "self") 
			{
				GameObject target = GameObject.Find(targetName);
				if (target == null)
					return "No object called " + targetName;
				targetStartID = target.GetComponent<OCActionController>().ID;
			} 
			else 
			{
				targetStartID = actionController.ID;
			}
			
			OCID targetEndID;
			if(targetEndName != null && targetEndName != "self")
			{
				GameObject targetEnd = GameObject.Find(targetEndName);
				if (targetEnd == null)
					return "No object called " + targetEndName;
				targetEndID = targetEnd.GetComponent<OCActionController>().ID;
			} 
			else 
			{
				targetEndID = actionController.ID;
			}
			
			// Get the action from the Action Controller
			OCAction action = 
				actionController.GetComponentsInChildren<OCAction>()
				.	Where(a => a.name == actionName)
				.	Cast<OCAction>()
				.	FirstOrDefault()
			;

			if (action == null)
				return "No action called \"" + actionName + "\".";
			
			actionController.StartAction(action, sourceID, targetStartID, targetEndID);

//			System.Reflection.ParameterInfo[] pinfo = action.pinfo; //getFreeArguments();
//			
//			if (pinfo.Length > 0) {
//				ArrayList args = new ArrayList ();
//				int i = 0;
//				int jmod = 3;
//				if (action.componentType == typeof(Avatar)) {
//					// Check we don't have too many arguments
//					if (pinfo.Length < arguments.Count - 3)
//						return "Expected " + pinfo.Length + " arguments, got " + (arguments.Count - 3) + ".";
//				} else {
//					// Check we don't have too many arguments, we don't need to
//					// provide the avatar
//					if (pinfo.Length - 1 < arguments.Count - 3)
//						return "Expected " + (pinfo.Length - 1) + " arguments, got " + (arguments.Count - 3) + ".";
//					i = 1;
//					jmod = 2;
//				}
//				// ignore last parameter if action uses a callback
//				int lengthModififer = 0;
//				if (action.usesCallback)
//					lengthModififer = 1; 
//				for (; i < pinfo.Length-lengthModififer; i++) {
//					// do type checking and conversion from strings to the expected type
//					// ignore 3 console arguments: the action name, avatar name,
//					//    and the object with the action (from console arguments)
//					int j = i + jmod;
//					if (j >= arguments.Count) {
//						// Not enough arguments, so must be a default argument
//						if (!pinfo [i].IsOptional)
//							return "Missing parameter " + pinfo [i].Name + " is not optional.";
//						args.Add (pinfo [i].DefaultValue);
//					} else {
//						arguments [j] = ((string)arguments [j]).Replace ("\"", "");
//						// Depending on the expected type we convert it differently
//						if (pinfo [i].ParameterType == typeof(GameObject)) {
//							// Parameters that are gameobjects... we just search for
//							// the name.
//							args.Add (GameObject.Find ((string)arguments [j]));
//							if (((GameObject)args [i]) == null) {
//								return "No game object called \"" + (string)arguments [j] + "\".";
//							}
//						} else if (pinfo [i].ParameterType == typeof(Avatar)) {
//							// Parameters that are Avatars... we just search for
//							// the name.
//							args.Add (OCARepository.GetOCA ((string)arguments [j]).GetComponent ("Avatar") as Avatar);
//							if ((Avatar)args [i] == null) {
//								return "No Avatar called \"" + (string)arguments [j] + "\".";
//							}
//						} else if (pinfo [i].ParameterType == typeof(int)) {
//							try {
//								args.Add (System.Int32.Parse ((string)arguments [j]));
//							} catch (System.FormatException ex) {
//								return "Error parsing string as int32: " + (string)arguments [j];
//							}
//						} else if (pinfo [i].ParameterType == typeof(float)) {
//							try {
//								args.Add (float.Parse ((string)arguments [j]));
//							} catch (System.FormatException ex) {
//								return "Error parsing string as float: " + (string)arguments [j];
//							}
//						} else if (pinfo [i].ParameterType == typeof(string)) {
//							args.Add ((string)arguments [j]);
//						} else {
//							return "Method " + actionName + " at slot " + i + " has argument of unsupported type \"" + pinfo [i].ParameterType +
//                            "\". Ask Joel how to implement support or ask him nicely to do it ;-).";
//						}
//					}
//				}
//				// even if this action supports callbacks, we don't pass our actionComplete callback because
//				// it's already called by the global event (which the Console class listens for).
//				AM.doAction (actionObjectID, actionName, args);
//			}

			return "Told avatar \"" + sourceName + "\" to do action \"" + actionName + "\"";
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
		/// Initializes a new instance of the <see cref="OpenCog.ActionCommand"/> class.  
		/// Generally, intitialization should occur in the Start or Awake
		/// functions, not here.
		/// </summary>
		public ActionCommand ()
		{
			_commandName = "do";
		}

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	}// class ActionCommand

}// namespace OpenCog




