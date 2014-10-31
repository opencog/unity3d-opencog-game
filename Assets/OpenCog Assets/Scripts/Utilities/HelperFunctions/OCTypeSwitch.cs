
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

namespace OpenCog.Serialization
{


/// <summary>
/// The OpenCog Type Switch idomatic class.  Adapted from here:
/// http://blogs.msdn.com/b/jaredpar/archive/2008/05/16/switching-on-types.aspx
/// Intended for use as a type of Switch Statement for user-defined types.
/// </summary>
#region Class Attributes

#endregion

public static class OCTypeSwitch
{

	/////////////////////////////////////////////////////////////////////////////

  #region Private Member Data

	/////////////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////////////

	#endregion

	/////////////////////////////////////////////////////////////////////////////

	#region Accessors and Mutators

	/////////////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////////////

	#endregion

	/////////////////////////////////////////////////////////////////////////////

	#region Public Member Classes

	/////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// A small class for storing meta-data for each case in our switch.
	/// </summary>
	public class CaseInfo
	{
		public bool IsDefault { get; set; }

		public Type Target { get; set; }

		public Action<object> Action { get; set; }
	}

	/////////////////////////////////////////////////////////////////////////////

	#endregion

	/////////////////////////////////////////////////////////////////////////////

	#region Public Member Functions

	/////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Do the switch between the specified cases on the source.
	/// </summary>
	/// <param name='source'>
	/// The source object on which actions will be executed.
	/// </param>
	/// <param name='cases'>
	/// The cases of our switch statement, encapsulated in CaseInfos.
	/// </param>
	public static void Do(object source, params CaseInfo[] cases)
	{
		var type = source.GetType();
		foreach(var entry in cases)
		{
			if(entry.IsDefault || entry.Target.IsAssignableFrom(type))
			{
				entry.Action(source);
				break;
			}
		}
	}
			
	/// <summary>
	/// Do the switch between the specified cases without a source object.
	/// </summary>
	/// <param name='cases'>
	/// The cases of our switch statement, encapsulated in CaseInfos.
	/// </param>
	public static void Do<T>(params CaseInfo[] cases)
	{
		var source = default(T);
		foreach(var entry in cases)
		{
			if(entry.IsDefault || entry.Target.IsAssignableFrom(typeof(T)))
			{
				entry.Action(source);
				break;
			}
		}
	}

	/// <summary>
	/// An idiomatic way to represent cases without parameters.
	/// </summary>
	/// <param name='action'>
	/// The action to execute on our source object.
	/// </param>
	/// <typeparam name='T'>
	/// The user-defined type we would like this case to be for.
	/// </typeparam>
	public static CaseInfo Case<T>(Action action)
	{
		return new CaseInfo()
		{
      Action = x => action(),
      Target = typeof(T)
    };
	}

	/// <summary>
	/// An idiomatic way to represent cases with a parameter of a user-defined
	/// type.
	/// </summary>
	/// <param name='action'>
	/// The action to execute on our source object.
	/// </param>
	/// <typeparam name='T'>
	/// The user-defined type we would like this case to be for.
	/// </typeparam>
	public static CaseInfo Case<T>(Action<T> action)
	{
		return new CaseInfo()
		{
    	Action = (x) => action((T)x),
    	Target = typeof(T)
    };
	}

	/// <summary>
	/// An idiomatic way to represent a default case.
	/// </summary>
	/// <param name='action'>
	/// The action to execute on our source object.
	/// </param>
	public static CaseInfo Default(Action action)
	{
		return new CaseInfo()
		{
    	Action = x => action(),
      IsDefault = true
    };
	}

	/////////////////////////////////////////////////////////////////////////////

	#endregion

	/////////////////////////////////////////////////////////////////////////////

	#region Private Member Functions

	/////////////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////////////

	#endregion

	/////////////////////////////////////////////////////////////////////////////

}// class OCTypeSwitch

}// namespace OpenCog



