
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
using System.Reflection;
using OpenCog.Serialization;
using ProtoBuf;
using UnityEngine;

namespace OpenCog
{

namespace Attributes
{

/// <summary>
/// The OpenCog Expose Properties Attribute.  Attributed classes will expose
/// their publicly accessible properties in the custom inspector editor.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[AttributeUsage( AttributeTargets.Class )]
#endregion
public class OCExposePropertyFieldsAttribute : Attribute
{

	/////////////////////////////////////////////////////////////////////////////

  #region Private Member Data

	/////////////////////////////////////////////////////////////////////////////

	private OCExposure _exposure;

	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

  #region Accessors and Mutators

	/////////////////////////////////////////////////////////////////////////////

	public OCExposure Exposure
	{
		get{ return _exposure;}
		set{ _exposure = value;}
	}

	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

  #region Public Member Functions

	/////////////////////////////////////////////////////////////////////////////

	public OCExposePropertyFieldsAttribute 
		(OCExposure exposure = OCExposure.PropertiesAndFields)
	{
		_exposure = exposure;
	}

	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

  #region Private Member Functions

	/////////////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

  #region Other Members

	/////////////////////////////////////////////////////////////////////////////

	public enum OCExposure
	{
		None
	, PublicFieldsOnly
	, FieldsOnly
	, PropertiesOnly
	, PublicPropertiesOnly
	, PropertiesAndFields
	};

	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

}// class OCExposeProperties

}// namespace Attributes

}// namespace OpenCog



