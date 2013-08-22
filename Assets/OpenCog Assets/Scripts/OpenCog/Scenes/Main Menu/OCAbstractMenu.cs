
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

namespace OpenCog.Scenes.MainMenu
{

/// <summary>
/// The OpenCog OCAbstractMenu.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCAbstractMenu : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	[SerializeField] protected UnityEngine.Font _font;
			
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

	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	protected virtual void OnGUI() {
		GUI.depth = -1;
		
		Font oldFont = GUI.skin.font;
		GUI.skin.font = _font;
		
		GUILayout.BeginArea( GetMenuArea() );
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
					OnMenuGUI();
				GUILayout.EndVertical();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		
		GUI.skin.font = oldFont;
	}
	
	private static UnityEngine.Rect GetMenuArea() {
		float offset = Screen.width*0.08f;
		UnityEngine.Rect rect = new Rect(offset, 0, 0, 0);
		rect.xMax = Screen.width;
		rect.yMax = Screen.height-offset;
		return rect;
	}
	
	protected virtual void OnMenuGUI() {
		
	}
	
	protected void SwitchTo<T>() where T : OCMonoBehaviour {
		enabled = false;
		GetComponent<T>().enabled = true;
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCAbstractMenu

}// namespace OpenCog




