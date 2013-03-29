using UnityEngine;
using System.Collections.Generic;
using System;

[AddComponentMenu("VoxelEngine/BlockSet")]
[ExecuteInEditMode]
public class BlockSet : ScriptableObject {
	
	[SerializeField] private string data = "";
	private Atlas[] atlases = new Atlas[0];
	private Block[] blocks = new Block[0];
	
	
	void OnEnable() {
		BlockSetImport.Import(this, data);
	}
	
	public void SetAtlases(Atlas[] atlases) {
		this.atlases = atlases;
	}
	public Atlas[] GetAtlases() {
		return atlases;
	}
	public Atlas GetAtlas(int i) {
		if(i<0 || i>=atlases.Length) return null;
		return atlases[i];
	}
	
	public Material[] GetMaterials(int count) {
		Material[] materials = new Material[count];
		for(int i=0; i<count; i++) {
			materials[i] = atlases[i].GetMaterial();
		}
		return materials;
	}
	
	public void SetBlocks(Block[] blocks) {
		this.blocks = blocks;
	}
	public Block[] GetBlocks() {
		return blocks;
	}
	
	public int GetBlockCount() {
		return blocks.Length;
	}
	
	public Block GetBlock(int index) {
		if(index < 0 || index >= blocks.Length) return null;
		return blocks[index];
	}
	
	public Block GetBlock(string name) {
		foreach(Block block in blocks) {
			if(block.GetName() == name) return block;
		}
		return null;
	}
	
	public T GetBlock<T>(string name) where T : Block {
		foreach(Block block in blocks) {
			if(block.GetName() == name && block is T) return (T)block;
		}
		return null;
	}
	
	public Block[] GetBlocks(string name) {
		List<Block> list = new List<Block>();
		foreach(Block block in blocks) {
			if(block.GetName() == name) list.Add(block);
		}
		return list.ToArray();
	}
	
	public void SetData(string data) {
		this.data = data;
	}
	public string GetData() {
		return data;
	}
	
}
