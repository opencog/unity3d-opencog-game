using UnityEngine;
using System.Collections;
using OpenCog.Embodiment;

public class Openable : MonoBehaviour {

	public bool isOpen = false;

	void Start()
	{
		OCStateChangesRegister.RegisterState(gameObject, this, "isOpen");
	}

}
