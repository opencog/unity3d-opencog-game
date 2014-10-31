
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
/// The OpenCog Enum Property Toggle Attribute.  Attributed properties or
/// fields will only display if the specified enum is a given value.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
#endregion
public class OCEnumPropertyToggleAttribute : Attribute
{

  /////////////////////////////////////////////////////////////////////////////

  #region Private Member Data

  /////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The enum field name that will toggle the attribute display.
	/// </summary>
	private string _enumField;

	/// <summary>
	/// The enum value that will determine whether to display.
	/// </summary>
	private object _enumValue;

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Accessors and Mutators

  /////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Gets or sets the enum field name.
	/// </summary>
	/// <value>
	/// The enum field name.
	/// </value>
	public string EnumField
	{
		get { return _enumField; }
		set { _enumField = value; }
	}

	/// <summary>
	/// Gets or sets a value indicating whether this
	/// <see cref="OpenCog.Attributes.OCenumPropertyToggleAttribute"/>
	/// will display the attributed property or field with a given enum value.
	/// </summary>
	/// <value>
	/// The value of the enumeration in question.
	/// </value>
	public object EnumValue
	{
		get { return _enumValue; }
		set { _enumValue = value; }
	}

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Public Member Functions

  /////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Initializes a new instance of the
	/// <see cref="OpenCog.Attributes.OCenumPropertyToggleAttribute"/>
	/// class.
	/// </summary>
	/// <param name='enumField'>
	/// Enum field name.
	/// </param>
	/// <param name='enumValue'>
	/// Enum value.
	/// </param>
  public OCEnumPropertyToggleAttribute(string enumField, object enumValue)
	{
		this.EnumField = enumField;
		this.EnumValue  = enumValue;
	}

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Private Member Functions

  /////////////////////////////////////////////////////////////////////////////

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

}// class OCenumPropertyToggleAttribute

}// namespace Attributes

}// namespace OpenCog


