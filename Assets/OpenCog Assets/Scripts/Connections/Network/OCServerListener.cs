
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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using OpenCog.Attributes;
using OpenCog.Extensions;
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
/// The OpenCog OCServerListener.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCServerListener : OCSingletonMonoBehaviour<OCServerListener>
{ 

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	private bool _shouldStop;
	private TcpListener _listener;
	private OCNetworkElement _networkElement;
	private OCMessageHandler _messageHandler;
	private List<System.Net.Sockets.Socket> _sockets;
	private bool _isReady;
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		
	public static OCServerListener Instance
	{
		get {
			return GetInstance<OCServerListener>();
		}
	}
		
	public bool IsReady
	{
		get { return _isReady; }
		set { _isReady = value; }
	}
		
	public List<System.Net.Sockets.Socket> Sockets
	{
		get { return _sockets; }	
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	/// <summary>
	/// Raises the enable event when OCServerListener is loaded.
	/// </summary>
	public void OnEnable()
	{
//		//Initialize();
//		if (_networkElement != null)
//			OCLogger.Fine("Server Listener for " + _networkElement.gameObject.name + " is enabled.");
//		else
//			OCLogger.Debugging("Cannot emit OnEnable message with _networkElement.gameObject.name because _networkElement == null");
	}
		
	/// <summary>
	/// Raises the disable event when OCServerListener goes out of scope.
	/// </summary>
	public void OnDisable()
	{
//		if (_networkElement != null)
//			OCLogger.Fine("Server Listener for " + _networkElement.gameObject.name + 
//				" is disabled.");
//		else
//			OCLogger.Debugging("Cannot emit OnDisable message with _networkElement.gameObject.name because _networkElement == null");	
	}


		
	public IEnumerator Listen()
	{
		OCLogger.Normal ("OCServerListener::Listen has a networkelement with GUID " + _networkElement.VerificationGuid);
			
		try
		{
			if (_listener == null)
			{
				_listener = new TcpListener(_networkElement.IP, _networkElement.Port);
				
				_listener.Start();	
					
				OCLogger.Normal ("Now listening on " + _networkElement.IP + ":" + _networkElement.Port + "...");
					
				OpenCog.Utility.Console.Console console = OpenCog.Utility.Console.Console.Instance;
				console.AddConsoleEntry("Listening for connection callback...", "Unity World", OpenCog.Utility.Console.Console.ConsoleEntry.Type.RESULT);
			}
			
		}
		catch(SocketException se)
		{
			OCLogger.Normal ("Whoops, something went wrong making a TCPListener: " + se.Message);
			//OCLogger.Error(se.Message);
			yield break;
		}
		
		while(!_shouldStop)
		{
			if(!_listener.Pending())
			{
				//OCLogger.Normal (System.DateTime.Now.ToString ("HH:mm:ss.fff") + ": Nope, not pending...");
				if (_shouldStop)
					OCLogger.Normal("Which is funny, because IT SHOULDN'T BE HERE BECAUSE _shouldStop IS TRUE!!");	
				// If listener is not pending, sleep for a while to relax the CPU.
				yield return new UnityEngine.WaitForSeconds(0.5f);
			}
			else
			{
				OCLogger.Normal ("Yep, pending!");
					
//				try
//				{
					OCLogger.Warn ("Accepting socket from listener...");

                    _sockets.Add(_listener.AcceptSocket());

					OCLogger.Warn ("Socket accepted...");

                    OpenCog.Utility.Console.Console console = OpenCog.Utility.Console.Console.Instance;

                    // Two sockets must be accepted, for data and
                    // control, we don't know which one is which
                    // though
                    if (_sockets.Count == 1)
                    {
                        console.AddConsoleEntry("First callback received, initializing MessageHandler...",
                                                "Unity World", OpenCog.Utility.Console.Console.ConsoleEntry.Type.RESULT);
                        new OldMessageHandler(OCNetworkElement.Instance, _sockets.Last()).start();
                    }
                    else if (_sockets.Count == 2)
                    {
                        console.AddConsoleEntry("Second and last callback received, initializing MessageHandler...",
                                                "Unity World", OpenCog.Utility.Console.Console.ConsoleEntry.Type.RESULT);
                        new OldMessageHandler(OCNetworkElement.Instance, _sockets.Last()).start();

                        _isReady = true;
                        _shouldStop = true;
					
                        OCLogger.Normal ("_shouldStop is now TRUE!");

                        console.AddConsoleEntry("MessageHandler online, ready to receive messages!", "Unity World", OpenCog.Utility.Console.Console.ConsoleEntry.Type.RESULT);

                        yield return new UnityEngine.WaitForSeconds(0.1f);
                    }

//					OCLogger.Normal ("Ok, I'm going to make a new MessageHandler and call StartProcessing now...");
//						
//					if (_messageHandler == null)
//						_messageHandler = OCMessageHandler.Instance;
//					
//					if (_messageHandler == null)
//						OCLogger.Normal ("No handler?? I just made it!!");
//					
//					_messageHandler.UpdateMessagesSync(workSocket);
					//_messageHandler.UpdateMessages(workSocket);
						
//					OCLogger.Normal ("Well...did anything happen?");
//				}
//				catch( SocketException se )
//				{
//					//OCLogger.Error(se.Message);
//						OCLogger.Normal (se.Message);
//				}
			}
		}
	}

	new public void Awake()
	{
	}
		
	/// <summary>
	/// Initializes this instance.  Set default values here.
	/// </summary>
	public void Initialize(OCNetworkElement networkElement)
	{
		_networkElement = networkElement;
        _sockets = new List<System.Net.Sockets.Socket>();
		_shouldStop = false;			
	}
	
	/// <summary>
	/// Uninitializes this instance.  Cleanup refernces here.
	/// </summary>
	override protected void Uninitialize()
	{
	}			
		
	public void Stop()
	{
		_shouldStop = true;
			
		if (_listener == null)
		{
			OCLogger.Normal ("_listener == null, nothing to call Stop on...");
		}
		else
		{
			try
			{
				_listener.Stop();
				_listener = null;
			}
			catch(SocketException se)
			{
				OCLogger.Error(se.Message);
			}		
		}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	

		
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		
		
	/// <summary>
	/// Initializes a new instance of the 
	/// <see cref="OpenCog.Network.OCServerListener"/> class.  Initialization 
	/// occurs in the OnEnable function, not here.
	/// </summary>
	/// <param name='networkElement'>
	/// Network element.
	/// </param>
//	public OCServerListener(OCNetworkElement networkElement)
//	{
//		_networkElement = networkElement;
//	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCServerListener.Network

}// namespace OpenCog




