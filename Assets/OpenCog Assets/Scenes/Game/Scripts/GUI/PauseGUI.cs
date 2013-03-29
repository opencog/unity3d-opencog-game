using UnityEngine;
using System.Collections;

public class PauseGUI : MonoBehaviour {
	
	private const string help = "Esc - Pause/Resume\n" +
								"E - Open the inventory";
	
	void OnResume() {
		enabled = false;
	}
	
	void OnPause() {
		enabled = true;
	}
	

	void OnGUI() {
		GUILayout.BeginArea( GetMenuArea() );
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
					DrawMenu();
				GUILayout.EndVertical();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
	}
	
	private void DrawMenu() {
		if( GUILayout.Button("Resume", GUILayout.ExpandWidth(false)) ) {
			GameStateManager.IsPlaying = true;
		}
		DrawSunSlider();
		GUILayout.Box(help, GUILayout.ExpandWidth(false));
		if( GUILayout.Button("Menu", GUILayout.ExpandWidth(false)) ) {
			GameStateManager.IsPlaying = true;
			Screen.showCursor = true;
			Application.LoadLevel("MainMenu");
		}
		if( GUILayout.Button("Quit", GUILayout.ExpandWidth(false)) ) {
			Application.Quit();
		}
	}
	
	
	private static Rect GetMenuArea() {
		float offset = Screen.width*0.08f;
		Rect rect = new Rect(offset, 0, 0, 0);
		rect.xMax = Screen.width;
		rect.yMax = Screen.height-offset;
		return rect;
	}
	
	private void DrawSunSlider() {
		const float min = (float) SunLightComputer.MIN_LIGHT / SunLightComputer.MAX_LIGHT;
		const float max = 1;
		Vector3 color = (Vector4)RenderSettings.ambientLight;
		color.Normalize();
		float light = RenderSettings.ambientLight.r;
		
		GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label("Sun");
			light = GUILayout.HorizontalSlider(light, min, max, GUILayout.Width(Screen.width/3f) );
		GUILayout.EndHorizontal();
		
		light = Mathf.Clamp(light, min, 1f);
		RenderSettings.ambientLight = new Color(light, light, light, 1f);
	}
	
}
