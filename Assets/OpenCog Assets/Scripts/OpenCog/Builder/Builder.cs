using UnityEngine;
using System.Collections;

public class Builder : MonoBehaviour {
	
	[SerializeField] private GameObject cursor;
	private Transform cameraTrans;
	private CharacterCollider characterCollider;
	private Map map;
	private Block selectedBlock;
	
	void Awake() {
		cameraTrans = transform.GetComponentInChildren<Camera>().transform;
		characterCollider = GetComponent<CharacterCollider>();
		map = (Map) GameObject.FindObjectOfType( typeof(Map) );
		cursor = (GameObject)GameObject.Instantiate(cursor);
	}
	
	public void SetSelectedBlock(Block block) {
		selectedBlock = block;
	}

	public Block GetSelectedBlock() {
		return selectedBlock;
	}
	
	// Update is called once per frame
	void Update () {
		if(Screen.showCursor) return;
		
		if( Input.GetKeyDown(KeyCode.LeftControl) ) {
			Vector3i? point = GetCursor(false);
			if(point.HasValue) {
				byte sun = map.GetSunLightmap().GetLight(point.Value);
				byte light = map.GetLightmap().GetLight(point.Value);
				Debug.Log("Sun "+" "+sun+"  Light "+light);
			}
		}
		
		if( Input.GetKeyDown(KeyCode.RightControl) ) {
			Vector3i? point = GetCursor(true);
			if(point.HasValue) {
				byte sun = map.GetSunLightmap().GetLight(point.Value);
				byte light = map.GetLightmap().GetLight(point.Value);
				Debug.Log("Sun "+sun+"  Light "+light);
			}
		}
		
		if( Input.GetMouseButtonDown(0) ) {
			Vector3i? point = GetCursor(true);
			if(point.HasValue) {
				map.SetBlockAndRecompute(new BlockData(), point.Value);
			}
		}
		
		if( Input.GetMouseButtonDown(1) ) {
			Vector3i? point = GetCursor(false);
			if(point.HasValue) {
				bool empty = !BlockCharacterCollision.GetContactBlockCharacter(point.Value, transform.position, characterCollider).HasValue;
				if(empty) {
					BlockData block = new BlockData( selectedBlock );
					block.SetDirection( GetDirection(-transform.forward) );
					map.SetBlockAndRecompute(block, point.Value);
				}
			}
		}
		
		Vector3i? cursor = GetCursor(true);
		this.cursor.SetActive(cursor.HasValue);
		if(cursor.HasValue) {
			this.cursor.transform.position = cursor.Value;
		}
		
	}
	
	private Vector3i? GetCursor(bool inside) {
		Ray ray = new Ray(cameraTrans.position, cameraTrans.forward);
		Vector3? point =  MapRayIntersection.Intersection(map, ray, 10);
		if( point.HasValue ) {
			Vector3 pos = point.Value;
			if(inside) pos += ray.direction*0.01f;
			if(!inside) pos -= ray.direction*0.01f;
			int posX = Mathf.RoundToInt(pos.x);
			int posY = Mathf.RoundToInt(pos.y);
			int posZ = Mathf.RoundToInt(pos.z);
			return new Vector3i(posX, posY, posZ);
		}
		return null;
	}
	
	private static BlockDirection GetDirection(Vector3 dir) {
		if( Mathf.Abs(dir.z) >= Mathf.Abs(dir.x) ) {
			// 0 или 180
			if(dir.z >= 0) return BlockDirection.Z_PLUS;
			return BlockDirection.Z_MINUS;
		} else {
			// 90 или 270
			if(dir.x >= 0) return BlockDirection.X_PLUS;
			return BlockDirection.X_MINUS;
		}
	}
	
}
