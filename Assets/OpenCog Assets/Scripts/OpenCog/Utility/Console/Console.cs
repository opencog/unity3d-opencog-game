
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
using UnityEngine;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog
{

/// <summary>
/// The OpenCog Console.
/// </summary>
#region Class Attributes
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class Console : OCMonoBehaviour
	{

		//---------------------------------------------------------------------------

	#region Private Member Data

		//---------------------------------------------------------------------------
		
		/// <summary>
		/// An example variable.  Don't fall into the trap of making all variables
		/// public (I know Unity encourages you to do this).  Instead, make use of
		/// public properties whenever possible.
		/// </summary>
		static private Console m_console; // singleton 
		public GUISkin m_GUISkin; /// the skin the console will use
		public GUIStyle m_commandStyle; /// style for my commands
		private float m_currentYPosition; // The current y-position of the console.
		private Movement m_movementState = Movement.NONE; // Keeps track of the current movement state of the console.
		private bool m_isShown = false; // Keeps track of the current display status of the console.
		private float m_panelHeight; // Height of the console panel.
		private Rect m_panelRect; // Rect describing the area the console panel covers.
		private Vector2 m_scrollPosition; // The scroll position in the panel text (history)
		private ArrayList m_consoleEntries; // Previous entries to the console.
		private string m_currentInput = ""; // The text currently being entered.
		private LinkedList<string> m_inputHistory; // A history of input for up arrow support
		private LinkedListNode<string> m_inputHistoryCurrent = null; /// Current inputHistory position
		OCInputController m_inputController; // InputController to capture and return the consumer of input.
		private string m_defaultCommand; // Default command??
		private Hashtable m_commandTable = new Hashtable (); // Command history table, relation to m_consoleEntries is not yet clear.
		private ArrayList m_completionPossibilities; // Potential commands to be used in command completion.
		private bool m_isShowingCompletionOptions = false; // Whether we are currently showing completion options
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
			Input.eatKeyPressOnTextFieldFocus = false;
		
			m_panelHeight = Screen.height * 0.30f;
			m_inputController = (GameObject.FindWithTag ("OCInputController") as GameObject).GetComponent<OCInputController> ();
			// Initialise position
			m_currentYPosition = -m_panelHeight;
			m_panelRect = new Rect (0, m_currentYPosition, Screen.width, m_panelHeight);
			// If user has made console visible using public property then ensure
			// it starts appearing
			if (m_isShown)
				this.m_movementState = Movement.APPEARING;
		
			// TOFIX:
			// ActionManager.globalActionCompleteEvent += new ActionCompleteHandler (notifyActionComplete);

			// Initialise support
			m_consoleEntries = new ArrayList ();
			m_inputHistory = new LinkedList<string> ();
			if (m_defaultCommand == null || m_defaultCommand == "") {
				if (m_commandTable.Contains ("say")) {
					m_defaultCommand = "say";
				}
			}
			m_completionPossibilities = new ArrayList ();

			// add history of commands here... good way of storing test cases
			m_inputHistory.AddFirst ("/do Avatar self MoveToObject \"Soccer Ball\"");
			m_inputHistory.AddFirst ("/do Avatar \"Soccer Ball\" Kick 3000");
			m_inputHistory.AddFirst ("/do Avatar \"Soccer Ball\" PickUp");
			m_inputHistory.AddFirst ("/do Avatar \"Soccer Ball\" Drop");
			m_inputHistory.AddFirst ("/list Avatar");
			m_inputHistory.AddFirst ("/load npc");
			
			OCLogger.Fine (gameObject.name + " is started.");
		}

		/// <summary>
		/// Update is called once per frame.
		/// </summary>
		public void Update ()
		{
			m_panelHeight = Screen.height * 0.30f;

			if (Input.GetKeyDown (KeyCode.BackQuote)) {
				// If the window is already visible and isn't disappearing...
				if (m_isShown && m_movementState != Movement.DISAPPEARING) {
					CloseChatWindow ();
				} else {
					m_isShown = true;
					m_inputController.setCharacterControl (false);
                
					m_movementState = Movement.APPEARING;
				}
            
			}
		
			// Below this line is only relevent when console is active
			if (!this.IsActive ())
				return;
			
			if (Input.GetKeyDown (KeyCode.Return) && 
                m_currentInput.Length > 0) { // &&
				// GUI.GetNameOfFocusedControl() == "CommandArea")
				this.ProcessConsoleLine (m_currentInput);
				m_currentInput = ""; // blank input field
				m_inputHistoryCurrent = null; // reset current position in input history
			}
			// Implement input history using up/down arrow
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				if (m_inputHistoryCurrent == null) {
					// TODO save current output so that we can push down to restore
					// previously written text
					m_inputHistoryCurrent = m_inputHistory.First;
				} else if (m_inputHistoryCurrent.Next != null) {
					m_inputHistoryCurrent = m_inputHistoryCurrent.Next;
				}
				if (m_inputHistoryCurrent != null)
					m_currentInput = m_inputHistoryCurrent.Value;
			} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
				if (m_inputHistoryCurrent != null && m_inputHistoryCurrent.Previous != null) {
					m_inputHistoryCurrent = m_inputHistoryCurrent.Previous;
				}
				if (m_inputHistoryCurrent != null)
					m_currentInput = m_inputHistoryCurrent.Value;
			}
			if (m_isShown)
				m_inputController.setCharacterControl (false);
			
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
		/// Raises the enable event when Console is loaded.
		/// </summary>
		public void OnEnable ()
		{
			OCLogger.Fine (gameObject.name + " is enabled.");
		}

		/// <summary>
		/// Raises the disable event when Console goes out of scope.
		/// </summary>
		public void OnDisable ()
		{
			OCLogger.Fine (gameObject.name + " is disabled.");
		}

		/// <summary>
		/// Raises the destroy event when Console is about to be destroyed.
		/// </summary>
		public void OnDestroy ()
		{
			Uninitialize ();
			OCLogger.Fine (gameObject.name + " is about to be destroyed.");
		}
		
		public bool IsActive ()
		{
			if (this.m_movementState != Movement.DISAPPEARING && m_isShown)
				return true;
			else
				return false;
		}
		
		static public Console get ()
		{
			return m_console;
		}

		public void OnGUI ()
		{
			if (m_GUISkin) {
				m_commandStyle = m_GUISkin.label;
				m_commandStyle.normal.textColor = Color.blue;
				GUI.skin = m_GUISkin;
			}
		
			if (m_isShown) {
				int movementSpeed = 10;
				if (m_movementState == Movement.APPEARING) {
					m_currentYPosition = m_currentYPosition + movementSpeed;
					if (m_currentYPosition >= 0) {
						m_currentYPosition = 0;
						m_movementState = Movement.NONE;
					}
				} else if (m_movementState == Movement.DISAPPEARING) {
					m_currentYPosition = m_currentYPosition - movementSpeed;
					if (m_currentYPosition <= -m_panelHeight) {
						m_currentYPosition = -m_panelHeight;
						m_movementState = Movement.NONE;
						m_isShown = false;
					}
				}
				m_panelRect = new Rect (0, m_currentYPosition, Screen.width, m_panelHeight);
				m_panelRect = GUI.Window (1, m_panelRect, GlobalConsolePanel, "Command");
				GUI.FocusWindow (0);
				if (m_movementState != Movement.DISAPPEARING) {
				
					GUI.FocusWindow (1);
					FocusControl ();
				}
			}
		}
		
		public void AddSpeechConsoleEntry (string content, string sender, string listener)
		{
			string str = "[" + sender + " -> " + listener + "]: " + content;
			AddConsoleEntry (str, sender, ConsoleEntry.Type.RESULT);
		}
		
		public void AddCommand (ConsoleCommand cc)
		{
			m_commandTable [cc.GetName ()] = cc;
		}

		public void RemoveCommand (string cmdName)
		{
			m_commandTable.Remove (cmdName);
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
			// TOFIX:
			//Input.eatKeyPressOnTextFieldFocus = false;
			m_console = this;
		}
	
		/// <summary>
		/// Uninitializes this instance.  Cleanup refernces here.
		/// </summary>
		private void Uninitialize ()
		{
		}
		
		private void GlobalConsolePanel (int id)
		{
			// Begin a scroll view. All rects are calculated automatically - 
			// it will use up any available screen space and make sure contents flow correctly.
			// This is kept small with the last two parameters to force scrollbars to appear.
			m_scrollPosition = GUILayout.BeginScrollView (m_scrollPosition);
	
			lock (m_consoleEntries) {
				foreach (ConsoleEntry entry in m_consoleEntries) {
					GUILayout.BeginHorizontal ();
					// Here, we format things slightly differently for each
					// ConsoleEntry type.
					m_commandStyle.wordWrap = true;
					if (entry.type == ConsoleEntry.Type.ERROR) {
						// Display errors in red.
						m_commandStyle.normal.textColor = Color.red;
						GUILayout.Label (entry.msg, m_commandStyle);
						GUILayout.FlexibleSpace ();
					} else if (entry.type == ConsoleEntry.Type.SAY) {
						// Display talk in green
						m_commandStyle.normal.textColor = Color.black;
						GUILayout.Label ("> ");
						m_commandStyle.normal.textColor = Color.green;
						GUILayout.Label (entry.msg, m_commandStyle);
						GUILayout.FlexibleSpace ();
					} else if (entry.type == ConsoleEntry.Type.COMMAND) {
						// Display commands in blue
						m_commandStyle.normal.textColor = Color.black;
						GUILayout.Label ("> ");
						m_commandStyle.normal.textColor = Color.blue;
						GUILayout.Label (entry.msg, m_commandStyle);
						GUILayout.FlexibleSpace ();
					} else if (entry.type == ConsoleEntry.Type.RESULT) {
						// Display results in black
						m_commandStyle.normal.textColor = Color.black;
						GUILayout.Label (entry.msg, m_commandStyle);
						GUILayout.FlexibleSpace ();
					}
                
					GUILayout.EndHorizontal ();
                
				}
			}
			// End the scrollview we began above.
			GUILayout.EndScrollView ();
		
			// Remove backquote character from text input as this
			// enables/disables the the console
			char chr = Event.current.character;
			if (chr == '`') {
				Event.current.character = '\0';
			}
			GUI.SetNextControlName ("CommandArea");
			this.m_currentInput = GUILayout.TextField (this.m_currentInput);
		
			GUI.DragWindow ();
		}
		
		private void FocusControl ()
		{
			GUI.FocusControl ("CommandArea");
		}
		
		private void CloseChatWindow ()
		{
			// we leave the showChat as true as it will get switched off once the disappearing movement is complete
			// Mysteriously commented out....:
			// this.showChat = true;
			// Set movement state to start disappearing
			this.m_movementState = Movement.DISAPPEARING;
			// Re-enable the character controller for player movement
			
			m_inputController.setCharacterControl (true);
		}
		
		private void AddConsoleEntry (string str, string sender, ConsoleEntry.Type type)
		{
			if (str == null)
				return; // No message, ignore it

			// Create console entry
			ConsoleEntry entry = new ConsoleEntry ();
			entry.t = System.DateTime.Now;
			entry.sender = sender;
			entry.msg = str;
			if (sender == null) {
				entry.mine = true;
			} else
				entry.mine = false;
			entry.type = type;

			// Add to list
			lock (m_consoleEntries) {
				m_consoleEntries.Add (entry);
			}
		
			// Prune oldest entries
			if (m_consoleEntries.Count > 50)
				m_consoleEntries.RemoveAt (0);

			// Ensure we are at the bottom...
			// TODO Do this in a non-brittle way... i.e. find actual maximum value
			m_scrollPosition.y = 1000000;	
		}

		private bool TabComplete (string context)
		{
			if (! context.StartsWith ("/"))
				return false;
			ArrayList tokens = SplitCommandLine (context);
        
			ArrayList possibilities = new ArrayList ();
			// Find possible completions

			if (possibilities.Count > 1) {
				// Only show completions if there are more than one...
				m_isShowingCompletionOptions = true;
				m_completionPossibilities = possibilities;
			} else if (possibilities.Count == 1) {
				// just complete the token
				//inputField = contextExceptLastToken + " " + possibilities[0];
				m_isShowingCompletionOptions = false;
				m_completionPossibilities = null;
			} else {
				// no possible completions
            
			}
			return true;
		}
		
		private void ProcessConsoleLine (string text)
		{
			if (text == "")
				return;
			if (text.StartsWith ("/")) {
				AddConsoleEntry (text, null, ConsoleEntry.Type.COMMAND);
				string cmdline = text.Remove (0, 1); // remove leading slash
				ArrayList args = SplitCommandLine (cmdline);
				if (args != null) {
					string cmd = args [0] as string;
					// check if we recognise this command
					if (!m_commandTable.Contains (cmd)) {
						// we don't know about the command
						AddConsoleEntry ("error: unknown command " + (string)cmd, null, ConsoleEntry.Type.ERROR);
					} else {
						ConsoleCommand cc = m_commandTable [cmd] as ConsoleCommand;
						args.RemoveAt (0); // remove actual command name
						string result = cc.Run (args);
						AddConsoleEntry (result, null, ConsoleEntry.Type.RESULT);
					}
				}
			} else if (m_defaultCommand != null) {
				// assume the input should be sent to whatever is the default
				// command (usually just chat)
				ConsoleCommand cc = m_commandTable [m_defaultCommand] as ConsoleCommand;
				ArrayList args = SplitCommandLine (text);
				if (args != null) {
					AddConsoleEntry (text, null, ConsoleEntry.Type.SAY);
					string result = cc.Run (args);
					AddConsoleEntry (result, null, ConsoleEntry.Type.RESULT);
				}
			}
			m_inputHistory.AddFirst (text);
			m_inputHistoryCurrent = null;
		}
		
		private ArrayList SplitCommandLine (string command)
		{
			string text = command.TrimEnd ('\n');
			string[] words = text.Split (' ');

			ArrayList joined = new ArrayList ();
			// join elements surrounded by quotes
			bool speechOpen = false;
			int speechStartIndex = 0;
			for (int i=0; i < words.Length; i++) {
				// ignore double spaces
				if (words [i].Length == 0)
					continue;
				if (speechOpen) {
					if (words [i] [words [i].Length - 1] == '\"') {
						// Allow escaped speech marks at the start of words
						if (words [i] [words [i].Length - 2] == '\\')
							continue;
						// Otherwise this is a terminating speech mark
						speechOpen = false;
						string temp = "";
						for (int j=speechStartIndex; j <= i; j++) {
							temp += words [j];
							// Add space between words within string
							temp += " ";
						}
						// Remove trailing space
						temp = temp.Substring (0, temp.Length - 1);
						joined.Add (temp.Trim ('\"'));
					}
				} else if (words [i] [0] == '\"') {
					if (words [i] [words [i].Length - 1] == '\"') {
						joined.Add (words [i].Trim ('\"'));
						continue;
					}
					// Found an opening speech mark
					speechStartIndex = i;
					speechOpen = true;
				} else {
					// just add tokens otherwise
					joined.Add (words [i].Trim ('\"'));
				}
			}
			if (speechOpen) {
				// unterminated string...
				AddConsoleEntry ("error: no matching \" character.", null, ConsoleEntry.Type.ERROR);
				return null;
			}
			return joined;
		}
			
		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Other Members

		//---------------------------------------------------------------------------		

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenCog.Console"/> class.  
		/// Generally, intitialization should occur in the Start or Awake
		/// functions, not here.
		/// </summary>
		public Console ()
		{
		}
		
		enum Movement // The different movement states the console can have.
		{ 
			APPEARING, 
			DISAPPEARING, 
			NONE 
		};
		
		private class ConsoleEntry
		{
			public enum Type
			{
				COMMAND,
				RESULT,
				SAY,
				ERROR
			};
			public Type type;
			public System.DateTime t; // the time that the command was executed
			public string commandName; // The command
			public string receiver; // should be replaced by Avatar class
			public string sender;   // should be replaced by Avatar class
			public string msg = "";
			public bool mine = true; // Judge if the message is mine.
		}
		
		public abstract class ConsoleCommand : OCMonoBehaviour
		{
			public void Start ()
			{
				Console.get ().AddCommand (this);
			}
			/// <summary>
			/// Run the command, whatever it may be and return the result as a string.
			/// </summary>
			abstract public string Run (ArrayList arguments);
			
			/// <summary>
			/// return the command signature.
			/// An ordered list of KeyValuePair<Type,int>... the int is the number of
			/// that type (0 == unlimited).
			/// </summary>
			abstract public ArrayList GetSignature ();

			abstract public string GetName ();
			
			protected string m_commandName = "empty";
		}
		
		// TOFIX: Replace below with real OCInputController;
		/// <summary>
		/// OC input controller. Stub for yet to be designed and implented OCInputController.
		/// </summary>
		private class OCInputController : OCMonoBehaviour
		{
			public void setCharacterControl (bool bFalseOrTrue)
			{
				
			}
		}
		
		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	}// class Console

}// namespace OpenCog




