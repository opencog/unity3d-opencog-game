
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
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using UnityEngine;
using OpenCog.Entities;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog
{

/// <summary>
/// The OpenCog OCGameStateManager. Controls behaviors like pausing.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCGameStateManager : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	private static OCGameStateManager _manager;
	private static GameObject _player;
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		
	private static OCGameStateManager Manager {
		get {
			if(_manager == null)
					_manager = (OCGameStateManager) GameObject.FindObjectOfType( typeof(OCGameStateManager) );

			return _manager;
		}
	}

	public static bool IsPause {
		set {
			if(value) Time.timeScale = 1f/10000f;
			if(!value) Time.timeScale = 1;
			Screen.showCursor = value;
			if(value) Manager.SendMessage("OnPause", SendMessageOptions.DontRequireReceiver);
			if(!value) Manager.SendMessage("OnResume", SendMessageOptions.DontRequireReceiver);
		}
		get {
			return Time.timeScale <= 0.0001f;
		}
	}
	public static bool IsPlaying {
		set {
			IsPause = !value;
		}
		get {
			return !IsPause;
		}
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public void Start() {
		Screen.showCursor = false;
		_player = GameObject.FindGameObjectWithTag("Player");
			IsPause = false;
	}
	
	/// <summary>
	/// The main update loop for OCGameStateManager. This function is responsible for pausing, 
	/// and also handles unit tests.
	/// </summary>
	public void Update() 
	{
		


		if(Application.platform != RuntimePlatform.LinuxPlayer)
			Screen.lockCursor = !Screen.showCursor;
		
		// allow for screenshots?
		if(Input.GetKeyDown(KeyCode.Tab)) {
			Screen.showCursor = true;
			Screen.lockCursor = false;
		}
		
		//pause le game!
		if(Input.GetKeyDown(KeyCode.Escape)) {
			IsPause = !IsPause;
			_player.GetComponent<OCInputController>().enabled = !IsPause;
			_player.GetComponent<OCCharacterMotor>().enabled = !IsPause;
			_player.GetComponent<MouseLook>().enabled = !IsPause;
		}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCGameStateManager

}// namespace OpenCog




