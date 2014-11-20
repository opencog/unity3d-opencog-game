
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
using OpenCog.Embodiment;
using OpenCog.Utilities.Logging;

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
		get {
			return this._blockType;
		}
		set {
			_blockType = value;
			if(_map != null)
				_goalBlockType = _map.GetBlockSet().GetBlock(_blockType);
		}
	}	

	public OCBlock GoalBlockType 
	{
		get {
			return this._goalBlockType;
		}
		set {
			_goalBlockType = value;
		}
	}		
		
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------
		
	public IEnumerator Start()
	{
		_map = OCMap.Instance;//(OCMap)GameObject.FindObjectOfType (typeof(OCMap));
			
		_goalBlockType = 	_map.GetBlockSet().GetBlock(_blockType);
				
		List3D<OCChunk> chunks = _map.GetChunks ();
			
		// Since this is probably bogging down the gameplay, switch it to block creation only.
		FindGoalBlockPositionInChunks(chunks);
			
		while (Application.isPlaying) 
		{
			yield return new WaitForSeconds (1.0f);
			UpdateGoal();
		}
//		yield return 0;
	}
		
	public void UpdateGoal()
	{

		//List3D<OCChunk> chunks = _map.GetChunks ();
			
		// Since this is probably bogging down the gameplay, switch it to block creation only.
		//FindGoalBlockPositionInChunks(chunks);
			
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
					
				OpenCog.Embodiment.OCPhysiologicalEffect nearHomeEffect = OCPhysiologicalEffect.CreateInstance<OCPhysiologicalEffect>();
				nearHomeEffect.CostLevelProp = OpenCog.Embodiment.OCPhysiologicalEffect.CostLevel.NONE;
			
				nearHomeEffect.FitnessChange = integrityChange;
					
				//UnityEngine.Debug.Log ("Increasing Integrity by '" + integrityChange.ToString() + "'");
			
				agiPhysModel.ProcessPhysiologicalEffect(nearHomeEffect);
			}
		}
			
		if(GoalBlockPos != Vector3i.zero && _map.GetBlock(GoalBlockPos).IsEmpty())
		{
			OpenCog.Utility.Console.Console console = OpenCog.Utility.Console.Console.Instance;
				console.AddConsoleEntry("I perceive no more " + _goalBlockType.GetName() + " blocks in my world.", "AGI Robot", OpenCog.Utility.Console.Console.ConsoleEntry.Type.SAY);
			System.Console.WriteLine(OCLogSymbol.RUNNING + "No more " + _goalBlockType.GetName() + " are reported");
			
			GoalBlockPos = Vector3i.zero;

			OCAction[] actions = gameObject.GetComponentsInChildren<OCAction>();

			foreach(OCAction action in actions)
			{
				if(action.EndTarget != null) action.EndTarget.transform.position = Vector3.zero;
				if(action.StartTarget != null) action.StartTarget.transform.position = Vector3.zero;
			}
		}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	public void FindGoalBlockPositionInChunks(List3D<OCChunk> chunks)
	{
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
		
		//bool doesGoalExist = false;
		
		//Get rid of some Nullreference errors if the avatar
		//cannot connect properly. 
		if(GoalBlockType == null)
		{
			return;
		}
		
		GameObject goalBlockChild = GameObject.Find(GoalBlockType.GetName());
			
		if(goalBlockChild == null)
		{
			return;
		}
		GameObject goalBlocks = goalBlockChild.transform.parent.gameObject;
			
		if(goalBlocks == null)
		{
			return;
		}
		foreach(Transform candidateTransform in goalBlocks.transform)
		{
			GameObject candidateBlock = candidateTransform.gameObject;
			Vector3 candidatePos = candidateBlock.transform.position;
			Vector3 candidateVec = candidatePos - sourcePos;
			//OCBlockData blockData = _map.GetBlock(new Vector3i(candidatePos.x, candidatePos.y, candidatePos.z));
			//if(!blockData.IsEmpty() && blockData.block.GetName() == _goalBlockType.GetName())
			{
				//doesGoalExist = true;
				if(candidateVec.sqrMagnitude < distanceVec.sqrMagnitude)
				{
					GoalBlockPos = new Vector3i((int)candidatePos.x, (int)candidatePos.y, (int)candidatePos.z);
					distanceVec = candidateVec;
					
					MoveTargetsIfNecessary ();

					System.Console.WriteLine(OCLogSymbol.RUNNING + "I found a " + _goalBlockType.GetName() + " block nearby: " + GoalBlockPos + "!");
				}
			}
		}


//			//distanceVec = new Vector3(1000,1000,1000);
//			for(int cx=chunks.GetMinX(); cx<chunks.GetMaxX(); ++cx)
//			{
//				for(int cy=chunks.GetMinY(); cy<chunks.GetMaxY(); ++cy)
//				{
//					for(int cz=chunks.GetMinZ(); cz<chunks.GetMaxZ(); ++cz)
//					{
//						Vector3i chunkPos = new Vector3i(cx, cy, cz);
//						OCChunk chunk = chunks.SafeGet(chunkPos);
//						if(chunk != null)
//						{
//							for(int z=0; z<OCChunk.SIZE_Z; z++)
//							{
//								for(int x=0; x<OCChunk.SIZE_X; x++)
//								{
//									for(int y=0; y<OCChunk.SIZE_Y; y++)
//									{
//										Vector3i localPos = new Vector3i(x, y, z);
//										OCBlockData blockData = chunk.GetBlock(localPos);
//										Vector3i candidatePos = OCChunk.ToWorldPosition(chunk.GetPosition(), localPos);
//										Vector3 candidateVec = ((Vector3)candidatePos) - sourcePos;
//										if(!blockData.IsEmpty() && blockData.block.GetName() == _goalBlockType.GetName())
//										{
//											doesGoalExist = true;
//											if(candidateVec.sqrMagnitude < distanceVec.sqrMagnitude)
//											{
//												GoalBlockPos = candidatePos;
//												distanceVec = candidateVec;
//												
//												MoveTargetsIfNecessary ();
//
//												Debug.Log("We found some " + _goalBlockType.GetName() + " nearby: " + GoalBlockPos + "!");
//											}
//										}
//									}
//								}
//							}
//						}
//					}
//				}
//			}
		
	}
		
	/// <summary>
	/// Moves the targets if necessary.
	/// </summary>
	public void MoveTargetsIfNecessary ()
	{
		if(_ShouldMoveTargets)
		{
			OCAction[] actions = gameObject.GetComponentsInChildren<OCAction>();
		
			foreach(OCAction action in actions)
			{
				action.EndTarget.transform.position = new Vector3(GoalBlockPos.x, GoalBlockPos.y, GoalBlockPos.z);
				action.StartTarget.transform.position = gameObject.transform.position;
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




