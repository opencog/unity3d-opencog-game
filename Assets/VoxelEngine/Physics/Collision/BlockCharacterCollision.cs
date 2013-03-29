using UnityEngine;
using System.Collections;

public class BlockCharacterCollision {

	
	public static Contact? GetContactBlockCharacter(Vector3 blockPos, Vector3 pos, CharacterCollider collider) {
		Contact contactA = GetClosestPoint(blockPos, pos+collider.bottom);
		Contact contactB = GetClosestPoint(blockPos, pos+collider.top);
		Contact contact = contactA;
		if(contactB.sqrDistance < contact.sqrDistance) contact = contactB;
		
		if(contact.sqrDistance > collider.radius*collider.radius) return null;
		
        Vector3 dir = contact.delta.normalized * collider.radius;
        Vector3 capsulePoint = contact.b + dir;
		contact.b = capsulePoint;
		return contact;
	}
	
	private static Contact GetClosestPoint(Vector3 blockPos, Vector3 point) {
		Vector3 blockMin = blockPos - Vector3.one/2f;
		Vector3 blockMax = blockPos + Vector3.one/2f;
		Vector3 closest = point;
		for(int i=0; i<3; i++) {
			if (closest[i] > blockMax[i]) closest[i] = blockMax[i];
        	if (closest[i] < blockMin[i]) closest[i] = blockMin[i];
		}
		return new Contact(closest, point);
	}
	
	
}


public struct Contact {
		
	public Vector3 a, b;
	
	public float sqrDistance {
		get {
			return (a-b).sqrMagnitude;
		}
	}
	
	public Vector3 delta {
		get {
			return a-b;
		}
	}
		
	public Contact(Vector3 a, Vector3 b) {
		this.a = a;
		this.b = b;
	}
		
}
