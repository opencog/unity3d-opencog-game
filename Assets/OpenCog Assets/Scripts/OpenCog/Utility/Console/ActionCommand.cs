
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
		private int m_ExampleVar;

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
		/// Raises the enable event when ActionCommand is loaded.
		/// </summary>
		public void OnEnable ()
		{
			OCLogger.Fine (gameObject.name + " is enabled.");
		}

		/// <summary>
		/// Raises the disable event when ActionCommand goes out of scope.
		/// </summary>
		public void OnDisable ()
		{
			OCLogger.Fine (gameObject.name + " is disabled.");
		}

		/// <summary>
		/// Raises the destroy event when ActionCommand is about to be destroyed.
		/// </summary>
		public void OnDestroy ()
		{
			Uninitialize ();
			OCLogger.Fine (gameObject.name + " is about to be destroyed.");
		}
		
		public override string Run (ArrayList arguments)
		{
			//if (arguments.Count != 2) return "Wrong number of arguments";
			string avatarName = (string)arguments [0];
			string objectName = (string)arguments [1];
			string actionName = (string)arguments [2];

			// Get the appropriate avatar and gameobject
			UnityEngine.GameObject avatarObject = OCARepository.GetOCA (avatarName);
			Avatar avatarScript = avatarObject.GetComponent ("Avatar") as Avatar;
			if (avatarScript == null)
				return "No Avatar script on avatar \"" + avatarName + "\".";
			if (avatarObject.tag == "Player")
				return "Avatar \"" + avatarName + "\" is a player!";

			// Get the object
			// if objectName is "self" then assume the script is on the avatar
			int actionObjectID;
			if (objectName != "self") {
				UnityEngine.GameObject OCObjects = UnityEngine.GameObject.Find ("Objects") as UnityEngine.GameObject;
				UnityEngine.GameObject theActionObject = OCObjects.transform.FindChild (objectName).gameObject;
				if (theActionObject == null)
					return "No object called " + objectName;
				actionObjectID = theActionObject.GetInstanceID ();
			} else {
				actionObjectID = avatarObject.GetInstanceID ();
			}

			// Get the action summary from the Action Manager
			ActionManager AM = avatarScript.GetComponent<ActionManager> () as ActionManager;
			ActionSummary action = AM.getActionSummary (actionObjectID, actionName);
			if (action == null)
				return "No action called \"" + actionName + "\".";

			System.Reflection.ParameterInfo[] pinfo = action.pinfo; //getFreeArguments();
			if (pinfo.Length > 0) {
				ArrayList args = new ArrayList ();
				int i = 0;
				int jmod = 3;
				if (action.componentType == typeof(Avatar)) {
					// Check we don't have too many arguments
					if (pinfo.Length < arguments.Count - 3)
						return "Expected " + pinfo.Length + " arguments, got " + (arguments.Count - 3) + ".";
				} else {
					// Check we don't have too many arguments, we don't need to
					// provide the avatar
					if (pinfo.Length - 1 < arguments.Count - 3)
						return "Expected " + (pinfo.Length - 1) + " arguments, got " + (arguments.Count - 3) + ".";
					i = 1;
					jmod = 2;
				}
				// ignore last parameter if action uses a callback
				int lengthModififer = 0;
				if (action.usesCallback)
					lengthModififer = 1; 
				for (; i < pinfo.Length-lengthModififer; i++) {
					// do type checking and conversion from strings to the expected type
					// ignore 3 console arguments: the action name, avatar name,
					//    and the object with the action (from console arguments)
					int j = i + jmod;
					if (j >= arguments.Count) {
						// Not enough arguments, so must be a default argument
						if (!pinfo [i].IsOptional)
							return "Missing parameter " + pinfo [i].Name + " is not optional.";
						args.Add (pinfo [i].DefaultValue);
					} else {
						arguments [j] = ((string)arguments [j]).Replace ("\"", "");
						// Depending on the expected type we convert it differently
						if (pinfo [i].ParameterType == typeof(GameObject)) {
							// Parameters that are gameobjects... we just search for
							// the name.
							args.Add (GameObject.Find ((string)arguments [j]));
							if (((GameObject)args [i]) == null) {
								return "No game object called \"" + (string)arguments [j] + "\".";
							}
						} else if (pinfo [i].ParameterType == typeof(Avatar)) {
							// Parameters that are Avatars... we just search for
							// the name.
							args.Add (OCARepository.GetOCA ((string)arguments [j]).GetComponent ("Avatar") as Avatar);
							if ((Avatar)args [i] == null) {
								return "No Avatar called \"" + (string)arguments [j] + "\".";
							}
						} else if (pinfo [i].ParameterType == typeof(int)) {
							try {
								args.Add (System.Int32.Parse ((string)arguments [j]));
							} catch (System.FormatException ex) {
								return "Error parsing string as int32: " + (string)arguments [j];
							}
						} else if (pinfo [i].ParameterType == typeof(float)) {
							try {
								args.Add (float.Parse ((string)arguments [j]));
							} catch (System.FormatException ex) {
								return "Error parsing string as float: " + (string)arguments [j];
							}
						} else if (pinfo [i].ParameterType == typeof(string)) {
							args.Add ((string)arguments [j]);
						} else {
							return "Method " + actionName + " at slot " + i + " has argument of unsupported type \"" + pinfo [i].ParameterType +
                            "\". Ask Joel how to implement support or ask him nicely to do it ;-).";
						}
					}
				}
				// even if this action supports callbacks, we don't pass our actionComplete callback because
				// it's already called by the global event (which the Console class listens for).
				AM.doAction (actionObjectID, actionName, args);
			}

			return "Told avatar \"" + avatarName + "\" to do action \"" + actionName + "\"";
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
			m_commandName = "do";
		}

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	}// class ActionCommand

}// namespace OpenCog




