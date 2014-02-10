using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	[System.Serializable]
	public class Region
	{
		public Vector3i lowerCorner;
		public Vector3i upperCorner;
		
		public Region(int lowerX, int lowerY, int lowerZ, int upperX, int upperY, int upperZ)
		{
			lowerCorner = new Vector3i(lowerX, lowerY, lowerZ);
			upperCorner = new Vector3i(upperX, upperY, upperZ);
		}
		
		public Region(Vector3i lowerCorner, Vector3i upperCorner)
		{
			this.lowerCorner = lowerCorner;
			this.upperCorner = upperCorner;
		}
		
		public override string ToString()
		{
			return string.Format("Region({0}, {1}, {2}, {3}, {4}, {5})", 
				lowerCorner.x, lowerCorner.y, lowerCorner.z, 
				upperCorner.x, upperCorner.y, upperCorner.z);
		}
	}
}