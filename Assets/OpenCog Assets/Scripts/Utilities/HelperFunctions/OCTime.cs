
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
/// The OpenCog DeltaTime extension.  Basically a wrapper class for
/// Time.deltaTime and Time.TimeSinceLevelLoad to make pausing easier.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
#endregion

public class OCTime
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------


	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------

	/// <summary>
	/// Gets or sets the delta time.
	/// </summary>
	/// <value>
	/// The delta time.
	/// </value>
	public float DeltaTime
	{
		get {return Time.deltaTime;}
		//set {Time.deltaTime = value;}
	}

	/// <summary>
	/// Gets or sets the time since level load.
	/// </summary>
	/// <value>
	/// The time since level load.
	/// </value>
	public float TimeSinceLevelLoad
	{
		get {return Time.timeSinceLevelLoad;}
		//set {Time.timeSinceLevelLoad = value;}
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

}// class OCTime

}// namespace Extensions            

}// namespace OpenCog




