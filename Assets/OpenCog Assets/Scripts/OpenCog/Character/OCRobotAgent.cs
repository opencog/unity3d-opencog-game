
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

using System;
using System.Collections;
using Behave.Runtime;
using OpenCog.Actions;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ProtoBuf;
using UnityEngine;
using Tree = Behave.Runtime.Tree;
using TreeType = BLOpenCogCharacterBehaviours.TreeType;

namespace OpenCog
{

/// <summary>
/// The OpenCog OCRobotAgent.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposeProperties]
[Serializable]
#endregion
public class OCRobotAgent : OCMonoBehaviour, IAgent
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------

	private Tree m_Tree;

	private Hashtable m_IdleParams;

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------


			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------	

	#region Constructors

	//---------------------------------------------------------------------------
		
	/// <summary>
	/// Initializes a new instance of the <see cref="OpenCog.OCRobotAgent"/> class.
	/// Generally, intitialization should occur in the Start function.
	/// </summary>
	public OCRobotAgent()
	{
	}			

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public IEnumerator Start()
	{
			m_Tree =
				BLOpenCogCharacterBehaviours.InstantiateTree
				( TreeType.CharacterBehaviours_RobotExploreBehaviour
				, this
				)
			;

			while(Application.isPlaying && m_Tree != null)
			{
				yield return new WaitForSeconds (1.0f / m_Tree.Frequency);
				AIUpdate();
			}


	}

	public BehaveResult	 Tick (Tree sender, bool init)
	{
//			Debug.Log
//			(
//				"Got ticked by unhandled " + (BLOpenCogCharacterBehaviours.IsAction( sender.ActiveID ) ? "action" : "decorator")
//			+ ( BLOpenCogCharacterBehaviours.IsAction( sender.ActiveID )
//				? ((BLOpenCogCharacterBehaviours.ActionType)sender.ActiveID).ToString()
//				: ((BLOpenCogCharacterBehaviours.DecoratorType)sender.ActiveID).ToString()
//				)
//			);

			return BehaveResult.Success;
	}

//	public BehaveResult TickIdleAction(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data)
//	{
//			Debug.Log("In Robot Idle...");
//
//			return BehaveResult.Success;
//	}

	public BehaveResult IdleAction
	{
			// tick handler
			get
			{
				if(animation.IsPlaying("idle"))
				{
					return BehaveResult.Running;
				}

				if(animation.isPlaying)
				{
					Debug.Log("In OCRobotAgent.IdleAction, Failure");
					return BehaveResult.Failure;
				}

				OCIdleAction idleAction = gameObject.GetComponent<OCIdleAction>();
				idleAction.Execute();

				Debug.Log("In OCRobotAgent.IdleAction, Success");
				return BehaveResult.Success;
			}

			// reset handler
			set
			{
			}
	}



	public void	 Reset (Tree sender)
	{
	}

	public int	 SelectTopPriority (Tree sender, params int[] IDs)
	{
			return IDs[0];
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
			
	private void AIUpdate()
	{
			m_Tree.Tick();
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Member Classes

	//---------------------------------------------------------------------------		

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCRobotAgent

}// namespace OpenCog




