
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

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Builder
{

/// <summary>
/// The OpenCog OCCrossBuilder.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCCrossBuilder
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
		if(!onlyLight)
		{
			BuildCross((Vector3)localPos, worldPos, map, mesh);
		}
		BuildCrossLight(map, worldPos, mesh);
	}

	public static OpenCog.Builder.OCMeshBuilder Build(OpenCog.BlockSet.BaseBlockSet.OCCrossBlock cross)
	{
		OpenCog.Builder.OCMeshBuilder mesh = new OpenCog.Builder.OCMeshBuilder();
		
		mesh.AddIndices(0, indices);
		mesh.AddVertices(vertices, Vector3.zero);
		mesh.AddNormals(normals);
		mesh.AddTexCoords(cross.GetFaceUV());
		mesh.AddTexCoords(cross.GetFaceUV());
		mesh.AddTexCoords(cross.GetFaceUV());
		mesh.AddTexCoords(cross.GetFaceUV());
		mesh.AddColors(new Color(0, 0, 0, 1), vertices.Length);
		
		return mesh;
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	private static Vector3[] vertices = new Vector3[] {
	// face a
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		
	//face b
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, -0.5f),
		
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, -0.5f),
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

	private static void BuildCross(Vector3 localPos, Vector3i worldPos, OpenCog.Map.OCMap map, OpenCog.Builder.OCMeshBuilder mesh)
	{
		OCCrossBlock cross = (OCCrossBlock)map.GetBlock(worldPos).block;
		
		mesh.AddIndices(cross.AtlasID, indices);
		mesh.AddVertices(vertices, localPos);
		mesh.AddNormals(normals);
		mesh.AddTexCoords(cross.GetFaceUV());
		mesh.AddTexCoords(cross.GetFaceUV());
		mesh.AddTexCoords(cross.GetFaceUV());
		mesh.AddTexCoords(cross.GetFaceUV());
	}
	
	private static void BuildCrossLight(OpenCog.Map.OCMap map, Vector3i pos, OpenCog.Builder.OCMeshBuilder mesh)
	{
		UnityEngine.Color color = OpenCog.Builder.OCBuildUtils.GetBlockLight(map, pos);
		mesh.AddColors(color, vertices.Length);
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCCrossBuilder

}// namespace OpenCog




