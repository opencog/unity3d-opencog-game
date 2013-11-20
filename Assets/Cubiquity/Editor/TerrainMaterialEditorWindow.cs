using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	public class TerrainMaterialEditorWindow : EditorWindow
	{
		private TerrainMaterial material;
			
		public static void EditMaterial(TerrainMaterial materialToEdit)
		{
			TerrainMaterialEditorWindow window = ScriptableObject.CreateInstance<TerrainMaterialEditorWindow>();
			window.material = materialToEdit;
			window.ShowUtility();
		}
		
		void OnGUI()
		{
			EditorGUILayout.LabelField("Instructions", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("Please choose a texture to assign to this material slot. You can also adjust the scale and offset of your selected texture." , MessageType.None);
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("Texture", EditorStyles.boldLabel);
			Texture2D oldTexture = material.diffuseMap;
			Texture2D newTexture = EditorGUILayout.ObjectField(oldTexture,typeof(Texture),false, GUILayout.Width(80), GUILayout.Height(80)) as Texture2D;
			if(oldTexture != newTexture)
			{
				material.diffuseMap = newTexture;
				HandleUtility.Repaint();
			}
			
			EditorGUILayout.LabelField("Scale", EditorStyles.boldLabel);
			material.scale = DrawVectorEditor(material.scale, 0.1f, 64.0f);
			
			EditorGUILayout.LabelField("Offset", EditorStyles.boldLabel);
			material.offset = DrawVectorEditor(material.offset, 0.0f, 1.0f);
		}
		
		Vector3 DrawVectorEditor(Vector3 vecToEdit, float min, float max)
		{
			Vector3 result;
			
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("X", GUILayout.Width(10));
				result.x = EditorGUILayout.Slider(vecToEdit.x, min, max);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Y", GUILayout.Width(10));
				result.y = EditorGUILayout.Slider(vecToEdit.y, min, max);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Z", GUILayout.Width(10));
				result.z = EditorGUILayout.Slider(vecToEdit.z, min, max);
			EditorGUILayout.EndHorizontal();
			
			return result;
		}
	}
}
