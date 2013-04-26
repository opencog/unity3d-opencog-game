
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
	
	private bool m_ShouldStop;
	private TcpListener m_Listener;
	private OCNetworkElement m_NetworkElement;
			
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
		Initialize();
		OCLogger.Fine("Server Listener for " + m_NetworkElement.gameObject.name + 
			" is enabled.");
	}
		
	/// <summary>
	/// Raises the disable event when OCServerListener goes out of scope.
	/// </summary>
	public void OnDisable()
	{
		OCLogger.Fine("Server Listener for " + m_NetworkElement.gameObject.name + 
			" is disabled.");
	}

	/// <summary>
	/// Raises the destroy event when OCServerListener is about to be destroyed.
	/// </summary>
	public void OnDestroy()
	{
		Uninitialize();
		OCLogger.Fine("Server Listener for " + m_NetworkElement.gameObject.name + 
			" is about to be destroyed.");
	}
		
	public IEnumerator Listen()
	{
		try
		{
			m_Listener = new 
				TcpListener
				(	IPAddress.Parse(m_NetworkElement.IPAddress)
				, m_NetworkElement.PortNumber
				)
			;
			
			m_Listener.Start();
		}
		catch(SocketException se)
		{
			OCLogger.Error(se.Message);
			yield break;
		}
			
		while(!m_ShouldStop)
		{
			if(!m_Listener.Pending())
			{
				// If listener is pending, sleep for a while to relax the CPU.
				yield return new WaitForSeconds(0.05f);
			}
			else
			{
				try
				{
					Socket workSocket = m_Listener.AcceptSocket();
					new
				}
			}
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
	private void Initialize()
	{
		m_NetworkElement = networkElement;
		m_ShouldStop = false;			
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
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCServerListener.Network

}// namespace OpenCog




