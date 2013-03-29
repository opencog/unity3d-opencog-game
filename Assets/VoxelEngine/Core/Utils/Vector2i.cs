using UnityEngine;
using System.Collections;

public struct Vector2i {

	public int x, y;
	
	public static readonly Vector2i zero  = new Vector2i(0, 0);
	public static readonly Vector2i one   = new Vector2i(1, 1);
	public static readonly Vector2i up    = new Vector2i(0, 1);
	public static readonly Vector2i down  = new Vector2i(0, -1);
	public static readonly Vector2i left  = new Vector2i(-1, 0);
	public static readonly Vector2i right = new Vector2i(1, 0);
	
	public Vector2i(int x, int y) {
		this.x = x;
		this.y = y;
	}
	
	public override int GetHashCode() {
		return x.GetHashCode () ^ y.GetHashCode () << 2;
	}
	
	public override bool Equals(object other) {
		if (!(other is Vector2i)) return false;
		Vector2i vector = (Vector2i) other;
		return x == vector.x && 
			   y == vector.y;
	}
	
	public override string ToString() {
		return "Vector2i("+x+" "+y+")";
	}
	
	public static Vector2i Min(Vector2i a, Vector2i b) {
		return new Vector2i(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
	}
	public static Vector2i Max(Vector2i a, Vector2i b) {
		return new Vector2i(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
	}
	
	public static bool operator == (Vector2i a, Vector2i b) {
		return a.x == b.x && 
			   a.y == b.y;
	}
	
	public static bool operator != (Vector2i a, Vector2i b) {
		return a.x != b.x ||
			   a.y != b.y;
	}
	
	public static Vector2i operator - (Vector2i a, Vector2i b) {
		return new Vector2i( a.x-b.x, a.y-b.y );
	}
	
	public static Vector2i operator + (Vector2i a, Vector2i b) {
		return new Vector2i( a.x+b.x, a.y+b.y );
	}
	
}
