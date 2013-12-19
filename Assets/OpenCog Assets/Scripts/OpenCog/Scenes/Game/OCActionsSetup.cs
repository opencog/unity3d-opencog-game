
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
using OpenCog.Actions;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using Transform = UnityEngine.Transform;
using GameObject = UnityEngine.GameObject;
using System.Collections.Generic;
using System.Linq;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog
{

/// <summary>
/// The OpenCog OCActionsSetup.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCActionsSetup : OCMonoBehaviour
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

	public void Awake()
	{
		CullUselessChildren(gameObject);
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	private void CullUselessChildren(GameObject parent)
	{
		foreach(Transform child in parent.transform)
		{
			List<OCAction> actions = 
				child.gameObject.GetComponentsInChildren<OCAction>(true).ToList();
				
			//actions.AddRange(child.gameObject.GetComponents<OCAction>().ToList());
				
			if(actions == null || actions.Count == 0)
			{
				GameObject.Destroy(child.gameObject);
			}
			else
			{
				if(child.gameObject.GetComponent<OCAction>() == null)
					CullUselessChildren(child.gameObject);
			}
		}
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCActionsSetup

}// namespace OpenCog




