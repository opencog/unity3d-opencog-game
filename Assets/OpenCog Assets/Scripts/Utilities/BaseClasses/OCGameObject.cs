
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
/// The OpenCog GameObject extensions.
/// </summary>
#region Class Attributes

#endregion
public static class OCGameObject
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
	/// Gets the component of the specified type safely.
	/// </summary>
	/// <returns>
	/// The component of the specified type.
	/// </returns>
	/// <param name='obj'>
	/// This is an extension for the GameObject class.
	/// </param>
	/// <typeparam name='T'>
	/// The type of component to get.
	/// </typeparam>
	public static T GetSafeComponent<T>(this GameObject obj) where T : MonoBehaviour
	{
		T component = obj.GetComponent<T>();
		
		if(component == null)
		{
			Debug.LogError("Expected to find component of type "
		         + typeof(T) + " but found none", obj);
		}
		
		return component;
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

}// class OCGameObject

}// namespace Extensions

}// namespace OpenCog




