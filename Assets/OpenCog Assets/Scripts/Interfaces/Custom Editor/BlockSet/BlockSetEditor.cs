#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System;
using OpenCog.BlockSet;
using OpenCog.BlockSet.BaseBlockSet;

[CustomEditor(typeof(OCBlockSet))]
public class BlockSetEditor : Editor {
	
	private enum Mode {
		AtlasSet, BlockSet, XML
	}
	
	private Type[] blockTypes;
	private Mode mode = Mode.BlockSet;

	private OCBlockSet blockSet;
	
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
		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<OCBlockSet>(), path+"NewBlockSet.asset");
	}
	
	void OnEnable() {
		blockSet = (OCBlockSet)target;

//		bool allBlocksNull = true;
//
//		for (int i = 0; i < blockSet.Blocks.Length; i++)
//		{
//			if (blockSet.Blocks[i] != null)
//				allBlocksNull = false;
//		}
//
//		if (allBlocksNull)
//			OCBlockSetImport.Import(blockSet, blockSet.Data);

		Type[] types = Assembly.GetAssembly(typeof(OCBlock)).GetTypes();
		List<Type> list = new List<Type>();
		foreach(Type type in types) {
			if(type.IsSubclassOf(typeof(OCBlock))) list.Add(type);
		}

		blockTypes = list.ToArray();
	}
	
	
	public override void OnInspectorGUI() {
		EditorGUIUtility.LookLikeControls();
		
		Mode oldMode = mode;
		mode = (Mode)BlockEditorUtils.Toolbar(mode);
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
			if(oldMode != mode) xml = blockSet.Data;
			
			xmlScrollPosition = GUILayout.BeginScrollView(xmlScrollPosition);
				GUIStyle style = new GUIStyle(GUI.skin.box);
				style.alignment = TextAnchor.UpperLeft;
				xml = EditorGUILayout.TextArea(xml, GUILayout.ExpandWidth(true));
				blockSet.Data = xml;
			GUILayout.EndScrollView();
			
			if(GUILayout.Button("Import")) {
				OCBlockSetImport.Import(blockSet, blockSet.Data);
				GUI.changed = true;
			}
		}
		
		if(GUI.changed) {
			string data = BlockSetExport.Export(blockSet);
			blockSet.Data = data;
			EditorUtility.SetDirty(blockSet);
		}
	}
	
	private static void DrawAtlasesList( OCBlockSet blockSet ) {
		OCAtlas[] list = blockSet.Atlases;
		GUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true));
		{
			selectedAtlas = BlockEditorUtils.DrawList(selectedAtlas, list);
			EditorGUILayout.Separator();
		
			GUILayout.BeginHorizontal();
				if(GUILayout.Button("Add")) {
					ArrayUtility.Add<OCAtlas>(ref list, new OCAtlas());
					selectedAtlas = list.Length - 1;
					GUI.changed = true;
				}
				if(GUILayout.Button("Remove") && selectedAtlas != -1) {
					Undo.RecordObject (blockSet, "Remove atlas");
					ArrayUtility.RemoveAt<OCAtlas>(ref list, selectedAtlas);
					selectedAtlas = Mathf.Clamp(selectedAtlas, 0, list.Length - 1);
					GUI.changed = true;
				}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		blockSet.Atlases = list;
	}
	
	private static void DrawAtlasEditor(OCAtlas atlas) {
		GUILayout.BeginVertical(GUI.skin.box);
			Material material = (Material) EditorGUILayout.ObjectField("Material", atlas.Material, typeof(Material), true);
			atlas.Material = material;
				
			int w = EditorGUILayout.IntField("Width", atlas.Width);
			if(w < 1) w = 1; 
			atlas.Width = w;
				
			int h = EditorGUILayout.IntField("Height", atlas.Height);
			if(h < 1) h = 1; 
			atlas.Height = h;
				
			bool alpha = EditorGUILayout.Toggle("Alpha", atlas.IsAlpha);
			atlas.IsAlpha = alpha;
		GUILayout.EndVertical();
	}
	
	private void DrawBlockSet(OCBlockSet blockSet) {
		GUILayout.BeginVertical(GUI.skin.box);
		
		int oldSelectedBlock = selectedBlock;

		// Next line pushes the blockSet to BlockSetViewer
		selectedBlock = OpenCog.BlockSetViewer.SelectionGrid(blockSet, selectedBlock, GUILayout.MinHeight(200), GUILayout.MaxHeight(300));
		if(selectedBlock != oldSelectedBlock) GUIUtility.keyboardControl = 0;
		
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
		foreach(Type type in blockTypes) {
			string name = type.Name;

			if(name.EndsWith("Block")) name = name.Substring(0, name.Length-5);
			if(GUILayout.Button(name)) {
				OCBlock newBlock = (OCBlock) CreateInstance(type);
				newBlock.SetName("New "+type.ToString());
				newBlock.Init(blockSet);
				
				OCBlock[] blocks = blockSet.Blocks;
				ArrayUtility.Add<OCBlock>(ref blocks, newBlock);
				blockSet.Blocks = blocks;
				selectedBlock = blocks.Length-1;
				EditorGUIUtility.keyboardControl = 0;
				GUI.changed = true;
			}
		}
		GUILayout.EndHorizontal();
		
		if( GUILayout.Button("Remove") ) {
			OCBlock[] blocks = blockSet.Blocks;
			ArrayUtility.RemoveAt<OCBlock>(ref blocks, selectedBlock);
			blockSet.Blocks = blocks;
			selectedBlock = Mathf.Clamp(selectedBlock, 0, blocks.Length-1);
			GUI.changed = true;
		}
		
		GUILayout.EndVertical();
	}
	
}
#endif
