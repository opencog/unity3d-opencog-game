using UnityEngine;
using System.Collections;
using OpenCog.Embodiment;

public class BackToLife : MonoBehaviour {

	public Material realMat;
	public GameObject backToLifeEffectPrefab;
	public bool is_alive = false;

	// Use this for initialization
	void Start () 
	{
		OCStateChangesRegister.RegisterState(gameObject, this, "is_alive");
	}

	public void bringBackToLife()
	{
		is_alive = true;
		StartCoroutine("displayBackToLife");

	}

	IEnumerator displayBackToLife()
	{
		GameObject backToLifeEffect = GameObject.Instantiate(backToLifeEffectPrefab) as GameObject;
		backToLifeEffect.transform.position = transform.position;
		backToLifeEffect.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
		Destroy (backToLifeEffect, 3f);
		yield return new WaitForSeconds(3f);
		MeshRenderer meshRender = gameObject.GetComponentInChildren<MeshRenderer> () as MeshRenderer;
		meshRender.material = realMat;

	}

}
