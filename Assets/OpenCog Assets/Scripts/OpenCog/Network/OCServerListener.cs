
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
using System.Net.Sockets;
using OpenCog.Attributes;
using OpenCog.Extensions;
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

	/// <summary>
	/// Raises the destroy event when OCServerListener is about to be destroyed.
	/// </summary>
	public void OnDestroy()
	{
//		Uninitialize();
//		OCLogger.Fine("Server Listener for " + _networkElement.gameObject.name + 
//			" is about to be destroyed.");
	}
		
	public IEnumerator Listen()
	{
		UnityEngine.Debug.Log ("OCServerListener::Listen has a networkelement with GUID " + _networkElement.VerificationGuid);
			
		try
		{
			if (_listener == null)
			{
				_listener = new TcpListener(_networkElement.IP, _networkElement.Port);
				
				_listener.Start();	
					
				UnityEngine.Debug.Log ("Now listening on " + _networkElement.IP + ":" + _networkElement.Port + "...");
			}
			
		}
		catch(SocketException se)
		{
			UnityEngine.Debug.Log ("Whoops, something went wrong making a TCPListener: " + se.Message);
			//OCLogger.Error(se.Message);
			yield break;
		}
		
		while(!_shouldStop)
		{
			if(!_listener.Pending())
			{
				UnityEngine.Debug.Log (System.DateTime.Now.ToString ("HH:mm:ss.fff") + ": Nope, not pending...");
				// If listener is not pending, sleep for a while to relax the CPU.
				yield return new UnityEngine.WaitForSeconds(0.5f);
			}
			else
			{
				UnityEngine.Debug.Log ("Yep, pending!");
					
//				try
//				{
					UnityEngine.Debug.Log ("Accepting socket from listener...");
						
					Socket workSocket = _listener.AcceptSocket();
						
					UnityEngine.Debug.Log ("Ok, I'm going to make a new MessageHandler and call StartProcessing now...");
						
					if (_messageHandler == null)
						_messageHandler = OCMessageHandler.Instance;
					
					if (_messageHandler == null)
						UnityEngine.Debug.Log ("No handler?? I just made it!!");
					
					_messageHandler.UpdateMessagesSync(workSocket);
					//_messageHandler.UpdateMessages(workSocket);
						
					UnityEngine.Debug.Log ("Well...did anything happen?");
//				}
//				catch( SocketException se )
//				{
//					//OCLogger.Error(se.Message);
//						UnityEngine.Debug.Log (se.Message);
//				}
			}
		}
	}
		
	/// <summary>
	/// Initializes this instance.  Set default values here.
	/// </summary>
	public void Initialize(OCNetworkElement networkElement)
	{
		_networkElement = networkElement;
		_shouldStop = false;			
	}
	
	/// <summary>
	/// Uninitializes this instance.  Cleanup refernces here.
	/// </summary>
	public void Uninitialize()
	{
	}			
		
	public void Stop()
	{
		_shouldStop = true;
			
		if (_listener == null)
		{
			UnityEngine.Debug.Log ("_listener == null, nothing to call Stop on...");
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




