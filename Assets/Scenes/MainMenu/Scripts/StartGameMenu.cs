using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartGameMenu : AbstractMenu {
	
	[SerializeField] private BlockSet[] blockSetList;
	
	protected override void OnMenuGUI() {
		foreach(BlockSet blockset in blockSetList) {
			if(GUILayout.Button(blockset.name)) {
				GameSetup.blockSet = blockset;
				Application.LoadLevel("Game");
				return;
			}
		}
		
		if(GUILayout.Button("Back")) {
			SwitchTo<MainMenu>();
		}
	}
	
	
	
}
