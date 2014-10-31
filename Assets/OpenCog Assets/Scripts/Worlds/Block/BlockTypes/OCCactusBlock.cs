
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

namespace OpenCog.BlockSet.BaseBlockSet
{

/// <summary>
/// The OpenCog OCCactusBlock.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion

#pragma warning disable 0649 // Field is never assigned to, and will always have its default value
public class OCCactusBlock : OCBlock
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	[SerializeField] 
	private int _side, _top, _bottom;
		
	private Vector2[] _sideVectors, _topVectors, _bottomVectors;
			
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

	public override void Init(OCBlockSet blockSet) {
		base.Init(blockSet);
		_sideVectors = ToTexCoords(_side);
		_topVectors = ToTexCoords(_top);
		_bottomVectors = ToTexCoords(_bottom);
	}
	
	public override Rect GetPreviewFace() {
		return ToRect(_side);
	}
	
	public Vector2[] GetFaceUV(OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace face) {
		switch (face) {
			case OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace.Front: return _sideVectors;
			case OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace.Back: return _sideVectors;
			
			case OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace.Right: return _sideVectors;
			case OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace.Left: return _sideVectors;
			
			case OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace.Top: return _topVectors;
			case OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace.Bottom: return _bottomVectors;
		}
		return null;
	}
	
	public override void Build(Vector3i localPos, Vector3i worldPos, OpenCog.Map.OCMap map, OpenCog.Builder.OCMeshBuilder mesh, bool onlyLight) {
		OpenCog.Builder.OCCactusBuilder.Build(localPos, worldPos, map, mesh, onlyLight);
	}
	
	public override OpenCog.Builder.OCMeshBuilder Build() {
		return OpenCog.Builder.OCCactusBuilder.Build(this);
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

}// class OCCactusBlock

}// namespace OpenCog




