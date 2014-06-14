using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	[ExecuteInEditMode]
	public class TerrainVolumeCollider : VolumeCollider
	{
		public override Mesh BuildMeshFromNodeHandle(uint nodeHandle)
		{
			Mesh collisionMesh = new Mesh();
			collisionMesh.hideFlags = HideFlags.DontSave;

			// Get the data from Cubiquity.
			int[] indices = CubiquityDLL.GetIndicesMC(nodeHandle);		
			CubiquitySmoothVertex[] cubiquityVertices = CubiquityDLL.GetVerticesMC(nodeHandle);			
			
			// Create the arrays which we'll copy the data to.	
			Vector3[] vertices = new Vector3[cubiquityVertices.Length];
			
			for(int ct = 0; ct < cubiquityVertices.Length; ct++)
			{
				// Get the vertex data from Cubiquity.
				vertices[ct] = new Vector3(cubiquityVertices[ct].x, cubiquityVertices[ct].y, cubiquityVertices[ct].z);
			}
			
			// Assign vertex data to the meshes.
			collisionMesh.vertices = vertices;
			collisionMesh.triangles = indices;
			
			return collisionMesh;
		}
	}
}
