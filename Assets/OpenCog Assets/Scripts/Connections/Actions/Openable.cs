using UnityEngine;
using System.Collections;
using OpenCog.Embodiment;

public class Openable : MonoBehaviour {

	public bool is_open = false;

	void Start()
	{
		OCStateChangesRegister.RegisterState(gameObject, this, "is_open");
	}

}
