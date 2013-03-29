using UnityEngine;
using System.Collections;

public class ChunkRenderer : MonoBehaviour {
	
	private BlockSet blockSet;
	private Chunk chunk;
	
	private bool dirty = false, lightDirty = false;
	
	private MeshFilter filter;
	
	
	public static ChunkRenderer CreateChunkRenderer(Vector3i pos, Map map, Chunk chunk) {
		GameObject go = new GameObject("("+pos.x+" "+pos.y+" "+pos.z+")", typeof(MeshFilter), typeof(MeshRenderer), typeof(ChunkRenderer));
		go.transform.parent = map.transform;
		go.transform.localPosition = new Vector3(pos.x*Chunk.SIZE_X, pos.y*Chunk.SIZE_Y, pos.z*Chunk.SIZE_Z);
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
		
		ChunkRenderer chunkRenderer = go.GetComponent<ChunkRenderer>();
		chunkRenderer.blockSet = map.GetBlockSet();
		chunkRenderer.chunk = chunk;
		
		go.renderer.castShadows = false;
		go.renderer.receiveShadows = false;
		
		return chunkRenderer;
	}
	
	
	void Awake() {
		filter = GetComponent<MeshFilter>();
	}
	
	
	void Update() {
		if(dirty) {
			Build();
			dirty = lightDirty = false;
		}
		if(lightDirty) {
			BuildLighting();
			lightDirty = false;
		}
	}
	
	private void Build() {
		filter.sharedMesh = ChunkBuilder.BuildChunk(filter.sharedMesh, chunk);
		
		if(filter.sharedMesh == null) {
			Destroy(gameObject);
			return;
		}
		
		renderer.sharedMaterials = blockSet.GetMaterials(filter.sharedMesh.subMeshCount);
	}
	
	private void BuildLighting() {
		if(filter.sharedMesh != null) {
			ChunkBuilder.BuildChunkLighting(filter.sharedMesh, chunk);
		}
	}
	
	public void SetDirty() {
		dirty = true;
	}
	public void SetLightDirty() {
		lightDirty = true;
	}
	
	
	
}

