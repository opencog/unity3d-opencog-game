using UnityEngine;
using System.Collections.Generic;

public class CubeBuilder {
	
	public static CubeFace[] faces = new CubeFace[] {
		CubeFace.Front,
		CubeFace.Back,
		CubeFace.Right,
		CubeFace.Left,
		CubeFace.Top,
		CubeFace.Bottom,
	};
	
	public static Vector3i[] directions = new Vector3i[] {
		Vector3i.forward, Vector3i.back,
		Vector3i.right, Vector3i.left,
		Vector3i.up, Vector3i.down
	};
	
	public static Vector3[][] vertices = new Vector3[][] {
		//Front
		new Vector3[] {
			new Vector3(-0.5f, -0.5f, 0.5f),
			new Vector3(-0.5f,  0.5f, 0.5f),
			new Vector3( 0.5f,  0.5f, 0.5f),
			new Vector3( 0.5f, -0.5f, 0.5f),
		},
		//Back
		new Vector3[] {
			new Vector3( 0.5f, -0.5f, -0.5f),
			new Vector3( 0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, -0.5f),
		},
		//Right
		new Vector3[] {
			new Vector3(0.5f, -0.5f,  0.5f),
			new Vector3(0.5f,  0.5f,  0.5f),
			new Vector3(0.5f,  0.5f, -0.5f),
			new Vector3(0.5f, -0.5f, -0.5f),
		},
		//Left
		new Vector3[] {
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f,  0.5f,  0.5f),
			new Vector3(-0.5f, -0.5f,  0.5f),
			
		},
		//Top
		new Vector3[] {
			new Vector3( 0.5f, 0.5f, -0.5f),
			new Vector3( 0.5f, 0.5f,  0.5f),
			new Vector3(-0.5f, 0.5f,  0.5f),
			new Vector3(-0.5f, 0.5f, -0.5f),
		},
		//Bottom
		new Vector3[] {
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f,  0.5f),
			new Vector3( 0.5f, -0.5f,  0.5f),
			new Vector3( 0.5f, -0.5f, -0.5f),
		},
	};
	
	public static Vector3[][] normals = new Vector3[][] {
		new Vector3[] {
			Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
		},
		new Vector3[] {
			Vector3.back, Vector3.back, Vector3.back, Vector3.back,
		},
		new Vector3[] {
			Vector3.right, Vector3.right, Vector3.right, Vector3.right,
		},
		new Vector3[] {
			Vector3.left, Vector3.left, Vector3.left, Vector3.left,
		},
		new Vector3[] {
			Vector3.up, Vector3.up, Vector3.up, Vector3.up,
		},
		new Vector3[] {
			Vector3.down, Vector3.down, Vector3.down, Vector3.down,
		},
	};
	
	
	public static void Build(Vector3i localPos, Vector3i worldPos, Map map, MeshBuilder mesh, bool onlyLight) {
		BlockData block = map.GetBlock(worldPos);
		CubeBlock cube = (CubeBlock) block.block;
		BlockDirection direction = block.GetDirection();
		
		for(int i=0; i<6; i++) {
			CubeFace face = faces[i];
			Vector3i dir = directions[i];
			Vector3i nearPos = worldPos + dir;
			if( IsFaceVisible(map, nearPos) ) {
				if(!onlyLight) BuildFace(face, cube, direction, localPos, mesh);
				BuildFaceLight(face, map, worldPos, mesh);
			}
		}
	}
	
	private static bool IsFaceVisible(Map map, Vector3i nearPos) {
		Block block = map.GetBlock(nearPos).block;
		return !(block is CubeBlock) || block.IsAlpha();
	}
	
	private static void BuildFace(CubeFace face, CubeBlock cube, BlockDirection direction, Vector3 localPos, MeshBuilder mesh) {
		int iFace = (int)face;
		
		mesh.AddFaceIndices( cube.GetAtlasID() );
		mesh.AddVertices(vertices[iFace], localPos);
		mesh.AddNormals( normals[iFace] );
		mesh.AddTexCoords( cube.GetFaceUV(face, direction) );
	}
	
	private static void BuildFaceLight(CubeFace face, Map map, Vector3i pos, MeshBuilder mesh) {
		foreach(Vector3 ver in vertices[(int)face]) {
			Color color = BuildUtils.GetSmoothVertexLight(map, pos, ver, face);
			mesh.AddColor( color );
		}
	}
	
	public static MeshBuilder Build(CubeBlock cube) {
		MeshBuilder mesh = new MeshBuilder();
		for(int i=0; i<vertices.Length; i++) {
			mesh.AddFaceIndices( 0 );
			mesh.AddVertices( vertices[i], Vector3.zero );
			mesh.AddNormals( normals[i] );
			
			Vector2[] texCoords = cube.GetFaceUV((CubeFace)i, BlockDirection.Z_PLUS);
			mesh.AddTexCoords(texCoords);
			mesh.AddFaceColor( new Color(0,0,0,1) );
		}
		return mesh;
	}
	
	
}