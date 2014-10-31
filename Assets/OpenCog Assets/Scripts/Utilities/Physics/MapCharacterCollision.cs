using UnityEngine;
using System.Collections;

public class MapCharacterCollision {
	
	public static void Collision(OpenCog.Map.OCMap map, CharacterController controller) {
		for(int i=0; i<3; i++) {
			Contact? _contact = GetClosestContact(map, controller);
			if(!_contact.HasValue) break;
			Contact contact = _contact.Value;
			controller.transform.position += contact.delta;
			//@TODO: Figure out a better way to do map / character collisions...
			//controller.collider.OnCollision(contact.b, contact.delta.normalized);
		}
	}
	
	private static Contact? GetClosestContact(OpenCog.Map.OCMap map, CharacterController controller) {
		Vector3 pos = controller.transform.position;
		int x1 = Mathf.FloorToInt(pos.x-controller.radius);
		int y1 = Mathf.FloorToInt(pos.y);
		int z1 = Mathf.FloorToInt(pos.z-controller.radius);
		
		int x2 = Mathf.CeilToInt(pos.x+controller.radius);
		int y2 = Mathf.CeilToInt(pos.y+controller.height);
		int z2 = Mathf.CeilToInt(pos.z+controller.radius);
		
		Contact? contact = null;
		for(int x=x1; x<=x2; x++) {
			for(int y=y1; y<=y2; y++) {
				for(int z=z1; z<=z2; z++) {
					OpenCog.Map.OCBlockData block = map.GetBlock(x, y, z);
					if(block.IsSolid()) {
						Contact? _newContact = BlockCharacterCollision.GetContactBlockCharacter(new Vector3i(x, y, z), pos, controller);
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
//		return null;
	}
	
}

