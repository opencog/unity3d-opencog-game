using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SunLightComputer {
	
	public const byte MIN_LIGHT = 1;
	public const byte MAX_LIGHT = 15;
	public const byte STEP_LIGHT = 1;
	
	private static List<Vector3i> list = new List<Vector3i>();
	

	public static void ComputeRayAtPosition(Map map, int x, int z) {
		int maxY = map.GetMaxY( x, z );
		map.GetSunLightmap().SetSunHeight(maxY+1, x, z);
	}
	
	private static void Scatter(Map map, List<Vector3i> list) { // рассеивание
		SunLightMap lightmap = map.GetSunLightmap();
        for(int i=0; i<list.Count; i++) {
            Vector3i pos = list[i];
			if(pos.y<0) continue;
			
			BlockData block = map.GetBlock(pos);
			int light = lightmap.GetLight(pos) - LightComputerUtils.GetLightStep(block);
            if(light <= MIN_LIGHT) continue;
			
            foreach(Vector3i dir in Vector3i.directions) {
				Vector3i nextPos = pos + dir;
				block = map.GetBlock(nextPos);
                if( block.IsAlpha() && lightmap.SetMaxLight((byte)light, nextPos) ) {
                	list.Add( nextPos );
                }
				if(!block.IsEmpty()) LightComputerUtils.SetLightDirty(map, nextPos);
            }
        }
    }
	
	
	public static void RecomputeLightAtPosition(Map map, Vector3i pos) {
		SunLightMap lightmap = map.GetSunLightmap();
		int oldSunHeight = lightmap.GetSunHeight(pos.x, pos.z);
		ComputeRayAtPosition(map, pos.x, pos.z);
		int newSunHeight = lightmap.GetSunHeight(pos.x, pos.z);
		
		if(newSunHeight < oldSunHeight) { // свет опустился
			// добавляем свет
			list.Clear();
            for (int ty = newSunHeight; ty <= oldSunHeight; ty++) {
				pos.y = ty;
                lightmap.SetLight(MIN_LIGHT, pos);
                list.Add( pos );
            }
            Scatter(map, list);
		}
		if(newSunHeight > oldSunHeight) { // свет поднялся
			// удаляем свет
			list.Clear();
            for (int ty = oldSunHeight; ty <= newSunHeight; ty++) {
				pos.y = ty;
				list.Add( pos );
            }
            RemoveLight(map, list);
		}
		
		if(newSunHeight == oldSunHeight) {
			if( map.GetBlock(pos).IsAlpha() ) {
				UpdateLight(map, pos);
			} else {
				RemoveLight(map, pos);
			}
		}
	}
	
	
	private static void UpdateLight(Map map, Vector3i pos) {
        list.Clear();
		foreach(Vector3i dir in Vector3i.directions) {
			list.Add( pos + dir );
		}
        Scatter(map, list);
	}
    
	private static void RemoveLight(Map map, Vector3i pos) {
        list.Clear();
		list.Add(pos);
        RemoveLight(map, list);
    }
	
	private static void RemoveLight(Map map, List<Vector3i> list) {
		SunLightMap lightmap = map.GetSunLightmap();
		foreach(Vector3i pos in list) {
			lightmap.SetLight(MAX_LIGHT, pos);
		}
		
		List<Vector3i> lightPoints = new List<Vector3i>();
		for(int i=0; i<list.Count; i++) {
            Vector3i pos = list[i];
			if(pos.y<0) continue;
			if(lightmap.IsSunLight(pos.x, pos.y, pos.z)) {
				lightPoints.Add( pos );
				continue;
			}
			byte light = (byte) (lightmap.GetLight(pos) - STEP_LIGHT);
			lightmap.SetLight(MIN_LIGHT, pos);
            if (light <= MIN_LIGHT) continue;
			
			foreach(Vector3i dir in Vector3i.directions) {
				Vector3i nextPos = pos + dir;
				BlockData block = map.GetBlock(nextPos);
				if(block.IsAlpha()) {
					if(lightmap.GetLight(nextPos) <= light) {
						list.Add( nextPos );
					} else {
						lightPoints.Add( nextPos );
					}
				}
				if(!block.IsEmpty()) LightComputerUtils.SetLightDirty(map, nextPos);
			}	
		}
		
        Scatter(map, lightPoints);
    }
	
}


public class ChunkSunLightComputer {
	
	private const byte MIN_LIGHT = 1;
	private const byte MAX_LIGHT = 15;
	private const byte STEP_LIGHT = 1;
	
	private static List<Vector3i> list = new List<Vector3i>();
	
	public static void ComputeRays(Map map, int cx, int cz) {
		int x1 = cx*Chunk.SIZE_X-1;
		int z1 = cz*Chunk.SIZE_Z-1;
		
		int x2 = x1+Chunk.SIZE_X+2;
		int z2 = z1+Chunk.SIZE_Z+2;
		
		for(int z=z1; z<z2; z++) {
			for(int x=x1; x<x2; x++) {
				SunLightComputer.ComputeRayAtPosition(map, x, z);
			}
		}
	}
	
	
	public static void Scatter(Map map, ColumnMap columnMap, int cx, int cz) {
		int x1 = cx*Chunk.SIZE_X;
		int z1 = cz*Chunk.SIZE_Z;
		
		int x2 = x1+Chunk.SIZE_X;
		int z2 = z1+Chunk.SIZE_Z;
		
		SunLightMap lightmap = map.GetSunLightmap();
		list.Clear();
		for(int x=x1; x<x2; x++) {
			for(int z=z1; z<z2; z++) {
				int maxY = ComputeMaxY(lightmap, x, z)+1;
				for(int y=0; y<maxY; y++) {
					if(lightmap.GetLight(x, y, z) > MIN_LIGHT) {
						list.Add( new Vector3i(x, y, z) );
					}
				}
			}
		}
		Scatter(map, columnMap, list);
	}
	
	private static void Scatter(Map map, ColumnMap columnMap, List<Vector3i> list) { // рассеивание
		SunLightMap lightmap = map.GetSunLightmap();
        for(int i=0; i<list.Count; i++) {
            Vector3i pos = list[i];
			if(pos.y<0) continue;
			
			BlockData block = map.GetBlock(pos);
			int light = lightmap.GetLight(pos) - LightComputerUtils.GetLightStep(block);
            if(light <= MIN_LIGHT) continue;
			
			Vector3i chunkPos = Chunk.ToChunkPosition(pos);
			if(!columnMap.IsBuilt(chunkPos.x, chunkPos.z)) continue;
			
            foreach(Vector3i dir in Vector3i.directions) {
				Vector3i nextPos = pos + dir;
				block = map.GetBlock(nextPos);
                if( block.IsAlpha() && lightmap.SetMaxLight((byte)light, nextPos) ) {
                	list.Add( nextPos );
                }
				if(!block.IsEmpty()) LightComputerUtils.SetLightDirty(map, nextPos);
            }
        }
    }
	
	private static int ComputeMaxY(SunLightMap lightmap, int x, int z) {
		int maxY = lightmap.GetSunHeight(x, z);
		maxY = Mathf.Max(maxY, lightmap.GetSunHeight(x-1, z  ));
		maxY = Mathf.Max(maxY, lightmap.GetSunHeight(x+1, z  ));
		maxY = Mathf.Max(maxY, lightmap.GetSunHeight(x,   z-1));
		maxY = Mathf.Max(maxY, lightmap.GetSunHeight(x,   z+1));
		return maxY;
	}
	
}