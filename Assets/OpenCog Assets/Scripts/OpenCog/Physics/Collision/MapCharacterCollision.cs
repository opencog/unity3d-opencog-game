using UnityEngine;
using System.Collections;

public class MapCharacterCollision {
	
	public static void Collision(Map map, CharacterCollider collider) {
		for(int i=0; i<3; i++) {
			Contact? _contact = GetClosestContact(map, collider);
			if(!_contact.HasValue) break;
			Contact contact = _contact.Value;
			collider.transform.position += contact.delta;
			collider.OnCollision(contact.b, contact.delta.normalized);
		}
	}
	
	private static Contact? GetClosestContact(Map map, CharacterCollider collider) {
		Vector3 pos = collider.transform.position;
		int x1 = Mathf.FloorToInt(pos.x-collider.radius);
		int y1 = Mathf.FloorToInt(pos.y);
		int z1 = Mathf.FloorToInt(pos.z-collider.radius);
		
		int x2 = Mathf.CeilToInt(pos.x+collider.radius);
		int y2 = Mathf.CeilToInt(pos.y+collider.height);
		int z2 = Mathf.CeilToInt(pos.z+collider.radius);
		
		Contact? contact = null;
		for(int x=x1; x<=x2; x++) {
			for(int y=y1; y<=y2; y++) {
				for(int z=z1; z<=z2; z++) {
					BlockData block = map.GetBlock(x, y, z);
					if(block.IsSolid()) {
						Contact? _newContact = BlockCharacterCollision.GetContactBlockCharacter(new Vector3i(x, y, z), pos, collider);
						if(_newContact.HasValue && _newContact.Value.delta.magnitude > float.Epsilon) {
							Contact newContact = _newContact.Value;
							if(!contact.HasValue || newContact.sqrDistance > contact.Value.sqrDistance) {
								contact = newContact;
							}
						}
					}
				}
			}
		}
		
		return contact;
	}
	
}

