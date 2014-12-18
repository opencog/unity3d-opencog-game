
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
using OpenCog.Master;
using OpenCog.Utilities.Logging;

#region Usings, Namespaces, and Pragmas

using System.Collections;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using OpenCog.Map;
using OpenCog.BlockSet.BaseBlockSet;
using GameObject = UnityEngine.GameObject;
using System.Linq;
using OpenCog.Utility;
using UnityEngine;
using OpenCog.Actions;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog
{

/// <summary>
/// The OpenCog OCCreateBlockEffect.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
    
#endregion
//NOTE: this class pulls the weight of actually destroying blocks for us. It is applied to the OCActions of a character; not to blocks themselves. 
public class OCCreateBlockEffect : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

    #region Private Member Data

	//---------------------------------------------------------------------------
    
	[SerializeField]
	string
		_BlockType = "Ice";
            
	//---------------------------------------------------------------------------

    #endregion

	//---------------------------------------------------------------------------

    #region Accessors and Mutators

	//---------------------------------------------------------------------------
        
	public enum CreationPosition
	{
		Target,
		Forward,
		ForwardBelow
	} 

	public CreationPosition CreationPos;

	//---------------------------------------------------------------------------

    #endregion

	//---------------------------------------------------------------------------

    #region Public Member Functions

	//---------------------------------------------------------------------------

	public void CreateBlock(Vector3i? point)
	{
		if(point.HasValue)
		{
			OCMap map = OCMap.Instance;//(OCMap)GameObject.FindSceneObjectsOfType(typeof(OCMap)).FirstOrDefault();

			OCBlock block = map.GetBlockSet().GetBlock(_BlockType);
				
			OCGoalController[] goalControllers = (OCGoalController[])GameObject.FindObjectsOfType(typeof(OCGoalController));
			
			foreach(OCGoalController goalController in goalControllers)
			{
				if(goalController.GoalBlockType == block)
				{
					goalController.FindGoalBlockPositionInChunks(map.GetChunks());
				}
			}
				
			Debug.Log(OCLogSymbol.DEBUG + "AddSelectedVoxel called from CreateBlockEffect");
			GameManager.world.voxels.AddSelectedVoxel(point.Value, -transform.forward, block);

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

}// class OCCreateBlockEffect

}// namespace OpenCog




