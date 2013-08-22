using UnityEngine;

namespace OpenCog.Map
{

	public enum OCBlockDirection : byte
	{
		Z_PLUS,
		X_PLUS,
		Z_MINUS,
		X_MINUS
	}

	public struct OCBlockData
	{

		public OpenCog.BlockSet.BaseBlockSet.OCBlock block;
		private OCBlockDirection direction;
	
		private int _globalX;
		private int _globalY;
		private int _globalZ;
	
		public int GlobalX
		{
			get {return _globalX;}
			set {_globalX = value;}
		}
	
		public int GlobalY
		{
			get {return _globalY;}
			set {_globalY = value;}
		}
	
		public int GlobalZ
		{
			get {return _globalZ;}
			set {_globalZ = value;}
		}
	
		// TODO: May need to be re-enabled...hope not, since all blockdata that gets instantiated should get its coordinates too.
	//	public BlockData(Block block) {
	//		this.block = block;
	//		direction = BlockDirection.Z_PLUS;
	//	}
	
		public OCBlockData(OpenCog.BlockSet.BaseBlockSet.OCBlock block, Vector3i globalPosition) {
			this.block = block;
			this._globalX = globalPosition.x;
			this._globalY = globalPosition.y;
			this._globalZ = globalPosition.z;
			direction = OCBlockDirection.Z_PLUS;
		}
		
		public void SetDirection(OCBlockDirection direction) {
			this.direction = direction;
		}
		public OCBlockDirection GetDirection() {
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
			return block is OpenCog.BlockSet.BaseBlockSet.OCFluidBlock;
		}
	
	}
}
