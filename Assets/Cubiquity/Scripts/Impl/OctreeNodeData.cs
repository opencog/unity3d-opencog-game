using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	public class OctreeNodeData : MonoBehaviour
	{
		[System.NonSerialized]
		public int meshLastSyncronised;
		[System.NonSerialized]
		public Vector3 lowerCorner;
		[System.NonSerialized]
		public GameObject[,,] children;
		
		// Use this for initialization
		void Awake()
		{
			//meshLastSyncronised = 0;
		}
		
		public void OnEnable()
		{
		}
		
		// Update is called once per frame
		void Update()
		{	
		}
		
		public GameObject GetChild(uint x, uint y, uint z)
		{
			if(children != null)
			{
				return children[x, y, z];
			}
			else
			{
				return null;
			}
		}
		
		public void SetChild(uint x, uint y, uint z, GameObject gameObject)
		{
			if(children == null)
			{
				children = new GameObject[2, 2, 2];
			}
			
			children[x, y, z] = gameObject;
		}
	}
}
