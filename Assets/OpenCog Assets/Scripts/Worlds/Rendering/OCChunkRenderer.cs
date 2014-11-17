
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
using OpenCog.Utilities.Logging;

#region Usings, Namespaces, and Pragmas

using System.Collections;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using OpenCog.Utility;

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
	private OpenCog.Map.OCChunk _chunk;

	private bool _dirty = false, _lightDirty = false;

	private UnityEngine.MeshFilter _filter;

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------

		public OpenCog.BlockSet.OCBlockSet BlockSet
		{
			get { return _blockSet; }
			set { _blockSet = value; }
		}

		public OpenCog.Map.OCChunk Chunk
		{
			get { return _chunk; }
			set { _chunk = value; }
		}
		
		public bool IsDirty
		{
			get { return _dirty; }
			set { _dirty = true; }
		}

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
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is awake.");
	}

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start()
	{
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is started.");
	}

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	public void Update()
	{
		if(_dirty) {
			Build();
			_dirty = _lightDirty = false;
		}
		if(_lightDirty) {
			BuildLighting();
			_lightDirty = false;
		}

		//System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is updated.");	
	}
		
	/// <summary>
	/// Reset this instance to its default values.
	/// </summary>
	public void Reset()
	{
		Uninitialize();
		Initialize();
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is reset.");	
	}

	/// <summary>
	/// Raises the enable event when OCChunkRenderer is loaded.
	/// </summary>
	public void OnEnable()
	{
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is enabled.");
	}

	/// <summary>
	/// Raises the disable event when OCChunkRenderer goes out of scope.
	/// </summary>
	public void OnDisable()
	{
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is disabled.");
	}

	/// <summary>
	/// Raises the destroy event when OCChunkRenderer is about to be destroyed.
	/// </summary>
	public void OnDestroy()
	{
		Uninitialize();
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is about to be destroyed.");
	}

	public static OCChunkRenderer CreateChunkRenderer(Vector3i pos, OpenCog.Map.OCMap map, OCChunk chunk) {
		UnityEngine.GameObject go = new UnityEngine.GameObject("("+pos.x+" "+pos.y+" "+pos.z+")", typeof(UnityEngine.MeshFilter), typeof(UnityEngine.MeshRenderer), typeof(OpenCog.Map.OCChunkRenderer));
		go.transform.parent = map.transform;
		go.transform.localPosition = new UnityEngine.Vector3(pos.x*OpenCog.Map.OCChunk.SIZE_X, pos.y*OpenCog.Map.OCChunk.SIZE_Y, pos.z*OpenCog.Map.OCChunk.SIZE_Z);
		go.transform.localRotation = UnityEngine.Quaternion.identity;
		go.transform.localScale = UnityEngine.Vector3.one;
		
		OCChunkRenderer chunkRenderer = go.GetComponent<OCChunkRenderer>();
		chunkRenderer.BlockSet = map.GetBlockSet();
		chunkRenderer.Chunk = chunk;

		go.renderer.castShadows = false;
		go.renderer.receiveShadows = false;
		
		return chunkRenderer;
	}

	public void SetDirty() {
		if (!_dirty)
		{
			_dirty = true;
			//UnityEngine.Debug.Log ("I just made the chunk at [" + _chunk.GetPosition().x + ", " + _chunk.GetPosition().y + ", " + _chunk.GetPosition().z + "] dirty.");	
		}
	}
	public void SetLightDirty() {
		_lightDirty = true;
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
		_filter = GetComponent<UnityEngine.MeshFilter>();
	}
	
	/// <summary>
	/// Uninitializes this instance.  Cleanup refernces here.
	/// </summary>
	private void Uninitialize()
	{

	}

	private void Build() {
		_filter.sharedMesh = OpenCog.Builder.OCChunkBuilder.BuildChunk(_filter.sharedMesh, _chunk);

		if(_filter.sharedMesh == null) {
			Destroy(gameObject);
			return;
		}

		renderer.sharedMaterials = _blockSet.GetMaterials(_filter.sharedMesh.subMeshCount);
	}

	private void BuildLighting() {
		if(_filter.sharedMesh != null) {
			OpenCog.Builder.OCChunkBuilder.BuildChunkLighting(_filter.sharedMesh, _chunk);
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




