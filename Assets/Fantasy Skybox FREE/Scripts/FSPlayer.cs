// Fantasy Skybox FREE version: 1.2
// Author: Gold Experience TeamDev (http://www.ge-team.com/)
// Support: geteamdev@gmail.com
// Please direct any bugs/comments/suggestions to geteamdev@gmail.com

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion

/***************
* FSPlayer class.
* 
* 	This class handles
* 		- Switches the Skybox
* 		- Updates Directional light
* 		- Updates Render Settings
* 		- Responds the Trigger events
* 		- Shows details on GUIs.
* 
* 	More info:
* 
* 		Skybox
* 		http://docs.unity3d.com/Documentation/Components/class-Skybox.html
* 		
* 		How do I Make a Skybox?
*		https://docs.unity3d.com/Documentation/Manual/HOWTO-UseSkybox.html
* 
* 		Render Settings
* 		http://docs.unity3d.com/Documentation/Components/class-RenderSettings.html
* 
* 		Directional light
* 		https://docs.unity3d.com/Documentation/Components/class-Light.html
* 
* 		Lights
* 		https://docs.unity3d.com/Documentation/Manual/Lights.html
* 
* 		Box Collider
* 		https://docs.unity3d.com/Documentation/Components/class-BoxCollider.html
* 
***************/

/***************
* Fog and Ambient colors reference.
* There are render settings and light preset pictures in Assets/Fantasy Skybox FREE/Presets folder
* 
* 	FS Night 01A		
* 		Fog Color: 30,50,100,255			Color(0.1176470588235294f,0.196078431372549f,0.392156862745098f,1f)
* 		Ambient Light: 35,35,35,255			Color(0.1372549019607843f,0.1372549019607843f,0.1372549019607843f,1f)
* 									
* 	FS Night 01B
* 		Fog Color: 20,40,100,255			Color(0.0784313725490196f,0.1568627450980392f,0.392156862745098f,1f)
* 		Ambient Light: 35,35,35,255			Color(0.1372549019607843f,0.1372549019607843f,0.1372549019607843f,1f)
* 
* 	FS Sunny 01A		
* 		Fog Color: 110,180,255,255			Color(0.431372549f,0.705882353f,1f,1f)
* 		Ambient Light: 160,160,170,255		Color(0.62745098f,0.62745098f,0.666666667f,1f)
* 									
* 	FS Sunny 01B
* 		Fog Color: 90,210,255,255			Color(0.352941176f,0.823529412f,1f,1f)
* 		Ambient Light: 160,180,180,255		Color(0.62745098f,0.705882353f,0.705882353f,1f)
* 
* 
* ***************/

public class FSPlayer : MonoBehaviour
{

#region Variables
	
	// Light
	public Light[] m_LightList;
	
	// Skybox
	public Material[] m_SkyboxList;
	
	// Fog
	Color[] FogColorList;
	
	// Ambient
	Color[] AmbientLightList;
	
	// Index to current Skybox
	int m_CurrentSkyBox = 0;
	
#endregion {Variables}
	
// ######################################################################
// MonoBehaviour Functions
// ######################################################################

#region Component Segments

	// Use this for initialization
	void Start ()
	{
		// Init Fog colors array
		FogColorList = new Color[4];

		// Fog Night
		FogColorList[0] =	new Color(0.1176470588235294f,0.196078431372549f,0.392156862745098f,1f);
		FogColorList[1] =	new Color(0.0784313725490196f,0.1568627450980392f,0.392156862745098f,1f);

		// Fog Sunny
		FogColorList[2] =	new Color(0.431372549f,0.705882353f,1f,1f);
		FogColorList[3] =	new Color(0.352941176f,0.823529412f,1f,1f);

		// Init Ambient light colors array
		AmbientLightList = new Color[4];

		// Ambient Night
		AmbientLightList[0] =	new Color(0.1372549019607843f,0.1372549019607843f,0.1372549019607843f,1f);
		AmbientLightList[1] =	new Color(0.1372549019607843f,0.1372549019607843f,0.1372549019607843f,1f);

		// Ambient Sunny
		AmbientLightList[2] =	new Color(0.62745098f,0.705882353f,0.705882353f,1f);
		AmbientLightList[3] =	new Color(0.705882353f,0.705882353f,0.705882353f,1f);
		
		SwitchSkyBox(0);
	}
	
