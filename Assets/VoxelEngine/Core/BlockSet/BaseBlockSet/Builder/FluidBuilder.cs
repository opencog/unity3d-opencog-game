using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FluidBuilder {
	
	public static void Build(Vector3i localPos, Vector3i worldPos, Map map, MeshBuilder mesh, bool onlyLight) {
		FluidBlock fluid = (FluidBlock) map.GetBlock(worldPos).block;
		
		for(int i=0; i<6; i++) {
			CubeFace face = CubeBuilder.faces[i];
			Vector3i dir = CubeBuilder.directions[i];
			Vector3i nearPos = worldPos + dir;
			if( IsFaceVisible(map, nearPos, face) ) {
				if(!onlyLight) BuildFace(face, fluid, (Vector3)localPos, mesh);
				BuildFaceLight(face, map, worldPos, mesh);
			}
		}
	}
	
	private static bool IsFaceVisible(Map map, Vector3i nearPos, CubeFace face) {
		if(face == CubeFace.Top) {
			BlockData block = map.GetBlock(nearPos);
			return block.IsEmpty() || !block.IsFluid();
		} else {
			return map.GetBlock(nearPos).IsEmpty();
		}
	}
	
	private static void BuildFace(CubeFace face, FluidBlock fluid, Vector3 localPos, MeshBuilder mesh) {
		int iFace = (int)face;
		
		mesh.AddFaceIndices( fluid.GetAtlasID() );
		mesh.AddVertices( CubeBuilder.vertices[iFace], localPos );
		mesh.AddNormals( CubeBuilder.normals[iFace] );
		mesh.AddTexCoords( fluid.GetFaceUV() );
	}
	
	private static void BuildFaceLight(CubeFace face, Map map, Vector3i pos, MeshBuilder mesh) {
		Vector3i dir = CubeBuilder.directions[ (int) face ];
		Color color = BuildUtils.GetBlockLight( map, pos + dir );
		mesh.AddFaceColor( color );
	}
	
	public static MeshBuilder Build(FluidBlock fluid) {
		MeshBuilder mesh = new MeshBuilder();
		for(int i=0; i<CubeBuilder.vertices.Length; i++) {
			mesh.AddFaceIndices( 0 );
			mesh.AddVertices( CubeBuilder.vertices[i], Vector3.zero );
			mesh.AddNormals( CubeBuilder.normals[i] );
			mesh.AddTexCoords( fluid.GetFaceUV() );
			mesh.AddFaceColor( new Color(0,0,0,1) );
		}
		return mesh;
	}
	
}
