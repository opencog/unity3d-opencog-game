
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
using OpenCog.Utilities.Logging;

#region Usings, Namespaces, and Pragmas

using System.Collections;
using System.Collections.Generic;
using OpenCog.Attributes;
using OpenCog.Extensions;
using OpenCog.Network;
using GUISkin = UnityEngine.GUISkin;
using GUIStyle = UnityEngine.GUIStyle;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using OpenCog.Utility;

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

	private bool _isFeelingTextureMapInitialized = false;

	private bool _isShowingPanel = true;
	// the skin the console will use
	private UnityEngine.GUISkin _panelSkin = null;
	// style for label
	private UnityEngine.GUIStyle _boxStyle;
	// A map from feeling names to textures. The texture needs to be created dynamically
	// whenever a new feeling is added.
	private Dictionary<string, UnityEngine.Texture2D> _feelingTextureMap;
	// We need to initialize the feeling to texture map at the first time of obtaining the
	// feeling information.


	private OCConnectorSingleton _connector;

	private UnityEngine.Rect _panel;

	private UnityEngine.Vector2 _scrollPosition;
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
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is awake.");
	}

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start()
	{
		_connector = GetComponent<OCConnectorSingleton>() as OCConnectorSingleton;
		_feelingTextureMap = new Dictionary<string, UnityEngine.Texture2D>();

		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is started.");
	}

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	public void Update()
	{
		//System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is updated.");	
	}
		
	/// <summary>
	/// Reset this instance to its default values.
	/// </summary>
	public void Reset()
	{
		Uninitialize();
		Initialize();
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is reset.");	
	}

	/// <summary>
	/// Raises the enable event when OCFeelingPanel is loaded.
	/// </summary>
	public void OnEnable()
	{
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is enabled.");
	}

	/// <summary>
	/// Raises the disable event when OCFeelingPanel goes out of scope.
	/// </summary>
	public void OnDisable()
	{
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is disabled.");
	}

	/// <summary>
	/// Raises the destroy event when OCFeelingPanel is about to be destroyed.
	/// </summary>
	public void OnDestroy()
	{
		Uninitialize();
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is about to be destroyed.");
	}

	public void ShowPanel()
	{
		_isShowingPanel = true;
	}

	public void HidePanel()
	{
		_isShowingPanel = false;
	}

	public void OnGUI()
	{
		if(_panelSkin != null)
		{
			UnityEngine.GUI.skin = _panelSkin;
		}

		if(_isShowingPanel)
		{
			_panel = new UnityEngine.Rect(UnityEngine.Screen.width * 0.65f, UnityEngine.Screen.height * 0.7f, UnityEngine.Screen.width * 0.35f, UnityEngine.Screen.height * 0.3f);
			_panel = UnityEngine.GUI.Window(2, _panel, FeelingMonitorPanel, gameObject.name + " Feeling Panel");
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
		Dictionary<string, float> feelingValueMap = _connector.FeelingValueMap;
		if(feelingValueMap.Count == 0)
		{
			return;
		}

		_scrollPosition = UnityEngine.GUILayout.BeginScrollView(_scrollPosition);

		float feelingBarWidth = UnityEngine.Screen.width * 0.3f;

		_boxStyle = _panelSkin.box;
		lock(feelingValueMap)
		{
			int topOffset = 5;
			foreach(string feeling in feelingValueMap.Keys)
			{
				if(!_isFeelingTextureMapInitialized)
				{
					float r = UnityEngine.Random.value;
					float g = UnityEngine.Random.value;
					float b = UnityEngine.Random.value;
					UnityEngine.Color c = new UnityEngine.Color(r, g, b, 0.6f);
					UnityEngine.Texture2D t = new UnityEngine.Texture2D(1, 1);
					t.SetPixel(0, 0, c);
					t.Apply();
					_feelingTextureMap[feeling] = t;
				}
				float value = feelingValueMap[feeling];

				// Set the texture of background.
				_boxStyle.normal.background = _feelingTextureMap[feeling];
				UnityEngine.GUILayout.BeginHorizontal();
				UnityEngine.GUILayout.Label(feeling + ": ", _panelSkin.label, UnityEngine.GUILayout.MaxWidth(_panel.width * 0.3f));
				UnityEngine.GUILayout.Box("", _boxStyle, UnityEngine.GUILayout.Width(feelingBarWidth * value), UnityEngine.GUILayout.Height(16));
				UnityEngine.GUILayout.EndHorizontal();
				topOffset += 15;
			}
			// We only need to initialize the map at the first time.
			if(!_isFeelingTextureMapInitialized)
			{
				_isFeelingTextureMapInitialized = true;
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




