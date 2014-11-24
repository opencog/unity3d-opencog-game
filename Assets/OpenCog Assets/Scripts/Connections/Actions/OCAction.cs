
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
using System.Collections.Generic;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ProtoBuf;
using UnityEngine;
using OCID = System.Guid;
using Result = Behave.Runtime.BehaveResult;
using System.Linq;
using OpenCog.Map;
using System.Reflection;
using OpenCog.Utility;
using OpenCog.Embodiment;
using OpenCog.Utilities.Logging;

namespace OpenCog
{

namespace Actions
{

/// <summary>
/// The OpenCog Action.  Defines the actions that a character can execute.
/// Supercedes the old ActionSummary, MetaAction, ActionResult, and
/// AnimationSummary interfaces.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
#pragma warning disable 0414
#endregion
public class OCAction : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
			
	/// <summary>
	/// 	Logically, actions in the game world may have up to one low-, mid-, and
	/// high-level descriptor.  This is to allow OpenCog to specify plans using
	/// the highest-level of descriptiveness possible, but technically actions
	/// have a more discrete meaning.  In the context of the game world, each 
	/// action is fully-described with exactly one low-, mid-, and high-level 
	/// descriptor.  Under-described actions are implemented using behaviour
	/// trees composed of fully-described actions, but OpenCog doesn't need to 
	/// know all these details.  Since it's possible that OpenCog will request 
	/// an action which is under-, fully-, or even over-described, we allow 
	/// actions to have a list of descriptors.  We expect that in the vast
	/// majority of cases, we'll use exactly three.
	/// </summary>
	[SerializeField]
	private List<string> _Descriptors;

	[SerializeField]
	private List<string> _Preconditions;

	[SerializeField]
	private List<string> _Invariants;

	[SerializeField]
	private List<string> _Postconditions;

	[SerializeField]
	private GameObject _Source;

	[SerializeField]
	private GameObject _StartTarget;

	[SerializeField]
	private GameObject _EndTarget;
			
	private List<OCAnimationEffect> _AnimationEffects;
	private List<OCCreateBlockEffect> _CreateBlockEffects;
	private List<OCDestroyBlockEffect> _DestroyBlockEffects;
	private List<OCTransferBlockEffect> _TransferBlockEffects;

	private OCActionController _ActionController;

	private OCMap _Map;
			
	[SerializeField]
	private bool _blockOnFail = true;
			
	[SerializeField]
	private bool _blockOnRunning = true;

	private bool _moveToTarget = false;

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
			
	public string FullName
	{
		get { return _Descriptors.Aggregate((a, b) => a + b); }
	}
			
	public List<string> Descriptors 
	{
		get {return _Descriptors;}
		set {_Descriptors = value;}
	}			
			
	public bool ShouldStart
	{
		get
		{
			bool shouldStart = true;
			foreach(OCActionCondition precondition in PreCondition.GetInvocationList())
			{
				if(_ActionController != null && _ActionController.Step != null)
				{
					if(_ActionController.Step.Arguments.Source != null)
						_Source = _ActionController.Step.Arguments.Source;
					if(_ActionController.Step.Arguments.StartTarget != null)
						_StartTarget = _ActionController.Step.Arguments.StartTarget;
					if(_ActionController.Step.Arguments.EndTarget != null)
						_EndTarget = _ActionController.Step.Arguments.EndTarget;
				}
				shouldStart &= precondition(this, new OCActionArgs(_Source, _StartTarget, _EndTarget));
				if(shouldStart == false)
				{
							if((this.FullName == "WalkForwardLeftMove" || this.FullName == "HoldBothHandsTransfer") && precondition.Method.Name != "IsNoEndTargetOrNotAnimating" && precondition.Method.Name != "IsSourceNotRunningAction") 
						System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "In OCAction.ShouldStart, Precondition Not Yet True: " + precondition.Method.Name);
					break;
				}
			}
			return shouldStart;
		}
	}
			
	public bool ShouldContinue
	{
		get
		{
			bool shouldContinue = true;
			foreach(OCActionCondition invariant in InvariantCondition.GetInvocationList())
			{
				if(_ActionController != null && _ActionController.Step != null)
				{
					if(_ActionController.Step.Arguments.Source != null)
						_Source = _ActionController.Step.Arguments.Source;
					if(_ActionController.Step.Arguments.StartTarget != null)
						_StartTarget = _ActionController.Step.Arguments.StartTarget;
					if(_ActionController.Step.Arguments.EndTarget != null)
						_EndTarget = _ActionController.Step.Arguments.EndTarget;
				}		
				shouldContinue &= invariant(this, new OCActionArgs(_Source, _StartTarget, _EndTarget));
				if(shouldContinue == false)
					break;
			}
			return shouldContinue;
		}
	}		
			
