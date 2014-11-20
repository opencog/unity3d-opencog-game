
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
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using UnityEngine;
using OpenCog.Entities;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Utility.Console
{

/// <summary>
/// The OpenCog Console.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class Console : OCSingletonMonoBehaviour<Console>
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
		
	/// <summary>
	/// An example variable.  Don't fall into the trap of making all variables
	/// public (I know Unity encourages you to do this).  Instead, make use of
	/// public properties whenever possible.
	/// </summary>
	static private Console _console; // singleton 
	public GUISkin _GUISkin; /// the skin the console will use
	public GUIStyle _commandStyle; /// style for my commands
	private float _currentYPosition; // The current y-position of the console.
	private Movement _movementState = Movement.NONE; // Keeps track of the current movement state of the console.
	private bool _isShown = false; // Keeps track of the current display status of the console.
	private float _panelHeight; // Height of the console panel.
	private Rect _panelRect; // Rect describing the area the console panel covers.
	private Vector2 _scrollPosition; // The scroll position in the panel text (history)
	private ArrayList _consoleEntries; // Previous entries to the console.
	private string _currentInput = ""; // The text currently being entered.
	private LinkedList<string> _inputHistory; // A history of input for up arrow support
	private LinkedListNode<string> _inputHistoryCurrent = null; /// Current inputHistory position

	private string _defaultCommand; // Default command??
	private Hashtable _commandTable = new Hashtable(); // Command history table, relation to_consoleEntries is not yet clear.
	private ArrayList _completionPossibilities; // Potential commands to be used in command completion.
	private bool _isShowingCompletionOptions = false; // Whether we are currently showing completion options
	private bool _showChat = true;
	private string _lastLine = "";

	// DEPRECATED: Search this file for the object below and re-enable all calls to it.
	//OpenCog.Entities.OCInputController _inputController; // InputController to capture and return the consumer of input.
	GameObject _player = null;


	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------

	public string DefaultCommand
		{
			get { return _defaultCommand; }
			set { _defaultCommand = value; }
		}

	public bool ShowChat
		{
			get { return _showChat; }
			set { _showChat = value; }
		}
		
	public static Console Instance
	{
		get 
		{ 
			return OpenCog.Utility.Console.Console.GetInstance<Console>();
		}
		
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start()
	{
	
		
		_panelHeight = Screen.height * 0.30f;


		// DEPRECATED: Re-enable line below
		//_inputController = (GameObject.FindWithTag("OCInputController") as GameObject).GetComponent<OCInputController>();

			// DEPRECATED: Disable line below
		//_inputController = (GameObject.FindWithTag("CharacterInputController") as GameObject).GetComponent<CharacterInputController>();

		_player = GameObject.FindGameObjectWithTag("Player");
		//_inputController = OpenCog.Entities.OCInputController.Instance;


		// Initialise position
		_currentYPosition = -_panelHeight;
		_panelRect = new Rect(0, _currentYPosition, Screen.width, _panelHeight);
		// If user has made console visible using public property then ensure
		// it starts appearing
		if(_isShown)
		{
			this._movementState = Movement.APPEARING;
		}

		// DEPRECATED: Removed this, may need checking.
		//OCActionController.globalActionCompleteEvent += new ActionCompleteHandler (notifyActionComplete);

		// Initialise support
		_consoleEntries = new ArrayList();
		_inputHistory = new LinkedList<string>();
		if(_defaultCommand == null || _defaultCommand == "")
		{
			if(_commandTable.Contains("say"))
			{
				_defaultCommand = "say";
			}
		}
		_completionPossibilities = new ArrayList();

		// add history of commands here... good way of storing test cases
		_inputHistory.AddFirst("/do Avatar self MoveToObject \"Soccer Ball\"");
		_inputHistory.AddFirst("/do Avatar \"Soccer Ball\" Kick 3000");
		_inputHistory.AddFirst("/do Avatar \"Soccer Ball\" PickUp");
		_inputHistory.AddFirst("/do Avatar \"Soccer Ball\" Drop");
		_inputHistory.AddFirst("/list Avatar");
		_inputHistory.AddFirst("/load AGI_Robot");
			
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is started.");
	}

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	public void Update()
	{

	}
		
	/// <summary>
	/// Reset this instance to its default values.
	/// </summary>
	public void Reset()
	{
		Uninitialize();
		Initialize();
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is reset.");	
	}

	/// <summary>
	/// Raises the enable event when Console is loaded.
	/// </summary>
	public void OnEnable()
	{
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is enabled.");
	}

	/// <summary>
	/// Raises the disable event when Console goes out of scope.
	/// </summary>
	public void OnDisable()
	{
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is disabled.");
	}

	/// <summary>
	/// Raises the destroy event when Console is about to be destroyed.
	/// </summary>
	public void OnDestroy()
	{
		Uninitialize();
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is about to be destroyed.");
	}
		
	public bool IsActive()
	{
		if(this._movementState != Movement.DISAPPEARING && _isShown)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
		
	public static Console get()
	{
		return _console;
	}

	public void OnGUI()
	{
			HandleAppearance();
			HandleInput();
	}

	public void HandleInput()
	{
		if(!Event.current.isKey) return;
		if(Event.current.type != EventType.keyUp) return;
		
		//First we need to make sure we can display and hide the interface
		if(Event.current.keyCode == KeyCode.BackQuote)
		{
			// If the window is already visible and isn't disappearing...
			if(_isShown && _movementState != Movement.DISAPPEARING)
			{
				CloseChatWindow();
				_player.GetComponent<OCInputController>().enabled = true;
				_player.GetComponent<OCCharacterMotor>().enabled = true;
				_player.GetComponent<MouseLook>().enabled = true;	
				_isShown = false;
			}
			else
			{
				_isShown = true;
				_player.GetComponent<OCInputController>().enabled = false;
				_player.GetComponent<OCCharacterMotor>().enabled = false;
				_player.GetComponent<MouseLook>().enabled = false;
				
				_movementState = Movement.APPEARING;
			}
			Event.current.Use();
			return;
		}
		
		//then we don't want to grab input unless the console is open
		if(!IsActive()) return;

		switch(Event.current.keyCode)
		{
			
			//consume the console command!
			case KeyCode.Return: case KeyCode.KeypadEnter:

				//use up the event
				Event.current.Use ();

				//check to make sure we have any input to send
				if(_currentInput.Length <= 0) break;

				//do le stuff with le input
				this.ProcessConsoleLine(_currentInput);
				_currentInput = ""; // blank input field
				_inputHistoryCurrent = null; // reset current position in input history

				break;
			// Implement input history using up/down arrow
			case KeyCode.UpArrow:
				if(_inputHistoryCurrent == null)
				{
					// TODO [LEGACY]: save current output so that we can push down to restore
					// previously written text
					_inputHistoryCurrent = _inputHistory.First;
				}
				else if(_inputHistoryCurrent.Next != null)
				{
					_inputHistoryCurrent = _inputHistoryCurrent.Next;
				}
				if(_inputHistoryCurrent != null)
				{
					_currentInput = _inputHistoryCurrent.Value;
				}
				Event.current.Use ();
				break;
			case KeyCode.DownArrow:
				if(_inputHistoryCurrent != null && _inputHistoryCurrent.Previous != null)
				{
					_inputHistoryCurrent = _inputHistoryCurrent.Previous;
				}
				if(_inputHistoryCurrent != null)
				{
					_currentInput = _inputHistoryCurrent.Value;
				}
				Event.current.Use ();
				break;
		};
			

			//DEPRECATED: Fix this
			if(_isShown)
			{
				//_inputController.SetCharacterControl(false);
			}
	}

	public void HandleAppearance()
	{
		_panelHeight = Screen.height * 0.30f;

		if(_GUISkin)
		{
			_commandStyle = _GUISkin.label;
			_commandStyle.normal.textColor = Color.blue;
			GUI.skin = _GUISkin;

			Debug.Log("_GUISkin = true");
		}
		
		if(_isShown)
		{
			int movementSpeed = 10;
			if(_movementState == Movement.APPEARING)
			{
				_currentYPosition = _currentYPosition + movementSpeed;
				if(_currentYPosition >= 0)
				{
					_currentYPosition = 0;
					_movementState = Movement.NONE;
				}
			}
			else
			if(_movementState == Movement.DISAPPEARING)
			{
				_currentYPosition = _currentYPosition - movementSpeed;
				if(_currentYPosition <= -_panelHeight)
				{
					_currentYPosition = -_panelHeight;
					_movementState = Movement.NONE;
					_isShown = false;
				}
			}
			_panelRect = new Rect(0, _currentYPosition, Screen.width, _panelHeight);
			_panelRect = GUI.Window(1, _panelRect, GlobalConsolePanel, "Command");
			GUI.FocusWindow(0);
			if(_movementState != Movement.DISAPPEARING)
			{
				
				GUI.FocusWindow(1);
				FocusControl();
			}

			//Debug.Log("_isShown = true");
		}
					
	}
		
	public void AddSpeechConsoleEntry(string content, string sender, string listener)
	{
		string str = "[" + sender + " -> " + listener + "]: " + content;
		AddConsoleEntry(str, sender, ConsoleEntry.Type.RESULT);
	}
		
	public void AddCommand(ConsoleCommand cc)
	{
		_commandTable[cc.GetName()] = cc;
	}

	public void RemoveCommand(string cmdName)
	{
		_commandTable.Remove(cmdName);
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	/// <summary>
	/// Initializes this instance.  Set default values here.
	/// </summary>
	override protected void Initialize()
	{

		_console = this;
	}
	
	/// <summary>
	/// Uninitializes this instance.  Cleanup refernces here.
	/// </summary>
	private void Uninitialize()
	{
	}
		
	private void GlobalConsolePanel(int id)
	{
	

		// Begin a scroll view. All rects are calculated automatically - 
		// it will use up any available screen space and make sure contents flow correctly.
		// This is kept small with the last two parameters to force scrollbars to appear.
		_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
		
		if(_consoleEntries != null)
		{
			lock(_consoleEntries)
			{
				foreach(ConsoleEntry entry in _consoleEntries)
				{
					GUILayout.BeginHorizontal();
					// Here, we format things slightly differently for each
					// ConsoleEntry type.
					_commandStyle.wordWrap = true;
					if(entry.type == ConsoleEntry.Type.ERROR)
					{
						// Display errors in red.
						_commandStyle.normal.textColor = Color.red;
						GUILayout.Label(entry.msg, _commandStyle);
						GUILayout.FlexibleSpace();
					}
					else if(entry.type == ConsoleEntry.Type.SAY)
					{
						// Display talk in green
						_commandStyle.normal.textColor = Color.black;
						GUILayout.Label("> ");
						_commandStyle.normal.textColor = Color.green;
						GUILayout.Label(entry.msg, _commandStyle);
						GUILayout.FlexibleSpace();
					}
					else if(entry.type == ConsoleEntry.Type.COMMAND)
					{
						// Display commands in blue
						_commandStyle.normal.textColor = Color.black;
						GUILayout.Label("> ");
						_commandStyle.normal.textColor = Color.blue;
						GUILayout.Label(entry.msg, _commandStyle);
						GUILayout.FlexibleSpace();
					}
					else if(entry.type == ConsoleEntry.Type.RESULT)
					{
						// Display results in black
						_commandStyle.normal.textColor = Color.black;
						GUILayout.Label(entry.msg, _commandStyle);
						GUILayout.FlexibleSpace();
					}
	                
					GUILayout.EndHorizontal();
	                
				}
			}
		}
	
		// End the scrollview we began above.
		GUILayout.EndScrollView();
		
		// Remove backquote character from text input as this
		// enables/disables the the console
		char chr = Event.current.character;
		if(chr == '`')
		{
			Event.current.character = '\0';
		}
		GUI.SetNextControlName("CommandArea");
		this._currentInput = GUILayout.TextField(this._currentInput);
		
		GUI.DragWindow();

	}
		
	private void FocusControl()
	{
		GUI.FocusControl("CommandArea");
	}
		
	private void CloseChatWindow()
	{
		// we leave the showChat as true as it will get switched off once the disappearing movement is complete
		// Mysteriously commented out....:
		// this.showChat = true;
		// Set movement state to start disappearing
		this._movementState = Movement.DISAPPEARING;
		// Re-enable the character controller for player movement
			
		//_inputController.SetCharacterControl(true);
	}
		
	public void AddConsoleEntry(string str, string sender, ConsoleEntry.Type type)
	{
		if(str == null)
		{
			return;
		} // No message, ignore it
			
		// Same message...ignore it.
		if (str == _lastLine)
		{
			return;	
		}
		
	

		// Create console entry
		ConsoleEntry entry = new ConsoleEntry();
		entry.t = System.DateTime.Now;
		entry.sender = sender;
		entry.msg = str;
		if(sender == null)
		{
			entry.mine = true;
		}
		else
		{
			entry.mine = false;
		}
		entry.type = type;

		// Add to list
		lock(_consoleEntries)
		{
			_consoleEntries.Add(entry);
		}
		
		// Prune oldest entries
		if(_consoleEntries.Count > 50)
		{
			_consoleEntries.RemoveAt(0);
		}

		// Ensure we are at the bottom...
		// TODO [TASK]: Do this in a non-brittle way... i.e. find actual maximum value
		_scrollPosition.y = 1000000;
			
		_lastLine = str;
	}

	private bool TabComplete(string context)
	{
		if(! context.StartsWith("/"))
		{
			return false;
		}
		//ArrayList tokens = SplitCommandLine(context);
        
		ArrayList possibilities = new ArrayList();
		// Find possible completions

		if(possibilities.Count > 1)
		{
			// Only show completions if there are more than one...
			_isShowingCompletionOptions = true;
			_completionPossibilities = possibilities;
		}
		else
		if(possibilities.Count == 1)
		{
			// just complete the token
			//inputField = contextExceptLastToken + " " + possibilities[0];
			_isShowingCompletionOptions = false;
			_completionPossibilities = null;
		}
		else
		{
			// no possible completions
            
		}
		return true;
	}
		
	private void ProcessConsoleLine(string text)
	{
		if(text == "")
		{
			return;
		}
		if(text.StartsWith("/"))
		{
			AddConsoleEntry(text, null, ConsoleEntry.Type.COMMAND);
			string cmdline = text.Remove(0, 1); // remove leading slash
			ArrayList args = SplitCommandLine(cmdline);
			if(args != null)
			{
				string cmd = args[0] as string;
				// check if we recognise this command
				if(!_commandTable.Contains(cmd))
				{
					// we don't know about the command
					AddConsoleEntry("error: unknown command " + (string)cmd, null, ConsoleEntry.Type.ERROR);
				}
				else
				{
					ConsoleCommand cc = _commandTable[cmd] as ConsoleCommand;
					args.RemoveAt(0); // remove actual command name
					string result = cc.Run(args);
					AddConsoleEntry(result, null, ConsoleEntry.Type.RESULT);
						
//					AddConsoleEntry("Printing command line arguments...", "CommandLineArgumentor", ConsoleEntry.Type.COMMAND);
						
					try {
						string[] commandLineArgs = System.Environment.GetCommandLineArgs();
						
						for (int iString = 0; iString < commandLineArgs.Length; iString++)
						{
							UnityEngine.Debug.Log ("Command line arg[" + iString + "] = " + commandLineArgs[iString]);
								
//							AddConsoleEntry("Command line arg[" + iString + "] = " + commandLineArgs[iString], "CommandLineArgumentor", ConsoleEntry.Type.SAY);
						}
					} catch (System.Exception ex) {
						UnityEngine.Debug.Log ("OCWorldGenerator::Awake: An error occurred while parsing commandline args: " + ex.ToString());
					}
				}
			}
		}
		else
		if(_defaultCommand != null)
		{
			// assume the input should be sent to whatever is the default
			// command (usually just chat)
			ConsoleCommand cc = _commandTable[_defaultCommand] as ConsoleCommand;
			ArrayList args = SplitCommandLine(text);
			if(args != null)
			{
				AddConsoleEntry(text, null, ConsoleEntry.Type.SAY);
				string result = cc.Run(args);
				AddConsoleEntry(result, null, ConsoleEntry.Type.RESULT);
			}
		}
		_inputHistory.AddFirst(text);
		_inputHistoryCurrent = null;
	}
		
	private ArrayList SplitCommandLine(string command)
	{
		string text = command.TrimEnd('\n');
		string[] words = text.Split(' ');

		ArrayList joined = new ArrayList();
		// join elements surrounded by quotes
		bool speechOpen = false;
		int speechStartIndex = 0;
		for(int i=0; i < words.Length; i++)
		{
			// ignore double spaces
			if(words[i].Length == 0)
			{
				continue;
			}
			if(speechOpen)
			{
				if(words[i][words[i].Length - 1] == '\"')
				{
					// Allow escaped speech marks at the start of words
					if(words[i][words[i].Length - 2] == '\\')
					{
						continue;
					}
					// Otherwise this is a terminating speech mark
					speechOpen = false;
					string temp = "";
					for(int j=speechStartIndex; j <= i; j++)
					{
						temp += words[j];
						// Add space between words within string
						temp += " ";
					}
					// Remove trailing space
					temp = temp.Substring(0, temp.Length - 1);
					joined.Add(temp.Trim('\"'));
				}
			}
			else
			if(words[i][0] == '\"')
			{
				if(words[i][words[i].Length - 1] == '\"')
				{
					joined.Add(words[i].Trim('\"'));
					continue;
				}
				// Found an opening speech mark
				speechStartIndex = i;
				speechOpen = true;
			}
			else
			{
				// just add tokens otherwise
				joined.Add(words[i].Trim('\"'));
			}
		}
		if(speechOpen)
		{
			// unterminated string...
			AddConsoleEntry("error: no matching \" character.", null, ConsoleEntry.Type.ERROR);
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
	public Console()
	{
	}
		
	enum Movement // The different movement states the console can have.
	{ 
		APPEARING, 
		DISAPPEARING, 
		NONE 
	};
		
	public class ConsoleEntry
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
		public void Start()
		{
			Console.get().AddCommand(this);
		}
		/// <summary>
		/// Run the command, whatever it may be and return the result as a string.
		/// </summary>
		abstract public string Run(ArrayList arguments);
			
		/// <summary>
		/// return the command signature.
		/// An ordered list of KeyValuePair<Type,int>... the int is the number of
		/// that type (0 == unlimited).
		/// </summary>
		abstract public ArrayList GetSignature();

		abstract public string GetName();
			
		protected string _commandName = "empty";
	}
		
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class Console

}// namespace OpenCog




