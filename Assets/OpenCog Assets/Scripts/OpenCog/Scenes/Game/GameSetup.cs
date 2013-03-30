using UnityEngine;
using System.Collections;

public class GameSetup : MonoBehaviour {
	
	public static BlockSet blockSet;

	void OnLevelWasLoaded(int level) {
		if(blockSet != null) GetComponent<Map>().SetBlockSet(blockSet);
	}
	
}
