
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
using ProtoBuf;
using UnityEngine;

namespace OpenCog
{

namespace Extensions
{

/// <summary>
/// The OpenCog Transform extensions.
/// </summary>
#region Class Attributes

#endregion

public static class OCTransform
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------


	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
            
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

	/// <summary>
	/// Sets the x coordinate of the transform's position.
	/// </summary>
	/// <param name='transform'>
	/// This is an extension of the Transform class.
	/// </param>
	/// <param name='x'>
	/// The new value of the x coordinate.
	/// </param>
	public static void SetX(this Transform transform, float x)
	{
		Vector3 newPosition =
    	new Vector3(x, transform.position.y, transform.position.z);

		transform.position = newPosition;
	}

	/// <summary>
	/// Sets the y coordinate of the transform's position.
	/// </summary>
	/// <param name='transform'>
	/// This is an extension of the Transform class.
	/// </param>
	/// <param name='y'>
	/// The new value of the y coordinate.
	/// </param>
	public static void SetY(this Transform transform, float y)
	{
		Vector3 newPosition =
    	new Vector3(transform.position.x, y, transform.position.z);

		transform.position = newPosition;
	}

	/// <summary>
	/// Sets the z coordinate of the transform's position.
	/// </summary>
	/// <param name='transform'>
	/// This is an extension of the Transform class.
	/// </param>
	/// <param name='z'>
	/// The new value of the z coordinate.
	/// </param>
	public static void SetZ(this Transform transform, float z)
	{
		Vector3 newPosition =
    	new Vector3(transform.position.x, transform.position.y, z);

		transform.position = newPosition;
	}

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

}// class OCTransform

}// namespace Extensions

}// namespace OpenCog




