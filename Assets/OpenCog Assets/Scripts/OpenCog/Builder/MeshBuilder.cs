using UnityEngine;
using System.Collections.Generic;

public class MeshBuilder {
	
	private List<Vector3> vertices = new List<Vector3>();
	private List<Vector2> uv = new List<Vector2>();
	private List<Vector3> normals = new List<Vector3>();
	private List<Color> colors = new List<Color>();
	private List<int>[] indices = new List<int>[0];
	
	
	
	public void AddVertices(Vector3[] vertices, Vector3 offset) {
		foreach(Vector3 v in vertices) {
			this.vertices.Add( v + offset );
		}
	}
	
	public void AddNormals(Vector3[] normals) {
		this.normals.AddRange(normals);
	}
	
	public void AddColor(Color color) {
		colors.Add(color);
	}
	
	public void AddFaceColor(Color color) {
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}
	
	public void AddColors(Color color, int count) {
		for(int i=0; i<count; i++) {
			colors.Add(color);
		}
	}
	
	public void AddTexCoords(Vector2[] uv) {
		this.uv.AddRange(uv);
	}
	
	public void AddFaceIndices(int materialIndex) {
		int offset = vertices.Count;
		List<int> indices = GetIndices(materialIndex);
		indices.Add( offset+2 );
		indices.Add( offset+1 );
		indices.Add( offset+0 );
		
		indices.Add( offset+3 );
		indices.Add( offset+2 );
		indices.Add( offset+0 );
	}
	
	public void AddIndices(int materialIndex, int[] indices) {
		int offset = vertices.Count;
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
	
	public List<Color> GetColors() {
		return colors;
	}
	
	public void Clear() {
		vertices.Clear();
		uv.Clear();
		normals.Clear();
		colors.Clear();
		foreach(List<int> list in indices) {
			list.Clear();
		}
	}
	
	public Mesh ToMesh(Mesh mesh) {
		if(vertices.Count == 0) {
			GameObject.Destroy(mesh);
			return null;
		}
		
		if(mesh == null) mesh = new Mesh();
		
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.colors = colors.ToArray();
		mesh.normals = normals.ToArray();
		mesh.uv = uv.ToArray();
		
		mesh.subMeshCount = indices.Length;
		for(int i=0; i<indices.Length; i++) {
			mesh.SetTriangles( indices[i].ToArray(), i );
		}
		
		return mesh;
	}
	
}
