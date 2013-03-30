using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CactusBuilder {
	
	
	private static Vector3[][] vertices = new Vector3[][] {
		//Front
		new Vector3[] {
			new Vector3(-0.5f, -0.5f, 0.4375f),
			new Vector3(-0.5f,  0.5f, 0.4375f),
			new Vector3( 0.5f,  0.5f, 0.4375f),
			new Vector3( 0.5f, -0.5f, 0.4375f),
		},
		//Back
		new Vector3[] {
			new Vector3( 0.5f, -0.5f, -0.4375f),
			new Vector3( 0.5f,  0.5f, -0.4375f),
			new Vector3(-0.5f,  0.5f, -0.4375f),
			new Vector3(-0.5f, -0.5f, -0.4375f),
		},
		//Right
		new Vector3[] {
			new Vector3(0.4375f, -0.5f,  0.5f),
			new Vector3(0.4375f,  0.5f,  0.5f),
			new Vector3(0.4375f,  0.5f, -0.5f),
			new Vector3(0.4375f, -0.5f, -0.5f),
		},
		//Left
		new Vector3[] {
			new Vector3(-0.4375f, -0.5f, -0.5f),
			new Vector3(-0.4375f,  0.5f, -0.5f),
			new Vector3(-0.4375f,  0.5f,  0.5f),
			new Vector3(-0.4375f, -0.5f,  0.5f),
			
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
	
	private static Vector3[][] normals = new Vector3[][] {
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
		CactusBlock cactus = (CactusBlock) map.GetBlock(worldPos).block;
		
		for(int i=0; i<6; i++) {
			CubeFace face = CubeBuilder.faces[i];
			Vector3i dir = CubeBuilder.directions[i];
			Vector3i nearPos = worldPos + dir;
			
			if( IsFaceVisible(map, face, nearPos) ) {
				if(!onlyLight) BuildFace(face, cactus, (Vector3)localPos, mesh);
				BuildFaceLight(face, map, worldPos, mesh);
			}
		}
	}
	
	private static bool IsFaceVisible(Map map, CubeFace face, Vector3i nearPos) {
		if(face == CubeFace.Bottom || face == CubeFace.Top) {
			Block block = map.GetBlock(nearPos).block;
			if(block is CubeBlock && !block.IsAlpha()) return false;
			if(block is CactusBlock) return false;
		}
		return true;
	}
	
	private static void BuildFace(CubeFace face, CactusBlock cactus, Vector3 localPos, MeshBuilder mesh) {
		int iFace = (int)face;
		
		mesh.AddFaceIndices( cactus.GetAtlasID() );
		mesh.AddVertices( vertices[iFace], localPos );
		mesh.AddNormals( normals[iFace] );
		mesh.AddTexCoords( cactus.GetFaceUV(face) );
	}
	
	private static void BuildFaceLight(CubeFace face, Map map, Vector3i pos, MeshBuilder mesh) {
		foreach(Vector3 ver in vertices[(int)face]) {
			Color color = BuildUtils.GetSmoothVertexLight(map, pos, ver, face);
			mesh.AddColor( color );
		}
	}
	
	public static MeshBuilder Build(CactusBlock cactus) {
		MeshBuilder mesh = new MeshBuilder();
		for(int i=0; i<vertices.Length; i++) {
			mesh.AddFaceIndices( 0 );
			mesh.AddVertices( vertices[i], Vector3.zero );
			mesh.AddNormals( normals[i] );
			
			Vector2[] texCoords = cactus.GetFaceUV((CubeFace)i);
			mesh.AddTexCoords(texCoords);
			mesh.AddFaceColor( new Color(0,0,0,1) );
		}
		return mesh;
	}
	
	
}
