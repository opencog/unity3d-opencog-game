
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
using OpenCog.BlockSet.BaseBlockSet;
using OpenCog.Extensions;
using OpenCog.Map;
using UnityEngine;
using DateTime = System.DateTime;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using System.Collections.Generic;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Actions
{

/// <summary>
/// The OpenCog OCGoalController.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCGoalController : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	private Vector3i _goalBlockPos = Vector3i.zero;
		
	private OCBlock _goalBlockType;
		
	private OCMap _map;
		
	[SerializeField]
	private string _blockType = "TNT";
		
	[SerializeField]
	private bool _ShouldMoveTargets = true;
		
	[SerializeField]
	private Dictionary<string, float> _goalNameToChangeRatePerSecond = new Dictionary<string, float>()
	{
		{"Integrity", 1.0f}
	};
		
	[SerializeField]
	private float _distanceAttenuation = 1.0f;
	
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		
	public Vector3i GoalBlockPos 
	{
		get { return _goalBlockPos;}
		set { _goalBlockPos = value;}
	}

	public string BlockType 
	{
		get { return this._blockType; }
		set { _blockType = value; }
	}	
		
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------
		
	public IEnumerator Start()
	{
		_map = (OCMap)GameObject.FindObjectOfType (typeof(OCMap));
			
		_goalBlockType = 	_map.GetBlockSet().GetBlock(_blockType);
				
			
		while (Application.isPlaying) 
		{
			yield return new WaitForSeconds (10.0f);
			UpdateGoal();
		}
	}
		
	public void UpdateGoal()
	{
		Debug.Log("In OCGoalController::UpdateGoal...");	
		
		List3D<OCChunk> chunks = _map.GetChunks ();

		FindGoalBlockPositionInChunks(chunks);
			
		Vector3 sourcePos = gameObject.transform.position;
		Vector3 distanceVec = ((Vector3)GoalBlockPos) - sourcePos;
			
		float integrityChange = _goalNameToChangeRatePerSecond["Integrity"]/(distanceVec.magnitude * _distanceAttenuation);
			
		integrityChange = integrityChange / 10;
			
		UnityEngine.GameObject[] agiArray = UnityEngine.GameObject.FindGameObjectsWithTag("OCAGI");
			
		if (integrityChange > 0.04)
		{
			for (int iAGI = 0; iAGI < agiArray.Length; iAGI++)
			{
				UnityEngine.GameObject agiObject = agiArray[iAGI];
					
				OpenCog.Embodiment.OCPhysiologicalModel agiPhysModel = agiObject.GetComponent<OpenCog.Embodiment.OCPhysiologicalModel>();
					
				OpenCog.Embodiment.OCPhysiologicalEffect nearHomeEffect = new OpenCog.Embodiment.OCPhysiologicalEffect(OpenCog.Embodiment.OCPhysiologicalEffect.CostLevel.NONE);
			
				nearHomeEffect.FitnessChange = integrityChange;
					
				//UnityEngine.Debug.Log ("Increasing Integrity by '" + integrityChange.ToString() + "'");
			
				agiPhysModel.ProcessPhysiologicalEffect(nearHomeEffect);
			}
		}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	private void FindGoalBlockPositionInChunks(List3D<OCChunk> chunks)
	{
		//GoalBlockPos = Vector3i.zero;
		Vector3 sourcePos = gameObject.transform.position;
		Vector3 distanceVec = ((Vector3)GoalBlockPos) - sourcePos;
			
//		//		if(distanceVec.y < -1.0f + 0.5f && distanceVec.y > -1.0f - 0.5f)
//		if(distanceVec.sqrMagnitude < 2.25f)
//		{
//			Debug.Log("We've arrived at our goal TNT block...");
//			map.SetBlockAndRecompute(new OCBlockData(), TargetBlockPos);
//			TargetBlockPos = Vector3i.zero;
//
//			OCAction[] actions = gameObject.GetComponentsInChildren<OCAction>();
//
//			foreach(OCAction action in actions)
//			{
//				action.EndTarget.transform.position = Vector3.zero;
//				action.StartTarget.transform.position = Vector3.zero;
//			}
//		}
		
			bool doesGoalExist = false;

			//distanceVec = new Vector3(1000,1000,1000);
			for(int cx=chunks.GetMinX(); cx<chunks.GetMaxX(); ++cx)
			{
				for(int cy=chunks.GetMinY(); cy<chunks.GetMaxY(); ++cy)
				{
					for(int cz=chunks.GetMinZ(); cz<chunks.GetMaxZ(); ++cz)
					{
						Vector3i chunkPos = new Vector3i(cx, cy, cz);
						OCChunk chunk = chunks.SafeGet(chunkPos);
						if(chunk != null)
						{
							for(int z=0; z<OCChunk.SIZE_Z; z++)
							{
								for(int x=0; x<OCChunk.SIZE_X; x++)
								{
									for(int y=0; y<OCChunk.SIZE_Y; y++)
									{
										Vector3i localPos = new Vector3i(x, y, z);
										OCBlockData blockData = chunk.GetBlock(localPos);
										Vector3i candidatePos = OCChunk.ToWorldPosition(chunk.GetPosition(), localPos);
										Vector3 candidateVec = ((Vector3)candidatePos) - sourcePos;
										if(!blockData.IsEmpty() && blockData.block.GetName() == _goalBlockType.GetName())
										{
											doesGoalExist = true;
											if(candidateVec.sqrMagnitude < distanceVec.sqrMagnitude)
											{
												GoalBlockPos = candidatePos;
												distanceVec = candidateVec;
												
												if(_ShouldMoveTargets)
												{
													OCAction[] actions = gameObject.GetComponentsInChildren<OCAction>();
	
													foreach(OCAction action in actions)
													{
														action.EndTarget.transform.position = new Vector3(GoalBlockPos.x, GoalBlockPos.y, GoalBlockPos.z);
														action.StartTarget.transform.position = gameObject.transform.position;
													}
												}

												Debug.Log("We found some " + _goalBlockType.GetName() + " nearby: " + GoalBlockPos + "!");
											}
										}
									}
								}
							}
						}
					}
				}
			}

			if(GoalBlockPos != Vector3i.zero && (!doesGoalExist || _map.GetBlock(GoalBlockPos).IsEmpty()))
			{
				Debug.Log("No more " + _goalBlockType.GetName() + "... :(");
				
				GoalBlockPos = Vector3i.zero;

				OCAction[] actions = gameObject.GetComponentsInChildren<OCAction>();

				foreach(OCAction action in actions)
				{
					action.EndTarget.transform.position = Vector3.zero;
					action.StartTarget.transform.position = Vector3.zero;
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

}// class OCGoalController

}// namespace OpenCog




