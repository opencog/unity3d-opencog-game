using UnityEngine;
using System;
using System.Collections;

namespace Cubiquity
{
	[System.Serializable]
	public class TerrainMaterial
	{
		public Texture2D diffuseMap;
		
		public Vector3 scale = new Vector3(4.0f, 4.0f, 4.0f);
		public Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f);
	}
}
