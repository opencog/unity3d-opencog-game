using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;

public class BlockEditor {
	
	private static Matrix4x4 atlasMatrix = Matrix4x4.identity;
	private static int selectedFace = 0;

	public static void DrawBlockEditor(Block block, BlockSet blockSet) {
		GUILayout.BeginVertical(GUI.skin.box);
		{
			string name = EditorGUILayout.TextField("Name", block.GetName());
			block.SetName( FixNameString(name) );
			
			int atlas = EditorGUIUtils.Popup( "Atlas", block.GetAtlasID(), blockSet.GetAtlases() );
			block.SetAtlasID(atlas);
		
			int light = EditorGUILayout.IntField("Light", block.GetLight());
			block.SetLight(light);
		}
		GUILayout.EndVertical();
		
		Texture texture = block.GetTexture();
		if(texture != null) {
			FieldInfo field = DrawFacesList(block, texture);
			int face = (int)field.GetValue(block);
			DrawFaceEditor(ref face, block.GetAtlas(), ref atlasMatrix);
			field.SetValue(block, face);
		}
	}
	
	private static FieldInfo DrawFacesList(Block block, Texture texture) {
		List<FieldInfo> fields = GetFaces(block);
		Rect[] faces = new Rect[fields.Count];
		string[] names = new string[fields.Count];
		for(int i=0; i<fields.Count; i++) {
			int pos = (int) fields[i].GetValue( block );
			faces[i] = block.ToRect(pos);
			names[i] = FixNameString(fields[i].Name);
		}
		
		selectedFace = Mathf.Clamp(selectedFace, 0, fields.Count-1);
		selectedFace = DrawFacesList(texture, faces, names, selectedFace);
		return fields[selectedFace];
	}
	
	private static int DrawFacesList(Texture texture, Rect[] faces, string[] names, int selected) {
		GUILayout.BeginHorizontal(GUI.skin.box);
		GUILayout.FlexibleSpace();
		{
			GUILayout.BeginVertical();
			{
				Rect rect = GUILayoutUtility.GetAspectRect( faces.Length, GUILayout.MaxWidth(64*faces.Length) );
				rect.width /= faces.Length;
				for(int i=0; i<faces.Length; i++) {
					GUI.DrawTextureWithTexCoords(rect, texture, faces[i]);
					if(Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)) {
						selected = i;
						Event.current.Use();
					}
					rect.x += rect.width;
				}
				if(names.Length > 1) {
					GUILayout.Space(4);
					Rect toolbarRect = GUILayoutUtility.GetRect(0, GUI.skin.button.CalcHeight(GUIContent.none, 0), GUILayout.MaxWidth(64*faces.Length));
					selected = GUI.Toolbar(toolbarRect, selected, names);
				}
			}
			GUILayout.EndVertical();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		return selected;
	}
	
	private static List<FieldInfo> GetFaces(Block block) {
		FieldInfo[] fields = block.GetType().GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
		List<FieldInfo> list = new List<FieldInfo>();
		foreach(FieldInfo field in fields) {
			if(field.FieldType == typeof(int)) list.Add(field);
		}
		return list;
	}
	
	private static string FixNameString(string name) {
		if(name.Length > 0) {
			return char.ToUpper( name[0] ) + name.Substring(1);
		}
		return name;
	}
	
	private static void DrawFaceEditor(ref int face, Atlas atlas, ref Matrix4x4 matrix) {
		GUILayout.BeginVertical(GUI.skin.box);
			Texture texture = atlas.GetTexture();
			Rect rect = GUILayoutUtility.GetAspectRect((float)texture.width/texture.height);
		GUILayout.EndVertical();
		
		Matrix4x4 rectMatrix = Matrix4x4.Scale( new Vector3(rect.width, rect.height, 0) ) * matrix;
		Matrix4x4 invRectMatrix = matrix.inverse * Matrix4x4.Scale( new Vector3(1/rect.width, 1/rect.height, 0) );
		Matrix4x4 invertY = Matrix4x4.TRS(new Vector2(0, 1), Quaternion.identity, new Vector2(1, -1));
		
		bool mouseInRect = rect.Contains(Event.current.mousePosition);
		
		GUI.BeginGroup(rect);
		{
			Vector2 mouse = invRectMatrix.MultiplyPoint(Event.current.mousePosition); // local mouse [0..1]
			
			if(Event.current.type == EventType.Repaint) {
				Rect texturePosition = Mul(new Rect(0,0,1,1), rectMatrix);
				Rect faceRet = atlas.ToRect(face);
				faceRet = Mul(faceRet, rectMatrix*invertY);
				GUI.DrawTexture(texturePosition, texture);
				EditorGUIUtils.DrawRect( faceRet, Color.green );
			}
			
			if(Event.current.type == EventType.MouseDown && Event.current.button == 0 && mouseInRect) {
				Vector2 invMouse = invertY.MultiplyPoint( mouse );
				if(invMouse.x >= 0 && invMouse.x <= 1 && invMouse.y >= 0 && invMouse.y <= 1) {
					int posX = Mathf.FloorToInt( invMouse.x*atlas.GetWidth() );
					int posY = Mathf.FloorToInt( invMouse.y*atlas.GetHeight() );
					face = posY*atlas.GetWidth() + posX;
				
					GUI.changed = true;
					Event.current.Use();
				}
			}
			
			if(Event.current.type == EventType.MouseDrag && Event.current.button == 1 && mouseInRect) {
				Vector3 delta = Event.current.delta;
				delta.x /= rect.width;
				delta.y /= rect.height;
				
				Matrix4x4 offsetMatrix = Matrix4x4.TRS(delta, Quaternion.identity, Vector3.one);
				matrix = offsetMatrix*matrix;
			
				GUI.changed = true;
				Event.current.Use();
			}
			
			if(Event.current.type == EventType.ScrollWheel && mouseInRect) {
				float s = 0.95f;
				if(Event.current.delta.y < 0) s = 1.0f/s;
				
				Matrix4x4 offsetMatrix = Matrix4x4.TRS(mouse, Quaternion.identity, Vector3.one);
				matrix *= offsetMatrix;
				
				Matrix4x4 scaleMatrix = Matrix4x4.Scale(Vector3.one*s);
				matrix *= scaleMatrix;
				
				offsetMatrix = Matrix4x4.TRS(-mouse, Quaternion.identity, Vector3.one);
				matrix *= offsetMatrix;
			
				GUI.changed = true;
				Event.current.Use();
			}
		}
		GUI.EndGroup();
	}
	
	private static Rect Mul(Rect rect, Matrix4x4 matrix) {
		Vector3 pos = matrix.MultiplyPoint( new Vector3(rect.x, rect.y) );
		Vector3 size = matrix.MultiplyVector( new Vector3(rect.width, rect.height) );
		return new Rect( pos.x, pos.y, size.x, size.y );
	}
	
}
