using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryGUI : MonoBehaviour {
	
	private BlockSet blockSet;
	private Builder builder;
	
	private bool show = false;
	private Vector2 scrollPosition = Vector3.zero;

	
	void Awake () {
		Map map = (Map) GameObject.FindObjectOfType( typeof(Map) );
		blockSet = map.GetBlockSet();
		GameObject player = GameObject.FindGameObjectWithTag( "Player" );
		builder = (Builder) player.GetComponent<Builder>();
	}
	
	void Update () {
		if( Input.GetKeyDown(KeyCode.E) && GameStateManager.IsPlaying ) {
			show = !show;
			Screen.showCursor = show;
		}
		if(GameStateManager.IsPause) show = false;
	}
	
	void OnGUI() {
		if(show) {
			Rect window = new Rect(0, 0, Screen.width*0.5f, Screen.height*0.6f);
			window.center = new Vector2(Screen.width, Screen.height)/2f;
			GUILayout.Window(0, window, DoInventoryWindow, "Inventory");
		}
	}
	
	
	private void DoInventoryWindow(int windowID) {
		Block selected = builder.GetSelectedBlock();
		selected = DrawInventory(blockSet, ref scrollPosition, selected);
		builder.SetSelectedBlock(selected);
    }
	
	private static Block DrawInventory(BlockSet blockSet, ref Vector2 scrollPosition, Block selected) {
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		for(int i=0, y=0; i<blockSet.GetBlockCount(); y++) {
			GUILayout.BeginHorizontal();
			for(int x=0; x<8; x++, i++) {
				Block block = blockSet.GetBlock(i);
				if( DrawBlock(block, block == selected && selected != null) ) {
					selected = block;
				}
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
		return selected;
	}
	
	private static bool DrawBlock(Block block, bool selected) {
		Rect rect = GUILayoutUtility.GetAspectRect(1f);
		
		if(selected) GUI.Box(rect, GUIContent.none);
		
		Vector3 center = rect.center;
		rect.width -= 8;
		rect.height -= 8;
		rect.center = center;
		
		if(block != null) return block.DrawPreview(rect);
		return false;
	}
	
	
}
