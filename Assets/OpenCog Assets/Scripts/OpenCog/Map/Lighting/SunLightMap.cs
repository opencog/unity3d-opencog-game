using UnityEngine;

public class SunLightMap {
	
	private Map2D<short> rays = new Map2D<short>();
	private Map3D<byte> lights = new Map3D<byte>();
	
	public void SetSunHeight(int height, int x, int z) {
		rays.Set((short)height, x, z);
	}
	public int GetSunHeight(int x, int z) {
		return rays.Get(x, z);
	}
	public bool IsSunLight(int x, int y, int z) {
		return GetSunHeight(x, z) <= y;
	}
	private bool IsSunLight(Vector3i chunkPos, Vector3i localPos, int worldY) {
		Chunk2D<short> chunk = rays.GetChunk(chunkPos.x, chunkPos.z);
		return chunk != null && chunk.Get(localPos.x, localPos.z) <= worldY;
	}
	
	
	
	public bool SetMaxLight(byte light, Vector3i pos) {
		return SetMaxLight(light, pos.x, pos.y, pos.z);
	}
	public bool SetMaxLight(byte light, int x, int y, int z) {
		Vector3i chunkPos = Chunk.ToChunkPosition(x, y, z);
		Vector3i localPos = Chunk.ToLocalPosition(x, y, z);
		
		if( IsSunLight(chunkPos, localPos, y) ) return false;
		
		Chunk3D<byte> chunk = lights.GetChunkInstance(chunkPos);
		byte oldLight = chunk.Get(localPos);
		if(oldLight < light) {
			chunk.Set(light, localPos);
			return true;
		}
		return false;
	}
	
	
	public void SetLight(byte light, Vector3i pos) {
		SetLight(light, pos.x, pos.y, pos.z);
	}
	public void SetLight(byte light, int x, int y, int z) {
		lights.Set(light, x, y, z);
	}
	public void SetLight(byte light, Vector3i chunkPos, Vector3i localPos) {
		lights.GetChunkInstance(chunkPos).Set(light, localPos);
	}
	
	public byte GetLight(Vector3i pos) {
		return GetLight(pos.x, pos.y, pos.z);
	}
	public byte GetLight(int x, int y, int z) {
		Vector3i chunkPos = Chunk.ToChunkPosition(x, y, z);
		Vector3i localPos = Chunk.ToLocalPosition(x, y, z);
		return GetLight(chunkPos, localPos, y);
	}
	public byte GetLight(Vector3i chunkPos, Vector3i localPos, int worldY) {
		if(IsSunLight(chunkPos, localPos, worldY)) return SunLightComputer.MAX_LIGHT;
		
		Chunk3D<byte> chunk = lights.GetChunk(chunkPos);
		if(chunk != null) {
			byte light = chunk.Get(localPos);
			return (byte) Mathf.Max(SunLightComputer.MIN_LIGHT, light);
		}
		return SunLightComputer.MIN_LIGHT;
	}
	
}
