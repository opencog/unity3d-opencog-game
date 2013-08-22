
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

namespace OpenCog.Scenes.BlockSetViewer
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
	
	[SerializeField] private OpenCog.BlockSet.OCBlockSet[] blockSetList;
	private int index = 0;
	private Vector2 scrollPosition;

	private OpenCog.Scenes.BlockSetViewer.OCBlockSetViewer viewer;
	private OpenCog.BlockSet.OCBlockSet blockSet;

			
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
		index = Mathf.Clamp(index, 0, blockSet.BlockCount);
		BuildBlock( blockSet.GetBlock(index) );
	}

	void Start() {
		viewer = GetComponent<OpenCog.Scenes.BlockSetViewer.OCBlockSetViewer>();
		viewer.SetBlockSet( blockSetList[index] );
	}

	void OnGUI() {
		int oldIndex = index;
		Rect position = new Rect(0, 0, 180, Screen.height);
		index = DrawList(position, index, blockSetList, ref scrollPosition);

		if(index != oldIndex) {
			viewer.SetBlockSet( blockSetList[index] );
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
	
	private static int DrawList(Rect position, int selected, OpenCog.BlockSet.OCBlockSet[] list, ref Vector2 scrollPosition) {
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

	private void BuildBlock(OpenCog.BlockSet.BaseBlockSet.OCBlock block) {
		renderer.material = block.Atlas.Material;
		MeshFilter filter = GetComponent<MeshFilter>();
		block.Build().ToMesh(filter.mesh);
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




