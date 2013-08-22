
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

namespace OpenCog.BlockSet.BaseBlockSet
{

/// <summary>
/// The OpenCog OCGlassBlock.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCGlassBlock : OCBlock
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	[UnityEngine.SerializeField] 
	private int _face;
		
	[UnityEngine.SerializeField]
	private UnityEngine.GameObject _interior;
		
	private UnityEngine.Vector2[] _texCoords;
			
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

	public override void Init(OpenCog.BlockSet.OCBlockSet blockSet) {
		base.Init(blockSet);
		_texCoords = ToTexCoords(_face);
	}
	
	public override UnityEngine.Rect GetPreviewFace() {
		return ToRect(_face);
	}
	
	public UnityEngine.Vector2[] GetFaceUV() {
		return _texCoords;
	}
		
	public UnityEngine.GameObject GetInterior()
	{
		return _interior;
	}

	public void SetInterior (UnityEngine.GameObject interior)
	{
		_interior = interior;
	}
	
	public override void Build(Vector3i localPos, Vector3i worldPos, OpenCog.Map.OCMap map, OpenCog.Builder.OCMeshBuilder mesh, bool onlyLight) {
		OpenCog.Builder.OCGlassBuilder.Build(localPos, worldPos, map, mesh, onlyLight);
	}
	
	public override OpenCog.Builder.OCMeshBuilder Build() {
		return OpenCog.Builder.OCGlassBuilder.Build(this);
	}
	
	public override bool IsSolid() {
		return true;
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

}// class OCGlassBlock

}// namespace OpenCog




