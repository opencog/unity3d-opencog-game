using UnityEngine;
using System.Collections;

class LightComputerUtils {

	
	public static void SetLightDirty(Map map, Vector3i pos) {
		Vector3i chunkPos = Chunk.ToChunkPosition(pos);
		Vector3i localPos = Chunk.ToLocalPosition(pos);
		
		SetChunkLightDirty(map, chunkPos);
		
		if(localPos.x == 0) SetChunkLightDirty(map, chunkPos-Vector3i.right);
		if(localPos.y == 0) SetChunkLightDirty(map, chunkPos-Vector3i.up);
		if(localPos.z == 0) SetChunkLightDirty(map, chunkPos-Vector3i.forward);
		
		if(localPos.x == Chunk.SIZE_X-1) SetChunkLightDirty(map, chunkPos+Vector3i.right);
		if(localPos.y == Chunk.SIZE_Y-1) SetChunkLightDirty(map, chunkPos+Vector3i.up);
		if(localPos.z == Chunk.SIZE_Z-1) SetChunkLightDirty(map, chunkPos+Vector3i.forward);
	}
	
	private static void SetChunkLightDirty(Map map, Vector3i chunkPos) {
		Chunk chunkData = map.GetChunk(chunkPos);
		if(chunkData == null) return;
		ChunkRenderer chunk = chunkData.GetChunkRenderer();
		if(chunk == null) return;
		chunk.SetLightDirty();
	}
	
	public static int GetLightStep(BlockData block) {
		if(block.IsEmpty()) {
			return 1;
		} else {
			return 2;
		}
	}
	
}
