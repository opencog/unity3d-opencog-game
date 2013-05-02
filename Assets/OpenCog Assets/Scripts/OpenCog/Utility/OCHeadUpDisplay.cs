
/// Unity3D OpenCog World Embodiment Program
/// Copyright (C) 2013  Novamente			
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as
/// published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#region Usings, Namespaces, and Pragmas

using System.Collections;
using System.Collections.Generic;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog
{

/// <summary>
/// The OpenCog HUD.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCHeadUpDisplay : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
//	private Avatar m_selectedAvatar;
//
//	private Player m_player;

	private bool m_isShowingForcePanel = false;

	private bool m_isForceIncrease = true;

	private bool m_showPsiPanel = false;

	private bool m_isFeelingTextureMapInitialized = false;

	// We need to initialize the demand to texture map at the first time of obtaining the
	// demand information.
	private bool m_isDemandTextureMapInitialized = false;

	private float m_feelingBoxWidth;

	private float m_demandBoxWidth;

	private float m_lastTimeRenderForcePanel;

	private float m_currentForce = 1.0f;

	//private OpenCog.Embodiment.OCSocialInteraction m_socialInteraction = null;

	private UnityEngine.Texture2D m_barTexture, m_backgroundTexture;

	private UnityEngine.Vector2 m_currentScrollPosition = new UnityEngine.Vector2(0.0f, 0.0f);

	// the skin panel will use
	private UnityEngine.GUISkin m_panelSkin;
    
	// style for label
	private UnityEngine.GUIStyle m_boxStyle;

	private UnityEngine.GUITexture m_reticuleTexture;
    
	// A map from feeling names to textures. The texture needs to be created dynamically
	// whenever a new feeling is added.
	private Dictionary<string, UnityEngine.Texture2D> m_feelingTextureMap;
    
	// A map from demand names to textures. The texture needs to be created dynamically
	// whenever a new demandis added.
	private Dictionary<string, UnityEngine.Texture2D> m_demandTextureMap;
    
	// We need to initialize the feeling to texture map at the first time of obtaining the
	// feeling information.

	private OpenCog.Network.OCConnector m_connector;

	private UnityEngine.Rect m_panel;

	private UnityEngine.Vector2 m_panelScrollPosition;



	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------


	public bool IsShowingForcePanel
	{
			get {return m_isShowingForcePanel;}
	}

	public float CurrentForceVal
	{
			get {return currentForce;}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	/// <summary>
	/// Called when the script instance is being loaded.
	/// </summary>
	public void Awake()
	{
		Initialize();
		OCLogger.Fine(gameObject.name + " is awake.");
	}

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start()
	{
		Transform reticule = gameObject.transform.Find("Reticule");
		if(reticule == null)
		{
			Debug.LogError("No \"Reticule\" game object found");
		}
		m_reticuleTexture = reticule.gameObject.GetComponent<GUITexture>();
		m_reticuleTexture.pixelInset = new Rect(Screen.width / 2, Screen.height / 2, 16, 16);

		m_feelingTextureMap = new Dictionary<string, Texture2D>();
		m_demandTextureMap = new Dictionary<string, Texture2D>();
		
		ConstructForceTexture();

		OCLogger.Fine(gameObject.name + " is started.");
	}

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	public void Update()
	{
		m_reticuleTexture.pixelInset = new Rect(Screen.width / 2, Screen.height / 2, 16, 16);

		OCLogger.Fine(gameObject.name + " is updated.");
	}
		
	/// <summary>
	/// Reset this instance to its default values.
	/// </summary>
	public void Reset()
	{
		Uninitialize();
		Initialize();
		OCLogger.Fine(gameObject.name + " is reset.");	
	}

	/// <summary>
	/// Raises the enable event when HUD is loaded.
	/// </summary>
	public void OnEnable()
	{
		OCLogger.Fine(gameObject.name + " is enabled.");
	}

	/// <summary>
	/// Raises the disable event when HUD goes out of scope.
	/// </summary>
	public void OnDisable()
	{
		OCLogger.Fine(gameObject.name + " is disabled.");
	}

	/// <summary>
	/// Raises the destroy event when HUD is about to be destroyed.
	/// </summary>
	public void OnDestroy()
	{
		Uninitialize();
		OCLogger.Fine(gameObject.name + " is about to be destroyed.");
	}

//	public IEnumerator gettingForceFromHud(OpenCog.Embodiment.OCSocialInteraction currentSocialInteraction)
//	{
//		if(currentSocialInteraction == null)
//		{
//			Debug.LogError("To show the force panel but the SocialInteractioner is null!");
//			yield break;
//		}
//		m_socialInteraction = currentSocialInteraction;
//		showForcePanel();
//		while(true)
//		{
//			yield return new WaitForSeconds(0.1f);
//			if(!isShowForcePanel)
//			{
//				break;
//			}
//		}
//		yield break;
//	}

	public void hideForcePanel()
	{
		m_isShowingForcePanel = false;
	}

	void OnGUI()
	{
		// left inset, bottom right 
		int inset = 3;
		int width = 200;
		Rect window = new Rect(inset, Screen.height - Screen.height / 3 - inset, width, Screen.height / 3);
        
		if(m_selectedAvatar == null)
		{
			GUILayout.Window(2, window, ActionWindow, "Your Action List");
		}
		else
		{
			GUILayout.Window(2, window, ActionWindow, m_selectedAvatar.gameObject.transform.name + "'s Action List");

			// Psi (feeling, demand etc.) panel controlling section
			if(m_selectedAvatar.tag == "OCA")
			{
				m_connector = selectedAvatar.GetComponent<OCConnector>() as OCConnector;
				if(m_connector != null)
				{
					ShowPsiPanel();
				}
				// If the avatar has no connector it's a puppet Avatar controlled by the console only
				if(m_panelSkin != null)
				{
					GUI.skin = m_panelSkin;
				}
			}
			else
			{
				HidePsiPanel();
				m_connector = null;
			}
            
			if(showPsiPanel)
			{
				if(m_connector != null)
				{
					float theWidth = Screen.width * 0.25f;
					float theHeight = Screen.height / 3;
					panel = new Rect(Screen.width - theWidth - inset, Screen.height - theHeight - inset, theWidth, theHeight);
					GUILayout.Window(3, panel, PsiPanel, m_selectedAvatar.gameObject.transform.name + "'s Psi States Panel",
                                     GUILayout.MinWidth(theWidth), GUILayout.MinHeight(theHeight));
				}
			}
            
		}// if

		WorldGameObject world = GameObject.Find("World").GetComponent<WorldGameObject>();
		GUIStyle theStyle = new GUIStyle();
		theStyle.normal.background = world.storedBlockTexture;
		GUI.Box(new Rect(8, 8, 40, 40), "");
		GUI.Box(new Rect(12, 12, 32, 32), "", theStyle);
		
		// the force is looping between minForce and maxForce, util the mouse button released
		if(m_isShowingForcePanel)
		{
			float currentTime = Time.time;
			float deltaForce = (currentTime - lastTimeRenderForcePanel) * maxForce;
			if(m_isForceIncrease)
			{
				m_currentForce += deltaForce;
				if(m_currentForce > maxForce)
				{
					m_currentForce = maxForce;
					m_isForceIncrease = false;
				}
			}
			else
			{
				m_currentForce -= deltaForce;
				if(m_currentForce < minForce)
				{
					m_currentForce = minForce;
					m_isForceIncrease = true;
				}
			}

			DisplayForcePanel(m_currentForce);
			
			m_lastTimeRenderForcePanel = currentTime;
		}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	/// <summary>
	/// Initializes this instance.  Set default values here.
	/// </summary>
	private void Initialize()
	{
	}
	
	/// <summary>
	/// Uninitializes this instance.  Cleanup refernces here.
	/// </summary>
	private void Uninitialize()
	{
	}

	private void ShowPsiPanel()
	{
		showPsiPanel = true;
	}

	private void HidePsiPanel()
	{
		showPsiPanel = false;
	}

	private void  showForcePanel()
	{
		m_isShowingForcePanel = true;
		m_isForceIncrease = true;
		m_currentForce = 1.0f;
		m_lastTimeRenderForcePanel = Time.time;
	}

	private void showFeelings()
	{
		Dictionary<string, float> feelingValueMap = m_connector.FeelingValueMap;
        
		//panelScrollPosition = GUILayout.BeginScrollView(scrollPosition);
		m_feelingBoxWidth = panel.width * 0.58f;

		// Display feeling levels
		if(feelingValueMap.Count == 0)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Waiting for feeling update...", GUILayout.MaxWidth(panel.width));
			GUILayout.EndHorizontal();
		}
		else
		{
			lock(feelingValueMap)
			{
				foreach(string feeling in feelingValueMap.Keys)
				{
					float value = feelingValueMap[feeling];
					// Dark color is for a contrast to the actual feeling level
					Color dark = new Color(0, 0, 0, 0.6f);

					// Just use green for now. Perhaps later we should
					// have a configurable color for each feeling
					Color c = new Color(0, 255, 0, 0.6f);
					
					// Remove old bar texture and create a new one, because each frame, 
					// the unity will rearrange size and position of everytning shown on the screen
					if(feelingTextureMap.ContainsKey(feeling))
					{
						Destroy(feelingTextureMap[feeling]);
					}
					feelingTextureMap[feeling] = constructBarTexture(value, (int)feelingBoxWidth, c, dark);
					
					// Set the texture of background.
					boxStyle.normal.background = feelingTextureMap[feeling];
				
					// Show the label and bar for the feeling
					GUILayout.BeginHorizontal();
					GUILayout.Label(feeling + ": ", panelSkin.label, GUILayout.MaxWidth(panel.width * 0.4f));
					GUILayout.Box("", boxStyle, GUILayout.Width(feelingBoxWidth), GUILayout.Height(16));
					GUILayout.EndHorizontal();
				}
				
				GUILayout.Space(16f);
				
				// We only need to initialize the map at the first time.
				if(!m_isFeelingTextureMapInitialized)
				{
					m_isFeelingTextureMapInitialized = true;
				}
			}// lock
		}// if			
	}

	/**
	 * Retrieve demands stored in OCConnector and draw bars in the panel 
	 */	
	private void ShowDemands()
	{
		Dictionary<string, float> demandValueMap = m_connector.DemandValueMap;
		string currentDemandName = m_connector.CurrentDemandName;
        
		m_demandBoxWidth = panel.width * 0.58f;

		// Display demand satisfactions (i.e. truth values)
		if(demandValueMap.Count == 0)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Waiting for demand update...", GUILayout.MaxWidth(m_panel.width));
			GUILayout.EndHorizontal();
		}
		else
		{
			lock(demandValueMap)
			{
				foreach(string demand in demandValueMap.Keys)
				{
					float value = demandValueMap[demand];
					// Dark color is for a contrast to the actual feeling level
					Color dark = new Color(0, 0, 0, 0.6f);

					// Just use blue for now. Perhaps later we should
					// have a configurable color for each demand					
					Color c = (currentDemandName == demand) ? new Color(255, 0, 0, 0.6f) : 
                                                           new Color(0, 0, 255, 0.6f);
                    
					if(m_demandTextureMap.ContainsKey(demand))
					{
						Destroy(m_demandTextureMap[demand]);
					}
					m_demandTextureMap[demand] = ConstructBarTexture(value, (int)m_demandBoxWidth, c, dark);

					// Set the texture of background.
					m_boxStyle.normal.background = m_demandTextureMap[demand];
                    
					// Draw the label and bar for the demand
					GUILayout.BeginHorizontal();
					GUILayout.Label(demand + ": ", m_panelSkin.label, GUILayout.MaxWidth(m_panel.width * 0.4f));
					GUILayout.Box("", m_boxStyle, GUILayout.Width(demandBoxWidth), GUILayout.Height(16));
					GUILayout.EndHorizontal();
				}
                
				GUILayout.Space(16f); 
                
				// We only need to initialize the map at the first time.
				if(!m_isDemandTextureMapInitialized)
				{
					m_isDemandTextureMapInitialized = true;
				}
			}// lock
		}// if
	}
    
	private void PsiPanel(int id)
	{		
		// Set box style based on skin
		if(panelSkin != null)
		{
			boxStyle = panelSkin.box;
		}
		else
		{
			boxStyle = new GUIStyle();
		}
        
		GUILayout.BeginVertical();
        
		this.showDemands(); 
		this.showFeelings(); 	
        
		GUILayout.EndVertical();
	}

	private UnityEngine.Texture2D constructBarTexture(float x, int width, UnityEngine.Color main, UnityEngine.Color background)
	{
		UnityEngine.Texture2D t = new UnityEngine.Texture2D(width, 16);
		int i = 0;
		for(; i < width * x; i++)
		{
			for(int j = 0; j < 16; j++)
			{
				t.SetPixel(i, j, main);
			}
		}
		for(; i < width; i++)
		{
			for(int j = 0; j < 16; j++)
			{
				t.SetPixel(i, j, background);
			}
		}
		t.Apply();
		return t;
	}

	private void ActionWindow(int id)
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		GUIStyle commandStyle = new GUIStyle();

		// Use the selected avatar, or use the player object if no avatar selected
		Avatar avatar = selectedAvatar;
		if(selectedAvatar == null)
		{
			avatar = player;
		}
		if(avatar == null)
		{
			return;
		}
        
		lock(avatar.AM.currentActions)
		{
			foreach(ActionKey akey in avatar.AM.currentActions.Keys)
			{
				ActionSummary a = avatar.AM.currentActions[akey] as ActionSummary;
				if(a.playerVisible)
				{
					commandStyle.normal.textColor = Color.green;
				}
				else
				{
					commandStyle.normal.textColor = Color.white;				
				}
				GUILayout.Label(a.actionObject.name + ":" + a.actionName, commandStyle);
                
			}
		}
		// End the scrollview we began above.
		GUILayout.EndScrollView();
	}

	private void constructForceTexture()
	{
		float w = WIDTH;
		float h = HEIGHT;
		float top = Screen.height / 2;
		float left = Screen.width / 2;
		
		barTex = new Texture2D((int)w, (int)h);
		
		for(int i = 0; i<w; i++)
		{
			for(int j =0; j<h; j++)
			{
				barTex.SetPixel(i, j, new Color(((float)j) / h, 1.0f - ((float)j) / h, 0.2f, 1.0f));
			}
		}
		
		barTex.Apply();

		bgTex = new Texture2D((int)w, (int)h);
				
		for(int i = 0; i<w; i++)
		{
			for(int j =0; j<h; j++)
			{
				bgTex.SetPixel(i, j, new Color(0.14f, 0.15f, 0.16f, 0.7f));
			}
		}
		
		bgTex.Apply();
	}
	
	private void displayForcePanel(float currentforce)
	{
		//Color bgColor = GUI.backgroundColor;
		float w = WIDTH;
		float h = HEIGHT;
		float top = Screen.height / 2;
		float left = Screen.width / 2;
		
		GUI.DrawTexture(new Rect(left, top, w, h), m_backgroundTexture);
		GUI.Label(new Rect(left, top + 20.0f, 60.0f, 20.0f), MAX_FORCE.ToString());
		GUI.Label(new Rect(left, top + (h - 20.0f), 60.0f, 20.0f), MIN_FORCE.ToString());
		float forceHeight = m_currentForce / MAX_FORCE * (h - 40.0f);
		
		GUI.DrawTexture(new Rect(left + 20.0f, top + (h - 20.0f - forceHeight), w / 2, forceHeight), m_barTexture, ScaleMode.ScaleAndCrop);
		GUI.Label(new Rect(left + 30.0f, top + h / 2, 100.0f, 20.0f), "Force=" + m_currentForce.ToString());
		
		
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	/// <summary>
	/// Initializes a new instance of the <see cref="OpenCog.OCHeadUpDisplay"/> class.  
	/// Generally, intitialization should occur in the Start or Awake
	/// functions, not here.
	/// </summary>
	public OCHeadUpDisplay()
	{
	}

	private const float MIN_FORCE = 1.0f;

	private const float MAX_FORCE = 200.0f;

	private const float WIDTH = 80.0f;

	private const float HEIGHT = 200.0f;

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class HUD

}// namespace OpenCog




