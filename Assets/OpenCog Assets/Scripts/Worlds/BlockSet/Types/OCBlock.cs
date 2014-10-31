
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

namespace OpenCog.BlockSet.BaseBlockSet
{

/// <summary>
/// The OpenCog OCBlock.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public abstract class OCBlock : OCScriptableObject
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	[UnityEngine.SerializeField] private string _name;
	[UnityEngine.SerializeField] private int _atlasID;
	[UnityEngine.SerializeField] private int _light;

	private OCAtlas _atlas;
	private bool _alpha = false;
			
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

	public virtual void Init(OCBlockSet blockSet) {
		_atlas = blockSet.GetAtlas(_atlasID);
		if(_atlas != null) _alpha = _atlas.IsAlpha;
	}
	
	public UnityEngine.Rect ToRect(int pos) {
		if(_atlas != null) return _atlas.ToRect(pos);
		return default(UnityEngine.Rect);
	}

	public bool DrawPreview(UnityEngine.Rect position) {
		UnityEngine.Texture texture = GetTexture();
		if(texture != null) UnityEngine.GUI.DrawTextureWithTexCoords(position, texture, GetPreviewFace());
		return UnityEngine.Event.current.type == UnityEngine.EventType.MouseDown && position.Contains(UnityEngine.Event.current.mousePosition);
	}
	public abstract UnityEngine.Rect GetPreviewFace();
	
	public abstract void Build(Vector3i localPos, Vector3i worldPos, OpenCog.Map.OCMap map, OpenCog.Builder.OCMeshBuilder mesh, bool onlyLight);
	
	public abstract OpenCog.Builder.OCMeshBuilder Build();
	
	public void SetName(string name) {
		this._name = name;
	}
	public string GetName() {
		return _name;
	}

	public int AtlasID
	{
		get { return _atlasID; }
		set { _atlasID = value; }
	}

	public OCAtlas Atlas
		{
			get { return _atlas; }
		}


	public UnityEngine.Texture GetTexture() {
		if(_atlas != null)
				return _atlas.@Texture;
		return null;
	}

	public void SetLight(int light) {
		_light = UnityEngine.Mathf.Clamp(light, 0, 15);
	}
	public byte GetLight() {
		return (byte) _light;
	}
	
	public abstract bool IsSolid();
	
	public bool IsAlpha() {
		return _alpha;
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	protected UnityEngine.Vector2[] ToTexCoords(int pos) {
		UnityEngine.Rect rect = ToRect(pos);
		return new UnityEngine.Vector2[] {
			new UnityEngine.Vector2(rect.xMax, rect.yMin),
			new UnityEngine.Vector2(rect.xMax, rect.yMax),
			new UnityEngine.Vector2(rect.xMin, rect.yMax),
			new UnityEngine.Vector2(rect.xMin, rect.yMin),
		};
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCBlock

}// namespace OpenCog




