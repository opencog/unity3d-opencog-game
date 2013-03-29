using UnityEngine;
using System.Collections;

public class Map2D<I> {
	
	private List2D<Chunk2D<I>> chunks = new List2D<Chunk2D<I>>();
	private I defaultValue;
	
	public Map2D() {
		defaultValue = default(I);
	}
	
	public Map2D(I defaultValue) {
		this.defaultValue = defaultValue;
	}
	
	public void Set(I val, int x, int z) {
		Vector3i chunkPos = Chunk.ToChunkPosition(x, 0, z);
		Vector3i localPos = Chunk.ToLocalPosition(x, 0, z);
		Chunk2D<I> chunk = GetChunkInstance(chunkPos.x, chunkPos.z);
		chunk.Set(val, localPos.x, localPos.z);
	}
	
	public I Get(int x, int z) {
		Vector3i chunkPos = Chunk.ToChunkPosition(x, 0, z);
		Vector3i localPos = Chunk.ToLocalPosition(x, 0, z);
		Chunk2D<I> chunk = GetChunk(chunkPos.x, chunkPos.z);
		if(chunk != null) return chunk.Get(localPos.x, localPos.z);
		return defaultValue;
	}
	
	
	
	
	public Chunk2D<I> GetChunkInstance(int x, int z) {
		return chunks.GetInstance(x, z);
	}
	public Chunk2D<I> GetChunk(int x, int z) {
		return chunks.SafeGet(x, z);
	}
	
}

public class Chunk2D<I> {
	
	private I[,] chunk = new I[Chunk.SIZE_Z, Chunk.SIZE_X];
	
	public void Set(I val, int x, int z) {
		chunk[z, x] = val;
	}
	
	public I Get(int x, int z) {
		return chunk[z, x];
	}
	
}
