
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

namespace OpenCog.Embodiment
{

/// <summary>
/// The OpenCog StateChangesRegister.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCStateChangesRegister : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	private static List<StateInfo> _stateList = new List<StateInfo>();
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------

	public static List<StateInfo> StateList
	{
		get { return _stateList; }
		set { _stateList = value; }
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	// This is where states get registered... 
	public static void RegisterState(UnityEngine.GameObject go, UnityEngine.Behaviour bh, string stateName)
	{
		System.Diagnostics.Debug.Assert((go != null) && (bh != null) && (stateName != null));
			
		StateInfo aInfo = new StateInfo();
		aInfo.gameObject = go;
		aInfo.behaviour = bh;
		aInfo.stateName = stateName;
			
		_stateList.Add(aInfo);
			
		OCPerceptionCollector.Instance.AddNewState(aInfo);
			 
//		// I'm guessing the perception collector is owned by the AI controlled agent. So we're telling it here that is has perceived a state change...
//		// ...so that it can tell OpenCog...i.e. the agent in the cogserver...
//		UnityEngine.GameObject[] OCAs = UnityEngine.GameObject.FindGameObjectsWithTag("OCA");
//		
//		foreach(UnityEngine.GameObject OCA in OCAs)
//		{
//			OCPerceptionCollector pCollector = OCA.GetComponent<OCPerceptionCollector>() as OCPerceptionCollector;
//			if(pCollector != null)
//			{
//				pCollector.AddNewState(aInfo);
//			}
//		}
			
	}
		
	public static void UnregisterState(StateInfo aInfo)
	{
		if(StateList.Contains(aInfo))
		{
			StateList.Remove(aInfo);
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

	public struct StateInfo
	{
		public UnityEngine.GameObject gameObject;

		public UnityEngine.Behaviour behaviour;

		public string stateName;
		//public System.Object stateVariable; // the object reference to the state variable
		
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class StateChangesRegister

}// namespace OpenCog




