
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
using System.Collections.Generic;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Builder
{

/// <summary>
/// The OpenCog OCMeshBuilder.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCMeshBuilder
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	private List<UnityEngine.Vector3> _vertices = new List<UnityEngine.Vector3>();
	private List<UnityEngine.Vector2> _uv = new List<UnityEngine.Vector2>();
	private List<UnityEngine.Vector3> _normals = new List<UnityEngine.Vector3>();
	private List<UnityEngine.Color> _colors = new List<UnityEngine.Color>();
	// Not touching indices, it's used as the name of a locally declared variable as well...
	private List<int>[] indices = new List<int>[0];
			
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

	public void AddVertices(UnityEngine.Vector3[] vertices, UnityEngine.Vector3 offset) {
		foreach(UnityEngine.Vector3 v in vertices) {
			_vertices.Add( v + offset );
		}
	}
	
	public void AddNormals(UnityEngine.Vector3[] normals) {
		_normals.AddRange(normals);
	}
	
	public void AddColor(UnityEngine.Color color) {
		_colors.Add(color);
	}
	
	public void AddFaceColor(UnityEngine.Color color) {
		_colors.Add(color);
		_colors.Add(color);
		_colors.Add(color);
		_colors.Add(color);
	}
	
	public void AddColors(UnityEngine.Color color, int count) {
		for(int i=0; i<count; i++) {
			_colors.Add(color);
		}
	}
	
	public void AddTexCoords(UnityEngine.Vector2[] uv) {
		_uv.AddRange(uv);
	}
	
	public void AddFaceIndices(int materialIndex) {
		int offset = _vertices.Count;
		List<int> indices = GetIndices(materialIndex);
		indices.Add( offset+2 );
		indices.Add( offset+1 );
		indices.Add( offset+0 );
		
		indices.Add( offset+3 );
		indices.Add( offset+2 );
		indices.Add( offset+0 );
	}
	
	public void AddIndices(int materialIndex, int[] indices) {
		int offset = _vertices.Count;
		List<int> meshIndices = GetIndices(materialIndex);
		foreach(int index in indices) {
			meshIndices.Add( index + offset );
		}
	}
	
	public List<int> GetIndices(int index) {
		if(index >= indices.Length) {
			int oldLength = indices.Length;
			System.Array.Resize(ref indices, index+1);
			for(int i=oldLength; i<indices.Length; i++) {
				indices[i] = new List<int>();
			}
		}
		return indices[index];
	}
	
	public List<UnityEngine.Color> GetColors() {
		return _colors;
	}
	
	public void Clear() {
		_vertices.Clear();
		_uv.Clear();
		_normals.Clear();
		_colors.Clear();
		foreach(List<int> list in indices) {
			list.Clear();
		}
	}
	
	public UnityEngine.Mesh ToMesh(UnityEngine.Mesh mesh) {
		if(_vertices.Count == 0) {
			UnityEngine.GameObject.Destroy(mesh);
			return null;
		}
		
		if(mesh == null) mesh = new UnityEngine.Mesh();
		
		mesh.Clear();
		mesh.vertices = _vertices.ToArray();
		mesh.colors = _colors.ToArray();
		mesh.normals = _normals.ToArray();
		mesh.uv = _uv.ToArray();
		
		mesh.subMeshCount = indices.Length;
		for(int i=0; i<indices.Length; i++) {
			mesh.SetTriangles( indices[i].ToArray(), i );
		}
		
		return mesh;
	}
		
	public OCMeshBuilder FromMesh(UnityEngine.Mesh mesh) {

		if(mesh == null) return this;
			
		_vertices.AddRange(mesh.vertices);
		_colors.AddRange(mesh.colors);
		_normals.AddRange(mesh.normals);
		_uv.AddRange(mesh.uv);
		
		for(int i = 0; i < mesh.subMeshCount; ++i)
		{
			AddIndices(0, mesh.GetTriangles(i));
		}
		
		return this;
	}		

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCMeshBuilder

}// namespace OpenCog




