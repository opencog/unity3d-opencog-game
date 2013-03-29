using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Text;
using System.Reflection;
using System;
using System.Collections.Generic;

public class BlockSetImport {
	
	public static void Import(BlockSet blockSet, string xml) {
		if(xml != null && xml.Length > 0) {
			XmlDocument document = new XmlDocument();
			document.LoadXml(xml);
			ReadBlockSet(blockSet, document);
		}
		foreach(Block block in blockSet.GetBlocks()) {
			block.Init(blockSet);
		}
	}
	
	private static void ReadBlockSet(BlockSet blockSet, XmlDocument document) {
		XmlNode blockSetNode = FindNodeByName(document, "BlockSet");
		
		Atlas[] atlases = ReadAtlasList(blockSetNode);
		blockSet.SetAtlases(atlases);
		
		Block[] blocks = ReadBlockList(blockSetNode);
		blockSet.SetBlocks(blocks);
	}
	
	private static Atlas[] ReadAtlasList(XmlNode blockSetNode) {
		XmlNode atlasListNode = FindNodeByName(blockSetNode, "AtlasList");
		List<Atlas> list = new List<Atlas>();
		foreach(XmlNode node in atlasListNode.ChildNodes) {
			Atlas atlas = ReadAtlas(node);
			list.Add(atlas);
		}
		return list.ToArray();
	}
	
	private static Atlas ReadAtlas(XmlNode node) {
		Atlas atlas = new Atlas();
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
	
	private static Block[] ReadBlockList(XmlNode blockSetNode) {
		XmlNode node = FindNodeByName(blockSetNode, "BlockList");
		List<Block> list = new List<Block>();
		foreach(XmlNode childNode in node.ChildNodes) {
			Block block = ReadBlock(childNode);
			list.Add(block);
		}
		return list.ToArray();
	}
	
	private static Block ReadBlock(XmlNode node) {
		Type type = Type.GetType(node.Name);
		Block block = (Block) System.Activator.CreateInstance(type);
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
	
	private static FieldInfo GetField(Type type, string name) {
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
	private static object Parse(Type type, string val) {
		if(type == typeof(bool)) return bool.Parse(val);
		if(type == typeof(byte)) return byte.Parse(val);
		if(type == typeof(short)) return short.Parse(val);
		if(type == typeof(int)) return int.Parse(val);
		if(type == typeof(long)) return long.Parse(val);
		if(type == typeof(float)) return float.Parse(val);
		if(type == typeof(double)) return double.Parse(val);
		if(type == typeof(string)) return val;
		
		throw new Exception("Unsupported type: "+type.ToString());
	}
	
	private static void ReadResourceField(XmlNode fieldNode, object obj) {
		FieldInfo field = GetField(obj.GetType(), fieldNode.Name);
		UnityEngine.Object val = Resources.Load(fieldNode.InnerText, field.FieldType);
		field.SetValue(obj, val);
	}
	
}
