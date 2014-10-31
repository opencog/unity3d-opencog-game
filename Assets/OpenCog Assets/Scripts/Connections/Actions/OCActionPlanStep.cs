
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
using Behave.Runtime;
using OpenCog.Actions;
using OpenCog.Attributes;
using OpenCog.Extensions;
using OpenCog.Map;
using ProtoBuf;
using UnityEngine;
using ContextType = BLOCBehaviours.ContextType;
using OCID = System.Guid;
using Tree = Behave.Runtime.Tree;
using TreeType = BLOCBehaviours.TreeType;
using System.Linq;
using System.Xml;
using OpenCog.Utility;

//using OpenCog.Aspects;

namespace OpenCog
{

namespace Actions
{

#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
#endregion
public class OCActionPlanStep : OCScriptableObject
{
	private Behave.Runtime.Tree _behaviour;
	
	private OCAction.OCActionArgs _arguments;
	
	public OCAction.OCActionArgs Arguments
	{
		get
		{
			return this._arguments;
		}
		set
		{
			_arguments = value;
		}
	}
	
	public Tree Behaviour
	{
		get
		{
			return this._behaviour;
		}
		set
		{
			_behaviour = value;
		}
	}
	
	static public int MaxRetries = 5;
	
	public int Retry = 0;
	
	public override string ToString()
	{
		return _behaviour.ToString() + ", " + _arguments.ToString(); 
	}
}

}
}

