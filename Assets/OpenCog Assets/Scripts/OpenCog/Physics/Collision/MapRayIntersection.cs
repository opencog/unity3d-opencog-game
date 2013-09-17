using UnityEngine;
using System.Collections;

public class MapRayIntersection {
	
	public static Vector3? Intersection(OpenCog.Map.OCMap map, Ray ray, float distance) {
		Vector3 start = ray.origin;
		Vector3 end = ray.origin+ray.direction*distance;
		int startX = Mathf.RoundToInt(start.x);
		int startY = Mathf.RoundToInt(start.y);
		int startZ = Mathf.RoundToInt(start.z);
		int endX = Mathf.RoundToInt(end.x);
		int endY = Mathf.RoundToInt(end.y);
		int endZ = Mathf.RoundToInt(end.z);
		
		if(startX>endX) {
			int tmp = startX;
			startX = endX;
			endX = tmp;
		}
		
		if(startY>endY) {
			int tmp = startY;
			startY = endY;
			endY = tmp;
		}
		
		if(startZ>endZ) {
			int tmp = startZ;
			startZ = endZ;
			endZ = tmp;
		}
		
		float minDistance = distance;
		for(int z=startZ; z<=endZ; z++) {
			for(int y=startY; y<=endY; y++) {
				for(int x=startX; x<=endX; x++) {
					OpenCog.Map.OCBlockData block = map.GetBlock(x, y, z);
					if(block == null || block.IsEmpty() || block.IsFluid()) continue;
					float dis = BlockRayIntersection(new Vector3(x, y, z), ray);
					minDistance = Mathf.Min(minDistance, dis);
				}
			}
		}
		
		if(minDistance != distance) return ray.origin + ray.direction * minDistance;
		return null;
	}

	private static float BlockRayIntersection(Vector3 blockPos, Ray ray) {
  		float near = float.MinValue;
  		float far = float.MaxValue;
		
		for(int i = 0; i < 3; i++) {
    		float min = blockPos[i] - 0.5f;
    		float max = blockPos[i] + 0.5f;

    		float pos = ray.origin[i];
    		float dir = ray.direction[i];

    		// check for ray parallel to planes
    		if(Mathf.Abs(dir) <= float.Epsilon) {
      			// ray parallel to planes
      			if((pos < min) || (pos > max)) return float.MaxValue;
    		}

    		// ray not parallel to planes, so find parameters of intersections
    		float t0 = (min - pos) / dir;
    		float t1 = (max - pos) / dir;

    		// check ordering
			if( t0 > t1 ) {
				float tmp = t0;
				t0 = t1;
				t1 = tmp;
			}

    		// compare with current values
			near = Mathf.Max(t0, near);
			far = Mathf.Min(t1, far);

    		// check if ray misses entirely
    		if(near > far) return float.MaxValue;
    		if(far < 0.0f) return float.MaxValue;
  		}
		
		if(near > 0.0f) {
			return near;
		} else {
			return far;
		}
	}
	
	
}
