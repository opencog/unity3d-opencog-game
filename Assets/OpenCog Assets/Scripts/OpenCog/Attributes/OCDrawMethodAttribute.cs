
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
using System.Collections;
using System;
using ProtoBuf;

namespace OpenCog
{

namespace Attributes
{

/// <summary>
/// The OpenCog Draw Method Attribute.  Attributed properties and fields will
/// use a custom draw method to display in the custom inspector editor.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
#endregion
public class OCDrawMethodAttribute : Attribute
{

	/////////////////////////////////////////////////////////////////////////////

  #region Private Member Data

	/////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The string name of the custom draw method.
	/// </summary>
	private string _drawMethod = "";

	/// <summary>
	/// The parameters of the custom draw method.
	/// </summary>
	private object[] _parameters = null;

	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

  #region Accessors and Mutators

	/////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Gets or sets the draw method.
	/// </summary>
	/// <value>
	/// The draw method.
	/// </value>
	public string DrawMethod
	{
		get { return _drawMethod; }
		set { _drawMethod = value; }
	}

	/// <summary>
	/// Gets or sets the parameters.
	/// </summary>
	/// <value>
	/// The parameters.
	/// </value>
	public object[] Parameters
	{
		get { return _parameters; }
		set { _parameters = value; }
	}

	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

  #region Public Member Functions

	/////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Initializes a new instance of the
	/// <see cref="OpenCog.Attributes.OCDrawMethodAttribute"/>
	/// class.
	/// </summary>
	/// <param name='drawMethod'>
	/// Draw method.
	/// </param>
	public OCDrawMethodAttribute( string drawMethod )
	{
		this.DrawMethod = drawMethod;
		Parameters = new object[0];
	}

	/// <summary>
	/// Initializes a new instance of the
	/// <see cref="OpenCog.Attributes.OCDrawMethodAttribute"/>
	/// class.
	/// </summary>
	/// <param name='drawMethod'>
	/// Draw method.
	/// </param>
	/// <param name='parameters'>
	/// Parameters.
	/// </param>
	public OCDrawMethodAttribute( string drawMethod, params object[] parameters )
	{
		this.DrawMethod = drawMethod;
		this.Parameters = parameters;
	}

	/// <summary>
	/// Returns a string of the custom draw method's parameters.
	/// </summary>
	/// <returns>
	/// A string of comma delimited parameters.
	/// </returns>
	public string ParametersToString()
	{
		string parametersString = "";
		for( int i= 0; i < Parameters.Length; i++ )
		{
			parametersString += ", " + Parameters[ i ].ToString();
		}
		return parametersString;
	}

	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

  #region Private Member Functions

	/////////////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

}// class OCCustomDrawMethodAttribute

}// namespace Attributes

}// namespace OpenCog



