/*******************************************************************************
IMPORTANT NOTE - This file was contributed by one of our users. While the rest of
Cubiquity is distributed free for non-commercial use, the contents of this file
are under the more liberal zlib license. Please see the copyright notice below.
********************************************************************************
Copyright (c) 2013 Ian Joseph Fischer and David Williams

This software is provided 'as-is', without any express or implied
warranty. In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:

    1. The origin of this software must not be misrepresented; you must not
    claim that you wrote the original software. If you use this software
    in a product, an acknowledgment in the product documentation would be
    appreciated but is not required.

    2. Altered source versions must be plainly marked as such, and must not be
    misrepresented as being the original software.

    3. This notice may not be removed or altered from any source
    distribution. 	
*******************************************************************************/

using UnityEngine;

namespace Cubiquity
{
	[System.Serializable]
	public struct Vector3i
	{
		public int x, y, z;
		public static readonly Vector3i zero = new Vector3i(0, 0, 0);
		public static readonly Vector3i one = new Vector3i(1, 1, 1);
		public static readonly Vector3i forward = new Vector3i(0, 0, 1);
		public static readonly Vector3i back = new Vector3i(0, 0, -1);
		public static readonly Vector3i up = new Vector3i(0, 1, 0);
		public static readonly Vector3i down = new Vector3i(0, -1, 0);
		public static readonly Vector3i left = new Vector3i(-1, 0, 0);
		public static readonly Vector3i right = new Vector3i(1, 0, 0);	
		
		public Vector3i(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	
	
		public Vector3i(int x, int y)
		{
			this.x = x;
			this.y = y;
			this.z = 0;
		}
	
	
		public Vector3i(Vector3i a)
		{
			this.x = a.x;
			this.y = a.y;
			this.z = a.z;
		}
	
	
		public Vector3i(Vector3 a)
		{
			this.x = (int)a.x;
			this.y = (int)a.y;
			this.z = (int)a.z;
		}
	
	
		public int this[int key]
		{
			get
			{
				switch(key)
				{
					case 0:
					{
						return x;
					}
					case 1:
					{
						return y;
					}
					case 2:
					{
						return z;
					}
					default:
					{
						Debug.LogError("Invalid Vector3i index value of: " + key);
						return 0;
					}
				}
			}
			set
			{
				switch(key)
				{
					case 0:
					{
						x = value;
						return;
					}
					case 1:
					{
						y = value;
						return;
					}
					case 2:
					{
						z = value;
						return;
					}
					default:
					{
						Debug.LogError("Invalid Vector3i index value of: " + key);
						return;
					}
				}
			}
		}
	
	
		public static Vector3i Scale(Vector3i a, Vector3i b)
		{
			return new Vector3i(a.x * b.x, a.y * b.y, a.z * b.z);
		}
		
		public static float Distance(Vector3i a, Vector3i b)
		{
			return Mathf.Sqrt(DistanceSquared(a, b));
		}
	
	
		public static int DistanceSquared(Vector3i a, Vector3i b)
		{
			int dx = b.x - a.x;
			int dy = b.y - a.y;
			int dz = b.z - a.z;
			return dx * dx + dy * dy + dz * dz;
		}
		
		
		public override int GetHashCode()
		{
			// Microsoft use XOR in their example here: http://msdn.microsoft.com/en-us/library/ms173147.aspx
			// We also use shifting, as the compoents are typically small and this should reduce collisions.
			return x ^ (y << 8) ^ (z << 16);
		}
	
		
		public override bool Equals(object other)
		{
			if(!(other is Vector3i))
			{
				return false;
			}
			Vector3i vector = (Vector3i)other;
			return x == vector.x && 
				   y == vector.y && 
				   z == vector.z;
		}
	
		
		public override string ToString()
		{
			return string.Format("Vector3i({0} {1} {2})", x, y, z);
		}
	
		
		public static bool operator ==(Vector3i a, Vector3i b)
		{
			return a.x == b.x &&
				   a.y == b.y && 
				   a.z == b.z;
		}
	
		
		public static bool operator !=(Vector3i a, Vector3i b)
		{
			return a.x != b.x || 
				   a.y != b.y ||
				   a.z != b.z;
		}
	
		
		public static Vector3i operator -(Vector3i a, Vector3i b)
		{
			return new Vector3i(a.x - b.x, a.y - b.y, a.z - b.z);
		}
	
		
		public static Vector3i operator +(Vector3i a, Vector3i b)
		{
			return new Vector3i(a.x + b.x, a.y + b.y, a.z + b.z);
		}
	
	
		public static Vector3i operator *(Vector3i a, int d)
		{
			return new Vector3i(a.x * d, a.y * d, a.z * d);
		}
	
	
		public static Vector3i operator *(int d, Vector3i a)
		{
			return new Vector3i(a.x * d, a.y * d, a.z * d);
		}
	
		
		public static explicit operator Vector3(Vector3i v)
		{
			return new Vector3(v.x, v.y, v.z);
		}
	
		public static explicit operator Vector3i(Vector3 v)
		{
			return new Vector3i(v);
		}
	
	
		public static Vector3i Min(Vector3i lhs, Vector3i rhs)
		{
			return new Vector3i(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
		}
	
	
		public static Vector3i Max(Vector3i lhs, Vector3i rhs)
		{
			return new Vector3i(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
		}
	
	
		public static Vector3i Clamp(Vector3i value, Vector3i min, Vector3i max)
		{
			return new Vector3i(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y), Mathf.Clamp(value.z, min.z, max.z));
		}
	}
}
