using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour {
	
	private static GameStateManager _manager;
	private static GameStateManager manager {
		get {
			if(_manager == null) _manager = (GameStateManager) GameObject.FindObjectOfType( typeof(GameStateManager) );
			return _manager;
		}
	}
	
	public static bool IsPause {
		set {
			if(value) Time.timeScale = 1f/10000f;
			if(!value) Time.timeScale = 1;
			Screen.showCursor = value;
			if(value) manager.SendMessage("OnPause", SendMessageOptions.DontRequireReceiver);
			if(!value) manager.SendMessage("OnResume", SendMessageOptions.DontRequireReceiver);
		}
		get {
			return Time.timeScale <= 0.0001f;
		}
	}
	public static bool IsPlaying {
		set {
			IsPause = !value;
		}
		get {
			return !IsPause;
		}
	}

	
	void Start() {
		Screen.showCursor = false;
	}
	
	void Update() {
		Screen.lockCursor = !Screen.showCursor;
		
		if(Input.GetKeyDown(KeyCode.Tab)) {
			Screen.showCursor = true;
			Screen.lockCursor = false;
		}
		
		if(Input.GetKeyDown(KeyCode.Escape)) {
			IsPause = !IsPause;
		}
	}
	
}
