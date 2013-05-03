
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
using OpenCog.Attributes;
using OpenCog.Extensions;
using IAsyncResult = System.IAsyncResult;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using System.IO;
using System.Text;

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
public class OCNetworkElement : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
		
	private static readonly int m_CONNECTION_TIMEOUT = 10;
	private static readonly string m_WHITESPACE = " ";
	private static readonly string m_NEWLINE = "\n";
	private static readonly string m_FAILED_MESSAGE = "FAILED";
	private static readonly string m_OK_MESSAGE = "OK";
	
	// Settings of this network element instance.
	private string m_ID;
	private IPAddress m_IP;
	private int m_Port;
		
	// Settings of router.
	private string m_RouterID;
	private IPAddress m_RouterIP;
	private int m_RouterPort;
		
	/// <summary>
	/// Server listener to make this network element acting as a server.
	/// </summary>
	private OCServerListener m_Listener;
		
	// Unread messages
	private System.Object m_UnreadMessagesLock = new object();
	private int m_UnreadMessagesCount;
		
	/// <summary>
	/// Queue used to store received messages from router. Uses a concurrent
	/// implementation of the queue interface.
	/// </summary>
	private Queue<OCMessage> m_MessageQueue = new Queue<OCMessage>();
		
	/// <summary>
	/// A hashset to record the unavailable end points.
	/// </summary>
	private HashSet<string> m_UnavailableElements = new HashSet<string>();
		
	/// <summary>
	/// Client socket to talk to the router.
	/// </summary>
	private Socket m_ClientSocket;
		
	/// <summary>
	/// Flag to check if the connection between this network element and router
  /// has been established.
	/// </summary>
	private bool m_IsEstablished = false;
	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		
	public static int CONNECTION_TIMEOUT 
	{
		get { return m_CONNECTION_TIMEOUT;}
	}	
		
	public static string WHITESPACE 
	{
		get { return m_WHITESPACE;}
	}
		
	public string FAILEDMESSAGE 
	{
		get { return m_FAILED_MESSAGE;}
	}

	public string NEWLINE 
	{
		get { return m_NEWLINE;}
	}

	public string OKMESSAGE 
	{
		get { return m_OK_MESSAGE;}
	}

	public bool IsEstablished 
	{
		get { return m_IsEstablished;}
		set { m_IsEstablished = value;}
	}

	public IPAddress IP 
	{
		get { return this.m_IP;}
		set { m_IP = value;}
	}

	public int Port 
	{
		get { return this.m_Port;}
		set { m_Port = value;}
	}		
		
	/// <summary>
  /// Check if there are unread messages not yet pull from router.
  /// </summary>
	public bool HaveUnreadMessages
	{
		get { return (m_UnreadMessagesCount > 0);}
	}		
		
	protected bool IsElementAvailable(string id)
	{
		return !m_UnavailableElements.Contains(id);
	}
	
	public void MarkAsUnavailable(string id)
	{
		if (IsElementAvailable(id))
		{
			m_UnavailableElements.Add(id); 	
		}
		
		if (m_RouterID.Equals(id))
		{
			// Oops, router is unavailable!
			// Reset the unread message number.
			lock (m_UnreadMessagesLock)
			{
				m_UnreadMessagesCount = 0;
			}
		}
	}		
		
  /// <summary>
  /// Mark a network element as available.
  /// </summary>
  /// <param name="id">Network element id</param>
	public void MarkAsAvailable(string id)
	{
		if (!IsElementAvailable(id))
		{
			m_UnavailableElements.Remove(id);
		}
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
		OCLogger.Fine(gameObject.name + " is awake.");
	}

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start()
	{
		OCLogger.Fine(gameObject.name + " is started.");
	}

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	public void Update()
	{
		OCLogger.Fine(gameObject.name + " is updated.");	
	}
		
	/// <summary>
	/// Reset this instance to its default values.
	/// </summary>
	public void Reset()
	{
		Uninitialize();
		Initialize();
		OCLogger.Fine(gameObject.name + " is reset.");	
	}

	/// <summary>
	/// Raises the enable event when OCNetworkElement is loaded.
	/// </summary>
	public void OnEnable()
	{
		OCLogger.Fine(gameObject.name + " is enabled.");
	}

	/// <summary>
	/// Raises the disable event when OCNetworkElement goes out of scope.
	/// </summary>
	public void OnDisable()
	{
		OCLogger.Fine(gameObject.name + " is disabled.");
	}

	/// <summary>
	/// Raises the destroy event when OCNetworkElement is about to be destroyed.
	/// </summary>
	public void OnDestroy()
	{
		Uninitialize();
		OCLogger.Fine(gameObject.name + " is about to be destroyed.");
	}
		
  /// <summary>
  /// Notify a number of new messages from router.
  /// </summary>
  /// <param name="newMessagesNum">number of new arriving messages</param>
	public void NotifyNewMessages(int newMessagesNum)
	{
		log.Debugging("Notified about new messages in Router.");
		lock(this.unreadMessagesLock)
		{
			unreadMessagesNum += newMessagesNum;
			log.Debugging("Unread messages [" + this.unreadMessagesNum + "]");
		}
	}
	
  /// <summary>
  /// Pull unread messages from router. 
  /// </summary>
  /// <param name="messages">A list of unread messages</param>
	public void PullMessage(List<OCMessage> messages)
	{
		lock(m_MessageQueue)
		{
			foreach(OCMessage msg in messages)
			{
				m_MessageQueue.Enqueue(msg);
			}
		}
		lock(m_UnreadMessagesLock)
		{
			m_UnreadMessagesCount -= messages.Count;
		}
	}
	
  /// <summary>
  /// Pull a message from router.
  /// </summary>
  /// <param name="message">An unread message</param>
	public void PullMessage(OCMessage message)
	{
		lock(m_MessageQueue)
		{
			m_MessageQueue.Enqueue(message);	
		}
		
		lock(m_UnreadMessagesLock)
		{
			m_UnreadMessagesCount--;
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
		m_ID = id;
		m_Port = OCPortManager.AllocatePort();
        m_IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
		m_RouterIP = IPAddress.Parse(this.routerIpString);
		m_RouterPort = OCConfig.Instance.getInt("ROUTER_PORT", 16312);
		
		listener = new OCServerListener(this);
		
		StartCoroutine(Connect());
		StartCoroutine(m_Listener.Listen());
		StartCoroutine(RequestMessage(1));			
	}
	
	/// <summary>
	/// Uninitializes this instance.  Cleanup refernces here.
	/// </summary>
	private void Uninitialize()
	{
		StopCoroutine("m_Listener.Listen");
		StopCoroutine("RequestMessage");
		m_Listener.stop();
		disconnect();
		OCPortManager.ReleasePort(m_Port);			
	}
		
	private IEnumerator Connect()
	{
		Socket asyncSocket = new 
			Socket
			( AddressFamily.InterNetwork
			, SocketType.Stream
			, ProtocolType.Tcp
			)
		;
			
		IPEndPoint ipe = new IPEndPoint(m_RouterIP, m_RouterPort);
			
		OCLogger.Debugging("Start Connecting to router");
			
		// Start the async connection request.
		System.IAsyncResult ar = asyncSocket
		.	BeginConnect
			(	ipe
			, new AsyncCallback(ConnectCallback)
			, asyncSocket
			)
		;
			
		yield return new WaitForSeconds(0.1f);
			
		int retryTimes = CONNECTION_TIMEOUT;
		while(!ar.IsCompleted)
		{
			retryTimes--;
			if(retryTimes == 0)
			{
				OCLogger.Warn("Connection timed out.");
				yield break;
			}
				
			yield return new WaitForSeconds(0.1f);
		}
	}
					
	/// <summary>
	/// Async callback function to be invoked once the connection is established. 
	/// </summary>
	/// <param name='ar'>
	/// Async result <see cref="IAsyncResult"/>
	/// </param>
	private void ConnectCallback(IAsyncResult ar)
	{
		try 
		{
			// Retrieve the socket from the state object.
			m_ClientSocket = (Socket) ar.AsyncState;
			// Complete the connection.
			m_ClientSocket.EndConnect(ar);

            established = true;

			OCLogger.Debugging("Socket connected to router.");
			
			LoginRouter();
		}
		catch (Exception e)
		{
			OCLogger.Warn(e.ToString());
		}
	}
		
	private void LoginRouter()
	{
		string command = "LOGIN " + this.myID + WHITESPACE + 
						this.myIP.ToString() + WHITESPACE + this.myPort + 
                        NEWLINE;
		Send(command);
	}
		
	/// <summary>
	/// Disconnect the network element from the router.
	/// </summary>
	private void Disconnect()
	{
		LoginRouter();
		if(m_ClientSocket != null)
		{
			m_ClientSocket.Shutdown(SocketShutdown.Both);
			m_ClientSocket.Close();
			m_ClientSocket = null;
		}
	}
		
 	/// <summary>
  /// Logout this network element from router by sending a "logout" command.
  /// </summary>
	private void LogoutRouter()
	{
		string command = "LOGOUT " + m_ID + NEWLINE;
		Send(command);
	}
		
	/// <summary>
	/// Send the raw text data to router by socket.
	/// </summary>
	/// <param name="text">raw text to be sent</param>
	/// <returns>Send result</returns>
	private bool Send(string text)
	{
    if (m_ClientSocket == null) return false;

		lock (m_ClientSocket)
		{
      if (!m_ClientSocket.Connected)
      {
          m_IsEstablished = false;
          m_ClientSocket = null;
          return false;
      }
			
			try 
			{
				Stream s = new NetworkStream(m_ClientSocket);
				StreamReader sr = new StreamReader(s);
				StreamWriter sw = new StreamWriter(s);

				sw.Write(text);
				sw.Flush();
				
				//byte[] byteArr = Encoding.UTF8.GetBytes(message);
				//this.socket.Send(byteArr);
				sr.Close();
				sw.Close();
				s.Close();
			}
			catch (Exception e)
			{
				OCLogger.Error(e.ToString());
				return false;
			}
		}
		return true;
	}
		
	private bool SendMessage(OCMessage message)
	{
		string payload = message.getPlainTextRepresentation();
		
		if (payload.Length == 0)
		{
			OCLogger.Error("Invalid empty command given.");
			return false;
		}
		
		string[] lineArr = payload.Split('\n');
		int numberOfLines = lineArr.Length;
		
		StringBuilder command = new StringBuilder("NEW_MESSAGE ");
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
	/// Request unread messages from router.
	/// Should be invoked in some Update() function to make it check messages
	/// in certain interval.
	/// </summary>
	private IEnumerator RequestMessage(int limit)
	{
		while (true)
		{
			if (HaveUnreadMessages())
			{	
				string command = "REQUEST_UNREAD_MESSAGES " + m_ID + 
                                 WHITESPACE + limit + NEWLINE;
				Send(command);
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	/// <summary>
	/// Convenience method that makes NetworkElements act as an usual server.
	/// This method will be called once per frame by MonoBehavior instance.
	/// </summary>
	private void Pulse()
	{
		if(m_MessageQueue.Count > 0)
		{
			//long startTime = DateTime.Now.Ticks;
			Queue<OCMessage> messagesToProcess;
			lock(m_MessageQueue)
			{
				messagesToProcess = new Queue<OCMessage>(m_MessageQueue);
				m_MessageQueue.Clear();
			}
			foreach(OCMessage msg in messagesToProcess)
			{
				if(msg == null) OCLogger.Error("Null message to process.");
				OCLogger.Fine("Handle message from [" + msg.SourceID +
				          "]. Content: " + msg.getPlainTextRepresentation());
				bool mustExit = ProcessNextMessage(msg);
				if(mustExit) break;
			}
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

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCNetworkElement

}// namespace OpenCog.Network




