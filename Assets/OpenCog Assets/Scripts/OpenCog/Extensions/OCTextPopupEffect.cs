
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
/// The OpenCog OCTextPopupEffect.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCTextPopupEffect : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	[SerializeField]
	private UnityEngine.Camera _cameraToLookAt;
		
	[SerializeField]
	private UnityEngine.GUIText _text = new UnityEngine.GUIText();
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		
	public UnityEngine.GUIText Text 
	{
		get {return this._text;}
		set {_text = value;}
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public void Update()
	{
		Vector3 v = _cameraToLookAt.transform.position - transform.position;
 
     	v.x = v.z = 0.0f;
 
     	transform.LookAt( _cameraToLookAt.transform.position - v ); 
 
     	transform.Rotate(0,180,0);
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

	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCTextPopupEffect

}// namespace OpenCog




