using UnityEngine;
using System.Collections;

namespace OpenCog.Map
{
	public class OCChunk
	{
		public const int SIZE_X_BITS = 4;
		public const int SIZE_Y_BITS = 4;
		public const int SIZE_Z_BITS = 4;
		
		public const int SIZE_X = 16; //<< SIZE_X_BITS;
		public const int SIZE_Y = 16; //<< SIZE_Y_BITS;
		public const int SIZE_Z = 16; //<< SIZE_Z_BITS;
	
		//private OCBlockData[,,] blocks = new OCBlockData[OCChunk.SIZE_Z, OCChunk.SIZE_Y, OCChunk.SIZE_X];
		private List3D<OCBlockData> blocks = new List3D<OCBlockData>(Vector3i.zero, new Vector3i(OCChunk.SIZE_Z, OCChunk.SIZE_Y, OCChunk.SIZE_X));
		private OpenCog.Map.OCMap map;
		private Vector3i position;
		private OCChunkRenderer chunkRenderer;
		
		private bool _isEmpty = true;
		
		public bool IsEmpty
		{
			get { return _isEmpty; }	
		}
		
		public OCChunk(OCMap map, Vector3i position) {
			this.map = map;
			this.position = position;
			for(int x=blocks.GetMinX(); x < blocks.GetMaxX(); ++x)
			{
				for(int y=blocks.GetMinY(); y < blocks.GetMaxY(); ++y)
				{
					for(int z=blocks.GetMinZ (); z < blocks.GetMaxZ (); ++z)
					{
						Vector3i pos = new Vector3i(x, y, z);
						blocks.Set(OCBlockData.CreateInstance<OCBlockData>().Init(null,pos), pos);
					}
				}
			}
		}
		
		public OCChunkRenderer GetChunkRendererInstance() {
			if(chunkRenderer == null) chunkRenderer = OCChunkRenderer.CreateChunkRenderer(position, map, this);
			return chunkRenderer;
		}
		public OCChunkRenderer GetChunkRenderer() {
			return chunkRenderer;
		}
		
		
		public void SetBlock(OCBlockData block, Vector3i pos) {
			SetBlock(block, pos.x, pos.y, pos.z);
		}
		public void SetBlock(OCBlockData block, int x, int y, int z) {
			blocks.Set(block, new Vector3i(z, y, x));
			
			_isEmpty = false;
		}
	
		public List3D<OCBlockData> GetBlocks()
		{
			return blocks;
		}
		
		public OCBlockData GetBlock(Vector3i pos) {
			return GetBlock(pos.x, pos.y, pos.z);
		}
		public OCBlockData GetBlock(int x, int y, int z) {
			return blocks.Get(new Vector3i(z, y, x));
		}
		
		
		public OCMap GetMap() {
			return map;
		}
		public Vector3i GetPosition() {
			return position;
		}
		
		
		public static bool FixCoords(ref Vector3i chunk, ref Vector3i local) {
			bool changed = false;
			if(local.x < 0) {
				chunk.x--;
				local.x += OCChunk.SIZE_X;
				changed = true;
			}
			if(local.y < 0) {
				chunk.y--;
				local.y += OCChunk.SIZE_Y;
				changed = true;
			}
			if(local.z < 0) {
				chunk.z--;
				local.z += OCChunk.SIZE_Z;
				changed = true;
			}
			
			if(local.x >= OCChunk.SIZE_X) {
				chunk.x++;
				local.x -= OCChunk.SIZE_X;
				changed = true;
			}
			if(local.y >= OCChunk.SIZE_Y) {
				chunk.y++;
				local.y -= OCChunk.SIZE_Y;
				changed = true;
			}
			if(local.z >= OCChunk.SIZE_Z) {
				chunk.z++;
				local.z -= OCChunk.SIZE_Z;
				changed = true;
			}
			return changed;
		}
		
		public static bool IsCorrectLocalPosition(Vector3i local) {
			return IsCorrectLocalPosition(local.x, local.y, local.z);
		}
		public static bool IsCorrectLocalPosition(int x, int y, int z) {
			return (x & SIZE_X-1) == x &&
				   (y & SIZE_Y-1) == y &&
				   (z & SIZE_Z-1) == z;
		}
		
		public static Vector3i ToChunkPosition(Vector3i point) {
			return ToChunkPosition( point.x, point.y, point.z );
		}
		public static Vector3i ToChunkPosition(int pointX, int pointY, int pointZ) {
			int chunkX = pointX >> SIZE_X_BITS;
			int chunkY = pointY >> SIZE_Y_BITS;
			int chunkZ = pointZ >> SIZE_Z_BITS;
			return new Vector3i(chunkX, chunkY, chunkZ);
		}
		
		public static Vector3i ToLocalPosition(Vector3i point) {
			return ToLocalPosition(point.x, point.y, point.z);
		}
		public static Vector3i ToLocalPosition(int pointX, int pointY, int pointZ) {
			int localX = pointX & (SIZE_X-1);
			int localY = pointY & (SIZE_Y-1);
			int localZ = pointZ & (SIZE_Z-1);
			return new Vector3i(localX, localY, localZ);
		}
		
		public static Vector3i ToWorldPosition(Vector3i chunkPosition, Vector3i localPosition) {
			int worldX = (chunkPosition.x << SIZE_X_BITS) + localPosition.x;
			int worldY = (chunkPosition.y << SIZE_Y_BITS) + localPosition.y;
			int worldZ = (chunkPosition.z << SIZE_Z_BITS) + localPosition.z;
			return new Vector3i(worldX, worldY, worldZ);
		}
		
		
	}
}
