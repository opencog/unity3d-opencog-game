using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OpenCog.Map.Lighting
{
	public class OCLightComputer {
	
		public const byte MIN_LIGHT = 1;
		public const byte MAX_LIGHT = 15;
		public const byte STEP_LIGHT = 1;
		
		private static List<Vector3i> list = new List<Vector3i>();
		
		public static void RecomputeLightAtPosition(OCMap map, Vector3i pos) 
		{
			if(map.GetBlock(pos) != null && !map.GetBlock(pos).IsEmpty())
			{
				OpenCog.Map.Lighting.OCLightMap lightmap = map.GetLightmap();
				int oldLight = lightmap.GetLight(pos);
				int light = map.GetBlock(pos).GetLight();
				
				if(oldLight > light) {
					RemoveLight(map, pos);
				}
				if(light > MIN_LIGHT) {
					Scatter(map, pos);
				}
			}
		}
		
		private static void Scatter(OCMap map, Vector3i pos) {
			list.Clear();
			list.Add( pos );
			Scatter(map, list);
		}
		private static void Scatter(OCMap map, List<Vector3i> list) { // рассеивание
			OCLightMap lightmap = map.GetLightmap();
			
			foreach( Vector3i pos in list ) {
				byte light = map.GetBlock(pos).GetLight();
				if(light > MIN_LIGHT) lightmap.SetMaxLight(light, pos);
			}
			
	        for(int i=0; i<list.Count; i++) {
	            Vector3i pos = list[i];
				if(pos.y<0) continue;
				
				OCBlockData block = map.GetBlock(pos);
	            int light = lightmap.GetLight(pos) - OCLightComputerUtils.GetLightStep(block);
	            if(light <= MIN_LIGHT) continue;
				
	            foreach(Vector3i dir in Vector3i.directions) {
					Vector3i nextPos = pos + dir;
					block = map.GetBlock(nextPos);
	                if( block.IsAlpha() && lightmap.SetMaxLight((byte)light, nextPos) ) {
	                	list.Add( nextPos );
	                }
					if(!block.IsEmpty()) OCLightComputerUtils.SetLightDirty(map, nextPos);
	            }
	        }
	    }
		
		private static void RemoveLight(OCMap map, Vector3i pos) {
	        list.Clear();
			list.Add(pos);
	        RemoveLight(map, list);
	    }
		
		private static void RemoveLight(OCMap map, List<Vector3i> list) {
			OCLightMap lightmap = map.GetLightmap();
			foreach(Vector3i pos in list) {
				lightmap.SetLight(MAX_LIGHT, pos);
			}
			
			List<Vector3i> lightPoints = new List<Vector3i>();
			for(int i=0; i<list.Count; i++) {
	            Vector3i pos = list[i];
				if(pos.y<0) continue;
				
				int light = lightmap.GetLight(pos) - STEP_LIGHT;
				
				lightmap.SetLight(MIN_LIGHT, pos);
	            if (light <= MIN_LIGHT) continue;
				
				foreach(Vector3i dir in Vector3i.directions) {
					Vector3i nextPos = pos + dir;
					OCBlockData block = map.GetBlock(nextPos);
					
					if(block.IsAlpha()) {
						if(lightmap.GetLight(nextPos) <= light) {
							list.Add( nextPos );
						} else {
							lightPoints.Add( nextPos );
						}
					}
					if(block.GetLight() > MIN_LIGHT) {
						lightPoints.Add( nextPos );
					}
					
					if(!block.IsEmpty()) OCLightComputerUtils.SetLightDirty(map, nextPos);
				}	
			}
			
			
	        Scatter(map, lightPoints);
	    }
		
		
	}


}
