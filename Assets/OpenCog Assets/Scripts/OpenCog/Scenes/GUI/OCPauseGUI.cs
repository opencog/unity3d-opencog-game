
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

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog
{

/// <summary>
/// The OpenCog OCPauseGUI.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCPauseGUI : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	

			
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

	public void OnResume() {
		enabled = false;
	}
	
	public void OnPause() {
		enabled = true;
	}
	

	public void OnGUI() {
		GUILayout.BeginArea( GetMenuArea() );
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
					DrawMenu();
				GUILayout.EndVertical();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	private void DrawMenu() {
		if( GUILayout.Button("Resume", GUILayout.ExpandWidth(false)) ) {
			OCGameStateManager.IsPlaying = true;
		}
		DrawSunSlider();
		GUILayout.Box(_help, GUILayout.ExpandWidth(false));
		if( GUILayout.Button("Menu", GUILayout.ExpandWidth(false)) ) {
			OCGameStateManager.IsPlaying = true;
			Screen.showCursor = true;
			Application.LoadLevel("MainMenu");
		}
		if( GUILayout.Button("Quit", GUILayout.ExpandWidth(false)) ) {
			Application.Quit();
		}
	}
	
	
	private static UnityEngine.Rect GetMenuArea() {
		float offset = Screen.width*0.08f;
		Rect rect = new UnityEngine.Rect(offset, 0, 0, 0);
		rect.xMax = Screen.width;
		rect.yMax = Screen.height-offset;
		return rect;
	}
	
	private void DrawSunSlider() {
		const float min = (float) OpenCog.Map.Lighting.OCSunLightComputer.MIN_LIGHT / OpenCog.Map.Lighting.OCSunLightComputer.MAX_LIGHT;
		const float max = 1;
		Vector3 color = (Vector4)RenderSettings.ambientLight;
		color.Normalize();
		float light = RenderSettings.ambientLight.r;
		
		GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label("Sun");
			light = GUILayout.HorizontalSlider(light, min, max, GUILayout.Width(Screen.width/3f) );
		GUILayout.EndHorizontal();
		
		light = Mathf.Clamp(light, min, 1f);
		RenderSettings.ambientLight = new Color(light, light, light, 1f);
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	private const string _help = "Esc - Pause/Resume\n E - Open the inventory";

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCPauseGUI

}// namespace OpenCog




