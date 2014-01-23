
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
using OpenCog.Utility;
using UnityEngine;

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
	public class OCTransferBlockEffect : OCMonoBehaviour
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
		
		public void TransferBlock(Vector3i? origin, Vector3i? destination)
		{
			if(origin.HasValue && destination.HasValue)
			{
				OCMap map = OCMap.Instance;//(OCMap)GameObject.FindSceneObjectsOfType(typeof(OCMap)).FirstOrDefault();
				
				OCGoalController[] goalControllers = (OCGoalController[])GameObject.FindObjectsOfType(typeof(OCGoalController));

				foreach(Transform battery in map.BatteriesSceneObject.transform)
				{
					if(VectorUtil.AreVectorsEqual(battery.position, new Vector3((float)origin.Value.x, (float)origin.Value.y, (float)origin.Value.z)))
						battery.position = new Vector3((float)destination.Value.x, (float)destination.Value.y, (float)destination.Value.z);
				}

				OCBlockData block = map.GetBlock(origin.Value);

				map.SetBlockAndRecompute(block, destination.Value);
				map.SetBlockAndRecompute(OCBlockData.CreateInstance<OCBlockData>().Init(null, origin.Value), origin.Value);
				
				foreach(OCGoalController goalController in goalControllers)
				{
					if(goalController.GoalBlockType == block.block)
					{
						goalController.FindGoalBlockPositionInChunks(map.GetChunks());
					}
				}
				
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
		
	}// class OCDestroyBlockEffect
	
}// namespace OpenCog
