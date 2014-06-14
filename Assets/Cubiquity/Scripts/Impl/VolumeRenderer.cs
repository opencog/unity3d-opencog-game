using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	public abstract class VolumeRenderer : MonoBehaviour
	{
		public Material material;
		public abstract Mesh BuildMeshFromNodeHandle(uint nodeHandle);
	}
}
