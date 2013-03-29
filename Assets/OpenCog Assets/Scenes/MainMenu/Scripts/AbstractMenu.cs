using UnityEngine;
using System.Collections;

public class AbstractMenu : MonoBehaviour {
	
	[SerializeField] protected Font font;
	
	protected virtual void OnGUI() {
		GUI.depth = -1;
		
		Font oldFont = GUI.skin.font;
		GUI.skin.font = font;
		
		GUILayout.BeginArea( GetMenuArea() );
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
					OnMenuGUI();
				GUILayout.EndVertical();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		
		GUI.skin.font = oldFont;
	}
	
	private static Rect GetMenuArea() {
		float offset = Screen.width*0.08f;
		Rect rect = new Rect(offset, 0, 0, 0);
		rect.xMax = Screen.width;
		rect.yMax = Screen.height-offset;
		return rect;
	}
	
	protected virtual void OnMenuGUI() {
		
	}
	
	protected void SwitchTo<T>() where T : MonoBehaviour {
		enabled = false;
		GetComponent<T>().enabled = true;
	}
	
	
}
