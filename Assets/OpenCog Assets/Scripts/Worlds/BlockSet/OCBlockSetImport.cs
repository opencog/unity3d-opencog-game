
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
using System.Xml;
using System.IO;
using System.Text;
using System.Reflection;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using UnityEngine;
using OpenCog.BlockSet.BaseBlockSet;
using System.Linq;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.BlockSet
{

/// <summary>
/// The OpenCog OCBlockSetImport.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCBlockSetImport : OCMonoBehaviour
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

	public static void Import(OCBlockSet blockSet, string xml) {
		if(xml != null && xml.Length > 0) {
			XmlDocument document = new XmlDocument();
			document.LoadXml(xml);
			ReadBlockSet(blockSet, document);
		}
		foreach(BaseBlockSet.OCBlock block in blockSet.Blocks) {
			block.Init(blockSet);
		}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	private static void ReadBlockSet(OCBlockSet blockSet, XmlDocument document) {
		XmlNode blockSetNode = FindNodeByName(document, "OCBlockSet");

		OCAtlas[] atlases = ReadAtlasList(blockSetNode);
		blockSet.Atlases = atlases;
		
		BlockSet.BaseBlockSet.OCBlock[] blocks = ReadBlockList(blockSetNode);

		//Debug.Log("Are we setting a bunch of null blocks?");

		bool allNull = true;

		for (int i = 0; i < blocks.Length; i++)
		{
			if (blocks[i] != null)
				allNull = false;
		}

		if (allNull)
			Debug.Log("Yep, we're setting a bunch of null blocks!");
		else
			//Debug.Log("Nope, they're ok!");

		blockSet.Blocks = blocks;
	}
	
	private static OCAtlas[] ReadAtlasList(XmlNode blockSetNode) {
		XmlNode atlasListNode = FindNodeByName(blockSetNode, "OCAtlasList");
		List<OCAtlas> list = new List<OCAtlas>();
		foreach(XmlNode node in atlasListNode.ChildNodes) {
			OCAtlas atlas = ReadAtlas(node);
			list.Add(atlas);
		}
		return list.ToArray();
	}
	
	private static OCAtlas ReadAtlas(XmlNode node) {
		OCAtlas atlas = new OCAtlas();
		foreach(XmlNode fieldNode in node) {
			FieldInfo field = GetField(atlas.GetType(), fieldNode.Name);
			if(field.FieldType.IsSubclassOf(typeof(UnityEngine.Object))) {
				ReadResourceField(fieldNode, atlas);
			} else {
				ReadField(fieldNode, atlas);
			}
		}
		return atlas;
	}
	
	private static OpenCog.BlockSet.BaseBlockSet.OCBlock[] ReadBlockList(XmlNode blockSetNode) {
		XmlNode node = FindNodeByName(blockSetNode, "OCBlockList");
		List<BaseBlockSet.OCBlock> list = new List<BaseBlockSet.OCBlock>();
		foreach(XmlNode childNode in node.ChildNodes) {
			OpenCog.BlockSet.BaseBlockSet.OCBlock block = ReadBlock(childNode);
			list.Add(block);
		}
		return list.ToArray();
	}
	
	private static OCBlock ReadBlock(XmlNode node) {
		System.Type type = System.Type.GetType("OpenCog.BlockSet.BaseBlockSet." + node.Name);

//		if (type == null)
//		{
//			Debug.Log("Failed to get a type for block '" + node.Name + "', attempting lookup with unqualified name...");
//
//			type = System.Type.GetType(node.Name);
//
//			if (type == null)
//				Debug.Log("Nope, that failed too...type is still null...");
//			else
//				Debug.Log("Wahey, that actually worked!");
//		}
//
//
//
//
//		if (type != null)
//			Debug.Log("Reading block, name = " + node.Name + ", type = " + type.ToString());
//		else
//			Debug.Log("Reading block, name = " + node.Name + ", type == null");
			
		OCBlock block = (OCBlock) ScriptableObject.CreateInstance(type);
		foreach(XmlNode childNode in node) {
			ReadField(childNode, block);
		}
		return block;
	}

	private static XmlNode FindNodeByName(XmlNode node, string name) {
		foreach(XmlNode child in node) {
			if(child.Name.Equals(name)) return child;
		}
		return null;
	}
	
	private static FieldInfo GetField(System.Type type, string name) {
		FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
		if(field != null) return field;
		if(type != typeof(object)) return GetField(type.BaseType, name);
		return null;
	}
	
	private static void ReadField(XmlNode fieldNode, object obj) {
		FieldInfo field = GetField(obj.GetType(), fieldNode.Name);
		object val = Parse(field.FieldType, fieldNode.InnerText);
		field.SetValue(obj, val);
	}
	private static object Parse(System.Type type, string val) {
		if(type == typeof(bool)) return bool.Parse(val);
		if(type == typeof(byte)) return byte.Parse(val);
		if(type == typeof(short)) return short.Parse(val);
		if(type == typeof(int)) return int.Parse(val);
		if(type == typeof(long)) return long.Parse(val);
		if(type == typeof(float)) return float.Parse(val);
		if(type == typeof(double)) return double.Parse(val);
		if(type == typeof(string)) return val;
		if(type == typeof(UnityEngine.GameObject)) return Resources.FindObjectsOfTypeAll(typeof(GameObject)).Where(i => i.name == val).LastOrDefault();
		
		throw new System.Exception("Unsupported type: "+type.ToString());
	}
	
	private static void ReadResourceField(XmlNode fieldNode, object obj) {
		FieldInfo field = GetField(obj.GetType(), fieldNode.Name);
		UnityEngine.Object val = UnityEngine.Resources.Load(fieldNode.InnerText, field.FieldType);
		field.SetValue(obj, val);
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCBlockSetImport

}// namespace OpenCog




