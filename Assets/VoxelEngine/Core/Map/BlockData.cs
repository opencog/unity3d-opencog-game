using UnityEngine;

public enum BlockDirection : byte {
	Z_PLUS,
	X_PLUS,
	Z_MINUS,
	X_MINUS
}

public struct BlockData {
	
	public Block block;
	private BlockDirection direction;
	
	public BlockData(Block block) {
		this.block = block;
		direction = BlockDirection.Z_PLUS;
	}
	
	public void SetDirection(BlockDirection direction) {
		this.direction = direction;
	}
	public BlockDirection GetDirection() {
		return direction;
	}
	
	public byte GetLight() {
		if(block == null) return 0;
		return block.GetLight();
	}
	
	public bool IsEmpty() {
		return block == null;
	}
	
	public bool IsAlpha() {
		return IsEmpty() || block.IsAlpha();
	}
	
	public bool IsSolid() {
		return block != null && block.IsSolid();
	}
	
	public bool IsFluid() {
		return block is FluidBlock;
	}
	
}