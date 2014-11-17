
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

/// <summary>
/// The OpenCog OCException.
/// </summary>
#region Class Attributes

#endregion
public class OCException : Exception
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

	#region Other Members

	//---------------------------------------------------------------------------		
		
	/// <summary>
	/// Initializes a new instance of the 
	/// <see cref="OpenCog.OCException"/> class.
	/// </summary>
	public OCException ()
	{
	}
	
	public OCException (string message) : base(message)
	{
	}
		
	public OCException (string message, Exception innerException)	
	: base(message, innerException)
	{
	}
		
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCException

}// namespace OpenCog