	public bool ShouldEnd
	{
		get
		{
			bool shouldEnd = true;
			foreach(OCActionCondition postcondition in PostCondition.GetInvocationList())
			{
				if(_ActionController != null && _ActionController.Step != null)
				{
					if(_ActionController.Step.Arguments.Source != null)
						_Source = _ActionController.Step.Arguments.Source;
					if(_ActionController.Step.Arguments.StartTarget != null)
						_StartTarget = _ActionController.Step.Arguments.StartTarget;
					if(_ActionController.Step.Arguments.EndTarget != null)
						_EndTarget = _ActionController.Step.Arguments.EndTarget;
				}		
				shouldEnd &= postcondition(this, new OCActionArgs(_Source, _StartTarget, _EndTarget));
				if(shouldEnd == false)
					break;
			}
			return shouldEnd;
		}
	}

	public GameObject EndTarget
	{
		get
		{
			return this._EndTarget;
		}
		set
		{
			_EndTarget = value;
		}
	}

	public GameObject Source
	{
		get
		{
			return this._Source;
		}
		set
		{
			_Source = value;
		}
	}

	public GameObject StartTarget
	{
		get
		{
			return this._StartTarget;
		}
		set
		{
			_StartTarget = value;
		}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public void Start()
	{
		_AnimationEffects = 
			gameObject.GetComponentsInChildren<OCAnimationEffect>().ToList();

		_CreateBlockEffects =
			gameObject.GetComponentsInChildren<OCCreateBlockEffect>().ToList();

		_DestroyBlockEffects =
			gameObject.GetComponentsInChildren<OCDestroyBlockEffect>().ToList();

		_TransferBlockEffects =
			gameObject.GetComponentsInChildren<OCTransferBlockEffect>().ToList();

		_Map = OCMap.Instance;//(OCMap)GameObject.FindSceneObjectsOfType(typeof(OCMap)).LastOrDefault();

		_ActionController = _Source.GetComponent<OCActionController>();

//		foreach(OCDelegate del in _Preconditions)
//		{
//			PreCondition += (OCActionCondition)del.Delegate;
//		}
//
//		foreach(OCDelegate del in _Invariants)
//		{
//			InvariantCondition += (OCActionCondition)del.Delegate;
//		}
//
//		foreach(OCDelegate del in _Postconditions)
//		{
//			PostCondition += (OCActionCondition)del.Delegate;
//		}

		foreach(string condition in _Preconditions)
		{
			PreCondition += (OCActionCondition)Delegate.CreateDelegate(typeof(OCActionCondition), typeof(OCAction).GetMethod(condition));
		}

		foreach(string condition in _Invariants)
		{
			InvariantCondition += (OCActionCondition)Delegate.CreateDelegate(typeof(OCActionCondition), typeof(OCAction).GetMethod(condition));
		}

		foreach(string condition in _Postconditions)
		{
			PostCondition += (OCActionCondition)Delegate.CreateDelegate(typeof(OCActionCondition), typeof(OCAction).GetMethod(condition));
		}

//		PreCondition += IsEndTarget;
//		InvariantCondition += IsSourceAnimating;
//		InvariantCondition += IsSourceNotRunningOtherActionsIgnoreIdle;
//		PostCondition += IsSourceRunningOtherActionsIgnoreIdle;
				
//		if(_EndTarget == null)
//			_EndTarget = GameObject.Find("EndPointStub");
//		if(_StartTarget == null)
//			_StartTarget = GameObject.Find("StartPointStub");
				
		DontDestroyOnLoad(this);
	}
			
	public Result ExecuteBehave()
	{
		return (Result)Execute();
	}
	
	
			
	//	The following can be used as Precondition, Invariant, or Postcondition
	//	delegates.
	
	public static bool IsTrue(OCAction action, OCActionArgs args)
	{
		return true;
	}
			
	public static bool IsFalse(OCAction action, OCActionArgs args)
	{
		return false;
	}

	public static bool IsGlobalInput(OCAction action, OCActionArgs args)
	{
		return UnityEngine.Input.anyKey;
	}

	public static bool IsNoGlobalInput(OCAction action, OCActionArgs args)
	{
		return !IsGlobalInput(action, args);
	}
	
	public static bool IsSourceGrounded(OCAction action, OCActionArgs args)
	{
		return args.Source.GetComponent<CharacterController>().isGrounded;
	}
			
	public static bool IsSourceNotGrounded(OCAction action, OCActionArgs args)
	{
		return !IsSourceGrounded(action, args);
	}
			
	public static bool IsEndTargetBelowClimbAngle(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePos = args.Source.transform.position;
		Vector3 endTargetPos = args.EndTarget.transform.position;
		float angle = Vector3.Angle(endTargetPos - sourcePos, Vector3.up);
				
		return angle > 45.0f;
	}
			
	public static bool IsEndTargetAboveClimbAngle(OCAction action, OCActionArgs args)
	{
		return !IsEndTargetBelowClimbAngle(action, args);
	}
			
	public static bool IsLocalAnimating(OCAction action, OCActionArgs args)
	{
		bool isAnimating = false;
				
		foreach(OCAnimationEffect afx in action._AnimationEffects)
		{
			isAnimating |= afx.Target.animation.IsPlaying(afx.StateName);
		}
				
		return isAnimating;
	}

	public static bool IsLocalNotAnimating(OCAction action, OCActionArgs args)
	{
		return !IsLocalAnimating(action, args);
	}

	public static bool IsLocalAnimatingAnythingElse(OCAction action, OCActionArgs args)
	{
		bool isAnimatingAnythingElse = false;

		foreach(OCAnimationEffect afx in action._AnimationEffects)
		{
			isAnimatingAnythingElse |= afx.IsPlayingButNotThis;
		}

		return isAnimatingAnythingElse;
	}

	public static bool IsLocalNotAnimatingAnythingElse(OCAction action, OCActionArgs args)
	{
		return !IsLocalAnimatingAnythingElse(action, args);
	}

	public static bool IsNotAnimatingAnything(OCAction action, OCActionArgs args)
	{
		return !args.Source.animation.isPlaying;
	}

	public static bool IsSourceAnimating(OCAction action, OCActionArgs args)
	{
		iTween itween = args.Source.GetComponent<iTween>();
		return itween != null && itween.isRunning;
	}

	public static bool IsSourceNotAnimating(OCAction action, OCActionArgs args)
	{
		return !IsSourceAnimating(action, args);
	}
			
	public static bool IsSourceAngry(OCAction action, OCActionArgs args)
	{
		OCPhysiologicalModel model = GameObject.FindGameObjectWithTag("OCAGI").GetComponent<OCPhysiologicalModel>();
		return model.Energy < 0.9;
	}
			
	public static bool IsNoEndTargetOrNotAnimating(OCAction action, OCActionArgs args)
	{
		return IsSourceNotAnimating(action, args) || IsNoEndTarget(action, args);
	}

	public static bool IsSourceNotIdlingAnimation(OCAction action, OCActionArgs args)
	{
		return IsSourceIdlingAnimation(action, args);
	}

	public static bool IsSourceIdlingAnimation(OCAction action, OCActionArgs args)
	{
		return !args.Source.animation.isPlaying || args.Source.animation.IsPlaying("idle");
	}
			
	public static bool IsPathOpenForSourceForwardDrop(OCAction action, OCActionArgs args)
	{
		OCMap map = OCMap.Instance;
				
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
				
		return 
			map.IsPathOpen
			(	args.Source.transform
			, charController.height
			, OCMap.PathDirection.ForwardDrop
			, default(Vector3i)
			)
		;
	}
			
	public static bool IsPathOpenForSourceForwardClimb(OCAction action, OCActionArgs args)
	{
		OCMap map = OCMap.Instance;

		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
				
		return 
			map.IsPathOpen
			(	args.Source.transform
			, charController.height
			, OCMap.PathDirection.ForwardClimb
			, default(Vector3i)
			)
		;
	}
			
	public static bool IsPathOpenForSourceForwardRun(OCAction action, OCActionArgs args)
	{
		OCMap map = OCMap.Instance;
				
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
				
		return 
			map.IsPathOpen
			(	args.Source.transform
			, charController.height
			, OCMap.PathDirection.ForwardRun
			, default(Vector3i)
			)
		;
	}			
			
	public static bool IsPathOpenForSourceForwardJump(OCAction action, OCActionArgs args)
	{
		OCMap map = OCMap.Instance;
				
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
				
		return 
			map.IsPathOpen
			(	args.Source.transform
			, charController.height
			, OCMap.PathDirection.ForwardJump
			, default(Vector3i)
			)
		;
	}

	public static bool IsPathOpenForSourceForwardWalk(OCAction action, OCActionArgs args)
	{
		OCMap map = OCMap.Instance;//OCMap.Instance;
				
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
				
		return 
			map.IsPathOpen
			(	args.Source.transform
			, charController.height
			, OCMap.PathDirection.ForwardWalk
			, default(Vector3i)
			)
		;
	}

	public static bool IsPathOpenForSourceForwardLeftWalk(OCAction action, OCActionArgs args)
	{
		OCMap map = OCMap.Instance;
		
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
		
		return 
			map.IsPathOpen
				(	args.Source.transform
				 , charController.height
				 , OCMap.PathDirection.ForwardLeftWalk
				 , default(Vector3i)
				 )
				;
	}

	public static bool IsPathOpenForSourceForwardRightWalk(OCAction action, OCActionArgs args)
	{
		OCMap map = OCMap.Instance;
		
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
		
		return 
			map.IsPathOpen
				(	args.Source.transform
				 , charController.height
				 , OCMap.PathDirection.ForwardRightWalk
				 , default(Vector3i)
				 )
				;
	}

	public static bool IsPathOpenForSourceForwardBlockEmpty(OCAction action, OCActionArgs args)
	{
		OCMap map = OCMap.Instance;
				
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
				
		return 
			map.IsPathOpen
			(	args.Source.transform
			, charController.height
			, OCMap.PathDirection.ForwardBlockEmpty
			, default(Vector3i)
			)
		;
	}

			public static bool IsPathOpenForSourceAdjacentBlockEmpty(OCAction action, OCActionArgs args)
			{
				OCMap map = OCMap.Instance;
				
				CharacterController charController = 
					args.Source.GetComponent<CharacterController>();
				
				return 
					map.IsPathOpen
						(	args.Source.transform
						 , charController.height
						 , OCMap.PathDirection.AdjacentBlockEmpty
						 , VectorUtil.Vector3ToVector3i(args.EndTarget.transform.position)
						 )
						;
			}

	public static bool IsPathOpenForSourceForwardBlockSolid(OCAction action, OCActionArgs args)
	{
		OCMap map = OCMap.Instance;
				
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
				
		return 
			map.IsPathOpen
			(	args.Source.transform
			, charController.height
			, OCMap.PathDirection.ForwardBlockSolid
			, default(Vector3i)
			)
		;
	}

	public static bool IsPathOpenForSourceAdjacentBlockSolid(OCAction action, OCActionArgs args)
	{
		OCMap map = OCMap.Instance;
		
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
		
		return 
			map.IsPathOpen
				(	args.Source.transform
				 , charController.height
				 , OCMap.PathDirection.AdjacentBlockSolid
				 , VectorUtil.Vector3ToVector3i(args.EndTarget.transform.position)
				 )
				;
	}

//	public static bool IsPathNotOpenForSourceForwardBlock(OCAction action, OCActionArgs args)
//	{
//		return !IsPathOpenForSourceForwardBlock(action, args);
//	}

	public static bool IsEndTargetAdjacent(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;

		Vector3 distance = targetPosition - sourcePosition;
		return distance.sqrMagnitude <= (1.4f + float.Epsilon);
	}

	public static bool IsEndTargetAdjacentOrDiagonal(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		
		Vector3 distance = targetPosition - sourcePosition;
		return distance.sqrMagnitude <= (2.5f + float.Epsilon);
	}

	public static bool IsEndTargetAdjacentBelow(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;

		sourcePosition = sourcePosition - args.Source.gameObject.transform.up;
		
		Vector3 distance = targetPosition - sourcePosition;
		return distance.sqrMagnitude <= (1.4f + float.Epsilon);
	}

	public static bool IsEndTargetForward(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceForward = args.Source.gameObject.transform.forward;
				
		Vector3 distance = targetPosition - sourcePosition;
		float projection = Vector3.Dot(distance, sourceForward);
		return projection >= 0.5f;
	}

	public static bool IsEndTargetLeft(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceLeft = -args.Source.gameObject.transform.right;
		
		Vector3 distance = targetPosition - sourcePosition;
		float projection = Vector3.Dot(distance, sourceLeft);
		return projection >= 0.5f;
	}

	public static bool IsEndTargetRight(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceRight = args.Source.gameObject.transform.right;
		
		Vector3 distance = targetPosition - sourcePosition;
		float projection = Vector3.Dot(distance, sourceRight);
		return projection >= 0.5f;
	}
			
	public static bool IsEndTargetCloseForward(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceForward = args.Source.gameObject.transform.forward;
				
		Vector3 distance = targetPosition - sourcePosition;
		float projection = Vector3.Dot(distance, sourceForward);
		return projection >= 0.25f && projection < 3.5;
	}
			
	public static bool IsEndTargetFarForward(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceForward = args.Source.gameObject.transform.forward;
				
		Vector3 distance = targetPosition - sourcePosition;
		float projection = Vector3.Dot(distance, sourceForward);
		return projection >= 3.5f;
	}

	public static bool IsEndTargetNotFarForward(OCAction action, OCActionArgs args)
	{
		return !IsEndTargetFarForward(action, args);
	}

	public static bool IsEndTargetUp(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceUp = args.Source.gameObject.transform.up;
				
		Vector3 distance = targetPosition - sourcePosition;
		float projection = Vector3.Dot(distance, sourceUp);
		return projection >= 0.5f;
	}
			
	public static bool IsEndTargetFarUp(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceUp = args.Source.gameObject.transform.up;
				
		Vector3 distance = targetPosition - sourcePosition;
		float projection = Vector3.Dot(distance, sourceUp);
		return projection >= 2.5f;
	}

	public static bool IsEndTargetNotFarUp(OCAction action, OCActionArgs args)
	{
		return !IsEndTargetFarUp(action, args);
	}

	public static bool IsEndTargetNotUp(OCAction action, OCActionArgs args)
	{
		return !IsEndTargetUp(action, args);
	}

	public static bool IsEndTargetCloseUp(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceUp = args.Source.gameObject.transform.up;

		Vector3 distance = targetPosition - sourcePosition;
		float projection = Vector3.Dot(distance, sourceUp);
		return projection >= 0.5f && projection < 2.5f;
	}

	public static bool IsEndTargetNotCloseUp(OCAction action, OCActionArgs args)
	{
		return !IsEndTargetCloseUp(action, args);
	}

	public static bool IsEndTargetNotAboveOrBelow(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceUp = args.Source.transform.up;
		Vector3 sourceForward = args.Source.transform.forward;
		Vector3 sourceRight = args.Source.transform.right;

		Vector3 distance = targetPosition - sourcePosition;
		float projUp = Vector3.Dot(distance, sourceUp);
		float projForward = Vector3.Dot(distance, sourceForward);
		float projRight = Vector3.Dot(distance, sourceRight);

		return Math.Abs(projUp) <= 1.5f && Math.Abs(projForward) > 0.5f && Math.Abs(projRight) > 0.5f;
	}

	public static bool IsEndTargetAboveOrBelow(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceUp = args.Source.transform.up;
		Vector3 sourceForward = args.Source.transform.forward;
		Vector3 sourceRight = args.Source.transform.right;

		Vector3 distance = targetPosition - sourcePosition;
		float projUp = Vector3.Dot(distance, sourceUp);
		float projForward = Vector3.Dot(distance, sourceForward);
		float projRight = Vector3.Dot(distance, sourceRight);

		return Math.Abs(projUp) >= 1.5f && Math.Abs(projForward) < 0.5f && Math.Abs(projRight) < 0.5f;
	}
			
	public static bool IsEndTargetAbove(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceUp = args.Source.transform.up;
		Vector3 sourceForward = args.Source.transform.forward;
		Vector3 sourceRight = args.Source.transform.right;

		Vector3 distance = targetPosition - sourcePosition;
		float projUp = Vector3.Dot(distance, sourceUp);
		float projForward = Vector3.Dot(distance, sourceForward);
		float projRight = Vector3.Dot(distance, sourceRight);

		return projUp >= 1.5f && Math.Abs(projForward) < 0.5f && Math.Abs(projRight) < 0.5f;
	}		
			
	public static bool IsEndTargetAhead(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceUp = args.Source.transform.up;
		Vector3 sourceForward = args.Source.transform.forward;
		Vector3 sourceRight = args.Source.transform.right;

		Vector3 distance = targetPosition - sourcePosition;
		float projUp = Vector3.Dot(distance, sourceUp);
		float projForward = Vector3.Dot(distance, sourceForward);
		float projRight = Vector3.Dot(distance, sourceRight);

		return Math.Abs(projUp) < 0.5f && projForward >= 0.5f && Math.Abs(projRight) < 0.5f;
	}
			
	public static bool IsEndTargetNotAhead(OCAction action, OCActionArgs args)
	{
		return !IsEndTargetAhead(action, args);
	}
			
	public static bool IsEndTargetMoreLeft(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceRight = args.Source.gameObject.transform.right;
		Vector3 sourceLeft = -args.Source.gameObject.transform.right;
		//Vector3 sourceForward = args.Source.gameObject.transform.forward;
				
		Vector3 sourceGoal = targetPosition - sourcePosition;
		float projectionLeft = Vector3.Dot(sourceGoal, sourceLeft);
		float projectionRight = Vector3.Dot(sourceGoal, sourceRight);
		//float projectionForward = Vector3.Dot(sourceGoal, sourceForward);
				
		return (projectionLeft) >= projectionRight;// && projectionLeft >= projectionForward;
	}
			
	public static bool IsEndTargetMoreRight(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceRight = args.Source.gameObject.transform.right;
		Vector3 sourceLeft = -args.Source.gameObject.transform.right;
		//Vector3 sourceForward = args.Source.gameObject.transform.forward;
				
		Vector3 sourceGoal = targetPosition - sourcePosition;
		float projectionLeft = Vector3.Dot(sourceGoal, sourceLeft);
		float projectionRight = Vector3.Dot(sourceGoal, sourceRight);
		//float projectionForward = Vector3.Dot(sourceGoal, sourceForward);
				
		return projectionRight > (projectionLeft);// && projectionRight > projectionForward;
	}

	public static bool IsNoEndTarget(OCAction action, OCActionArgs args)
	{
		return args.EndTarget.transform.position == Vector3.zero;
	}

	public static bool IsEndTarget(OCAction action, OCActionArgs args)
	{
		return args.EndTarget.transform.position != Vector3.zero;
	}

	public static bool IsSourceRunningAction(OCAction action, OCActionArgs args)
	{
		return action._ActionController.RunningActions.Count > 0 && action._ActionController.RunningActions.Contains(action.FullName);
	}
			
	public static bool IsSourceNotRunningAction(OCAction action, OCActionArgs args)
	{
		return !IsSourceRunningAction(action, args);
	}
			
	public static bool IsSourceNotAnimatingOrRunningAction(OCAction action, OCActionArgs args)
	{
		return IsSourceNotAnimating(action, args) || IsSourceRunningAction(action, args);
	}

	public static bool IsSourceIdling(OCAction action, OCActionArgs args)
	{
		return action._ActionController.RunningActions.Contains("StandIdleShow");
	}
			
	public static bool IsSourceNotIdling(OCAction action, OCActionArgs args)
	{
		return !IsSourceIdling(action, args);
	}
			
	public static bool DoesSourceNeedToTellEndTarget(OCAction action, OCActionArgs args)
	{
		return UnityEngine.Random.Range(0,100) > 75 && !action._ActionController.RunningActions.Contains("StandIdleTell");
	}

	public static bool IsSourceRunningOtherActions(OCAction action, OCActionArgs args)
	{
		return action._ActionController.RunningActions.Where(s => s != action.FullName).Count() > 0;
	}

	public static bool IsSourceNotRunningOtherActions(OCAction action, OCActionArgs args)
	{
		return !IsSourceRunningOtherActions(action, args);
	}

	public static bool IsSourceOnlyIdling(OCAction action, OCActionArgs args)
	{
		return action._ActionController.RunningActions.Where(s => s != "StandIdleShow").Count() == 0;
	}

	public static bool IsSourceNotOnlyIdling(OCAction action, OCActionArgs args)
	{
		return !IsSourceOnlyIdling(action, args);
	}

	public static bool IsSourceRunningOtherActionsIgnoreIdle(OCAction action, OCActionArgs args)
	{
		return action._ActionController.RunningActions.Where(s => s != action.FullName && s != "StandIdleShow").Count() > 0;
	}

	public static bool IsSourceNotRunningOtherActionsIgnoreIdle(OCAction action, OCActionArgs args)
	{
		return !IsSourceRunningOtherActionsIgnoreIdle(action, args);
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
			
	private ActionStatus Execute()
	{
		ActionStatus retVal = ActionStatus.NONE;		
				
		// Checks if all Preconditions are true.
		if(ShouldStart)
			return StartAction();//retVal = StartAction();
				
		// Checks if all Invariants are true.
		if(ShouldContinue && retVal == ActionStatus.NONE)
			return ContinueAction();//retVal = ContinueAction();

		// Checks if all Postconditions are true.
		if(ShouldEnd && retVal == ActionStatus.NONE)
			return EndAction();//retVal = EndAction();
				
//		if(Descriptors.Contains("Idle") && _EndTarget.transform.position != Vector3.zero && retVal == ActionStatus.NONE)
//		{
//			return ActionStatus.FAILURE;
//		}
//		else if(retVal != ActionStatus.NONE)
//			return retVal;
				
		OCActionArgs args = null;
				
		if(_ActionController.Step != null)
			args = _ActionController.Step.Arguments;		
				
		if(!_blockOnFail)
		{
			if(IsSourceRunningAction(this, null))
			{
				Debug.Log(" -- Action Failed, but will not block: " + FullName);
				
//						if(args.ActionPlanID == null && OCConnectorSingleton.Instance.IsEstablished)
//							OCConnectorSingleton.Instance.HandleOtherAgentActionResult(_ActionController.Step, false);		
			}
	
			return ActionStatus.SUCCESS;
		}
		else if(args == null || args.ActionPlanID == null)
		{
			if(IsSourceRunningAction(this, null))
			{
				Debug.LogWarning(" -- Action Failed: " + FullName);

						if(args.ActionPlanID == null && OCConnectorSingleton.Instance.IsEstablished)
						{
							_ActionController.Step.Arguments.ActionName = FullName;
							OCConnectorSingleton.Instance.HandleOtherAgentActionResult(_ActionController.Step, false);	
						}

			}


					
			return ActionStatus.FAILURE;
		}
//		else if(IsSourceNotRunningAction(this, null))
//		{
//			if(_ActionController.Step.Behaviour.Name == "Character.Move" || args.ActionName == "walk" || args.ActionName == "jump_toward")
//			{
//				Vector3 endTargetPos = args.EndTarget.transform.position;
//				Vector3 sourceTargetPos = args.Source.transform.position;
//						
//				sourceTargetPos.y -= 0.5f;
//						
//				if(args.EndTarget.transform.position == Vector3.zero || endTargetPos == sourceTargetPos)
//				{
//					OCConnectorSingleton.Instance.SendActionStatus(args.ActionPlanID, args.SequenceID, args.ActionName, true);
//					return ActionStatus.SUCCESS;
//				}
//			}
//					
//			if(_ActionController.Step.Behaviour.Name == "Character.TurnAndDestroy" || args.ActionName == "grab" || args.ActionName == "eat")
//			{
//				if(args.EndTarget.transform.position == Vector3.zero)
//				{
//					OCConnectorSingleton.Instance.SendActionStatus(args.ActionPlanID, args.SequenceID, args.ActionName, true);
//					return ActionStatus.SUCCESS;
//				}
//			}
//					
//			return ActionStatus.FAILURE;
//		}

//				if(args.ActionPlanID == null && OCConnectorSingleton.Instance.IsEstablished)
//				{
//					_ActionController.Step.Arguments.ActionName = FullName;
//					OCConnectorSingleton.Instance.HandleOtherAgentActionResult(_ActionController.Step, false);	
//				}

		return ActionStatus.FAILURE;
	}
			
	private ActionStatus StartAction()
	{
		//float timeStart =	OCTime.TimeSinceLevelLoad;
		//System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"Starting the " + FullName + " Action.");
		if(_blockOnFail && _blockOnRunning)
			_ActionController.RunningActions.Add(FullName);

		//@TODO: Fix this hack...
		if(Descriptors.Contains("Jump") || Descriptors.Contains("Climb"))
		{
			OCCharacterMotor motor = _Source.GetComponent<OCCharacterMotor>();
			motor.enabled = false;
			if(Descriptors.Contains("Jump"))
				_moveToTarget = true;
		}

		// Start animation effects
		if(_blockOnRunning || !_Source.animation.isPlaying)
		{
			foreach(OCAnimationEffect afx in _AnimationEffects)
			{
				if(_moveToTarget)
				{
					afx.MoveByX = Mathf.FloorToInt(_EndTarget.transform.position.x - _Source.transform.position.x + 0.5f);
					afx.MoveByZ = Mathf.FloorToInt(_EndTarget.transform.position.y - _Source.transform.position.y + 0.5f);
					// no idea why this one is negative
					afx.MoveByY = -Mathf.FloorToInt(_EndTarget.transform.position.z - _Source.transform.position.z + 0.5f);
				}
				afx.Play();
			}
		}

		foreach(OCCreateBlockEffect cbfx in _CreateBlockEffects)
		{
			if(cbfx.CreationPos == OCCreateBlockEffect.CreationPosition.Forward)
				cbfx.CreateBlock(VectorUtil.Vector3ToVector3i(_Source.transform.position + _Source.transform.forward));
			else if(cbfx.CreationPos == OCCreateBlockEffect.CreationPosition.ForwardBelow)
				cbfx.CreateBlock(VectorUtil.Vector3ToVector3i(_Source.transform.position + _Source.transform.forward - _Source.transform.up));
			else if(cbfx.CreationPos == OCCreateBlockEffect.CreationPosition.Target)
			{
				if(VectorUtil.AreVectorsEqual(_EndTarget.transform.position, _Source.transform.position))
				{
					Debug.LogError(OCLogSymbol.ERROR + "Don't Build a Block on top of yourself");
					return ActionStatus.EXCEPTION;
				}
				cbfx.CreateBlock(VectorUtil.Vector3ToVector3i(_EndTarget.transform.position));
			}
		}
				
		if(!Descriptors.Contains("Idle"))
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "Starting Action: " + FullName);		
			
		return ActionStatus.RUNNING;
	}
			
	private ActionStatus ContinueAction()
	{
		//System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"Continuing the " + FullName + " Action.");
				
//		if(!Descriptors.Contains("Idle"))
//			Debug.LogWarning("Continuing Action: " + FullName);		

		// Animation effects continue automatically
		if(_blockOnRunning)
			return ActionStatus.RUNNING;
		else
			return ActionStatus.SUCCESS;
	}
			
	private ActionStatus EndAction()
	{
		//System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +"Ending the " + FullName + " Action.");
				
		OCActionArgs args = null;
				
		if(_ActionController.Step != null)
			args = _ActionController.Step.Arguments;		
				
		if(_blockOnFail && _blockOnRunning)
			_ActionController.RunningActions.Remove(FullName);

		// End animation effects
		foreach(OCAnimationEffect afx in _AnimationEffects)
		{
			if(_moveToTarget)
			{
				afx.Initialize();
				_moveToTarget = false;
			}
			//afx.Stop();
		}

		foreach(OCTransferBlockEffect tbfx in _TransferBlockEffects)
		{
			Vector3i targetOrigin = VectorUtil.Vector3ToVector3i(args.EndTarget.transform.position);
			Vector3i targetDestination = VectorUtil.Vector3ToVector3i(args.Source.transform.position + args.Source.transform.forward);
			
			tbfx.TransferBlock(targetOrigin, targetDestination);
		}

		foreach(OCDestroyBlockEffect dbfx in _DestroyBlockEffects)
		{
//			Vector3i forward = VectorUtil.Vector3ToVector3i(_Source.transform.position + _Source.transform.forward);
//			Vector3i forwardUp = VectorUtil.Vector3ToVector3i(_Source.transform.position + _Source.transform.forward + _Source.transform.up);
//			Vector3i forwardUp2x = VectorUtil.Vector3ToVector3i(_Source.transform.position + _Source.transform.forward + 2*_Source.transform.up);	
//					
//			OCBlockData forwardBlock = _Map.GetBlock(forward);
//			OCBlockData forwardUpBlock = _Map.GetBlock(forwardUp);
//			OCBlockData forwardUp2xBlock = _Map.GetBlock(forwardUp2x);
//					
//			dbfx.DestroyBlock(forward);
//			dbfx.DestroyBlock(forwardUp);
//			dbfx.DestroyBlock(forwardUp2x);
				
			Vector3i targetPosition = VectorUtil.Vector3ToVector3i(args.EndTarget.transform.position);
			OCBlockData targetBlock = _Map.GetBlock(targetPosition);
			
			//NOTE: This actually destroys blocks like the battery
			dbfx.DestroyBlock(targetPosition);

			
			if(args.EndTarget) args.EndTarget.transform.position = Vector3.zero;
			if(args.StartTarget) args.StartTarget.transform.position = Vector3.zero;
					
			// This is just some example code for you Lake, that you can use to give energy to the robot after consuming a battery.
			//if((forwardBlock.block != null && forwardBlock.block.GetName() == "Battery") || (forwardUpBlock.block != null && forwardUpBlock.block.GetName() == "Battery") || (forwardUp2xBlock.block != null && forwardUp2xBlock.block.GetName() == "Battery"))
			if(targetBlock.block != null && targetBlock.block.GetName() == "Battery")
			{
				UnityEngine.GameObject[] agiArray;
				agiArray = UnityEngine.GameObject.FindGameObjectsWithTag("OCAGI") as GameObject[];

				
				int iAGI = 0; 
				if(iAGI < agiArray.Length) 
				{
					UnityEngine.GameObject agiObject = agiArray[iAGI];
				
					//args.EndTarget = agiObject;
				
					OCPhysiologicalModel agiPhysModel = agiObject.GetComponent<OCPhysiologicalModel>();
				
							OCPhysiologicalEffect batteryEatEffect = ScriptableObject.CreateInstance<OCPhysiologicalEffect>(); 
							batteryEatEffect.CostLevelProp = OCPhysiologicalEffect.CostLevel.NONE;
				
					batteryEatEffect.EnergyIncrease = 0.2f;
				
					agiPhysModel.ProcessPhysiologicalEffect(batteryEatEffect);

				}		
			}
		}

		//@TODO: Fix this hack...
		if(Descriptors.Contains("Jump") || Descriptors.Contains("Climb") || Descriptors.Contains("Fall"))
		{
			OCCharacterMotor motor = _Source.GetComponent<OCCharacterMotor>();
			motor.enabled = true;
		}
			
		if(!Descriptors.Contains("Idle"))
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "Ending Action: " + FullName);
			
			if(args.ActionPlanID == null && OCConnectorSingleton.Instance.IsEstablished)
			{
				_ActionController.Step.Arguments.ActionName = FullName;
				OCConnectorSingleton.Instance.HandleOtherAgentActionResult(_ActionController.Step, true);	
			}		
			
		return ActionStatus.SUCCESS;
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------
			
	[Serializable]
	public class OCActionArgs : EventArgs
	{
		/// <summary>
		/// All actions are initiated by a source character or game object in the
		/// scene with zero, one, or two possible targets.  These OCID's may refer
		/// to target objects themselves or simply dummy objects like waypoints
		/// that specify locations (or other properties) for the action to target.
		/// </summary>
		private GameObject _Source;
		private GameObject _StartTarget;
		private GameObject _EndTarget;
				
		public string ActionPlanID;
		public int SequenceID;
		public string ActionName;
			
		public OCActionArgs()
		{
			
		}
				
		public OCActionArgs(GameObject source, GameObject startTarget, GameObject endTarget)
		{
			_Source = source;
			_StartTarget = startTarget;
			_EndTarget = endTarget;
		}

		public GameObject EndTarget
		{
			get { return _EndTarget; }
			set { _EndTarget = value; }
		}

	
		public GameObject Source
		{
			get { return _Source;}
			set { _Source = value; }
		}


		public GameObject StartTarget
		{
			get { return _StartTarget;}
			set { _StartTarget = value; }
		}
	}

	/// <summary>
	/// The OpenCog Action Condition.
	/// </summary>
	public delegate bool OCActionCondition(OCAction action, OCActionArgs args);

	/// <summary>
	/// 	The Pre-, Continue-, and Post-Conditiona events are raised whenever we check
	/// for whether we should start, continue, or end an action, respectively.  
	/// See ShouldStart, ShouldContinue, and ShouldEnd above.
	/// </summary>

	/// <summary>
	/// 	The OpenCog Precondition delegate.  Defines a common interface for 
	/// testing whether the preconditions for a given action have been met.  For
	/// example, preconditions for the move action might include the condition 
	/// that we're not currently at our destination.
	/// </summary>
	public event OCActionCondition PreCondition;

	/// <summary>
	/// 	The OpenCog Invariant delegate.  Note that for actions, we use a 
	/// slightly different definition for invariance.  An invariant condition for
	/// an action is true only during execution of that action, unlike an 
	/// invariant condition for a class which is true for the entire life of the
	/// instance of that class.  We distinguish between "class invariance" and 
	/// "action invariance" such that the playing of an animation as the result 
	/// of an action would be considered action invariant but not class 
	/// invariant.  Similarly, while an animation may play for the entirety of
	/// execution for an action, it will not be playing when we check 
	/// preconditions or postconditions.  This is in contrast to class invariants
	/// which may change during execution but ultimately retain the same value
	/// from the start at the end.
	/// </summary>
	public event OCActionCondition InvariantCondition;

	/// <summary>
	/// 	The OpenCog Postcondition delegate.  Defines a common interface for
	/// testing whether the postconditions for a given action have been met.  For
	/// example, postconditions for the move action might include the condition
	/// that we've arrived at our destination.
	/// </summary>
	public event OCActionCondition PostCondition;

	// TODO [LEGACY]: Code below is a set of stubs that may not be needed in the final implementation.

	public enum ActionStatus 
	{ NONE
	, RUNNING = Result.Running
	, SUCCESS = Result.Success
	, FAILURE = Result.Failure
	, EXCEPTION 
	};

	// TODO [LEGACY]: Once OCAction is data driven, this function should determine action type from the actionData and create an OCAction with that data.
	public static OCAction CreateAction(System.Xml.XmlNode actionData, bool adjustCoordinates)
	{
				return new OpenCog.Actions.OCAction();
	}

	// TODO [LEGACY]: End of stub code

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCAction

}// namespace Actions

}// namespace OpenCog




