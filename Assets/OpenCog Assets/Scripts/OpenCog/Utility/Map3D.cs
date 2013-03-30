using UnityEngine;
using System.Collections;


public class Map3D<I> {
	
	private List3D<Chunk3D<I>> chunks = new List3D<Chunk3D<I>>();
	private I defaultValue;
	
	public Map3D() {
		defaultValue = default(I);
	}
	
	public Map3D(I defaultValue) {
		this.defaultValue = defaultValue;
	}
	
	public void Set(I val, Vector3i pos) {
		Set(val, pos.x, pos.y, pos.z);
	}
	public void Set(I val, int x, int y, int z) {
		Vector3i chunkPos = Chunk.ToChunkPosition(x, y, z);
		Vector3i localPos = Chunk.ToLocalPosition(x, y, z);
		Chunk3D<I> chunk = GetChunkInstance(chunkPos);
		chunk.Set(val, localPos);
	}
	
	public I Get(Vector3i pos) {
		return Get(pos.x, pos.y, pos.z);
	}
	public I Get(int x, int y, int z) {
		Vector3i chunkPos = Chunk.ToChunkPosition(x, y, z);
		Vector3i localPos = Chunk.ToLocalPosition(x, y, z);
		Chunk3D<I> chunk = GetChunk(chunkPos);
		if(chunk != null) return chunk.Get(localPos);
		return defaultValue;
	}
	
	
	
	
	public Chunk3D<I> GetChunkInstance(Vector3i pos) {
		return chunks.GetInstance(pos);
	}
	public Chunk3D<I> GetChunkInstance(int x, int y, int z) {
		return chunks.GetInstance(x, y, z);
	}
	
	public Chunk3D<I> GetChunk(Vector3i pos) {
		return chunks.SafeGet(pos);
	}
	public Chunk3D<I> GetChunk(int x, int y, int z) {
		return chunks.SafeGet(x, y, z);
	}
	
}

public class Chunk3D<I> {
	
	private I[,,] chunk = new I[Chunk.SIZE_Z, Chunk.SIZE_Y, Chunk.SIZE_X];
	
	public void Set(I val, Vector3i pos) {
		Set(val, pos.x, pos.y, pos.z);
	}
	public void Set(I val, int x, int y, int z) {
		chunk[z, y, x] = val;
	}
		
	public I Get(Vector3i pos) {
		return Get(pos.x, pos.y, pos.z);
	}
	public I Get(int x, int y, int z) {
		return chunk[z, y, x];
	}
}