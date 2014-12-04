
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
/// The OpenCog Bool Property Toggle Attribute.  Attributed properties or
/// fields will only display if the specified bool is a given value.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
#endregion
public class OCBoolPropertyToggleAttribute : Attribute
{

  /////////////////////////////////////////////////////////////////////////////

  #region Private Member Data

  /////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The boolean field name that will toggle the attribute display.
	/// </summary>
	private string _booleanField;

	/// <summary>
	/// The boolean value that will determine whether to display.
	/// </summary>
	private bool _equalsValue;

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Accessors and Mutators

  /////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Gets or sets the boolean field name.
	/// </summary>
	/// <value>
	/// The boolean field name.
	/// </value>
	public string BooleanField
	{
		get { return _booleanField; }
		set { _booleanField = value; }
	}

	/// <summary>
	/// Gets or sets a value indicating whether this
	/// <see cref="OpenCog.Attributes.OCBoolPropertyToggleAttribute"/>
	/// will display the attributed property or field on a true or false.
	/// </summary>
	/// <value>
	/// <c>true</c> if equals value; otherwise, <c>false</c>.
	/// </value>
	public bool EqualsValue
	{
		get { return _equalsValue; }
		set { _equalsValue = value; }
	}

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Public Member Functions

  /////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Initializes a new instance of the
	/// <see cref="OpenCog.Attributes.OCBoolPropertyToggleAttribute"/>
	/// class.
	/// </summary>
	/// <param name='booleanField'>
	/// Boolean field.
	/// </param>
	/// <param name='equalsValue'>
	/// Equals value.
	/// </param>
  public OCBoolPropertyToggleAttribute(string booleanField, bool equalsValue)
	{
		this.BooleanField = booleanField;
		this.EqualsValue  = equalsValue;
	}

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Private Member Functions

  /////////////////////////////////////////////////////////////////////////////

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

}// class OCBoolPropertyToggleAttribute

}// namespace Attributes

}// namespace OpenCog



