
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Text;
using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEditor;
using OpenCog.BlockSet;
using OpenCog.BlockSet.BaseBlockSet;

public class BlockSetExport {
	
	
	public static string Export(OCBlockSet blockSet) {
		XmlDocument document = new XmlDocument();
		WriteBlockSet(blockSet, document);
		
		StringWriter writer = new StringWriter(new StringBuilder());
        XmlTextWriter xmlWriter = new XmlTextWriter(writer);
		xmlWriter.Formatting = Formatting.Indented;
		
        document.Save( xmlWriter );
        return writer.ToString();
	}
	
	private static void WriteBlockSet(OCBlockSet blockSet, XmlDocument document) {
		XmlNode blockSetNode = document.CreateElement("OCBlockSet");
		document.AppendChild(blockSetNode);
		
		XmlNode atlasListNode = WriteAtlasList(blockSet.Atlases, document);
		blockSetNode.AppendChild(atlasListNode);
		
		XmlNode blockListNode = WriteBlockList(blockSet.Blocks, document);
		blockSetNode.AppendChild(blockListNode);
	}
	
	private static XmlNode WriteAtlasList(OCAtlas[] list, XmlDocument document) {
		XmlNode node = document.CreateElement("OCAtlasList");
		foreach(OCAtlas atlas in list) {
			XmlNode childNode = WriteAtlas(atlas, document);
			node.AppendChild(childNode);
		}
		return node;
	}
	
	private static XmlNode WriteAtlas(OCAtlas atlas, XmlDocument document) {
		XmlNode node = document.CreateElement("OCAtlas");
		FieldInfo[] fields = GetFields(atlas.GetType());
		foreach(FieldInfo field in fields) {
			if(field.FieldType.IsSubclassOf( typeof(UnityEngine.Object) )) {
				XmlNode childNode = WriteAssetField(field, atlas, document);
				node.AppendChild(childNode);
			} else {
				XmlNode childNode = WriteField(field, atlas, document);
				node.AppendChild(childNode);
			}
		}
		return node;
	}
	
	private static XmlNode WriteBlockList(OCBlock[] list, XmlDocument document) {
		XmlNode node = document.CreateElement("OCBlockList");
		foreach(OCBlock block in list) {
			XmlNode childNode = WriteBlock(block, document);
			node.AppendChild(childNode);
		}
		return node;
	}
	
	private static XmlNode WriteBlock(OCBlock block, XmlDocument document) {
		XmlNode node = document.CreateElement(block.GetType().Name);
		FieldInfo[] fields = GetFields(block.GetType());
		foreach(FieldInfo field in fields) {
			XmlNode childNode = WriteField(field, block, document);
			if(childNode != null) node.AppendChild(childNode);
		}
		return node;
	}
	
	private static FieldInfo[] GetFields(Type type) {
		List<FieldInfo> list = new List<FieldInfo>();
		GetFields(type, list);
		return list.ToArray();
	}
	private static void GetFields(Type type, List<FieldInfo> list) {
		if(type != typeof(object)) GetFields(type.BaseType, list);
		
		FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		foreach(FieldInfo field in fields) {
			if( field.IsPrivate && !Attribute.IsDefined(field, typeof(SerializeField)) ) continue;
			list.Add( field );
		}
	}
	
	private static XmlNode WriteField(FieldInfo field, object obj, XmlDocument document) {
		return WriteField(field.Name, field.GetValue(obj), document);
	}
	
	private static XmlNode WriteField(string name, object val, XmlDocument document) {
		XmlNode node = document.CreateElement(name);
		
		if(val != null)
		{
			if(val is GameObject)
			{
				node.InnerText = (val as GameObject).name;
			}
			else
			{
				node.InnerText = val.ToString();
			}
		}
		
		return node;
	}
	
	private static XmlNode WriteAssetField(FieldInfo field, object obj, XmlDocument document) {
		XmlNode node = document.CreateElement(field.Name);
		string path = AssetDatabase.GetAssetPath( (UnityEngine.Object)field.GetValue(obj) );
		int i = path.IndexOf("Resources/");
		if(i != -1) {
			i += "Resources/".Length;
			path = path.Substring(i, path.Length-i);
		}
		i = path.LastIndexOf('.');
		if(i != -1) path = path.Substring(0, i);
		
		node.InnerText = path;
		return node;
	}
	
}
#endif
