
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
/// The OpenCog OCChunkBuilder.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCChunkBuilder
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	private static OpenCog.Builder.OCMeshBuilder _meshData = new OpenCog.Builder.OCMeshBuilder();
			
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

	public static Mesh BuildChunk(Mesh mesh, OpenCog.Map.OCChunk chunk) {
		Build(chunk, false);
		return _meshData.ToMesh(mesh);
	}
	
	public static void BuildChunkLighting(Mesh mesh, OpenCog.Map.OCChunk chunk) {
		Build(chunk, true);
		mesh.colors = _meshData.GetColors().ToArray();
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	private static void Build(OpenCog.Map.OCChunk chunk, bool onlyLight) {
		OpenCog.Map.OCMap map = chunk.GetMap();
		_meshData.Clear();
		for(int z=0; z<OpenCog.Map.OCChunk.SIZE_Z; z++) {
			for(int y=0; y<OpenCog.Map.OCChunk.SIZE_Y; y++) {
				for(int x=0; x<OpenCog.Map.OCChunk.SIZE_X; x++) {
					OCBlockData blockData = chunk.GetBlock(x, y, z);
					if(blockData != null)
					{
						OCBlock block = blockData.block;
						if(block != null) 
						{
							Vector3i localPos = new Vector3i(x, y, z);
							Vector3i worldPos = OpenCog.Map.OCChunk.ToWorldPosition(chunk.GetPosition(), localPos);
							if(worldPos.y > 0)
								block.Build(localPos, worldPos, map, _meshData, onlyLight);
						}
					}
				}
			}
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

}// class OCChunkBuilder

}// namespace OpenCog




