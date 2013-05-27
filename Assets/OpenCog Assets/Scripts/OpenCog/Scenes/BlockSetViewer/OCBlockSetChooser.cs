
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

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Scenes.BlockSetViewwer
{

/// <summary>
/// The OpenCog OCBlockSetChooser.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCBlockSetChooser : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	[SerializeField] private OpenCog.BlockSet.OCBlockSet[] _blockSetList;
	private int _index = 0;
	// Member is not static, but a variable with the same name is used in DrawList
	private UnityEngine.Vector2 scrollPosition;

	private OCBlockSetViewer _viewer;
			
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

	public void SetBlockSet(OpenCog.BlockSet.OCBlockSet blockSet) {
		this.blockSet = blockSet;
		index = Mathf.Clamp(_index, 0, blockSet.GetBlockCount());
		BuildBlock( blockSet.GetBlock(index) );
	}

	public void Start() {
		_viewer = GetComponent<BlockSetViewer>();
		_viewer.SetBlockSet( _blockSetList[index] );
	}
	
	public void OnGUI() {
		int oldIndex = index;
		Rect position = new Rect(0, 0, 180, Screen.height);
		_index = DrawList(position, _index, blockSetList, ref scrollPosition);
		
		if(_index != oldIndex) {
			_viewer.SetBlockSet( _blockSetList[_index] );
		}
		
		GUILayout.BeginArea( new Rect(0, 0, Screen.width, Screen.height) );
		GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
				if( GUILayout.Button("Back") ) {
					Application.LoadLevel("MainMenu");
				}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	private static int DrawList(UnityEngine.Rect position, int selected, OpenCog.BlockSet.OCBlockSet[] list, ref UnityEngine.Vector2 scrollPosition) {
		GUILayout.BeginArea(position, GUI.skin.box);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		for(int i=0; i<list.Length; i++) {
			if( DrawItem(list[i].name, i == selected) ) {
				selected = i;
				Event.current.Use();
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		return selected;
	}

	private static bool DrawItem(string name, bool selected) {
		Rect position = GUILayoutUtility.GetRect(new GUIContent(name), GUI.skin.box);
		if(selected) GUI.Box(position, GUIContent.none);
		
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.padding = GUI.skin.box.padding;
		style.alignment = TextAnchor.MiddleLeft;
		
		GUI.Label(position, name, style);
		
		return Event.current.type == EventType.MouseDown && Event.current.button == 0 && position.Contains(Event.current.mousePosition);
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCBlockSetChooser

}// namespace OpenCog




