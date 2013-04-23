
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
using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace OpenCog.Aspects
{

/// <summary>
/// The OpenCog Log Aspect.
/// </summary>
#region Class Attributes

[Serializable]
[MulticastAttributeUsage(MulticastTargets.Method, Inheritance = MulticastInheritance.Multicast)]	
#endregion
public class OCLogAspect : OnMethodBoundaryAspect
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
		
	/// <summary>
	/// Initializes a new instance of the <see cref="OpenCog.OCLogAspect"/> class.
	/// Generally, intitialization should occur in the Start function.
	/// </summary>
//	public OCLogAspect()
//	{
//	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public override void OnEntry(MethodExecutionArgs args)
	{
		Debug.Log(Environment.NewLine);

		Debug.Log(string.Format("Entering [ {0} ] ...", args.Method));

		base.OnEntry(args);
	}

	public override void OnExit(MethodExecutionArgs args)
	{
		Debug.Log(string.Format("Leaving [ {0} ] ...", args.Method));

		base.OnExit(args);
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

}// class OCLogAspect

}// namespace OpenCog.Aspects




