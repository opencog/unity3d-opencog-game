using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Text;
using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEditor;

public class BlockSetExport {
	
	
	public static string Export(BlockSet blockSet) {
		XmlDocument document = new XmlDocument();
		WriteBlockSet(blockSet, document);
		
		StringWriter writer = new StringWriter(new StringBuilder());
        XmlTextWriter xmlWriter = new XmlTextWriter(writer);
		xmlWriter.Formatting = Formatting.Indented;
		
        document.Save( xmlWriter );
        return writer.ToString();
	}
	
	private static void WriteBlockSet(BlockSet blockSet, XmlDocument document) {
		XmlNode blockSetNode = document.CreateElement("BlockSet");
		document.AppendChild(blockSetNode);
		
		XmlNode atlasListNode = WriteAtlasList(blockSet.GetAtlases(), document);
		blockSetNode.AppendChild(atlasListNode);
		
		XmlNode blockListNode = WriteBlockList(blockSet.GetBlocks(), document);
		blockSetNode.AppendChild(blockListNode);
	}
	
	private static XmlNode WriteAtlasList(Atlas[] list, XmlDocument document) {
		XmlNode node = document.CreateElement("AtlasList");
		foreach(Atlas atlas in list) {
			XmlNode childNode = WriteAtlas(atlas, document);
			node.AppendChild(childNode);
		}
		return node;
	}
	
	private static XmlNode WriteAtlas(Atlas atlas, XmlDocument document) {
		XmlNode node = document.CreateElement("Atlas");
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
	
	private static XmlNode WriteBlockList(Block[] list, XmlDocument document) {
		XmlNode node = document.CreateElement("BlockList");
		foreach(Block block in list) {
			XmlNode childNode = WriteBlock(block, document);
			node.AppendChild(childNode);
		}
		return node;
	}
	
	private static XmlNode WriteBlock(Block block, XmlDocument document) {
		XmlNode node = document.CreateElement(block.GetType().ToString());
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
		node.InnerText = val.ToString();
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
