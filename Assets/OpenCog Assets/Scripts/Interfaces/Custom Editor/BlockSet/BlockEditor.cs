#if UNITY_EDITOR
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using OpenCog.BlockSet.BaseBlockSet;
using OpenCog.BlockSet;


public class BlockEditor {
	
	private static UnityEngine.Matrix4x4 atlasMatrix = UnityEngine.Matrix4x4.identity;
	private static int selectedFace = 0;

	public static void DrawBlockEditor(OCBlock block, OCBlockSet blockSet) {

		UnityEngine.GUILayout.BeginVertical(UnityEngine.GUI.skin.box);
		{
			string name = EditorGUILayout.TextField("Name", block.GetName());
			block.SetName( FixNameString(name) );
			
			if(block is OCGlassBlock)
			{
				OCGlassBlock glass = (OCGlassBlock)block;
				UnityEngine.GameObject interior = (UnityEngine.GameObject)EditorGUILayout.ObjectField("Interior", glass.GetInterior(), typeof(UnityEngine.GameObject), true, null);
				glass.SetInterior( interior );
			}

			int atlas = BlockEditorUtils.Popup( "Atlas", block.AtlasID, blockSet.Atlases );
			block.AtlasID = atlas;
		
			int light = EditorGUILayout.IntField("Light", block.GetLight());
			block.SetLight(light);
		}
		UnityEngine.GUILayout.EndVertical();
		
		UnityEngine.Texture texture = block.GetTexture();
		if(texture != null) {
			FieldInfo field = DrawFacesList(block, texture);
			int face = (int)field.GetValue(block);
			DrawFaceEditor(ref face, block.Atlas, ref atlasMatrix);
			field.SetValue(block, face);
		}
	}
	
	private static FieldInfo DrawFacesList(OCBlock block, UnityEngine.Texture texture) {
		List<FieldInfo> fields = GetFaces(block);
		UnityEngine.Rect[] faces = new UnityEngine.Rect[fields.Count];
		string[] names = new string[fields.Count];
		for(int i=0; i<fields.Count; i++) {
			int pos = (int) fields[i].GetValue( block );
			faces[i] = block.ToRect(pos);
			names[i] = FixNameString(fields[i].Name);
		}
		
		selectedFace = UnityEngine.Mathf.Clamp(selectedFace, 0, fields.Count-1);
		selectedFace = DrawFacesList(texture, faces, names, selectedFace);
		return fields[selectedFace];
	}
	
	private static int DrawFacesList(UnityEngine.Texture texture, UnityEngine.Rect[] faces, string[] names, int selected) {
		UnityEngine.GUILayout.BeginHorizontal(UnityEngine.GUI.skin.box);
		UnityEngine.GUILayout.FlexibleSpace();
		{
			UnityEngine.GUILayout.BeginVertical();
			{
				UnityEngine.Rect rect = UnityEngine.GUILayoutUtility.GetAspectRect( faces.Length, UnityEngine.GUILayout.MaxWidth(64*faces.Length) );
				rect.width /= faces.Length;
				for(int i=0; i<faces.Length; i++) {
					UnityEngine.GUI.DrawTextureWithTexCoords(rect, texture, faces[i]);
					if(UnityEngine.Event.current.type == UnityEngine.EventType.MouseDown && rect.Contains(UnityEngine.Event.current.mousePosition)) {
						selected = i;
						UnityEngine.Event.current.Use();
					}
					rect.x += rect.width;
				}
				if(names.Length > 1) {
					UnityEngine.GUILayout.Space(4);
					UnityEngine.Rect toolbarRect = UnityEngine.GUILayoutUtility.GetRect(0, UnityEngine.GUI.skin.button.CalcHeight(UnityEngine.GUIContent.none, 0), UnityEngine.GUILayout.MaxWidth(64*faces.Length));
					selected = UnityEngine.GUI.Toolbar(toolbarRect, selected, names);
				}
			}
			UnityEngine.GUILayout.EndVertical();
		}
		UnityEngine.GUILayout.FlexibleSpace();
		UnityEngine.GUILayout.EndHorizontal();
		return selected;
	}
	
