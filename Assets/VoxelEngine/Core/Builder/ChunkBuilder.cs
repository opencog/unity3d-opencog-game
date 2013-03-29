using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class ChunkBuilder {
	
	private static MeshBuilder meshData = new MeshBuilder();
	
	public static Mesh BuildChunk(Mesh mesh, Chunk chunk) {
		Build(chunk, false);
		return meshData.ToMesh(mesh);
	}
	
	public static void BuildChunkLighting(Mesh mesh, Chunk chunk) {
		Build(chunk, true);
		mesh.colors = meshData.GetColors().ToArray();
	}
	
	private static void Build(Chunk chunk, bool onlyLight) {
		Map map = chunk.GetMap();
		meshData.Clear();
		for(int z=0; z<Chunk.SIZE_Z; z++) {
			for(int y=0; y<Chunk.SIZE_Y; y++) {
				for(int x=0; x<Chunk.SIZE_X; x++) {
					Block block = chunk.GetBlock(x, y, z).block;
					if(block != null) {
						Vector3i localPos = new Vector3i(x, y, z);
						Vector3i worldPos = Chunk.ToWorldPosition(chunk.GetPosition(), localPos);
						if(worldPos.y > 0)
							block.Build(localPos, worldPos, map, meshData, onlyLight);
					}
				}
			}
		}
	}
	
}

