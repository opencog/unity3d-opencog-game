using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {
	
	
	void OnResume() {
		enabled = true;
	}
	
	void OnPause() {
		enabled = false;
	}
	
	
	void OnGUI() {
		int fps = (int) (1f/Time.deltaTime*Time.timeScale);
		GUILayout.Box( "FPS "+fps );
		
		Vector2 size = GUI.skin.label.CalcSize( new GUIContent("+") );
		Rect rect = new Rect(0, 0, size.x, size.y);
		rect.center = new Vector2(Screen.width, Screen.height)/2f;
		GUI.Label( rect, "+" );
	}
	
	
}
