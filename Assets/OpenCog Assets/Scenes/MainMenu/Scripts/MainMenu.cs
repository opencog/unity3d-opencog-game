using UnityEngine;
using System.Collections;

public class MainMenu : AbstractMenu {

	protected override void OnMenuGUI() {
		if( GUILayout.Button("START GAME") ) {
			SwitchTo<StartGameMenu>();
		}
		if( GUILayout.Button("BLOCKSET VIEWER") ) {
			Application.LoadLevel("BlockSetViewer");
		}
		if( GUILayout.Button("QUIT") ) {
			Application.Quit();
		}
	}
	
}
