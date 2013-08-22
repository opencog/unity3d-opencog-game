
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
using OpenCog.BlockSet.BaseBlockSet;
using UnityEngine;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Builder
{

/// <summary>
/// The OpenCog OCGlassBuilder.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCGlassBuilder
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

	public static void Build(Vector3i localPos, Vector3i worldPos, OpenCog.Map.OCMap map, OpenCog.Builder.OCMeshBuilder mesh, bool onlyLight)
	{
		OCGlassBlock glass = (OCGlassBlock)map.GetBlock(worldPos).block;
		
		for(int i=0; i<6; i++)
		{
			OCCubeBlock.CubeFace face = OpenCog.Builder.OCCubeBuilder.faces[i];
			Vector3i dir = OpenCog.Builder.OCCubeBuilder.directions[i];
			Vector3i nearPos = worldPos + dir;
			if(IsFaceVisible(map, nearPos, face))
			{
				if(!onlyLight)
				{
					BuildFace(face, glass, (UnityEngine.Vector3)localPos, mesh);
				}
				BuildFaceLight(face, map, worldPos, mesh);
			}
		}
			
		BuildInterior(glass, mesh);
	}

	public static OpenCog.Builder.OCMeshBuilder Build(OpenCog.BlockSet.BaseBlockSet.OCGlassBlock glass)
	{
		OpenCog.Builder.OCMeshBuilder mesh = new OpenCog.Builder.OCMeshBuilder();
		for(int i=0; i<OpenCog.Builder.OCCubeBuilder.vertices.Length; i++)
		{
			mesh.AddFaceIndices(0);
			mesh.AddVertices(OpenCog.Builder.OCCubeBuilder.vertices[i], UnityEngine.Vector3.zero);
			mesh.AddNormals(OpenCog.Builder.OCCubeBuilder.normals[i]);
			mesh.AddTexCoords(glass.GetFaceUV());
			mesh.AddFaceColor(new UnityEngine.Color(0, 0, 0, 1));
		}
			
		BuildInterior(glass, mesh);
			
		return mesh;
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	private static bool IsFaceVisible(OpenCog.Map.OCMap map, Vector3i nearPos, OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace face)
	{
		if(face == OCCubeBlock.CubeFace.Top)
		{
			OpenCog.Map.OCBlockData block = map.GetBlock(nearPos);
			return block.IsEmpty() || !block.IsFluid();
		}
		else
		{
			return map.GetBlock(nearPos).IsEmpty();
		}
	}
	
	private static void BuildFace(OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace face, OpenCog.BlockSet.BaseBlockSet.OCGlassBlock glass, UnityEngine.Vector3 localPos, OpenCog.Builder.OCMeshBuilder mesh)
	{
		int iFace = (int)face;
		
		mesh.AddFaceIndices(glass.AtlasID);
		mesh.AddVertices(OpenCog.Builder.OCCubeBuilder.vertices[iFace], localPos);
		mesh.AddNormals(OpenCog.Builder.OCCubeBuilder.normals[iFace]);
		mesh.AddTexCoords(glass.GetFaceUV());
	}

	public static void BuildInterior (OCGlassBlock glass, OpenCog.Builder.OCMeshBuilder mesh)
	{
		if(glass.GetInterior() == null) return;
			
//		MeshFilter[] meshes = glass.GetInterior().GetComponents<MeshFilter>();
//		foreach(MeshFilter filter in meshes)
//		{
//			mesh = mesh.FromMesh(filter.sharedMesh);
//		}
	}
	
	private static void BuildFaceLight(OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace face, OpenCog.Map.OCMap map, Vector3i pos, OpenCog.Builder.OCMeshBuilder mesh)
	{
		Vector3i dir = OpenCog.Builder.OCCubeBuilder.directions[(int)face];
		UnityEngine.Color color = OpenCog.Builder.OCBuildUtils.GetBlockLight(map, pos + dir);
		mesh.AddFaceColor(color);
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCGlassBuilder

}// namespace OpenCog




