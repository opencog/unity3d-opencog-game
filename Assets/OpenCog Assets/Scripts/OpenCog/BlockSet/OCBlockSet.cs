
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

namespace OpenCog.BlockSet
{

/// <summary>
/// The OpenCog OCBlockSet.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
[UnityEngine.AddComponentMenu("VoxelEngine/BlockSet")]
[UnityEngine.ExecuteInEditMode]
public class OCBlockSet : OCScriptableObject
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	[UnityEngine.SerializeField] private string _data = "";
	private OCAtlas[] _atlases = new OCAtlas[0];
	private BaseBlockSet.OCBlock[] _blocks = new BaseBlockSet.OCBlock[0];
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		
	void OnEnable() {
		OCBlockSetImport.Import(this, _data);
	}

	public OCAtlas[] Atlases
	{
		get { return _atlases; }
		set { _atlases = value; }
	}

	public OpenCog.BlockSet.BaseBlockSet.OCBlock[] Blocks
	{
		get { return _blocks; }
		set {_blocks = value; }
	}

	public string Data
	{
		get { return _data; }
		set { _data = value; }
	}

	public int BlockCount
	{
		get { return _blocks.Length; }
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public OCAtlas GetAtlas(int i) {
		if(i<0 || i>=_atlases.Length) return null;
		return _atlases[i];
	}
	
	public UnityEngine.Material[] GetMaterials(int count) {
		UnityEngine.Material[] materials = new UnityEngine.Material[count];

		for(int i=0; i<count; i++)
		{
			materials[i] = _atlases[i].@Material;
		}

		return materials;
	}

	public OpenCog.BlockSet.BaseBlockSet.OCBlock GetBlock(int index) {
		if(index < 0 || index >= _blocks.Length)
			return null;

		return _blocks[index];
	}
	
	public OpenCog.BlockSet.BaseBlockSet.OCBlock GetBlock(string name) {
		foreach(OpenCog.BlockSet.BaseBlockSet.OCBlock block in _blocks) {
			if(block.GetName() == name) return block;
		}
		return null;
	}
	
	public T GetBlock<T>(string name) where T : OpenCog.BlockSet.BaseBlockSet.OCBlock {
		foreach(OpenCog.BlockSet.BaseBlockSet.OCBlock block in _blocks) {
			if(block.GetName() == name && block is T) return (T)block;
		}
		return null;
	}
	
	public OpenCog.BlockSet.BaseBlockSet.OCBlock[] GetBlocks(string name) {
		List<OpenCog.BlockSet.BaseBlockSet.OCBlock> list = new List<OpenCog.BlockSet.BaseBlockSet.OCBlock>();
		foreach(OpenCog.BlockSet.BaseBlockSet.OCBlock block in _blocks) {
			if(block.GetName() == name) list.Add(block);
		}
		return list.ToArray();
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

}// class OCBlockSet

}// namespace OpenCog




