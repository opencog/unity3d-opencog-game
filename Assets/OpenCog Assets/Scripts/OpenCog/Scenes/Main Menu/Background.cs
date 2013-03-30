using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {
	
	[SerializeField] private Texture2D background;

	void OnGUI() {
		GUI.depth = 0;
		DrawBackground(background);
	}
	
	private static void DrawBackground(Texture2D image) {
		float scale = Mathf.Max( (float)Screen.width/image.width, (float)Screen.height/image.height );
		float width = image.width*scale;
		float height = image.height*scale;
		float x = Screen.width/2 - width/2;
		float y = Screen.height/2 - height/2;
		GUI.DrawTexture(new Rect(x, y, width, height), image);
	}
	
}
