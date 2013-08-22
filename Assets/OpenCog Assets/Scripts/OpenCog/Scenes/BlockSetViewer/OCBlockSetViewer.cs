
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
/// The OpenCog OCBlockSetViewer.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCBlockSetViewer : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	private OpenCog.BlockSet.OCBlockSet blockSet;

	private int index = 0;
	// Member is not static, but a variable with the same name is used in DrawList
	private UnityEngine.Vector2 scrollPosition;
			
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

	public void OnGUI() {
		Rect rect = new Rect(Screen.width-180, 0, 180, Screen.height);
		int oldIndex = index;
		index = DrawList(rect, index, blockSet.Blocks, ref scrollPosition);
		if(oldIndex != index) {
			BuildBlock( blockSet.GetBlock(index) );
		}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	private void BuildBlock(OpenCog.BlockSet.BaseBlockSet.OCBlock block) {
		renderer.material = block.Atlas.Material;
		MeshFilter filter = GetComponent<MeshFilter>();
		block.Build().ToMesh(filter.mesh);
	}

	private static int DrawList(UnityEngine.Rect position, int selected, OpenCog.BlockSet.BaseBlockSet.OCBlock[] list, ref UnityEngine.Vector2 scrollPosition) {
		GUILayout.BeginArea(position, GUI.skin.box);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		for(int i=0; i<list.Length; i++) {
			if(list[i] == null) continue;
			if( DrawItem(list[i], i == selected) ) {
				selected = i;
				Event.current.Use();
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		return selected;
	}
	
	private static bool DrawItem(OpenCog.BlockSet.BaseBlockSet.OCBlock block, bool selected) {
		Rect position = GUILayoutUtility.GetRect(0, 40, GUILayout.ExpandWidth(true));
		if(selected) GUI.Box(position, GUIContent.none);
		
		GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
		labelStyle.alignment = TextAnchor.MiddleLeft;
		
		Rect texRect = new Rect(position.x+4, position.y+4, position.height-8, position.height-8);
		block.DrawPreview(texRect);
		
		Rect labelRect = position;
		labelRect.xMin = texRect.xMax+4;
		GUI.Label(labelRect, block.GetName(), labelStyle);
		
		return Event.current.type == EventType.MouseDown && Event.current.button == 0 && position.Contains(Event.current.mousePosition);
	}

	public void SetBlockSet(OpenCog.BlockSet.OCBlockSet blockSet) {
		this.blockSet = blockSet;
		index = Mathf.Clamp(index, 0, blockSet.BlockCount);
		BuildBlock( blockSet.GetBlock(index) );
	}


	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCBlockSetViewer

}// namespace OpenCog




