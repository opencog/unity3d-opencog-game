
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
using System.Net;
using System.Net.Sockets;
using System.Xml;
using OpenCog.Attributes;
using OpenCog.Extensions;
using IAsyncResult = System.IAsyncResult;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using OpenCog.Utility;


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
	
#endregion
public class OCNetworkElement : OCSingletonMonoBehaviour<OCNetworkElement>
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
		

	
	// Settings of this network element instance.
	protected string _ID;

	private IPAddress _IP;

	private int _port;
		
	// Settings of router.
	private string _routerID;

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
	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		
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
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	/// <summary>
	/// Called when the script instance is being loaded.
	/// </summary>
	public void Awake()
	{
		Initialize();
		OCLogger.Fine(this.name + " is awake.");
	}

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start()
	{
		OCLogger.Fine(this.name + " is started.");
	}

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	public void Update()
	{
		OCLogger.Fine(this.name + " is updated.");
	}
		
	/// <summary>
	/// Reset this instance to its default values.
	/// </summary>
	public void Reset()
	{
		Uninitialize();
		Initialize();
		OCLogger.Fine(this.name + " is reset.");
	}

	/// <summary>
	/// Raises the enable event when OCNetworkElement is loaded.
	/// </summary>
	public void OnEnable()
	{
		OCLogger.Fine(this.name + " is enabled.");
	}

	/// <summary>
	/// Raises the disable event when OCNetworkElement goes out of scope.
	/// </summary>
	public void OnDisable()
	{
		OCLogger.Fine(this.name + " is disabled.");
	}

	/// <summary>
	/// Raises the destroy event when OCNetworkElement is about to be destroyed.
	/// </summary>
	public void OnDestroy()
	{
		Uninitialize();
		OCLogger.Fine(this.name + " is about to be destroyed.");
	}
		
	/// <summary>
	/// Notify a number of new messages from router.
	/// </summary>
	/// <param name="newMessagesNum">number of new arriving messages</param>
	public void NotifyNewMessages(int newMessagesNum)
	{
		OCLogger.Debugging("Notified about new messages in Router.");
		lock(_unreadMessagesLock)
		{
			_unreadMessagesCount += newMessagesNum;
			OCLogger.Debugging("Unread messages [" + _unreadMessagesCount + "]");
		}
	}
	
	/// <summary>
	/// Pull unread messages from router. 
	/// </summary>
	/// <param name="messages">A list of unread messages</param>
	public void PullMessage(List<OCMessage> messages)
	{
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
		lock(_messageQueue)
		{
			_messageQueue.Enqueue(message);	
		}
		
		lock(_unreadMessagesLock)
		{
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
	private void Initialize()
	{

	}

	protected void InitializeNetworkElement(string id)
	{
		_ID = id;
		_port = OCPortManager.AllocatePort();
		_IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
		// routerIpString appears to only be set in the obsoleted OldNetworkElement class in the old project...
		//_routerIP = IPAddress.Parse(this.routerIpString);
		// so we'll try the new way for now
		string strConfigIP = OCConfig.Instance.get("ROUTER_IP", "192.168.1.48");

		_routerIP = IPAddress.Parse(strConfigIP);
		_routerPort = OCConfig.Instance.getInt("ROUTER_PORT", 16312);
	
		_listener = new OCServerListener(this);

		StartCoroutine(Connect());
		StartCoroutine(_listener.Listen());
		StartCoroutine(RequestMessage(1));
	}
	
	/// <summary>
	/// Uninitializes this instance.  Cleanup refernces here.
	/// </summary>
	protected void Uninitialize()
	{
		StopCoroutine("Listener.Listen");
		StopCoroutine("RequestMessage");
		_listener.Stop();
		Disconnect();
		OCPortManager.ReleasePort(_port);
	}
		
	protected System.Collections.IEnumerator Connect()
	{
		Socket asyncSocket = new 
			Socket
			(AddressFamily.InterNetwork
			, SocketType.Stream
			, ProtocolType.Tcp
			);
			
		IPEndPoint ipe = new IPEndPoint(_routerIP, _routerPort);
			
		OCLogger.Debugging("Start Connecting to router");
			
		// Start the async connection request.
		System.IAsyncResult ar = asyncSocket
		.	BeginConnect
			(ipe
			, new System.AsyncCallback(ConnectCallback)
			, asyncSocket
			);
			
		yield return new UnityEngine.WaitForSeconds(0.1f);
			
		int retryTimes = CONNECTION_TIMEOUT;
		while(!ar.IsCompleted)
		{
			retryTimes--;
			if(retryTimes == 0)
			{
				OCLogger.Warn("Connection timed out.");
				yield break;
			}
				
			yield return new UnityEngine.WaitForSeconds(0.1f);
		}
	}

	/// <summary>
	/// Disconnect the network element from the router.
	/// </summary>
	private void Disconnect()
	{
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
			// Complete the connection.
			_clientSocket.EndConnect(ar);

			_isEstablished = true;

			OCLogger.Debugging("Socket connected to router.");
			
			LoginRouter();
		}
		catch(System.Exception e)
		{
			OCLogger.Warn(e.ToString());
		}
	}
		
	private void LoginRouter()
	{
		string command = "LOGIN " + _ID + WHITESPACE +
						_IP.ToString() + WHITESPACE + _port +
                        NEWLINE;
		Send(command);
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
			OCLogger.Error("Invalid empty command given.");
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
		
		bool result = Send(command.ToString());
		
		if(result)
		{
			OCLogger.Fine("Successful.");
		}
		else
		{
			OCLogger.Error("Failed to send messsage.");
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
		if(_clientSocket == null)
		{
			return false;
		}

		lock(_clientSocket)
		{
			if(!_clientSocket.Connected)
			{
				_isEstablished = false;
				_clientSocket = null;
				return false;
			}

			try
			{

				System.IO.Stream s = new System.Net.Sockets.NetworkStream(_clientSocket);
				System.IO.StreamReader sr = new System.IO.StreamReader(s);
				System.IO.StreamWriter sw = new System.IO.StreamWriter(s);

				sw.Write(text);
				sw.Flush();
				
				//byte[] byteArr = Encoding.UTF8.GetBytes(message);
				//this.socket.Send(byteArr);
				sr.Close();
				sw.Close();
				s.Close();
			}
			catch(System.Exception e)
			{
				OCLogger.Error(e.ToString());
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
		if(_messageQueue.Count > 0)
		{
			//long startTime = DateTime.Now.Ticks;
			Queue<OCMessage> messagesToProcess;
			lock(_messageQueue)
			{
				messagesToProcess = new Queue<OCMessage>(_messageQueue);
				_messageQueue.Clear();
			}
			foreach(OCMessage msg in messagesToProcess)
			{
				if(msg == null)
				{
					OCLogger.Error("Null message to process.");
				}

				OCLogger.Fine("Handle message from [" + msg.SourceID +
				          "]. Content: " + msg.ToString());
				bool mustExit = ProcessNextMessage(msg);
				if(mustExit)
				{
					break;
				}
			}
		}
	}

	public void MarkAsUnavailable(string id)
	{
		if(IsElementAvailable(id))
		{
			_unavailableElements.Add(id);
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
		if(!IsElementAvailable(id))
		{
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
	protected IEnumerator RequestMessage(int limit)
	{
		while(true)
		{
			if(_unreadMessagesCount > 0)
			{	
				string command = "REQUEST_UNREAD_MESSAGES " + _ID +
                                 WHITESPACE + limit + NEWLINE;
				Send(command);
			}
			yield return new UnityEngine.WaitForSeconds(0.1f);
		}
	}


			
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

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCNetworkElement

}// namespace OpenCog.Network




