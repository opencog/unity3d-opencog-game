
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
/// The OpenCog OCAction.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposeProperties]
[Serializable]
#endregion
public class OCAction : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------

	/// <summary>
	/// Indicates whether this action has an associated animation.
	/// </summary>
	private bool m_HasAnimation = false;

	/// <summary>
	/// The animation associated with this action, if any.
	/// </summary>
	private OCAnimation m_Animation = null;


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
		get {return m_HasAnimation;}
		set {m_HasAnimation = value;}
	}

	/// <summary>
	/// Gets or sets the animation associated with this action.
	/// </summary>
	/// <value>
	/// The animation.
	/// </value>
	[OCTooltip("The animation to play for this action.")]
	[OCBoolPropertyToggle("HasAnimation", true)]
	public OCAnimation Animation
	{
		get {return m_Animation;}
		set {m_Animation = value;}
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


	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
			
	
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Member Classes

	//---------------------------------------------------------------------------		

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCAction

}// namespace Actions

}// namespace OpenCog




