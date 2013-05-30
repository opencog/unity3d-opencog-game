
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

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.BlockSet
{

/// <summary>
/// The OpenCog OCAtlas.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCAtlas
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	[UnityEngine.SerializeField] private UnityEngine.Material _material;
	[UnityEngine.SerializeField] private int _width = 16, _height = 16;
	[UnityEngine.SerializeField] private bool _isAlpha = false;
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		
	public UnityEngine.Material @Material
	{
		get { return _material; }
		set { _material = value; }
	}

	public int Width
	{
		get { return _width; }
		set { _width = value; }
	}

	public int Height
	{
		get { return _height; }
		set { _height = value; }
	}

	public bool IsAlpha
	{
		get { return _isAlpha; }
		set { _isAlpha = value; }
	}

	public UnityEngine.Texture @Texture
	{
		get
		{
			if (_material)
				return _material.mainTexture;
			else
				return null;
		}
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public UnityEngine.Rect ToRect(int pos) {
		int x = pos % _width;
		int y = pos / _width;
		float w = 1f / _width;
		float h = 1f / _height;
		return new UnityEngine.Rect(x*w, y*h, w, h);
	}
	
	public override string ToString() {
		if(_material != null)
			return _material.name;

		return "Null";
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

}// class OCAtlas

}// namespace OpenCog




