
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
public class OCServerListener : OCScriptableObject
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	private bool _shouldStop;
	private TcpListener _listener;
	private OCNetworkElement _networkElement;
			
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
		try
		{
			_listener = new 
				TcpListener
				(	_networkElement.IP
				, _networkElement.Port
				)
			;
			
			_listener.Start();
		}
		catch(SocketException se)
		{
			OCLogger.Error(se.Message);
			yield break;
		}
			
		while(!_shouldStop)
		{
			if(!_listener.Pending())
			{
				// If listener is pending, sleep for a while to relax the CPU.
				yield return new UnityEngine.WaitForSeconds(0.05f);
			}
			else
			{
				try
				{
					Socket workSocket = _listener.AcceptSocket();
					new OCMessageHandler(_networkElement, workSocket).Start();
				}
				catch( SocketException se )
				{
					OCLogger.Error(se.Message);
				}
			}
		}
	}
		
	public void Stop()
	{
		_shouldStop = true;
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

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	/// <summary>
	/// Initializes this instance.  Set default values here.
	/// </summary>
	private void Initialize(OCNetworkElement networkElement)
	{
		_networkElement = networkElement;
		_shouldStop = false;			
	}
	
	/// <summary>
	/// Uninitializes this instance.  Cleanup refernces here.
	/// </summary>
	private void Uninitialize()
	{
	}	
		
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
	public OCServerListener(OCNetworkElement networkElement)
	{
		_networkElement = networkElement;
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCServerListener.Network

}// namespace OpenCog




