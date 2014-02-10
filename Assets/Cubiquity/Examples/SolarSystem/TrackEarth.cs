using UnityEngine;
using System.Collections;

public class TrackEarth : MonoBehaviour {

	GameObject earth;
	void Start()
	{
		earth = GameObject.Find("EarthOrbitPoint/EarthPlaceholder");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(earth != null)
		{
			transform.LookAt(earth.transform);
		}
	}
}
