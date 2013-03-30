using UnityEngine;
using System.Collections;

[System.Serializable]
public class Atlas {
	
	[SerializeField] private Material material;
	[SerializeField] private int width = 16, height = 16;
	[SerializeField] private bool alpha = false;
	
	public void SetMaterial(Material material) {
		this.material = material;
	}
	public Material GetMaterial() {
		return material;
	}
	
	public void SetWidth(int width) {
		this.width = width;
	}
	public int GetWidth() {
		return width;
	}
	
	public void SetHeight(int height) {
		this.height = height;
	}
	public int GetHeight() {
		return height;
	}
	
	public void SetAlpha(bool alpha) {
		this.alpha = alpha;
	}
	public bool IsAlpha() {
		return alpha;
	}
	
	public Texture GetTexture() {
		if(material) return material.mainTexture;
		return null;
	}
	
	public Rect ToRect(int pos) {
		int x = pos%width;
		int y = pos/width;
		float w = 1f/width;
		float h = 1f/height;
		return new Rect(x*w, y*h, w, h);
	}
	
	public override string ToString() {
		if(material != null) return material.name;
		return "Null";
	}
	
}
