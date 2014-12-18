
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
using UnityEngine;
using OpenCog.Utilities.Logging;

#region Usings, Namespaces, and Pragmas

using System.Collections;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using OpenCog.Map;
using GameObject = UnityEngine.GameObject;
using System.Linq;
using System.Collections.Generic;
using OpenCog.Actions;
using OpenCog.BlockSet.BaseBlockSet;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog
{

/// <summary>
/// The OpenCog OCDestroyBlockEffect.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
    
#endregion
public class OCDestroyBlockEffect : OCMonoBehaviour
{
	//cheese! I did this so OcUnitTests could see if any blocks had been destoryed XD
	private int blocksDestroyed = 0;

	public int BlocksDestroyed{ get { return blocksDestroyed; } }
	    
	//NOTE: this function pulls the weight of actually destroying blocks for us. It is applied to the OCActions of a character; not to blocks themselves. 
	public void DestroyBlock(Vector3i? point)
	{
		if(point.HasValue)
		{
			OCMap map = OCMap.Instance;//(OCMap)GameObject.FindSceneObjectsOfType(typeof(OCMap)).FirstOrDefault();
			
			//for updating goal controllers after deletion.
			OCBlock blockType = map.GetBlock(point.Value).block;
			OCGoalController[] goalControllers = (OCGoalController[])GameObject.FindObjectsOfType(typeof(OCGoalController));
			
			//actually sets the block to null and recomputes the chunk. 
			Debug.Log(OCLogSymbol.DEBUG + "DeleteSelectedVoxel called from CreateBlockEffect");
			GameManager.world.voxels.DeleteSelectedVoxel(point.Value);
			
			//re-update goals concerned with this block type
			foreach(OCGoalController goalController in goalControllers)
			{
				if(goalController.GoalBlockType == blockType)
				{
					goalController.FindGoalBlockPositionInChunks(map.GetChunks());
				}
			}

			blocksDestroyed++;
		}
	}

	    
}// class OCDestroyBlockEffect

}// namespace OpenCog




