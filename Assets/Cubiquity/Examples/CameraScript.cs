using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    public float mouseSensitivity = 15.0f;
 
    public float lookUpLimit = 60.0f;
	public float lookDownLimit = -60.0f;
	
	float yRotation = 0F;
	
	float Speed = 1.0f;
	
	void Start()
	{
		yRotation = -transform.localEulerAngles.x;
	}
	
    void Update()
    {     
		if(Input.GetMouseButton(1))
		{
	        float xRotation = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;    
	        yRotation += Input.GetAxis("Mouse Y") * mouseSensitivity;
	        yRotation = Mathf.Clamp (yRotation, lookDownLimit, lookUpLimit); 
	        transform.localEulerAngles = new Vector3(-yRotation, xRotation, 0);
			
			float vertical = Input.GetAxis("Vertical");
			float horizontal = Input.GetAxis("Horizontal");
			transform.Translate(horizontal * Speed, 0, vertical * Speed);
		}
    }
}