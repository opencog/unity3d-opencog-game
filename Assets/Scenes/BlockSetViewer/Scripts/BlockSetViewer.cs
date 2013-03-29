using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockSetViewer : MonoBehaviour {
	
	private BlockSet blockSet;
	private int index = 0;
	private Vector2 scrollPosition;
	
	public void SetBlockSet(BlockSet blockSet) {
		this.blockSet = blockSet;
		index = Mathf.Clamp(index, 0, blockSet.GetBlockCount());
		BuildBlock( blockSet.GetBlock(index) );
	}
	
	private void BuildBlock(Block block) {
		renderer.material = block.GetAtlas().GetMaterial();
		MeshFilter filter = GetComponent<MeshFilter>();
		block.Build().ToMesh(filter.mesh);
	}
	
	
	void OnGUI() {
		Rect rect = new Rect(Screen.width-180, 0, 180, Screen.height);
		int oldIndex = index;
		index = DrawList(rect, index, blockSet.GetBlocks(), ref scrollPosition);
		if(oldIndex != index) {
			BuildBlock( blockSet.GetBlock(index) );
		}
	}
	
	private static int DrawList(Rect position, int selected, Block[] list, ref Vector2 scrollPosition) {
		GUILayout.BeginArea(position, GUI.skin.box);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		for(int i=0; i<list.Length; i++) {
			if(list[i] == null) continue;
			if( DrawItem(list[i], i == selected) ) {
				selected = i;
				Event.current.Use();
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		return selected;
	}
	
	private static bool DrawItem(Block block, bool selected) {
		Rect position = GUILayoutUtility.GetRect(0, 40, GUILayout.ExpandWidth(true));
		if(selected) GUI.Box(position, GUIContent.none);
		
		GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
		labelStyle.alignment = TextAnchor.MiddleLeft;
		
		Rect texRect = new Rect(position.x+4, position.y+4, position.height-8, position.height-8);
		block.DrawPreview(texRect);
		
		Rect labelRect = position;
		labelRect.xMin = texRect.xMax+4;
		GUI.Label(labelRect, block.GetName(), labelStyle);
		
		return Event.current.type == EventType.MouseDown && Event.current.button == 0 && position.Contains(Event.current.mousePosition);
	}
	
}
