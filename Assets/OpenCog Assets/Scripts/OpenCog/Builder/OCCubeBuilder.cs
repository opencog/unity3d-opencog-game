
/// Unity3D OpenCog World Embodiment Program
/// Copyright (C) 2013  Novamente			
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as
/// published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#region Usings, Namespaces, and Pragmas

using System.Collections;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using UnityEngine;
using OpenCog.BlockSet.BaseBlockSet;
using OpenCog.Map;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Builder
{

/// <summary>
/// The OpenCog OCCubeBuilder.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCCubeBuilder
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	

			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		

			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public static OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace[] faces = new OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace[] {
		OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace.Front,
		OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace.Back,
		OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace.Right,
		OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace.Left,
		OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace.Top,
		OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace.Bottom,
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
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(0.5f, -0.5f, 0.5f),
		},
	//Back
		new Vector3[] {
			new Vector3(0.5f, -0.5f, -0.5f),
			new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, -0.5f),
		},
	//Right
		new Vector3[] {
			new Vector3(0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(0.5f, -0.5f, -0.5f),
		},
	//Left
		new Vector3[] {
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, -0.5f, 0.5f),
			
		},
	//Top
		new Vector3[] {
			new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, 0.5f, -0.5f),
		},
	//Bottom
		new Vector3[] {
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, -0.5f, -0.5f),
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
	
	public static void Build(Vector3i localPos, Vector3i worldPos, OpenCog.Map.OCMap map, OpenCog.Builder.OCMeshBuilder mesh, bool onlyLight)
	{
		OpenCog.Map.OCBlockData block = map.GetBlock(worldPos);
		OCCubeBlock cube = (OCCubeBlock)block.block;
		OpenCog.Map.OCBlockDirection direction = block.GetDirection();
		
		for(int i=0; i<6; i++)
		{
			OCCubeBlock.CubeFace face = faces[i];
			Vector3i dir = directions[i];
			Vector3i nearPos = worldPos + dir;
			if(IsFaceVisible(map, nearPos))
			{
				if(!onlyLight)
				{
					BuildFace(face, cube, direction, localPos, mesh);
				}
				BuildFaceLight(face, map, worldPos, mesh);
			}
		}
	}

	public static OpenCog.Builder.OCMeshBuilder Build(OpenCog.BlockSet.BaseBlockSet.OCCubeBlock cube)
	{
		OpenCog.Builder.OCMeshBuilder mesh = new OpenCog.Builder.OCMeshBuilder();
		for(int i=0; i<vertices.Length; i++)
		{
			mesh.AddFaceIndices(0);
			mesh.AddVertices(vertices[i], Vector3.zero);
			mesh.AddNormals(normals[i]);
			
			Vector2[] texCoords = cube.GetFaceUV((OCCubeBlock.CubeFace)i, OpenCog.Map.OCBlockDirection.Z_PLUS);
			mesh.AddTexCoords(texCoords);
			mesh.AddFaceColor(new Color(0, 0, 0, 1));
		}
		return mesh;
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	private static bool IsFaceVisible(OpenCog.Map.OCMap map, Vector3i nearPos)
	{
		OCBlockData blockData = map.GetBlock(nearPos);
		if(blockData == null) return false;
		OCBlock block = blockData.block;
		return !(block is OCCubeBlock) || block.IsAlpha();
	}
	
	private static void BuildFace(OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace face, OpenCog.BlockSet.BaseBlockSet.OCCubeBlock cube, OpenCog.Map.OCBlockDirection direction, Vector3 localPos, OpenCog.Builder.OCMeshBuilder mesh)
	{
		int iFace = (int)face;
		
		mesh.AddFaceIndices(cube.AtlasID);
		mesh.AddVertices(vertices[iFace], localPos);
		mesh.AddNormals(normals[iFace]);
		mesh.AddTexCoords(cube.GetFaceUV(face, direction));
	}
	
	private static void BuildFaceLight(OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace face, OpenCog.Map.OCMap map, Vector3i pos, OpenCog.Builder.OCMeshBuilder mesh)
	{
		foreach(Vector3 ver in vertices[(int)face])
		{
			Color color = OpenCog.Builder.OCBuildUtils.GetSmoothVertexLight(map, pos, ver, face);
			mesh.AddColor(color);
		}
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCCubeBuilder

}// namespace OpenCog




