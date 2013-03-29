using UnityEngine;
using System.Collections;

#pragma warning disable 0414 // The private field is assigned but its value is never used
public class TerrainGenerator {
	
	private const int WATER_LEVEL = 50;
	
	private NoiseArray2D terrainNoise = new NoiseArray2D(1/50f).SetOctaves(1);
	private NoiseArray3D terrainNoise3D = new NoiseArray3D(1/30f);
	
	private NoiseArray2D islandNoise = new NoiseArray2D(1/120f).SetOctaves(3);
	private NoiseArray3D islandNoise3D = new NoiseArray3D(1/40f);
	private NoiseArray3D caveNoise3D = new NoiseArray3D(1/50f);
	
	private Map map;
	
	private Block water;
	
	private Block grass;
	private Block dirt;
	private Block sand;
	private Block stone;
	
	private Block snow;
	private Block ice;
	
	public TerrainGenerator(Map map) {
		this.map = map;
		BlockSet blockSet = map.GetBlockSet();
		
		water = blockSet.GetBlock("Water");
		
		grass = blockSet.GetBlock("Grass");
		dirt = blockSet.GetBlock("Dirt");
		sand = blockSet.GetBlock("Sand");
		stone = blockSet.GetBlock("Stone");
		
		snow = blockSet.GetBlock("Snow");
		ice = blockSet.GetBlock("Ice");
	}
	
	public IEnumerator Generate(int cx, int cz) {
		terrainNoise.GenerateNoise(cx*Chunk.SIZE_X, cz*Chunk.SIZE_Z);
		islandNoise.GenerateNoise(cx*Chunk.SIZE_X, cz*Chunk.SIZE_Z);
		
		for(int cy=0; true; cy++) {
			Vector3i worldPos = Chunk.ToWorldPosition( new Vector3i(cx, cy, cz), Vector3i.zero );
			terrainNoise3D.GenerateNoise(worldPos);
			islandNoise3D.GenerateNoise(worldPos);
			caveNoise3D.GenerateNoise(worldPos);
			
			bool generated = GenerateChunk( new Vector3i(cx, cy, cz) );
			if(!generated) break;
			
			yield return null;
		}
		
	}
	
	private bool GenerateChunk(Vector3i chunkPos) {
		bool generated = false;
		for(int z=-1; z<Chunk.SIZE_Z+1; z++) {
			for(int x=-1; x<Chunk.SIZE_X+1; x++) {
				for(int y=0; y<Chunk.SIZE_Y; y++) {
					Vector3i worldPos = Chunk.ToWorldPosition(chunkPos, new Vector3i(x, y, z));
					
					if(worldPos.y <= WATER_LEVEL) {
						if(map.GetBlock(worldPos).IsEmpty()) map.SetBlock( water, worldPos );
						generated = true;
					}
					
					int terrainHeight = GetTerrainHeight(worldPos.x, worldPos.z);
					if( worldPos.y <= terrainHeight ) {
						GenerateBlockForBaseTerrain(worldPos);
						generated = true;
						continue;
					}
					
					int islandHeight = GetIslandHeight(worldPos.x, worldPos.z);
					if( worldPos.y <= islandHeight ) {
						GenerateBlockForIsland(worldPos, islandHeight-worldPos.y, islandHeight);
						generated = true;
						continue;
					}
					
				}
			}
		}
		return generated;
	}
	
	
	public void GeneratePlants(int cx, int cz) {
		for(int z=-1; z<Chunk.SIZE_Z+1; z++) {
			for(int x=-1; x<Chunk.SIZE_X+1; x++) {
				Vector3i worldPos = new Vector3i(cx*Chunk.SIZE_X+x, 0, cz*Chunk.SIZE_Z+z);
				worldPos.y = map.GetMaxY(worldPos.x, worldPos.z);
				for(; worldPos.y>=WATER_LEVEL; worldPos.y--) {
					if(map.GetBlock(worldPos).block == dirt && map.GetSunLightmap().GetLight(worldPos+Vector3i.up) > 5) {
						map.SetBlock( grass, worldPos );
					}
				}
			}
		}
	}
	
	
	private int GetTerrainHeight(int x, int z) {
		float height = terrainNoise.GetNoise(x, z) * 10;
		return (int) height + 20;
	}
	
	private int GetIslandHeight(int x, int z) {
		float height = islandNoise.GetNoise(x, z) * 50;
		return (int) height + 20;
	}
	
	private void GenerateBlockForBaseTerrain(Vector3i worldPos) {
		float noise = terrainNoise3D.GetNoise(worldPos.x, worldPos.y, worldPos.z);
		Block block = null;
		if( IsInRange(noise, 0,    0.2f) ) block = sand;
		if( IsInRange(noise, 0.2f, 0.6f) ) block = dirt;
		if( IsInRange(noise, 0.6f, 1f)   ) block = stone;
		if(block != null) map.SetBlock(block, worldPos);
	}
	
	private void GenerateBlockForIsland(Vector3i worldPos, int deep, int height) {
		if(caveNoise3D.GetNoise(worldPos.x, worldPos.y, worldPos.z) > 0.7f) return;
		float noise = islandNoise3D.GetNoise(worldPos.x, worldPos.y, worldPos.z);
		Block block = null;
		if( IsInRange(noise, 0,    0.2f) ) block = sand;
		if( IsInRange(noise, 0.2f, 0.6f) ) block = dirt;
		if( IsInRange(noise, 0.6f, 1f)   ) block = stone;
		if(block != null) map.SetBlock(block, worldPos);
	}
	
	private static bool IsInRange(float val, float min, float max) {
		return val >= min && val <= max;
	}
	
}

