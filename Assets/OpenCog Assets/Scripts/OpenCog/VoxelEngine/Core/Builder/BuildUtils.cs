using UnityEngine;
using System.Collections;

class BuildUtils {
	
	private static Color ComputeSmoothLight(Map map, Vector3i a, Vector3i b, Vector3i c, Vector3i d) {
		if(map.GetBlock(b).IsAlpha() || map.GetBlock(c).IsAlpha()) {
			Color c1 = GetBlockLight(map, a);
			Color c2 = GetBlockLight(map, b);
			Color c3 = GetBlockLight(map, c);
			Color c4 = GetBlockLight(map, d);
			return (c1 + c2 + c3 + c4)/4f;
		} else {
			Color c1 = GetBlockLight(map, a);
			Color c2 = GetBlockLight(map, b);
			Color c3 = GetBlockLight(map, c);
			return (c1 + c2 + c3)/3f;
		}
	}
	
	public static Color GetSmoothVertexLight(Map map, Vector3i pos, Vector3 vertex, CubeFace face) {
		// pos - позиция блока 
		// vertex позиция вершины относительно блока т.е. от -0.5 до 0.5
		int dx = (int)Mathf.Sign( vertex.x );
		int dy = (int)Mathf.Sign( vertex.y );
		int dz = (int)Mathf.Sign( vertex.z );
		
		Vector3i a, b, c, d;
		if(face == CubeFace.Left || face == CubeFace.Right) { // X
			a = pos + new Vector3i(dx, 0,  0);
			b = pos + new Vector3i(dx, dy, 0);
			c = pos + new Vector3i(dx, 0,  dz);
			d = pos + new Vector3i(dx, dy, dz);
		} else 
		if(face == CubeFace.Bottom || face == CubeFace.Top) { // Y
			a = pos + new Vector3i(0,  dy, 0);
			b = pos + new Vector3i(dx, dy, 0);
			c = pos + new Vector3i(0,  dy, dz);
			d = pos + new Vector3i(dx, dy, dz);
		} else { // Z
			a = pos + new Vector3i(0,  0,  dz);
			b = pos + new Vector3i(dx, 0,  dz);
			c = pos + new Vector3i(0,  dy, dz);
			d = pos + new Vector3i(dx, dy, dz);
		}
		
		if(map.GetBlock(b).IsAlpha() || map.GetBlock(c).IsAlpha()) {
			Color c1 = GetBlockLight(map, a);
			Color c2 = GetBlockLight(map, b);
			Color c3 = GetBlockLight(map, c);
			Color c4 = GetBlockLight(map, d);
			return (c1 + c2 + c3 + c4)/4f;
		} else {
			Color c1 = GetBlockLight(map, a);
			Color c2 = GetBlockLight(map, b);
			Color c3 = GetBlockLight(map, c);
			return (c1 + c2 + c3)/3f;
		}
	}
	
	public static Color GetBlockLight(Map map, Vector3i pos) {
		Vector3i chunkPos = Chunk.ToChunkPosition(pos);
		Vector3i localPos = Chunk.ToLocalPosition(pos);
		float light = (float) map.GetLightmap().GetLight( chunkPos, localPos ) / SunLightComputer.MAX_LIGHT;
		float sun = (float) map.GetSunLightmap().GetLight( chunkPos, localPos, pos.y ) / SunLightComputer.MAX_LIGHT;
		return new Color(light, light, light, sun);
	}
	
}