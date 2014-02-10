using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	public abstract class VolumeCollider : MonoBehaviour
	{
		public abstract Mesh BuildMeshFromNodeHandle(uint nodeHandle);
	}
}