	private static List<FieldInfo> GetFaces(OCBlock block) {
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
	
	private static void DrawFaceEditor(ref int face, OCAtlas atlas, ref UnityEngine.Matrix4x4 matrix) {
		UnityEngine.GUILayout.BeginVertical(UnityEngine.GUI.skin.box);
		UnityEngine.Texture texture = atlas.Texture;
		UnityEngine.Rect rect = UnityEngine.GUILayoutUtility.GetAspectRect((float)texture.width/texture.height);
		UnityEngine.GUILayout.EndVertical();
		
		UnityEngine.Matrix4x4 rectMatrix = UnityEngine.Matrix4x4.Scale( new UnityEngine.Vector3(rect.width, rect.height, 0) ) * matrix;
		UnityEngine.Matrix4x4 invRectMatrix = matrix.inverse * UnityEngine.Matrix4x4.Scale( new UnityEngine.Vector3(1/rect.width, 1/rect.height, 0) );
		UnityEngine.Matrix4x4 invertY = UnityEngine.Matrix4x4.TRS(new UnityEngine.Vector2(0, 1), UnityEngine.Quaternion.identity, new UnityEngine.Vector2(1, -1));
		
		bool mouseInRect = rect.Contains(UnityEngine.Event.current.mousePosition);
		
		UnityEngine.GUI.BeginGroup(rect);
		{
			UnityEngine.Vector2 mouse = invRectMatrix.MultiplyPoint(UnityEngine.Event.current.mousePosition); // local mouse [0..1]
			
			if(UnityEngine.Event.current.type == UnityEngine.EventType.Repaint) {
				UnityEngine.Rect texturePosition = Mul(new UnityEngine.Rect(0,0,1,1), rectMatrix);
				UnityEngine.Rect faceRet = atlas.ToRect(face);
				faceRet = Mul(faceRet, rectMatrix*invertY);
				UnityEngine.GUI.DrawTexture(texturePosition, texture);
				BlockEditorUtils.DrawRect( faceRet, UnityEngine.Color.green );
			}
			
			if(UnityEngine.Event.current.type == UnityEngine.EventType.MouseDown && UnityEngine.Event.current.button == 0 && mouseInRect) {
				UnityEngine.Vector2 invMouse = invertY.MultiplyPoint( mouse );
				if(invMouse.x >= 0 && invMouse.x <= 1 && invMouse.y >= 0 && invMouse.y <= 1) {
					int posX = UnityEngine.Mathf.FloorToInt( invMouse.x*atlas.Width );
					int posY = UnityEngine.Mathf.FloorToInt( invMouse.y*atlas.Height );
					face = posY*atlas.Width + posX;
				
					UnityEngine.GUI.changed = true;
					UnityEngine.Event.current.Use();
				}
			}
			
			if(UnityEngine.Event.current.type == UnityEngine.EventType.MouseDrag && UnityEngine.Event.current.button == 1 && mouseInRect) {
				UnityEngine.Vector3 delta = UnityEngine.Event.current.delta;
				delta.x /= rect.width;
				delta.y /= rect.height;
				
				UnityEngine.Matrix4x4 offsetMatrix = UnityEngine.Matrix4x4.TRS(delta, UnityEngine.Quaternion.identity, UnityEngine.Vector3.one);
				matrix = offsetMatrix*matrix;
			
				UnityEngine.GUI.changed = true;
				UnityEngine.Event.current.Use();
			}
			
			if(UnityEngine.Event.current.type == UnityEngine.EventType.ScrollWheel && mouseInRect) {
				float s = 0.95f;
				if(UnityEngine.Event.current.delta.y < 0) s = 1.0f/s;
				
				UnityEngine.Matrix4x4 offsetMatrix = UnityEngine.Matrix4x4.TRS(mouse, UnityEngine.Quaternion.identity, UnityEngine.Vector3.one);
				matrix *= offsetMatrix;
				
				UnityEngine.Matrix4x4 scaleMatrix = UnityEngine.Matrix4x4.Scale(UnityEngine.Vector3.one*s);
				matrix *= scaleMatrix;
				
				offsetMatrix = UnityEngine.Matrix4x4.TRS(-mouse, UnityEngine.Quaternion.identity, UnityEngine.Vector3.one);
				matrix *= offsetMatrix;
			
				UnityEngine.GUI.changed = true;
				UnityEngine.Event.current.Use();
			}
		}
		UnityEngine.GUI.EndGroup();
	}
	
	private static UnityEngine.Rect Mul(UnityEngine.Rect rect, UnityEngine.Matrix4x4 matrix) {
		UnityEngine.Vector3 pos = matrix.MultiplyPoint( new UnityEngine.Vector3(rect.x, rect.y) );
		UnityEngine.Vector3 size = matrix.MultiplyVector( new UnityEngine.Vector3(rect.width, rect.height) );
		return new UnityEngine.Rect( pos.x, pos.y, size.x, size.y );
	}
	
}

#endif
