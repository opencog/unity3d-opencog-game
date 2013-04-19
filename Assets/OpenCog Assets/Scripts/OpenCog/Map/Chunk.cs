using UnityEngine;
using System.Collections;

public class Chunk {
	
	public const int SIZE_X_BITS = 4;
	public const int SIZE_Y_BITS = 4;
	public const int SIZE_Z_BITS = 4;
	
	public const int SIZE_X = 1 << SIZE_X_BITS;
	public const int SIZE_Y = 1 << SIZE_Y_BITS;
	public const int SIZE_Z = 1 << SIZE_Z_BITS;

	private BlockData[,,] blocks = new BlockData[Chunk.SIZE_Z, Chunk.SIZE_Y, Chunk.SIZE_X];
	private Map map;
	private Vector3i position;
	private ChunkRenderer chunkRenderer;
	
	public Chunk(Map map, Vector3i position) {
		this.map = map;
		this.position = position;
	}
	
	public ChunkRenderer GetChunkRendererInstance() {
		if(chunkRenderer == null) chunkRenderer = ChunkRenderer.CreateChunkRenderer(position, map, this);
		return chunkRenderer;
	}
	public ChunkRenderer GetChunkRenderer() {
		return chunkRenderer;
	}
	
	
	public void SetBlock(BlockData block, Vector3i pos) {
		SetBlock(block, pos.x, pos.y, pos.z);
	}
	public void SetBlock(BlockData block, int x, int y, int z) {
		blocks[z, y, x] = block;
	}

	public BlockData[,,] GetBlocks()
	{
		return blocks;
	}
	
	public BlockData GetBlock(Vector3i pos) {
		return GetBlock(pos.x, pos.y, pos.z);
	}
	public BlockData GetBlock(int x, int y, int z) {
		return blocks[z, y, x];
	}
	
	
	public Map GetMap() {
		return map;
	}
	public Vector3i GetPosition() {
		return position;
	}
	
	
	public static bool FixCoords(ref Vector3i chunk, ref Vector3i local) {
		bool changed = false;
		if(local.x < 0) {
			chunk.x--;
			local.x += Chunk.SIZE_X;
			changed = true;
		}
		if(local.y < 0) {
			chunk.y--;
			local.y += Chunk.SIZE_Y;
			changed = true;
		}
		if(local.z < 0) {
			chunk.z--;
			local.z += Chunk.SIZE_Z;
			changed = true;
		}
		
		if(local.x >= Chunk.SIZE_X) {
			chunk.x++;
			local.x -= Chunk.SIZE_X;
			changed = true;
		}
		if(local.y >= Chunk.SIZE_Y) {
			chunk.y++;
			local.y -= Chunk.SIZE_Y;
			changed = true;
		}
		if(local.z >= Chunk.SIZE_Z) {
			chunk.z++;
			local.z -= Chunk.SIZE_Z;
			changed = true;
		}
		return changed;
	}
	
	public static bool IsCorrectLocalPosition(Vector3i local) {
		return IsCorrectLocalPosition(local.x, local.y, local.z);
	}
	public static bool IsCorrectLocalPosition(int x, int y, int z) {
		return (x & SIZE_X-1) == x &&
			   (y & SIZE_Y-1) == y &&
			   (z & SIZE_Z-1) == z;
	}
	
	public static Vector3i ToChunkPosition(Vector3i point) {
		return ToChunkPosition( point.x, point.y, point.z );
	}
	public static Vector3i ToChunkPosition(int pointX, int pointY, int pointZ) {
		int chunkX = pointX >> SIZE_X_BITS;
		int chunkY = pointY >> SIZE_Y_BITS;
		int chunkZ = pointZ >> SIZE_Z_BITS;
		return new Vector3i(chunkX, chunkY, chunkZ);
	}
	
	public static Vector3i ToLocalPosition(Vector3i point) {
		return ToLocalPosition(point.x, point.y, point.z);
	}
	public static Vector3i ToLocalPosition(int pointX, int pointY, int pointZ) {
		int localX = pointX & (SIZE_X-1);
		int localY = pointY & (SIZE_Y-1);
		int localZ = pointZ & (SIZE_Z-1);
		return new Vector3i(localX, localY, localZ);
	}
	
	public static Vector3i ToWorldPosition(Vector3i chunkPosition, Vector3i localPosition) {
		int worldX = (chunkPosition.x << SIZE_X_BITS) + localPosition.x;
		int worldY = (chunkPosition.y << SIZE_Y_BITS) + localPosition.y;
		int worldZ = (chunkPosition.z << SIZE_Z_BITS) + localPosition.z;
		return new Vector3i(worldX, worldY, worldZ);
	}
	
	
}
