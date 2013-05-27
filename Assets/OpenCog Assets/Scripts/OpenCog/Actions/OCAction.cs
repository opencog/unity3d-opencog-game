
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
using OpenCog.Attributes;
using OpenCog.Extensions;
using ProtoBuf;
using UnityEngine;

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
public abstract class OCAction : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------

	/// <summary>
	/// Indicates whether this action has an associated animation.
	/// </summary>
	private bool _hasAnimation = false;

	/// <summary>
	/// The animation associated with this action, if any.
	/// </summary>
	private OCAnimationEffect _animationEffect = null;

	private System.Guid _sourceID;

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------

	/// <summary>
	/// Gets or sets a value indicating whether this action has an associated
	/// animation.
	/// </summary>
	/// <value>
	/// <c>true</c> if this instance has an animation; otherwise, <c>false</c>.
	/// </value>
	public bool HasAnimation
	{
		get {return _hasAnimation;}
		set {_hasAnimation = value;}
	}

	/// <summary>
	/// Gets or sets the animation associated with this action.
	/// </summary>
	/// <value>
	/// The animation.
	/// </value>
	[OCTooltip("The animation to play for this action.")]
	[OCBoolPropertyToggle("HasAnimation", true)]
	public OCAnimationEffect AnimationEffect
	{
		get {return _animationEffect;}
		set {_animationEffect = value;}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------	

	#region Constructors

	//---------------------------------------------------------------------------


	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public abstract void Execute();

	public abstract bool IsExecuting();

	public abstract void Terminate();

	public abstract bool ShouldTerminate();

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

	// TODO: Code below is a set of stubs that may not be needed in the final implementation.

	public enum ActionStatus { NONE = 0, RUNNING = 1, SUCCESS = 2, FAILURE = 3, EXCEPTION = 4 };

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




