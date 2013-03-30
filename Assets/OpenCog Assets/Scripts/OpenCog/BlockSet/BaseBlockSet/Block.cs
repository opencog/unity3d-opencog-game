using UnityEngine;
using System.Collections;
using System;

public abstract class Block {
	
	[SerializeField] private string name;
	[SerializeField] private int atlas;
	[SerializeField] private int light;
	
	private Atlas _atlas;
	private bool alpha = false;
	
	public virtual void Init(BlockSet blockSet) {
		_atlas = blockSet.GetAtlas(atlas);
		if(_atlas != null) alpha = _atlas.IsAlpha();
	}
	
	public Rect ToRect(int pos) {
		if(_atlas != null) return _atlas.ToRect(pos);
		return default(Rect);
	}
	
	protected Vector2[] ToTexCoords(int pos) {
		Rect rect = ToRect(pos);
		return new Vector2[] {
			new Vector2(rect.xMax, rect.yMin),
			new Vector2(rect.xMax, rect.yMax),
			new Vector2(rect.xMin, rect.yMax),
			new Vector2(rect.xMin, rect.yMin),
		};
	}
	
	
	public bool DrawPreview(Rect position) {
		Texture texture = GetTexture();
		if(texture != null) GUI.DrawTextureWithTexCoords(position, texture, GetPreviewFace());
		return Event.current.type == EventType.MouseDown && position.Contains(Event.current.mousePosition);
	}
	public abstract Rect GetPreviewFace();
	
	public abstract void Build(Vector3i localPos, Vector3i worldPos, Map map, MeshBuilder mesh, bool onlyLight);
	
	public abstract MeshBuilder Build();
	
	public void SetName(string name) {
		this.name = name;
	}
	public string GetName() {
		return name;
	}
	
	public void SetAtlasID(int atlas) {
		this.atlas = atlas;
	}
	public int GetAtlasID() {
		return atlas;
	}
	public Atlas GetAtlas() {
		return _atlas;
	}
	public Texture GetTexture() {
		if(_atlas != null) return _atlas.GetTexture();
		return null;
	}
	
	public void SetLight(int light) {
		this.light = Mathf.Clamp(light, 0, 15);
	}
	public byte GetLight() {
		return (byte) light;
	}
	
	public abstract bool IsSolid();
	
	public bool IsAlpha() {
		return alpha;
	}
	
}