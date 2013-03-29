using UnityEngine;
using System.Collections;

#pragma warning disable 0649 // Field is never assigned to, and will always have its default value
public class CactusBlock : Block {
	
	[SerializeField] private int side, top, bottom;
	private Vector2[] _side, _top, _bottom;
	
	public override void Init(BlockSet blockSet) {
		base.Init(blockSet);
		_side = ToTexCoords(side);
		_top = ToTexCoords(top);
		_bottom = ToTexCoords(bottom);
	}
	
	public override Rect GetPreviewFace() {
		return ToRect(side);
	}
	
	public Vector2[] GetFaceUV(CubeFace face) {
		switch (face) {
			case CubeFace.Front: return _side;
			case CubeFace.Back: return _side;
			
			case CubeFace.Right: return _side;
			case CubeFace.Left: return _side;
			
			case CubeFace.Top: return _top;
			case CubeFace.Bottom: return _bottom;
		}
		return null;
	}
	
	public override void Build(Vector3i localPos, Vector3i worldPos, Map map, MeshBuilder mesh, bool onlyLight) {
		CactusBuilder.Build(localPos, worldPos, map, mesh, onlyLight);
	}
	
	public override MeshBuilder Build() {
		return CactusBuilder.Build(this);
	}
	
	public override bool IsSolid() {
		return true;
	}
	
}
