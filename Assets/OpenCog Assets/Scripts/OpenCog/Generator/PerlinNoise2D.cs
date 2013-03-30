using UnityEngine;
using System.Collections;

public class PerlinNoise2D {
	
	private float scale;
	private Vector2 offset = Vector2.zero;
	private float persistence = 0.5f;
	private int octaves = 5;
	
	public PerlinNoise2D(float scale) {
		this.scale = scale;
		offset = new Vector2( Random.Range(-100f, 100f), Random.Range(-100f, 100f) );
	}
	
	public PerlinNoise2D SetPersistence(float persistence) {
		this.persistence = persistence;
		return this;
	}
	
	public PerlinNoise2D SetOctaves(int octaves) {
		this.octaves = octaves;
		return this;
	}
	
	
	public float Noise(float x, float y) {
		x = x*scale + offset.x;
		y = y*scale + offset.y;
        float total = 0;
        float frq = 1, amp = 1;
        for (int i = 0; i < octaves; i++) {
            if(i >= 1) {
                frq *= 2;
                amp *= persistence;
            }
            total += Mathf.PerlinNoise(x*frq, y*frq) * amp;
        }
        return total;
    }

    /*private static float SmoothNoise(float x, float y) {
        int ix = Mathf.FloorToInt(x);
        float fx = x - ix;
        int iy = Mathf.FloorToInt(y);
        float fy = y - iy;

        float v1 = SmoothNoise(ix, iy);
        float v2 = SmoothNoise(ix + 1, iy);
        float v3 = SmoothNoise(ix, iy + 1);
        float v4 = SmoothNoise(ix + 1, iy + 1);

        float i1 = Mathf.Lerp(v1, v2, fx);
        float i2 = Mathf.Lerp(v3, v4, fx);

        return Mathf.Lerp(i1, i2, fy) + 0.5f;
    }
    
    private static float SmoothNoise(int x, int y) {
        float corners = ( Noise(x-1, y-1)+Noise(x+1, y-1)+Noise(x-1, y+1)+Noise(x+1, y+1) ) / 16f;
        float sides   = ( Noise(x-1, y)  +Noise(x+1, y)  +Noise(x, y-1)  +Noise(x, y+1) ) /  8f;
        float center  =  Noise(x, y) / 4f;
        return corners + sides + center;
    }
    
    private static float Noise(int x, int y) {
        int n = x + y * 57;
        n = (n<<13) ^ n;
        return ( 1 - ( (n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824f);    
    }*/
	
	
}

public class NoiseArray2D {
	
	private const int step = 4;
	
	private PerlinNoise2D noise;
	private float[,] map = new float[Chunk.SIZE_X+2, Chunk.SIZE_Z+2];
	private Vector2i offset;
	
	public NoiseArray2D(float scale) {
		noise = new PerlinNoise2D(scale);
	}
	
	public NoiseArray2D SetPersistence(float persistence) {
		noise.SetPersistence(persistence);
		return this;
	}
	
	public NoiseArray2D SetOctaves(int octaves) {
		noise.SetOctaves(octaves);
		return this;
	}
	
	public void GenerateNoise(int offsetX, int offsetY) {
		GenerateNoise( new Vector2i(offsetX, offsetY) );
	}
	
	public void GenerateNoise(Vector2i offset) {
		this.offset = offset;
		
		int sizeX = map.GetLength(0);
		int sizeZ = map.GetLength(1);
        for(int x=0; x<sizeX; x+=step) {
			for(int y=0; y<sizeZ; y+=step) {
				Vector2i a = new Vector2i(x, y) + offset;
				Vector2i b = a + new Vector2i(step, step);
                
                float v1 = noise.Noise(a.x, a.y);
                float v2 = noise.Noise(b.x, a.y);
                float v3 = noise.Noise(a.x, b.y);
                float v4 = noise.Noise(b.x, b.y);
				
				for(int tx=0; tx<step && x+tx<sizeX; tx++) {
					for(int ty=0; ty<step && y+ty<sizeZ; ty++) {
						float fx = (float)tx/step;
                        float fy = (float)ty/step;
                        float i1 = Mathf.Lerp(v1, v2, fx);
                        float i2 = Mathf.Lerp(v3, v4, fx);
                        map[x+tx, y+ty] = Mathf.Lerp(i1, i2, fy);
					}
                }
            }
        }
		
    }
	
	public float GetNoise(int x, int y) {
		x -= offset.x;
		y -= offset.y;
		return map[x+1, y+1];
	}
	
}
