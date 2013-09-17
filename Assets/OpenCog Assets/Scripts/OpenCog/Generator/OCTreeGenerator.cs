using UnityEngine;
using System.Collections;
using OpenCog.Map;

public class OCTreeGenerator {
	
	private OpenCog.Map.OCMap map;
	
	private OpenCog.BlockSet.BaseBlockSet.OCBlock wood;
	private OpenCog.BlockSet.BaseBlockSet.OCBlock leaves;
	
	public OCTreeGenerator(OpenCog.Map.OCMap map) {
		this.map = map;
		OpenCog.BlockSet.OCBlockSet blockSet = map.GetBlockSet();
		wood = blockSet.GetBlock("Wood");
		leaves = blockSet.GetBlock("Leaves");
	}
	
	public OCTreeGenerator(OpenCog.Map.OCMap map, OpenCog.BlockSet.BaseBlockSet.OCBlock wood, OpenCog.BlockSet.BaseBlockSet.OCBlock leaves) {
		this.map = map;
		this.wood = wood;
		this.leaves = leaves;
	}
	
	
	public void Generate(int x, int y, int z) {
		OpenCog.Map.OCBlockData block = map.GetBlock(x, y-1, z);
		if(block.IsEmpty() || !block.block.GetName().Equals("Dirt")) return;
		if(Random.Range(0f, 1f) > 0.2f) return;
		
		GenerateTree(x, y, z);
	}
	
	private void GenerateTree(int x, int y, int z) {
		GenerateLeaves( new Vector3i(x, y+6, z), new Vector3i(x, y+6, z) );
		for(int i=0; i<8; i++) {
			map.SetBlock(OCBlockData.CreateInstance<OCBlockData>().Init(wood, new Vector3i(x, y+i, z)), new Vector3i(x, y+i, z));
		}
	}
	
	private void GenerateLeaves(Vector3i center, Vector3i pos) {
		Vector3 delta = center - pos;
		delta.y *= 2;
		if( delta.sqrMagnitude > 6*6 ) return;
		if(!map.GetBlock(pos).IsEmpty()) return;
		
		map.SetBlock(leaves, pos);
		foreach(Vector3i dir in Vector3i.directions) {
			GenerateLeaves(center, pos+dir);
		}
	}
	
	private void GenerateLeaves(Vector3i center) {
		const int rad = 6;
		int x1 = center.x - rad;
		int y1 = center.y - rad;
		int z1 = center.z - rad;
		
		int x2 = center.x + rad;
		int y2 = center.y + rad;
		int z2 = center.z + rad;
		
		for(int x=x1; x<=x2; x++) {
			for(int y=y1; y<=y2; y++) {
				for(int z=z1; z<=z2; z++) {
					Vector3 delta = (Vector3)center - new Vector3i(x, y, z);
					delta.y *= 2;
					
					if( delta.sqrMagnitude > rad*rad ) continue;
					if( !map.GetBlock(x, y, z).IsEmpty() ) continue;
					
					map.SetBlock(leaves, new Vector3i(x, y, z));
				}
			}
		}
	}
	
	
	
	
}
