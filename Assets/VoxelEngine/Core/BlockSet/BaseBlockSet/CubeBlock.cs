using UnityEngine;
using System.Collections.Generic;

public enum CubeFace {
	Front  = 0,
	Back   = 1,
	Right  = 2,
	Left   = 3,
	Top    = 4,
	Bottom = 5,
}

#pragma warning disable 0649 // Field is never assigned to, and will always have its default value
public class CubeBlock : Block {
	
	[SerializeField] private int front, back, right, left, top, bottom;
	private Vector2[][] texCoords;
	
	public override void Init(BlockSet blockSet) {
		base.Init(blockSet);
		
		texCoords = new Vector2[][] {
			ToTexCoords(front), ToTexCoords(back),
			ToTexCoords(right), ToTexCoords(left),
			ToTexCoords(top),   ToTexCoords(bottom)
		};
	}
	
	public override Rect GetPreviewFace() {
		return ToRect( front );
	}
	
	public Vector2[] GetFaceUV(CubeFace face, BlockDirection dir) {
		face = TransformFace(face, dir);
		return texCoords[ (int)face ];
	}
	
	public static CubeFace TransformFace(CubeFace face, BlockDirection dir) {
		if(face == CubeFace.Top || face == CubeFace.Bottom) {
			return face;
		}
		
		//Front, Right, Back, Left
		//0      90     180   270
		
		int angle = 0;
		if(face == CubeFace.Right) angle = 90;
		if(face == CubeFace.Back)  angle = 180;
		if(face == CubeFace.Left)  angle = 270;
		
		if(dir == BlockDirection.X_MINUS) angle += 90;
		if(dir == BlockDirection.Z_MINUS) angle += 180;
		if(dir == BlockDirection.X_PLUS) angle += 270;
		
		angle %= 360;
		
		if(angle == 0) return CubeFace.Front;
		if(angle == 90) return CubeFace.Right;
		if(angle == 180) return CubeFace.Back;
		if(angle == 270) return CubeFace.Left;
		
		return CubeFace.Front;
	}
	
	
	public override void Build(Vector3i localPos, Vector3i worldPos, Map map, MeshBuilder mesh, bool onlyLight) {
		CubeBuilder.Build(localPos, worldPos, map, mesh, onlyLight);
	}
	
	public override MeshBuilder Build() {
		return CubeBuilder.Build(this);
	}
	
	public override bool IsSolid() {
		return true;
	}
	
}