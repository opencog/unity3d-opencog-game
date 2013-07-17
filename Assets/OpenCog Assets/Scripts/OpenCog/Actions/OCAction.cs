
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

	private OCActionController _ActionController;

	private OCMap _Map;

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
				OCActionArgs args = new OCActionArgs(_Source, _StartTarget, _EndTarget);
				if(_ActionController != null)
				{
					if(_ActionController.Step.Arguments.Source != null)
						args.Source = _ActionController.Step.Arguments.Source;
					if(_ActionController.Step.Arguments.StartTarget != null)
						args.StartTarget = _ActionController.Step.Arguments.StartTarget;
					if(_ActionController.Step.Arguments.EndTarget != null)
						args.EndTarget = _ActionController.Step.Arguments.EndTarget;
				}
				shouldStart &= precondition(this, args);
				if(shouldStart == false)
					break;
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
				OCActionArgs args = new OCActionArgs(_Source, _StartTarget, _EndTarget);
				if(_ActionController != null)
				{
					if(_ActionController.Step.Arguments.Source != null)
						args.Source = _ActionController.Step.Arguments.Source;
					if(_ActionController.Step.Arguments.StartTarget != null)
						args.StartTarget = _ActionController.Step.Arguments.StartTarget;
					if(_ActionController.Step.Arguments.EndTarget != null)
						args.EndTarget = _ActionController.Step.Arguments.EndTarget;
				}		
				shouldContinue &= invariant(this, args);
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
				OCActionArgs args = new OCActionArgs(_Source, _StartTarget, _EndTarget);
				if(_ActionController != null)
				{
					if(_ActionController.Step.Arguments.Source != null)
						args.Source = _ActionController.Step.Arguments.Source;
					if(_ActionController.Step.Arguments.StartTarget != null)
						args.StartTarget = _ActionController.Step.Arguments.StartTarget;
					if(_ActionController.Step.Arguments.EndTarget != null)
						args.EndTarget = _ActionController.Step.Arguments.EndTarget;
				}		
				shouldEnd &= postcondition(this, args);
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

		_Map = (OCMap)GameObject.FindSceneObjectsOfType(typeof(OCMap)).LastOrDefault();

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
				
		DontDestroyOnLoad(this);
	}
			
	public Result ExecuteBehave()
	{
		return (Result)Execute();
	}
	
	
			
	//	The following can be used as Precondition, Invariant, or Postcondition
	//	delegates.
	
	public static bool IsNoCondition(OCAction action, OCActionArgs args)
	{
		return true;
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
			
	public static bool IsEndTargetBelowClimbAngle(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePos = args.Source.transform.position;
		Vector3 endTargetPos = args.EndTarget.transform.position;
		float angle = Vector3.Angle(endTargetPos - sourcePos, Vector3.up);
				
		return angle > 45;
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
		OCMap map = (OCMap)GameObject.FindObjectOfType(typeof(OCMap));
				
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
				
		return 
			map.IsPathOpen
			(	action.gameObject.transform
			, charController.height
			, OCMap.PathDirection.ForwardDrop
			)
		;
	}
			
	public static bool IsPathOpenForSourceForwardClimb(OCAction action, OCActionArgs args)
	{
		OCMap map = (OCMap)GameObject.FindObjectOfType(typeof(OCMap));

		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
				
		return 
			map.IsPathOpen
			(	action.gameObject.transform
			, charController.height
			, OCMap.PathDirection.ForwardClimb
			)
		;
	}
			
	public static bool IsPathOpenForSourceForwardRun(OCAction action, OCActionArgs args)
	{
		OCMap map = (OCMap)GameObject.FindObjectOfType(typeof(OCMap));
				
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
				
		return 
			map.IsPathOpen
			(	action.gameObject.transform
			, charController.height
			, OCMap.PathDirection.ForwardRun
			)
		;
	}			
			
	public static bool IsPathOpenForSourceForwardJump(OCAction action, OCActionArgs args)
	{
		OCMap map = (OCMap)GameObject.FindObjectOfType(typeof(OCMap));
				
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
				
		return 
			map.IsPathOpen
			(	action.gameObject.transform
			, charController.height
			, OCMap.PathDirection.ForwardJump
			)
		;
	}

	public static bool IsPathOpenForSourceForwardWalk(OCAction action, OCActionArgs args)
	{
		OCMap map = (OCMap)GameObject.FindObjectOfType(typeof(OCMap));
				
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
				
		return 
			map.IsPathOpen
			(	action.gameObject.transform
			, charController.height
			, OCMap.PathDirection.ForwardWalk
			)
		;
	}

	public static bool IsPathOpenForSourceForwardBlockEmpty(OCAction action, OCActionArgs args)
	{
		OCMap map = (OCMap)GameObject.FindObjectOfType(typeof(OCMap));
				
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
				
		return 
			map.IsPathOpen
			(	action.gameObject.transform
			, charController.height
			, OCMap.PathDirection.ForwardBlockEmpty
			)
		;
	}

	public static bool IsPathOpenForSourceForwardBlockSolid(OCAction action, OCActionArgs args)
	{
		OCMap map = (OCMap)GameObject.FindObjectOfType(typeof(OCMap));
				
		CharacterController charController = 
			args.Source.GetComponent<CharacterController>();
				
		return 
			map.IsPathOpen
			(	action.gameObject.transform
			, charController.height
			, OCMap.PathDirection.ForwardBlockSolid
			)
		;
	}

//	public static bool IsPathNotOpenForSourceForwardBlock(OCAction action, OCActionArgs args)
//	{
//		return !IsPathOpenForSourceForwardBlock(action, args);
//	}

	public static bool IsEndTargetForward(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceForward = args.Source.gameObject.transform.forward;
				
		Vector3 distance = targetPosition - sourcePosition;
		float projection = Vector3.Dot(distance, sourceForward);
		return projection >= 0.5f;
	}
			
	public static bool IsEndTargetCloseForward(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceForward = args.Source.gameObject.transform.forward;
				
		Vector3 distance = targetPosition - sourcePosition;
		float projection = Vector3.Dot(distance, sourceForward);
		return projection >= 0.5f && projection < 3.5;
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
			
	public static bool IsEndTargetMoreLeft(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceRight = args.Source.gameObject.transform.right;
		Vector3 sourceLeft = -args.Source.gameObject.transform.right;
				
		Vector3 distance = targetPosition - sourcePosition;
		float projectionLeft = Vector3.Dot(distance, sourceLeft);
		float projectionRight = Vector3.Dot(distance, sourceRight);
				
		return (projectionLeft + 0.0f) > projectionRight;
	}
			
	public static bool IsEndTargetMoreRight(OCAction action, OCActionArgs args)
	{
		Vector3 sourcePosition = args.Source.gameObject.transform.position;
		Vector3 targetPosition = args.EndTarget.gameObject.transform.position;
		Vector3 sourceRight = args.Source.gameObject.transform.right;
		Vector3 sourceLeft = -args.Source.gameObject.transform.right;
				
		Vector3 distance = targetPosition - sourcePosition;
		float projectionLeft = Vector3.Dot(distance, sourceLeft);
		float projectionRight = Vector3.Dot(distance, sourceRight);
				
		return projectionRight > (projectionLeft + 0.0f);
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
		return action._ActionController.RunningActions.Contains(action.FullName);
	}
			
	public static bool IsSourceNotRunningAction(OCAction action, OCActionArgs args)
	{
		return !IsSourceRunningAction(action, args);
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
		// Checks if all Preconditions are true.
		if(ShouldStart)
			return StartAction();
				
		// Checks if all Invariants are true.
		if(ShouldContinue)
			return ContinueAction();

		// Checks if all Postconditions are true.
		if(ShouldEnd)
			return EndAction();

		return ActionStatus.FAILURE;
	}
			
	private ActionStatus StartAction()
	{
		//OCLogger.Debugging("Starting the " + FullName + " Action.");

		_ActionController.RunningActions.Add(FullName);

		// Start animation effects
		foreach(OCAnimationEffect afx in _AnimationEffects)
		{
			afx.Play();
		}

		foreach(OCCreateBlockEffect cbfx in _CreateBlockEffects)
		{
			cbfx.CreateBlock(VectorUtil.Vector3ToVector3i(_Source.transform.position + _Source.transform.forward));
		}

		//@TODO: Fix this hack...
		if(Descriptors.Contains("Jump") || Descriptors.Contains("Climb") || Descriptors.Contains("Fall"))
		{
			OCCharacterMotor motor = _Source.GetComponent<OCCharacterMotor>();
			motor.enabled = false;
		}
			
		return ActionStatus.RUNNING;
	}
			
	private ActionStatus ContinueAction()
	{
		//OCLogger.Fine("Continuing the " + FullName + " Action.");

		// Animation effects continue automatically
		return ActionStatus.RUNNING;
	}
			
	private ActionStatus EndAction()
	{
		//OCLogger.Debugging("Ending the " + FullName + " Action.");

		_ActionController.RunningActions.Remove(FullName);

		// End animation effects
		foreach(OCAnimationEffect afx in _AnimationEffects)
		{
			afx.Stop();
		}

		foreach(OCDestroyBlockEffect dbfx in _DestroyBlockEffects)
		{
			Vector3i forward = VectorUtil.Vector3ToVector3i(_Source.transform.position + _Source.transform.forward);
			Vector3i forwardUp = VectorUtil.Vector3ToVector3i(_Source.transform.position + _Source.transform.forward + _Source.transform.up);
			Vector3i forwardUp2x = VectorUtil.Vector3ToVector3i(_Source.transform.position + _Source.transform.forward + 2*_Source.transform.up);		
			dbfx.DestroyBlock(forward);
			dbfx.DestroyBlock(forwardUp);
			dbfx.DestroyBlock(forwardUp2x);
		}

		//@TODO: Fix this hack...
		if(Descriptors.Contains("Jump") || Descriptors.Contains("Climb") || Descriptors.Contains("Fall"))
		{
			OCCharacterMotor motor = _Source.GetComponent<OCCharacterMotor>();
			motor.enabled = true;
		}
			
		return ActionStatus.SUCCESS;
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------

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

	// TODO: Code below is a set of stubs that may not be needed in the final implementation.

	public enum ActionStatus 
	{ NONE
	, RUNNING = Result.Running
	, SUCCESS = Result.Success
	, FAILURE = Result.Failure
	, EXCEPTION 
	};

	// TODO: Once OCAction is data driven, this function should determine action type from the actionData and create an OCAction with that data.
	public static OCAction CreateAction(System.Xml.XmlNode actionData, bool adjustCoordinates)
	{
				return new OpenCog.Actions.OCAction();
	}

	// TODO: End of stub code

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCAction

}// namespace Actions

}// namespace OpenCog




