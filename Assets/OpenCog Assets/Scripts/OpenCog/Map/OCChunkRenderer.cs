
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

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Map
{

/// <summary>
/// The OpenCog OCChunkRenderer.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCChunkRenderer : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
		
	private OpenCog.BlockSet.OCBlockSet _blockSet;
	private OCChunk _chunk;

	private bool _dirty = false, _lightDirty = false;

	private UnityEngine.MeshFilter _filter;

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

	/// <summary>
	/// Called when the script instance is being loaded.
	/// </summary>
	public void Awake()
	{
		Initialize();
		OCLogger.Fine(gameObject.name + " is awake.");
	}

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start()
	{
		OCLogger.Fine(gameObject.name + " is started.");
	}

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	public void Update()
	{
		if(dirty) {
			Build();
			dirty = lightDirty = false;
		}
		if(lightDirty) {
			BuildLighting();
			lightDirty = false;
		}

		OCLogger.Fine(gameObject.name + " is updated.");	
	}
		
	/// <summary>
	/// Reset this instance to its default values.
	/// </summary>
	public void Reset()
	{
		Uninitialize();
		Initialize();
		OCLogger.Fine(gameObject.name + " is reset.");	
	}

	/// <summary>
	/// Raises the enable event when OCChunkRenderer is loaded.
	/// </summary>
	public void OnEnable()
	{
		OCLogger.Fine(gameObject.name + " is enabled.");
	}

	/// <summary>
	/// Raises the disable event when OCChunkRenderer goes out of scope.
	/// </summary>
	public void OnDisable()
	{
		OCLogger.Fine(gameObject.name + " is disabled.");
	}

	/// <summary>
	/// Raises the destroy event when OCChunkRenderer is about to be destroyed.
	/// </summary>
	public void OnDestroy()
	{
		Uninitialize();
		OCLogger.Fine(gameObject.name + " is about to be destroyed.");
	}

	public static OCChunkRenderer CreateChunkRenderer(Vector3i pos, OpenCog.Map.OCMap map, OCChunk chunk) {
		GameObject go = new GameObject("("+pos.x+" "+pos.y+" "+pos.z+")", typeof(MeshFilter), typeof(MeshRenderer), typeof(OCChunkRenderer));
		go.transform.parent = map.transform;
		go.transform.localPosition = new Vector3(pos.x*OCChunk.SIZE_X, pos.y*OCChunk.SIZE_Y, pos.z*OCChunk.SIZE_Z);
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
		
		OCChunkRenderer chunkRenderer = go.GetComponent<OCChunkRenderer>();
		chunkRenderer.blockSet = map.GetBlockSet();
		chunkRenderer.chunk = chunk;

		go.renderer.castShadows = false;
		go.renderer.receiveShadows = false;
		
		return chunkRenderer;
	}

	public void SetDirty() {
		dirty = true;
	}
	public void SetLightDirty() {
		lightDirty = true;
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	/// <summary>
	/// Initializes this instance.  Set default values here.
	/// </summary>
	private void Initialize()
	{
		filter = GetComponent<MeshFilter>();
	}
	
	/// <summary>
	/// Uninitializes this instance.  Cleanup refernces here.
	/// </summary>
	private void Uninitialize()
	{

	}

	private void Build() {
		filter.sharedMesh = ChunkBuilder.BuildChunk(filter.sharedMesh, chunk);

		if(filter.sharedMesh == null) {
			Destroy(gameObject);
			return;
		}

		renderer.sharedMaterials = blockSet.GetMaterials(filter.sharedMesh.subMeshCount);
	}

	private void BuildLighting() {
		if(filter.sharedMesh != null) {
			ChunkBuilder.BuildChunkLighting(filter.sharedMesh, chunk);
		}
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	/// <summary>
	/// Initializes a new instance of the <see cref="OpenCog.OCChunkRenderer"/> class.  
	/// Generally, intitialization should occur in the Start or Awake
	/// functions, not here.
	/// </summary>
	public OCChunkRenderer()
	{
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCChunkRenderer

}// namespace OpenCog




