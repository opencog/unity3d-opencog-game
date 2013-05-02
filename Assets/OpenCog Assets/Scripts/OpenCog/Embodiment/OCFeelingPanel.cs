
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

namespace OpenCog.Embodiment
{

/// <summary>
/// The OpenCog OCFeelingPanel.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCFeelingPanel : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------

	private bool m_isFeelingTextureMapInitialized = false;

	private bool m_isShowingPanel = true;
	// the skin the console will use
	private UnityEngine.GUISkin m_panelSkin;
	// style for label
	private UnityEngine.GUIStyle m_boxStyle;
	// A map from feeling names to textures. The texture needs to be created dynamically
	// whenever a new feeling is added.
	private Dictionary<string, UnityEngine.Texture2D> m_feelingTextureMap;
	// We need to initialize the feeling to texture map at the first time of obtaining the
	// feeling information.


	private OpenCog.Network.OCConnector m_connector;

	private UnityEngine.Rect m_panel;

	private UnityEngine.Vector2 m_scrollPosition;
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		
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
		m_connector = GetComponent<Network.OCConnector>() as Network.OCConnector;
		m_feelingTextureMap = new Dictionary<string, UnityEngine.Texture2D>();

		OCLogger.Fine(gameObject.name + " is started.");
	}

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	public void Update()
	{
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
	/// Raises the enable event when OCFeelingPanel is loaded.
	/// </summary>
	public void OnEnable()
	{
		OCLogger.Fine(gameObject.name + " is enabled.");
	}

	/// <summary>
	/// Raises the disable event when OCFeelingPanel goes out of scope.
	/// </summary>
	public void OnDisable()
	{
		OCLogger.Fine(gameObject.name + " is disabled.");
	}

	/// <summary>
	/// Raises the destroy event when OCFeelingPanel is about to be destroyed.
	/// </summary>
	public void OnDestroy()
	{
		Uninitialize();
		OCLogger.Fine(gameObject.name + " is about to be destroyed.");
	}

	public void ShowPanel()
	{
		m_isShowingPanel = true;
	}

	public void HidePanel()
	{
		m_isShowingPanel = false;
	}

	public void OnGUI()
	{
		if(m_panelSkin != null)
		{
			UnityEngine.GUI.skin = m_panelSkin;
		}

		if(m_isShowingPanel)
		{
			m_panel = new UnityEngine.Rect(UnityEngine.Screen.width * 0.65f, UnityEngine.Screen.height * 0.7f, UnityEngine.Screen.width * 0.35f, UnityEngine.Screen.height * 0.3f);
			m_panel = UnityEngine.GUI.Window(2, m_panel, FeelingMonitorPanel, gameObject.name + " Feeling Panel");
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

	private void FeelingMonitorPanel(int id)
	{
		Dictionary<string, float> feelingValueMap = m_connector.FeelingValueMap;
		if(feelingValueMap.Count == 0)
		{
			return;
		}

		m_scrollPosition = UnityEngine.GUILayout.BeginScrollView(m_scrollPosition);

		float feelingBarWidth = UnityEngine.Screen.width * 0.3f;

		m_boxStyle = m_panelSkin.box;
		lock(feelingValueMap)
		{
			int topOffset = 5;
			foreach(string feeling in feelingValueMap.Keys)
			{
				if(!m_isFeelingTextureMapInitialized)
				{
					float r = UnityEngine.Random.value;
					float g = UnityEngine.Random.value;
					float b = UnityEngine.Random.value;
					UnityEngine.Color c = new UnityEngine.Color(r, g, b, 0.6f);
					UnityEngine.Texture2D t = new UnityEngine.Texture2D(1, 1);
					t.SetPixel(0, 0, c);
					t.Apply();
					m_feelingTextureMap[feeling] = t;
				}
				float value = feelingValueMap[feeling];

				// Set the texture of background.
				m_boxStyle.normal.background = m_feelingTextureMap[feeling];
				UnityEngine.GUILayout.BeginHorizontal();
				UnityEngine.GUILayout.Label(feeling + ": ", m_panelSkin.label, UnityEngine.GUILayout.MaxWidth(m_panel.width * 0.3f));
				UnityEngine.GUILayout.Box("", m_boxStyle, UnityEngine.GUILayout.Width(feelingBarWidth * value), UnityEngine.GUILayout.Height(16));
				UnityEngine.GUILayout.EndHorizontal();
				topOffset += 15;
			}
			// We only need to initialize the map at the first time.
			if(!m_isFeelingTextureMapInitialized)
			{
				m_isFeelingTextureMapInitialized = true;
			}
		}

		UnityEngine.GUILayout.EndScrollView();
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	/// <summary>
	/// Initializes a new instance of the <see cref="OpenCog.OCFeelingPanel"/> class.  
	/// Generally, intitialization should occur in the Start or Awake
	/// functions, not here.
	/// </summary>
	public OCFeelingPanel()
	{
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCFeelingPanel

}// namespace OpenCog




