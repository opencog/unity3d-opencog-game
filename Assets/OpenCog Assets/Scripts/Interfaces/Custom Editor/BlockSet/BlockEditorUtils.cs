using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System;

public class BlockEditorUtils {
	
	public static Color SELECT_COLOR {
		get {
			return new Color( 61/255f, 128/255f, 223/255f );
		}
	}
	
	public static void DrawRect(Rect rect, Color color) {

		#if UNITY_EDITOR
		Vector3 a = new Vector3(rect.xMin, rect.yMin, 0);
		Vector3 b = new Vector3(rect.xMax, rect.yMin, 0);
		Vector3 c = new Vector3(rect.xMax, rect.yMax, 0);
		Vector3 d = new Vector3(rect.xMin, rect.yMax, 0);


		Handles.color = color;
		Handles.DrawLine(a, b);
		Handles.DrawLine(b, c);
		Handles.DrawLine(c, d);
		Handles.DrawLine(d, a);

		#endif
	}
	
	public static void FillRect(Rect rect, Color color) {

		#if UNITY_EDITOR
		Color oldColor = GUI.color;
		GUI.color = color;

		GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
		GUI.color = oldColor;

		#endif
	}
	
	public static int Popup(string label, int selected, object[] items) {
		string[] strings = new string[items.Length];
		for(int i=0; i<items.Length; i++) {
			if(items[i] != null) strings[i] = items[i].ToString();
		}
		#if UNITY_EDITOR
		return EditorGUILayout.Popup(label, selected, strings);
		#else 
		return 0;
		#endif
	}
	
	public static Enum Toolbar(Enum selected) {
		string[] names = Enum.GetNames(selected.GetType());
		int index = Array.IndexOf<string>(names, Enum.GetName(selected.GetType(), selected));
		index = GUILayout.Toolbar(index, names);
		return (Enum) Enum.Parse(selected.GetType(), names[index]);
	}
	
	public static int DrawList(int selected, IList list) {
		float labelHeight = GUI.skin.label.CalcHeight( GUIContent.none, 0 );
		Rect rect = GUILayoutUtility.GetRect(0, labelHeight*list.Count, GUILayout.ExpandWidth(true));
		Rect[] rects = GUIUtils.Separate(rect, 1, list.Count);
		for(int i=0; i<list.Count; i++) {
			Rect position = rects[i];
			object item = list[i];
			
			if(i == selected) FillRect(position, SELECT_COLOR);
			string name = item != null ? item.ToString() : "Null";
			GUI.Label(position, name);
		}
		
		
		GUI.BeginGroup(rect);
		if(Event.current.type == EventType.MouseDown) {
			float mouseY = Event.current.mousePosition.y;
			selected = Mathf.FloorToInt( mouseY / labelHeight );
			if(selected < 0 || selected >= list.Count) selected = -1;
			Event.current.Use();
		}
		GUI.EndGroup();
		
		return selected;
	}
	
	public static T GetStateObject<T>(int controlID) {
		return (T) GUIUtility.GetStateObject(typeof(T), controlID);
	}
	
	
}


public class Container<T> where T : struct {
	public T value;
	
	public Container() {
	}
	public Container(T value) {
		this.value = value;
	}
		
	public static implicit operator T(Container<T> c) {
		return c.value;
	}
}
