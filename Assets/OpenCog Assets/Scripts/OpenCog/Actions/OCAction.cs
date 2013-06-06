
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
	/// 	All actions are initiated by a source character or game object in the 
	/// scene with zero, one, or two possible targets.  These OCID's may refer
	/// to target objects themselves or simply dummy objects like waypoints
	/// that specify locations (or other properties) for the action to target.
	/// </summary>
	private OCID _SourceID;
	private OCID _StartTargetID;
	private OCID _EndTargetID;
			
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
	private List<string> _Descriptors;
			
	private bool _IsExecuting = false;

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
			
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
			foreach(OCPrecondition precondition in PreCheck.GetInvocationList())
			{
				shouldStart &= precondition(this);
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
			foreach(OCInvariant invariant in ContinueCheck.GetInvocationList())
			{
				shouldContinue &= invariant(this);
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
			foreach(OCPostcondition postcondition in PostCheck.GetInvocationList())
			{
				shouldEnd &= postcondition(this);
				if(shouldEnd == false)
					break;
			}
			return shouldEnd;
		}
	}			
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public Result ExecuteBehave()
	{
		return (Result)Execute();
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
			
	private ActionStatus Execute()
	{
		if(ShouldEnd)
			return ActionStatus.FAILURE;
				
		if(ShouldContinue)
			return ActionStatus.RUNNING;
				
		if(ShouldStart)
			return ActionStatus.SUCCESS;

		return ActionStatus.FAILURE;
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------
			
	/// <summary>
	/// 	The OpenCog Precondition delegate.  Defines a common interface for 
	/// testing whether the preconditions for a given action have been met.  For
	/// example, preconditions for the move action might include the condition 
	/// that we're not currently at our destination.
	/// </summary>
	public delegate bool OCPrecondition(OCAction action);
			
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
	public delegate bool OCInvariant(OCAction action);
			
	/// <summary>
	/// 	The OpenCog Postcondition delegate.  Defines a common interface for
	/// testing whether the postconditions for a given action have been met.  For
	/// example, postconditions for the move action might include the condition
	/// that we've arrived at our destination.
	/// </summary>
	public delegate bool OCPostcondition(OCAction action); 
			
	/// <summary>
	/// 	The Pre-, Continue-, and Post-Check events are raised whenever we check
	/// for whether we should start, continue, or end an action, respectively.  
	/// See ShouldStart, ShouldContinue, and ShouldEnd above.
	/// </summary>
	public event OCPrecondition PreCheck;
	public event OCInvariant ContinueCheck;
	public event OCPostcondition PostCheck;

	// TODO: Code below is a set of stubs that may not be needed in the final implementation.

	public enum ActionStatus 
	{ NONE
	, RUNNING = Result.Running
	, SUCCESS = Result.Success
	, FAILURE = Result.Failure
	, EXCEPTION 
	};

	private ActionStatus _status;
	public ActionStatus Status
	{
		get { return _status; }
		set { _status = value; }
	}

	public class Target
	{

	}

	private string _name;
	public string Name
	{
		get { return _name; }
		set { _name = value; }
	}

	// TODO: Once OCAction is data driven, this function should determine action type from the actionData and create an OCAction with that data.
	public static OCAction CreateAction(System.Xml.XmlNode actionData, bool adjustCoordinates)
	{
				return new OpenCog.Actions.OCIdleAction();
	}

	// TODO: End of stub code

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCAction

}// namespace Actions

}// namespace OpenCog