	// Update is called once per frame
	void Update ()
	{
		// User press Q key
		if(Input.GetKeyUp(KeyCode.Q))
		{
			SwitchSkyBox(-1);
		}
		// User press E key
		if(Input.GetKeyUp(KeyCode.E))
		{
			SwitchSkyBox(+1);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		Debug.Log("OnTriggerExit="+other.name);
		
		// Reset player position when user move it away from terrain
		this.transform.localPosition = new Vector3(0,1,0);
    }
	
	// OnGUI is called for rendering and handling GUI events.
	void OnGUI () {
		
		// Show version number
		GUI.Window(1, new Rect((Screen.width-220), 5, 210, 80), InfoWindow, "Info");
		
		// Show Help GUI window
		GUI.Window(2, new Rect((Screen.width-220), Screen.height-85, 210, 80), HelpWindow, string.Format("{0:00}/{1:00}",m_CurrentSkyBox+1, m_SkyboxList.Length) + " (" + m_SkyboxList[m_CurrentSkyBox].name +")");
		
	}
	
#endregion Component Segments
	
// ######################################################################
// Functions Functions
// ######################################################################

#region Functions

	void SwitchSkyBox(int DiffNum)
	{
		// update m_CurrentSkyBox
		m_CurrentSkyBox += DiffNum;
		
		// rounds m_CurrentSkyBox
		if(m_CurrentSkyBox<0)
		{
			m_CurrentSkyBox = m_SkyboxList.Length-1;
		}
		if(m_CurrentSkyBox>=m_SkyboxList.Length)
		{
			m_CurrentSkyBox = 0;
		}
		
		// switch skybox
		RenderSettings.skybox = m_SkyboxList[m_CurrentSkyBox];
		
		// Set active/deactive lights
		for(int i=0;i<m_LightList.Length;i++)
		{
			if(i==m_CurrentSkyBox)
			{
				m_LightList[i].gameObject.SetActive(true);
			}
			else
			{
				m_LightList[i].gameObject.SetActive(false);
			}
		}
		
		// Enable fog
		RenderSettings.fog = true;
		
		// Set the fog color
		if(m_CurrentSkyBox>=0 && m_CurrentSkyBox<FogColorList.Length)
		{
			RenderSettings.fogColor = FogColorList[m_CurrentSkyBox];
		}
		else
		{
			RenderSettings.fogColor = Color.white;
		}
		
		// Set the ambient lighting
		if(m_CurrentSkyBox>=0 && m_CurrentSkyBox<AmbientLightList.Length)
		{
			RenderSettings.ambientLight = AmbientLightList[m_CurrentSkyBox];
		}
		else
		{
			RenderSettings.ambientLight = Color.white;
		}
	}

	// Show Help window
	void HelpWindow(int id)
	{
		//GUI.Label(new Rect(12, 25, 240, 20), "Skybox: " + string.Format("{0:00}/{1:00}",m_CurrentSkyBox+1, m_SkyboxList.Length) + " (" + m_SkyboxList[m_CurrentSkyBox].name +")");
		GUI.Label(new Rect(12, 25, 240, 20), "W/S/A/D: Move player");
		GUI.Label(new Rect(12, 50, 240, 20), "Q/E: Switch Skybox");
	}

	// Show Info window
	void InfoWindow(int id)
	{
		GUI.Label(new Rect(15, 25, 240, 20), "Fantasy Skybox FREE 1.2");
		GUI.Label(new Rect(15, 50, 240, 20), "www.ge-team.com/pages");
	}

#endregion Functions
	
}
