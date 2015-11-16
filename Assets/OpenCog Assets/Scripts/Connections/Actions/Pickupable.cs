using UnityEngine;
using System.Collections;
using OpenCog.Embodiment;

public class Pickupable : MonoBehaviour {

	public GameObject holder = null;
	// Use this for initialization
	void Start () 
	{
		OCStateChangesRegister.RegisterState(gameObject, this, "holder");
	}

}
