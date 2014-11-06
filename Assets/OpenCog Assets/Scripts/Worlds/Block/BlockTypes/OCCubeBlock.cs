
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
/// The OpenCog OCCubeBlock.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
#pragma warning disable 0414
#endregion
public class OCCubeBlock : OCBlock
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	[UnityEngine.SerializeField] 
		private int _front = 0;
		private int _back = 0;
		private int _right = 0;
		private int _left = 0;
		private int _top = 0;
		private int _bottom = 0;
		
	private UnityEngine.Vector2[][] _texCoords;
			
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
		
		_texCoords = new UnityEngine.Vector2[][] {
			ToTexCoords(_front), ToTexCoords(_back),
			ToTexCoords(_right), ToTexCoords(_left),
			ToTexCoords(_top),   ToTexCoords(_bottom)
		};
	}
	
	public override UnityEngine.Rect GetPreviewFace() {
		return ToRect( _front );
	}
	
	public UnityEngine.Vector2[] GetFaceUV(CubeFace face, OpenCog.Map.OCBlockDirection dir) {
		face = TransformFace(face, dir);
		return _texCoords[ (int)face ];
	}
	
	public static CubeFace TransformFace(CubeFace face, OpenCog.Map.OCBlockDirection dir) {
		if(face == CubeFace.Top || face == CubeFace.Bottom) {
			return face;
		}
		
		//Front, Right, Back, Left
		//0      90     180   270
		
		int angle = 0;
		if(face == CubeFace.Right) angle = 90;
		if(face == CubeFace.Back)  angle = 180;
		if(face == CubeFace.Left)  angle = 270;
		
		if(dir == OpenCog.Map.OCBlockDirection.X_MINUS) angle += 90;
		if(dir == OpenCog.Map.OCBlockDirection.Z_MINUS) angle += 180;
		if(dir == OpenCog.Map.OCBlockDirection.X_PLUS) angle += 270;
		
		angle %= 360;
		
		if(angle == 0) return CubeFace.Front;
		if(angle == 90) return CubeFace.Right;
		if(angle == 180) return CubeFace.Back;
		if(angle == 270) return CubeFace.Left;
		
		return CubeFace.Front;
	}
	
	
	public override void Build(Vector3i localPos, Vector3i worldPos, OpenCog.Map.OCMap map, OpenCog.Builder.OCMeshBuilder mesh, bool onlyLight) {
		OpenCog.Builder.OCCubeBuilder.Build(localPos, worldPos, map, mesh, onlyLight);
	}
	
	public override OpenCog.Builder.OCMeshBuilder Build() {
		return OpenCog.Builder.OCCubeBuilder.Build(this);
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

	public enum CubeFace {
	Front  = 0,
	Back   = 1,
	Right  = 2,
	Left   = 3,
	Top    = 4,
	Bottom = 5,
}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCCubeBlock

}// namespace OpenCog




