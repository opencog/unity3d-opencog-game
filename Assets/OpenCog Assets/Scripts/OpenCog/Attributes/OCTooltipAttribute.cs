
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
using UnityEngine;
using System;
using System.Collections;
using ProtoBuf;

namespace OpenCog
{

namespace Attributes
{

/// <summary>
/// The OpenCog Tooltip Attribute.  Attributed properties and fields will
/// display a custom tooltip when moused over in a custom inspector editor.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
#endregion
public class OCTooltipAttribute : Attribute
{

  /////////////////////////////////////////////////////////////////////////////

  #region Private Member Data

  /////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The tooltip string to be displayed.
	/// </summary>
	private string _description;

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Accessors and Mutators

  /////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Gets or sets the tooltip.
	/// </summary>
	/// <value>
	/// The tooltip string.
	/// </value>
  public string Description
	{
		get { return _description; }
		set { _description = value; }
	}

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Public Member Functions

  /////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Initializes a new instance of the
	/// <see cref="OpenCog.Attributes.OCTooltipAttribute"/> class.
	/// </summary>
	/// <param name='tooltip'>
	/// Tooltip.
	/// </param>
  public 	OCTooltipAttribute(string tooltip)
	{
		this.Description = tooltip;
	}

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Private Member Functions

  /////////////////////////////////////////////////////////////////////////////

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

}// class OCInspectorTooltipAttribute

}// namespace Attributes

}// namespace OpenCog



