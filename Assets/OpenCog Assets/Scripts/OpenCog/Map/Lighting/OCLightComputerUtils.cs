using UnityEngine;
using System.Collections;

namespace OpenCog.Map.Lighting
{
	class OCLightComputerUtils {

	
		public static void SetLightDirty(OCMap map, Vector3i pos) {
			Vector3i chunkPos = OCChunk.ToChunkPosition(pos);
			Vector3i localPos = OCChunk.ToLocalPosition(pos);
			
			SetChunkLightDirty(map, chunkPos);
			
			if(localPos.x == 0) SetChunkLightDirty(map, chunkPos-Vector3i.right);
			if(localPos.y == 0) SetChunkLightDirty(map, chunkPos-Vector3i.up);
			if(localPos.z == 0) SetChunkLightDirty(map, chunkPos-Vector3i.forward);
			
			if(localPos.x == OCChunk.SIZE_X-1) SetChunkLightDirty(map, chunkPos+Vector3i.right);
			if(localPos.y == OCChunk.SIZE_Y-1) SetChunkLightDirty(map, chunkPos+Vector3i.up);
			if(localPos.z == OCChunk.SIZE_Z-1) SetChunkLightDirty(map, chunkPos+Vector3i.forward);
		}
		
		private static void SetChunkLightDirty(OCMap map, Vector3i chunkPos) {
			OCChunk chunkData = map.GetChunk(chunkPos);
			if(chunkData == null) return;
			OCChunkRenderer chunk = chunkData.GetChunkRenderer();
			if(chunk == null) return;
			chunk.SetLightDirty();
		}
		
		public static int GetLightStep(OCBlockData block) {
			if(block == null || block.IsEmpty()) {
				return 1;
			} else {
				return 2;
			}
		}
		
	}


}