using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrossBuilder {
	
	private static Vector3[] vertices = new Vector3[] {
		// face a
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f,  0.5f, -0.5f),
		new Vector3( 0.5f,  0.5f,  0.5f),
		new Vector3( 0.5f, -0.5f,  0.5f),
		
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f,  0.5f, -0.5f),
		new Vector3( 0.5f,  0.5f,  0.5f),
		new Vector3( 0.5f, -0.5f,  0.5f),
		
		//face b
		new Vector3(-0.5f, -0.5f,  0.5f),
		new Vector3(-0.5f,  0.5f,  0.5f),
		new Vector3( 0.5f,  0.5f, -0.5f),
		new Vector3( 0.5f, -0.5f, -0.5f),
		
		new Vector3(-0.5f, -0.5f,  0.5f),
		new Vector3(-0.5f,  0.5f,  0.5f),
		new Vector3( 0.5f,  0.5f, -0.5f),
		new Vector3( 0.5f, -0.5f, -0.5f),
	};
	
	private static Vector3[] normals = new Vector3[] {
		//face a
		new Vector3(-0.7f, 0, 0.7f),
		new Vector3(-0.7f, 0, 0.7f),
		new Vector3(-0.7f, 0, 0.7f),
		new Vector3(-0.7f, 0, 0.7f),
		
		-new Vector3(-0.7f, 0, 0.7f),
		-new Vector3(-0.7f, 0, 0.7f),
		-new Vector3(-0.7f, 0, 0.7f),
		-new Vector3(-0.7f, 0, 0.7f),
		
		//face b
		new Vector3(0.7f, 0, 0.7f),
		new Vector3(0.7f, 0, 0.7f),
		new Vector3(0.7f, 0, 0.7f),
		new Vector3(0.7f, 0, 0.7f),
		
		-new Vector3(0.7f, 0, 0.7f),
		-new Vector3(0.7f, 0, 0.7f),
		-new Vector3(0.7f, 0, 0.7f),
		-new Vector3(0.7f, 0, 0.7f),
	};
	
	private static int[] indices = new int[] {
		//face a
		2, 1, 0,
		3, 2, 0,
		//face a
		4, 6, 7,
		4, 5, 6,
		
		//face b
		10, 9, 8,
		11, 10, 8,
		//face b
		12, 14, 15,
		12, 13, 14
	};

	
	public static void Build(Vector3i localPos, Vector3i worldPos, Map map, MeshBuilder mesh, bool onlyLight) {
		if(!onlyLight) {
			BuildCross((Vector3)localPos, worldPos, map, mesh);
		}
		BuildCrossLight(map, worldPos, mesh);
	}
	
	private static void BuildCross(Vector3 localPos, Vector3i worldPos, Map map, MeshBuilder mesh) {
		CrossBlock cross = (CrossBlock) map.GetBlock(worldPos).block;
		
		mesh.AddIndices( cross.GetAtlasID(), indices );
		mesh.AddVertices(vertices, localPos);
		mesh.AddNormals(normals);
		mesh.AddTexCoords( cross.GetFaceUV() );
		mesh.AddTexCoords( cross.GetFaceUV() );
		mesh.AddTexCoords( cross.GetFaceUV() );
		mesh.AddTexCoords( cross.GetFaceUV() );
	}
	
	private static void BuildCrossLight(Map map, Vector3i pos, MeshBuilder mesh) {
		Color color = BuildUtils.GetBlockLight(map, pos);
		mesh.AddColors(color, vertices.Length);
	}
	
	public static MeshBuilder Build(CrossBlock cross) {
		MeshBuilder mesh = new MeshBuilder();
		
		mesh.AddIndices( 0, indices );
		mesh.AddVertices( vertices, Vector3.zero );
		mesh.AddNormals( normals );
		mesh.AddTexCoords( cross.GetFaceUV() );
		mesh.AddTexCoords( cross.GetFaceUV() );
		mesh.AddTexCoords( cross.GetFaceUV() );
		mesh.AddTexCoords( cross.GetFaceUV() );
		mesh.AddColors( new Color(0,0,0,1), vertices.Length );
		
		return mesh;
	}
	
}
