
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
using OpenCog.Master;


#region Usings, Namespaces, and Pragmas

using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Linq;
//using System.Net.NetworkInformation;
using OpenCog.Attributes;
using OpenCog.Extensions;
using IAsyncResult = System.IAsyncResult;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using OpenCog.Utility;
using OpenCog.Utilities.Logging;


//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Network
{

	/// <summary>
	/// The OpenCog Network Element.  
	/// </summary>
	#region Class Attributes

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	[OCExposePropertyFields]
	[Serializable]
	#pragma warning disable 0414
	#endregion
	public class OCNetworkElement : OCSingletonMonoBehaviour<OCNetworkElement>
	{

		//---------------------------------------------------------------------------
	 
		#region Private Member Data

		//---------------------------------------------------------------------------
			
		private bool _isNEInitialized = false; // Flag to check if the OAC to this avatar is alive.
		
		// Settings of this network element instance.
		protected string _ID;

		private IPAddress _IP;

		private int _port;
			
		// Settings of router.
		private string _routerID = null;
		private IPAddress _routerIP;

		private int _routerPort;
			
		/// <summary>
		/// Server listener to make this network element acting as a server.
		/// </summary>
		private OCServerListener _listener;
			
		// Unread messages
		private System.Object _unreadMessagesLock = new object();

		private int _unreadMessagesCount;
			
		/// <summary>
		/// Queue used to store received messages from router. Uses a concurrent
		/// implementation of the queue interface.
		/// </summary>
		private Queue<OCMessage> _messageQueue = new Queue<OCMessage>();
			
		/// <summary>
		/// A hashset to record the unavailable end points.
		/// </summary>
		private HashSet<string> _unavailableElements = new HashSet<string>();
			
		/// <summary>
		/// Client socket to talk to the router.
		/// </summary>
		private Socket _clientSocket;
			
		/// <summary>
		/// Flag to check if the connection between this network element and router
		/// has been established.
		/// </summary>
		protected bool _isEstablished = false;
		protected bool _isLoggedIn = false;
		protected bool _isListening = false;
		private bool _isHandlingMessages = false;
		protected bool _firstSendOfPhysiologicalFactors = true;
		
		protected string _verificationGuid ;
			
		protected ConnectionState _connectionState = ConnectionState.Disconnected;
			
		private OCMessageHandler _messageHandler;
			
		

		//---------------------------------------------------------------------------

		#endregion

		//---------------------------------------------------------------------------

		#region Accessors and Mutators

		//---------------------------------------------------------------------------
			
		public string VerificationGuid 
		{
			get 
			{ 
				if (_verificationGuid == null)
					_verificationGuid = System.Guid.NewGuid().ToString();
		
				return _verificationGuid;
			}
		}
			
		public bool IsEstablished
		{
			get { return _isEstablished;}
			set { _isEstablished = value;}
		}

		public IPAddress IP
		{
			get { return this._IP;}
			set { _IP = value;}
		}

		public int Port
		{
			get { return this._port;}
			set { _port = value;}
		}		
			
		/// <summary>
		/// Check if there are unread messages not yet pull from router.
		/// </summary>
		public bool HaveUnreadMessages
		{
			get { return _unreadMessagesCount > 0; }
		}		
			
		public static OCNetworkElement Instance
		{
			get {
				return GetInstance<OCNetworkElement>();
			}
		}
			
		public OCServerListener Listener
		{
			get { return _listener; } 
		}
			
		public bool IsHandlingMessages
		{
			get { return _isHandlingMessages; }
		 	set { _isHandlingMessages = value; }
				
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
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +this.name + " is started.");
		}

		/// <summary>
		/// Update is called once per frame.
		/// </summary>
		public void Update()
		{
			//UnityEngine.Debug.Log("Updating...");

			if (_isEstablished)
			{
				if (!_isListening)
					StartListening();
				else if (_listener.IsReady && !_isHandlingMessages) {
					StartHandling();
					Pulse();
				}
				else {
					Pulse();
				}		
			}
			
				
			//System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +this.name + " is updated.");
		}
			
		/// <summary>
		/// Reset this instance to its default values.
		/// </summary>
		public void Reset()
		{
			Uninitialize();
			Initialize();
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +this.name + " is reset.");
		}

		/// <summary>
		/// Raises the enable event when OCNetworkElement is loaded.
		/// </summary>
		public void OnEnable()
		{
			//System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +this.name + " is enabled.");
		}

		/// <summary>
		/// Raises the disable event when OCNetworkElement goes out of scope.
		/// </summary>
		public void OnDisable()
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +this.name + " is disabled.");
		}

		/// <summary>
		/// Raises the destroy event when OCNetworkElement is about to be destroyed.
		/// </summary>
		public void OnDestroy()
		{
			Uninitialize();
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +this.name + " is about to be destroyed.");
		}
			
		/// <summary>
		/// Notify a number of new messages from router.
		/// </summary>
		/// <param name="newMessagesNum">number of new arriving messages</param>
		public void NotifyNewMessages(int newMessagesNum)
		{
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"Notified about new messages in Router.");
			lock(_unreadMessagesLock)
			{
				_unreadMessagesCount += newMessagesNum;
				System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"Unread messages [" + _unreadMessagesCount + "]");
			}
		}
		
		/// <summary>
		/// Pull unread messages from router. 
		/// </summary>
		/// <param name="messages">A list of unread messages</param>
		public void PullMessage(List<OCMessage> messages)
		{
	//		UnityEngine.Debug.Log ("OCNetworkElement::PullMessage(List<OCMessage> messages)");
			lock(_messageQueue)
			{
				foreach(OCMessage msg in messages)
				{
					_messageQueue.Enqueue(msg);
				}
			}
			lock(_unreadMessagesLock)
			{
				_unreadMessagesCount -= messages.Count;
			}
		}
		
		/// <summary>
		/// Pull a message from router.
		/// </summary>
		/// <param name="message">An unread message</param>
		public void PullMessage(OCMessage message)
		{
	//		UnityEngine.Debug.Log ("OCNetworkElement::PullMessage(OCMessage)");
			lock(_messageQueue)
			{
	//			UnityEngine.Debug.Log ("Enqueueing a message (I hate this code!!)");
				_messageQueue.Enqueue(message);	
			}
			
			lock(_unreadMessagesLock)
			{
	//			UnityEngine.Debug.Log ("Taking unreadMessagesCount from " + _unreadMessagesCount + " to " + (_unreadMessagesCount - 1).ToString() + ".");
				_unreadMessagesCount--;
			}
		}
		
		/// <summary>
		/// Abstract method to be implemented by subclasses.
		/// </summary>
		/// <param name="message">OCMessage to be processed</param>
		/// <returns>True if the message is an "exit" command.</returns>
		public virtual bool ProcessNextMessage(OCMessage message)
		{
			UnityEngine.Debug.Log ("OCNetworkElement::ProcessNextMessage (I DON'T WANT TO BE HERE!!)");
				
			return false;
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
				_isNEInitialized = true;
		}
			
		private void StartHandling()
		{
			System.Console.WriteLine(OCLogSymbol.CONNECTION + "OCNetworkElement.StartHandling() has run");
				
			if (!_isHandlingMessages)
			{
				_isHandlingMessages = true;	
					
	//			if (_messageHandler == null)
	//				_messageHandler = OCMessageHandler.Instance;		
	//				
	//			StartCoroutine(_messageHandler.UpdateMessages(_listener.WorkSocket));
				
				// In the old code, this function is NEVER fully executed, since haveUnreadMessages is ALWAYS false anyway.
				//StartCoroutine(RequestMessage (1));
			}
				
			
		}
			
		private void StartListening()
		{
			if (_isLoggedIn)
			{
				UnityEngine.Debug.Log (OCLogSymbol.CONNECTION + "Router Logged In. OcServerListener.Listen() coroutine started by OcNetworkElement.StartListening()");
		
				_isListening = true;
					
				StartCoroutine(_listener.Listen());		
			}
		}

		protected void InitializeNetworkElement(string id)
		{
			//UnityEngine.Debug.Log ("In InitializeNetworkElement, my GUID is " + VerificationGuid);
			_ID = id;
			_port = OCPortManager.AllocatePort();

	// Temporarily reverted to:
			_IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];

	// See: https://github.com/opencog/unity3d-opencog-game/issues/15#issuecomment-36709935

	//		// http://stackoverflow.com/questions/1069103/how-to-get-my-own-ip-address-in-c
	//		IEnumerable<NetworkInterface> networkInterfaces = 
	//			from entry in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
	//			where entry.OperationalStatus.Equals (OperationalStatus.Up)
	//			select entry;
	//			
	//		foreach (NetworkInterface networkInterface in networkInterfaces)
	//		{
	//			IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
	//			UnicastIPAddressInformationCollection unicastAddresses = ipProperties.UnicastAddresses;
	//				
	//			foreach (UnicastIPAddressInformation unicastAddress in unicastAddresses)
	//			{
	//				if (unicastAddress.DuplicateAddressDetectionState == DuplicateAddressDetectionState.Preferred &&
	//					unicastAddress.AddressPreferredLifetime != System.UInt32.MaxValue)
	//						_IP = unicastAddress.Address;
	//			}
	//		}

			UnityEngine.Debug.Log (OCLogSymbol.CONNECTION + "IP:Port Address detected: " + _IP + ":" + _port);

			// routerIpString appears to only be set in the obsoleted OldNetworkElement class in the old project...
			//_routerIP = IPAddress.Parse(this.routerIpString);
			// so we'll try the new way for now
			string strConfigIP = OCConfig.Instance.get("ROUTER_IP", "192.168.1.48");

			_routerIP = IPAddress.Parse(strConfigIP);
			_routerPort = OCConfig.Instance.getInt("ROUTER_PORT", 16312);
				
	//		if (_routerIP.ToString() == string.Empty)
	//		{
	//			_routerIP = IPAddress.Parse ("158.132.219.182");
	//			_routerPort = 16312;
	//			UnityEngine.Debug.Log ("Using hardcoded IP: " + _routerIP.ToString() + ":" + _routerPort);
	//		}
		
			OCServerListener.Instance.InitFromNetwork(this);
			_listener = OCServerListener.Instance;

			//StartCoroutine(Connect());
			
			//UnityEngine.Debug.Log ("StartCoroutine(_listener.Listen())");
				
			if (bool.Parse(OCConfig.Instance.get("GENERATE_TICK_MESSAGE")))
					UnityEngine.Debug.Log (OCLogSymbol.CONNECTION + "Generation of tick messages is enabled.");
					
			//StartCoroutine(_listener.Listen());
			//StartCoroutine(RequestMessage(1));

			_isNEInitialized = true;
		}
		
		/// <summary>
		/// Uninitializes this instance.  Cleanup refernces here.
		/// </summary>
		protected void Uninitialize()
		{
			StopCoroutine("_listener.Listen");
			StopCoroutine("RequestMessage");
			//_listener.Stop();
			Disconnect();
			OCPortManager.ReleasePort(_port);

			_isNEInitialized = false;
		}
			
		protected System.Collections.IEnumerator Connect()
		{
			if (_connectionState == ConnectionState.Disconnected)
			{
				_connectionState = ConnectionState.Connecting;
					
				//UnityEngine.Debug.Log ("_connectionState == Disconnected, connecting...");	
				
				UnityEngine.Debug.Log (OCLogSymbol.CONNECTION + "Trying to Connect, current time is " + System.DateTime.Now.ToString ("HH:mm:ss.fff"));
				

				//NOTE: This InterNetwork flag is probably why our game and embodiment machines must be
				//on the same network in order to detect one another.
				//TODO [Task]: if we checked out our IP address ahead of time we could see if it was on 
				//the same network as us and throw a message about it. 
				Socket asyncSocket = new 
					Socket
					(AddressFamily.InterNetwork
					, SocketType.Stream
					, ProtocolType.Tcp
					);
					
				IPEndPoint ipe = new IPEndPoint(_routerIP, _routerPort);
					
					UnityEngine.Debug.Log(OCLogSymbol.CONNECTION + "Start Connecting to router on IP " + _routerIP + ":" + _routerPort + "...");
					
				// I'd kinda like to display this in the console...so people can see how it's connecting.
					
				OpenCog.Utility.Console.Console console = OpenCog.Utility.Console.Console.Instance;
					
				if (console == null)
					UnityEngine.Debug.LogWarning (OCLogSymbol.WARN + "Nope, grabbing the console didn't work...");		
				else
				{
					//UnityEngine.Debug.Log ("Awesome grabbing the console worked...");		
				
					console.AddConsoleEntry("Start Connecting to router on IP " + _routerIP + ":" + _routerPort + "...", "Unity World", OpenCog.Utility.Console.Console.ConsoleEntry.Type.RESULT);
				}
					
				// Start the async connection request.
				System.IAsyncResult ar = asyncSocket
				.	BeginConnect
					(ipe
					, new System.AsyncCallback(ConnectCallback)
					, asyncSocket
					);
					
				//UnityEngine.Debug.Log ("Error occurs after this...");
					
				yield return new UnityEngine.WaitForSeconds(3f);
					
				//UnityEngine.Debug.Log ("...but before this.");
					
				int retryTimes = CONNECTION_TIMEOUT;
				while(!ar.IsCompleted)
				{
					retryTimes--;
					if(retryTimes == 0)
					{
						UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "Connection timed out.");
						
						_isEstablished = false;
						_clientSocket = null;
						yield return false;
						yield break;
					}
						
					yield return new UnityEngine.WaitForSeconds(1.5f);
				}
			}
			else if (_connectionState == ConnectionState.Connecting)
			{
				System.Console.WriteLine(OCLogSymbol.CONNECTION +"_connectionState == Connecting, not doing anything...");	
				
			}
			else if (_connectionState == ConnectionState.Connected)
			{
				System.Console.WriteLine(OCLogSymbol.CONNECTION +"_connectionState == Connected, are you a mental?");
			}
		}

		/// <summary>
		/// Disconnect the network element from the router. TODO: I want to describe a race condition whereby,
		/// by having Disconnect print it is disconnecting, I slow down execution enough that Send() does not fire off
		/// the error message that we disconnected midSend.
		/// </summary>
		private void Disconnect()
		{
			Debug.Log(OCLogSymbol.CONNECTION + "Disconnecting OCNetworkElement");
			LogoutRouter();
			if(_clientSocket != null)
			{
				_clientSocket.Shutdown(SocketShutdown.Both);
				_clientSocket.Close();
				_clientSocket = null;
			}
		}
						
		/// <summary>
		/// Async callback function to be invoked once the connection is established. 
		/// </summary>
		/// <param name='ar'>
		/// Async result <see cref="IAsyncResult"/>
		/// </param>
		private void ConnectCallback(System.IAsyncResult ar)
		{
			try
			{
				// Retrieve the socket from the state object.
				_clientSocket = (Socket)ar.AsyncState;
					
				System.Console.WriteLine(OCLogSymbol.CONNECTION + "Retrieved socket from the state object...");
				// Complete the connection.
					
				_clientSocket.EndConnect(ar);
					
				System.Console.WriteLine(OCLogSymbol.CONNECTION + "Connection complete...");

				_isEstablished = true;
					
				_connectionState = ConnectionState.Connected;

				UnityEngine.Debug.Log(OCLogSymbol.CLEARED + "Socket connected to router.");

				//TODO [UNTESTED]: Does this resolve hanging? Yes, but it also prevents sending, lol.
				//Can we embed it in a loop with a wait?
				//_clientSocket.Blocking = false;
					
				// Can't write to the console here, it causes one of these:
	//			CompareBaseObjectsInternal  can only be called from the main thread.
	//			Constructors and field initializers will be executed from the loading thread when loading a scene.
	//			Don't use this function in the constructor or field initializers, instead move initialization code to the Awake or Start function.

					
	//			try {
	//				OpenCog.Utility.Console.Console console = OpenCog.Utility.Console.Console.Instance;
	//				console.AddConsoleEntry("Socket connected to Embodiment Router...Logging in...", "Unity World", OpenCog.Utility.Console.Console.ConsoleEntry.Type.COMMAND);		
	//			} catch (System.Exception ex) {
	//				
	//			}
				
				LoginRouter();
			}
			catch(System.Exception e)
			{
				UnityEngine.Debug.Log(e.ToString());
			}
		}
			
		private void LoginRouter()
		{
			string command = "LOGIN " + _ID + WHITESPACE +
							_IP.ToString() + WHITESPACE + _port +
	                        NEWLINE;
			UnityEngine.Debug.Log (OCLogSymbol.CONNECTION + "Starting router login process...sending command: " + command);
				
			Send(command);
			
			_isLoggedIn = true;
				
	//		OpenCog.Utility.Console.Console console = OpenCog.Utility.Console.Console.Instance;
	//		console.AddConsoleEntry("Login complete.", "Unity World", OpenCog.Utility.Console.Console.ConsoleEntry.Type.COMMAND);
		}

		/// <summary>
		/// Logout this network element from router by sending a "logout" command.
		/// </summary>
		private void LogoutRouter()
		{
			string command = "LOGOUT " + _ID + NEWLINE;
			Send(command);
		}

		protected bool SendMessage(OCMessage message)
		{
			string payload = message.ToString();
			
			if(payload.Length == 0)
			{
				UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "Invalid empty command given.");
				return false;
			}

			string[] lineArr = payload.Split('\n');
			int numberOfLines = lineArr.Length;
			
			System.Text.StringBuilder command = new System.Text.StringBuilder("NEW_MESSAGE ");
			command.Append(message.SourceID + WHITESPACE);
			command.Append(message.TargetID + WHITESPACE);
			command.Append((int)message.Type + WHITESPACE);
			command.Append(numberOfLines + NEWLINE);

			command.Append(payload + NEWLINE);
				
	//		if (message.Type != OCMessage.MessageType.TICK)
	//			UnityEngine.Debug.Log ("Sending: " + command.ToString ());
			
			bool result = Send(command.ToString());
			
			if(result)
			{
	//			UnityEngine.Debug.Log("Successful.");
			}
			else
			{
				UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "Failed to send messsage.");
				return false;
			}
			
			return true;
		}


		/// <summary>
		/// Send the raw text data to router by socket.
		/// </summary>
		/// <param name="text">raw text to be sent</param>
		/// <returns>Send result</returns>
		private bool Send(string text)
		{
			//UnityEngine.Debug.Log ("Sending raw text to router: " + text);
				
			if(_clientSocket == null)
			{
				return false;
			}

			lock(_clientSocket)
			{

				//FIXME [RACE]: I want to describe a race condition whereby, SOMETIMES
				// by having Disconnect print it is disconnecting, I slowed down execution enough that Send() does not fire off
				// the erroneously proced error message that we disconnected midSend (while the whole game is shutting down).
				//obviously this is not ideal at all, but I have documented it here. 

				//SO! I am trying out ways to make sure 'LogError' doesn't run IF the game is shutting down.
				//enabled didn't work. _isEstablished may be too much- but it DOES seem to work. I will keep the FixMe around
				//a little longer...
				
				if(!this._isEstablished)
				{
					UnityEngine.Debug.Log(OCLogSymbol.CONNECTION + "OCNetworkElement is shutting down; not sending message.");
					_isEstablished = false;
					_clientSocket = null;
					return false;
				}

				//UnityEngine.Debug.Log ("Are we even connected, we need to be connected to send something right...");
				if(!_clientSocket.Connected)
				{
				
					UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "We lost connection while sending!");
					_isEstablished = false;
					_clientSocket = null;
					return false;
				}
				//else
					//UnityEngine.Debug.Log ("Seems we're connected...");
				
				//TODO: Delete this, I'm using it as a reference
				/*bool canSend = true;
				while(canSend)
				{
					if(!_clientSocket.Pending())
					{
						//UnityEngine.Debug.Log (System.DateTime.Now.ToString ("HH:mm:ss.fff") + ": Nope, not pending...");
						if (_shouldStop)
							UnityEngine.Debug.LogError(OCLogSymbol.IMPOSSIBLE_ERROR + "OCServerListener.Listener() has TCPListener.Pending() reporting false. Which is funny, because IT SHOULDN'T BE HERE BECAUSE _shouldStop IS TRUE!!");	
						// If listener is not pending, sleep for a while to relax the CPU.
						yield return new UnityEngine.WaitForSeconds(0.5f);
					}
				*/
				
				try
				{

					System.IO.Stream s = new System.Net.Sockets.NetworkStream(_clientSocket);
					System.IO.StreamWriter sw = new System.IO.StreamWriter(s);

					sw.Write(text);
					sw.Flush();
					
					//byte[] byteArr = Encoding.UTF8.GetBytes(message);
					//this.socket.Send(byteArr);

					sw.Close();
					s.Close();
						
					//UnityEngine.Debug.Log ("OCNetworkElement::Send was succesful!");
				}
				catch(System.Exception e)
				{
					UnityEngine.Debug.LogError (OCLogSymbol.ERROR +"Something went wrong in OCNetworkElement::Send: " + e.Message);
					return false;
				}
			}
			return true;
		}


		/**
	     * Beautify the xml text, which means to add a newline after every node.
	     */
		protected string BeautifyXmlText(XmlDocument doc)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.NewLineChars = "\r\n";
			settings.NewLineHandling = NewLineHandling.Replace;

			XmlWriter writer = XmlWriter.Create(sb, settings);
			doc.Save(writer);
			writer.Close();

			return sb.ToString();
		}

		/// <summary>
		/// Convenience method that makes NetworkElements act as an usual server.
		/// This method will be called once per frame by MonoBehavior instance.
		/// </summary>
		protected void Pulse()
		{
	//		UnityEngine.Debug.Log ("Pulsing...");
			
			if(_isNEInitialized)
				if(_messageQueue.Count > 0)
				{
		//			UnityEngine.Debug.Log ("We gots messages! " + _messageQueue.Count + " in fact!");
						
		//			lock(_messageQueue)
		//			{
		//				int messageNumer = 0;
		//					
		//				foreach (OCMessage aMessage in _messageQueue)
		//				{
		//					UnityEngine.Debug.Log ("Message number " + messageNumer + " contains: " + aMessage.ToString());
		//				}
		//			}
						
					//long startTime = DateTime.Now.Ticks;
					Queue<OCMessage> messagesToProcess;
					lock(_messageQueue)
					{
						messagesToProcess = new Queue<OCMessage>(_messageQueue);
						_messageQueue.Clear();
					}
						
					//UnityEngine.Debug.Log ("Weird copy from _messageQueue to messagesToProcess is complete! Time to get loopy!");
						
					foreach(OCMessage msg in messagesToProcess)
					{
						if(msg == null)
						{
							UnityEngine.Debug.LogError(OCLogSymbol.ERROR + "Null message to process.");
						}

		//				UnityEngine.Debug.Log("Handle message from [" + msg.SourceID + "]. Content: " + msg.ToString());
						
						bool mustExit = ProcessNextMessage(msg);
						
						if(mustExit)
						{
							UnityEngine.Debug.Log (OCLogSymbol.CONNECTION + "ProcessNextMessage forced Exit from OCNetworkElement.Pulse()");
							break;
						}
					}
				}

	//		else
	//			UnityEngine.Debug.Log ("_messageQueue.Count == 0");
		}



		public void MarkAsUnavailable(string id)
		{
			if(IsElementAvailable(id))
			{
				//NOTE: At the time of writing, this means an error has happened on the opencog side. And
				//we're doomed, so we might as well crash. 
				UnityEngine.Debug.LogWarning (OCLogSymbol.WARN + "Adding element '" + id + "' to unavailable elements.\n");
				_unavailableElements.Add(id);

				//Handle this by going Kablooey and giving up on life!
				GameManager.control.QuitWithError(5);
			}

			if(_routerID != null)
			{
				if(_routerID.Equals(id))
				{
					// Oops, router is unavailable!
					// Reset the unread message number.
					lock(_unreadMessagesLock)
					{
						_unreadMessagesCount = 0;
					}
				}
			}
		}
		
		/// <summary>
		/// Mark a network element as available.
		/// </summary>
		/// <param name="id">Network element id</param>
		public void MarkAsAvailable(string id)
		{
			//UnityEngine.Debug.Log (OCLogSymbol.CONNECTION +"Marking element '" + id + "' as available.");
				
			if(!IsElementAvailable(id))
			{
				UnityEngine.Debug.Log (OCLogSymbol.CONNECTION + "Removing element '" + id + "' from unavailable elements.", this);
				//UnityEngine.Debug.Log(UnityEngine.StackTraceUtility.ExtractStackTrace ());
				_unavailableElements.Remove(id);
			}
		}

		protected XmlElement MakeXMLElementRoot(XmlDocument doc)
		{
			doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", ""));

			// Create the root element named "oc:embodiment-msg"
			XmlElement root = (XmlElement)doc.AppendChild(doc.CreateElement("oc", "embodiment-msg", "http://www.opencog.org/brain"));
			XmlAttribute schemaLocation = doc.CreateAttribute("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
			schemaLocation.Value = "http://www.opencog.org/brain BrainProxyAxon.xsd";
			root.SetAttributeNode(schemaLocation);
			return root;
		}

		protected bool IsElementAvailable(string id)
		{
			return !_unavailableElements.Contains(id);
		}

		/// <summary>
		/// Request unread messages from router.
		/// Should be invoked in some Update() function to make it check messages
		/// in certain interval.
		/// </summary>
	//	protected IEnumerator RequestMessage(int limit)
	//	{
	//		while(true)
	//		{
	//			if(_unreadMessagesCount > 0)
	//			{	
	//				string command = "REQUEST_UNREAD_MESSAGES " + _ID +
	//                                 WHITESPACE + limit + NEWLINE;
	//				Send(command);
	//			}
	//			yield return new UnityEngine.WaitForSeconds(0.1f);
	//		}
	//	}


				
		//---------------------------------------------------------------------------

		#endregion

		//---------------------------------------------------------------------------

		#region Other Members

		//---------------------------------------------------------------------------		

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="OpenCog.Network.OCNetworkElement"/> class.  Generally, 
		/// intitialization should occur in the Start or Awake
		/// functions, not here.
		/// </summary>
		public OCNetworkElement()
		{
		}
			
		public const int CONNECTION_TIMEOUT = 10;

		public const string WHITESPACE = " ";

		public const string NEWLINE = "\n";

		public const string FAILED_MESSAGE = "FAILED";

		public const string OK_MESSAGE = "OK";		
			
		public enum ConnectionState { Disconnected = 0, Connecting = 1, Connected = 2 }

		//---------------------------------------------------------------------------

		#endregion

		//---------------------------------------------------------------------------

	}// class OCNetworkElement

}// namespace OpenCog.Network




