
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
/// The OpenCog Int Slider Attribute.  Attributed properties and fields will
/// display in the custom inspector editor as a slider.  Min and max values
/// are capped.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
#endregion
public class OCIntSliderAttribute : Attribute
{

  /////////////////////////////////////////////////////////////////////////////

  #region Private Member Data

  /////////////////////////////////////////////////////////////////////////////

  /// <summary>
	/// The minimum value cap.
	/// </summary>
	private int _minValue;

	/// <summary>
	/// The maximum value cap.
	/// </summary>
	private int _maxValue;

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Accessors and Mutators

  /////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Gets or sets the minimum value.
	/// </summary>
	/// <value>
	/// The minimum value.
	/// </value>
	public int MinValue
	{
		get { return _minValue; }
		set { _minValue = value; }
	}

	/// <summary>
	/// Gets or sets the maximum value.
	/// </summary>
	/// <value>
	/// The maximum value.
	/// </value>
	public int MaxValue
	{
		get { return _maxValue; }
		set { _maxValue = value; }
	}

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Public Member Functions

  /////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Initializes a new instance of the
	/// <see cref="OpenCog.Attributes.OCIntSliderAttribute"/> class.
	/// </summary>
	/// <param name='minValue'>
	/// Minimum value.
	/// </param>
	/// <param name='maxValue'>
	/// Max value.
	/// </param>
	public OCIntSliderAttribute( int minValue, int maxValue )
	{
		this.MinValue = minValue;
		this.MaxValue = maxValue;
	}

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Private Member Functions

  /////////////////////////////////////////////////////////////////////////////

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

}// class OCIntSliderAttribute

}// namespace Attributes

}// namespace OpenCog



