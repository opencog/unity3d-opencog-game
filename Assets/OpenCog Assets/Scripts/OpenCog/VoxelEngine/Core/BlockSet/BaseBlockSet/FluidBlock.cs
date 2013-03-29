using UnityEngine;
using System.Collections;

#pragma warning disable 0649 // Field is never assigned to, and will always have its default value
public class FluidBlock : Block {
	
	[SerializeField] private int face;
	private Vector2[] texCoords;
	
	
	public override void Init(BlockSet blockSet) {
		base.Init(blockSet);
		texCoords = ToTexCoords(face);
	}
	
	public override Rect GetPreviewFace() {
		return ToRect(face);
	}
	
	public Vector2[] GetFaceUV() {
		return texCoords;
	}
	
	public override void Build(Vector3i localPos, Vector3i worldPos, Map map, MeshBuilder mesh, bool onlyLight) {
		FluidBuilder.Build(localPos, worldPos, map, mesh, onlyLight);
	}
	
	public override MeshBuilder Build() {
		return FluidBuilder.Build(this);
	}
	
	public override bool IsSolid() {
		return false;
	}
	
}
