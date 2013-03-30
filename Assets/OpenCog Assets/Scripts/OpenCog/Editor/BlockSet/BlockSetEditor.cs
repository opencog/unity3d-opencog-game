using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(BlockSet))]
public class BlockSetEditor : Editor {
	
	private enum Mode {
		AtlasSet, BlockSet, XML
	}
	
	private Type[] blockTypes;
	private Mode mode = Mode.BlockSet;

	private BlockSet blockSet;
	
	private static int selectedAtlas = -1;
	private static int selectedBlock = -1;
	
	private Vector2 xmlScrollPosition;
	private string xml;
	
	[MenuItem ("Assets/Create/VoxelEngine/BlockSet")]
	private static void CreateBlockSet() {
		string path = "Assets/";
		if(Selection.activeObject != null) {
			path = AssetDatabase.GetAssetPath(Selection.activeObject)+"/";
		}
		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<BlockSet>(), path+"NewBlockSet.asset");
	}
	
	void OnEnable() {
		blockSet = (BlockSet)target;
		
		Type[] types = Assembly.GetAssembly(typeof(Block)).GetTypes();
		List<Type> list = new List<Type>();
		foreach(Type type in types) {
			if(type.IsSubclassOf(typeof(Block))) list.Add(type);
		}
		blockTypes = list.ToArray();
		
	}
	
	
	public override void OnInspectorGUI() {
		EditorGUIUtility.LookLikeControls();
		
		Mode oldMode = mode;
		mode = (Mode)EditorGUIUtils.Toolbar(mode);
		EditorGUILayout.Separator();
		if(mode != oldMode) EditorGUIUtility.keyboardControl = 0;
		
		if(mode == Mode.AtlasSet) {
			DrawAtlasesList( blockSet );
			if( blockSet.GetAtlas(selectedAtlas) != null ) {
				DrawAtlasEditor( blockSet.GetAtlas(selectedAtlas) );
			}
		}
		if(mode == Mode.BlockSet) {
			DrawBlockSet( blockSet );
			EditorGUILayout.Separator();
		
			if( blockSet.GetBlock(selectedBlock) != null ) {
				BlockEditor.DrawBlockEditor( blockSet.GetBlock(selectedBlock), blockSet );
			}
		}
		if(mode == Mode.XML) {
			if(oldMode != mode) xml = blockSet.GetData();
			
			xmlScrollPosition = GUILayout.BeginScrollView(xmlScrollPosition);
				GUIStyle style = new GUIStyle(GUI.skin.box);
				style.alignment = TextAnchor.UpperLeft;
				xml = EditorGUILayout.TextArea(xml, GUILayout.ExpandWidth(true));
				blockSet.SetData(xml);
			GUILayout.EndScrollView();
			
			if(GUILayout.Button("Import")) {
				BlockSetImport.Import(blockSet, blockSet.GetData());
				GUI.changed = true;
			}
		}
		
		if(GUI.changed) {
			string data = BlockSetExport.Export(blockSet);
			blockSet.SetData(data);
			EditorUtility.SetDirty(blockSet);
		}
	}
	
	private static void DrawAtlasesList( BlockSet blockSet ) {
		Atlas[] list = blockSet.GetAtlases();
		GUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true));
		{
			selectedAtlas = EditorGUIUtils.DrawList(selectedAtlas, list);
			EditorGUILayout.Separator();
		
			GUILayout.BeginHorizontal();
				if(GUILayout.Button("Add")) {
					ArrayUtility.Add<Atlas>(ref list, new Atlas());
					selectedAtlas = list.Length - 1;
					GUI.changed = true;
				}
				if(GUILayout.Button("Remove") && selectedAtlas != -1) {
					Undo.RegisterUndo(blockSet, "Remove atlas");
					ArrayUtility.RemoveAt<Atlas>(ref list, selectedAtlas);
					selectedAtlas = Mathf.Clamp(selectedAtlas, 0, list.Length - 1);
					GUI.changed = true;
				}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		blockSet.SetAtlases(list);
	}
	
	private static void DrawAtlasEditor(Atlas atlas) {
		GUILayout.BeginVertical(GUI.skin.box);
			Material material = (Material) EditorGUILayout.ObjectField("Material", atlas.GetMaterial(), typeof(Material), true);
			atlas.SetMaterial(material);
				
			int w = EditorGUILayout.IntField("Width", atlas.GetWidth());
			if(w < 1) w = 1; 
			atlas.SetWidth(w);
				
			int h = EditorGUILayout.IntField("Height", atlas.GetHeight());
			if(h < 1) h = 1; 
			atlas.SetHeight(h);
				
			bool alpha = EditorGUILayout.Toggle("Alpha", atlas.IsAlpha());
			atlas.SetAlpha(alpha);
		GUILayout.EndVertical();
	}
	
	private void DrawBlockSet(BlockSet blockSet) {
		GUILayout.BeginVertical(GUI.skin.box); 
		
		int oldSelectedBlock = selectedBlock;
		selectedBlock = OpenCog.BlockSetViewer.SelectionGrid(blockSet, selectedBlock, GUILayout.MinHeight(200), GUILayout.MaxHeight(300));
		if(selectedBlock != oldSelectedBlock) GUIUtility.keyboardControl = 0;
		
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
		foreach(Type type in blockTypes) {
			string name = type.ToString();
			if(name.EndsWith("Block")) name = name.Substring(0, name.Length-5);
			if(GUILayout.Button(name)) {
				Block newBlock = (Block) System.Activator.CreateInstance(type);
				newBlock.SetName("New "+type.ToString());
				newBlock.Init(blockSet);
				
				Block[] blocks = blockSet.GetBlocks();
				ArrayUtility.Add<Block>(ref blocks, newBlock);
				blockSet.SetBlocks(blocks);
				selectedBlock = blocks.Length-1;
				EditorGUIUtility.keyboardControl = 0;
				GUI.changed = true;
			}
		}
		GUILayout.EndHorizontal();
		
		if( GUILayout.Button("Remove") ) {
			Block[] blocks = blockSet.GetBlocks();
			ArrayUtility.RemoveAt<Block>(ref blocks, selectedBlock);
			blockSet.SetBlocks(blocks);
			selectedBlock = Mathf.Clamp(selectedBlock, 0, blocks.Length-1);
			GUI.changed = true;
		}
		
		GUILayout.EndVertical();
	}
	
}
