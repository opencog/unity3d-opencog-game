using UnityEngine;
using System.Collections;

public class BlockSetChooser : MonoBehaviour {
	
	[SerializeField] private BlockSet[] blockSetList;
	private int index = 0;
	private Vector2 scrollPosition;
	
	private BlockSetViewer viewer;
	
	void Start() {
		viewer = GetComponent<BlockSetViewer>();
		viewer.SetBlockSet( blockSetList[index] );
	}
	
	void OnGUI() {
		int oldIndex = index;
		Rect position = new Rect(0, 0, 180, Screen.height);
		index = DrawList(position, index, blockSetList, ref scrollPosition);
		
		if(index != oldIndex) {
			viewer.SetBlockSet( blockSetList[index] );
		}
		
		GUILayout.BeginArea( new Rect(0, 0, Screen.width, Screen.height) );
		GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
				if( GUILayout.Button("Back") ) {
					Application.LoadLevel("MainMenu");
				}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private static int DrawList(Rect position, int selected, BlockSet[] list, ref Vector2 scrollPosition) {
		GUILayout.BeginArea(position, GUI.skin.box);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		for(int i=0; i<list.Length; i++) {
			if( DrawItem(list[i].name, i == selected) ) {
				selected = i;
				Event.current.Use();
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		return selected;
	}
	
	private static bool DrawItem(string name, bool selected) {
		Rect position = GUILayoutUtility.GetRect(new GUIContent(name), GUI.skin.box);
		if(selected) GUI.Box(position, GUIContent.none);
		
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.padding = GUI.skin.box.padding;
		style.alignment = TextAnchor.MiddleLeft;
		
		GUI.Label(position, name, style);
		
		return Event.current.type == EventType.MouseDown && Event.current.button == 0 && position.Contains(Event.current.mousePosition);
	}
	
}
