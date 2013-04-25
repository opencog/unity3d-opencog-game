
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

#region Namespaces
using System;
using System.Collections;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ProtoBuf;
using UnityEngine;
#endregion

namespace OpenCog
{

/// <summary>
/// The OpenCog Test2.
/// </summary>
#region Class Attributes
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
#endregion
public class Test2 : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------

	private int m_ExamplePrivateVar = 0;

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------

	public int ExamplePublicVar
	{
		get{ return m_ExamplePrivateVar; }

		set{ m_ExamplePrivateVar = value; }
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	/// <summary>
	/// Called when the script instance is being loaded.
	/// </summary>
	public void Awake()
	{
	}

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start()
	{
	}

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	public void Update()
	{
	}

	/// <summary>
	/// Called once per frame after all Update calls
	/// </summary>
	public void LateUpdate()
	{
	}

	/// <summary>
	/// Raises the enable event when Test2 is loaded.
	/// </summary>
	public void OnEnable()
	{
		Debug.Log
		(
			string.Format
			(
				"In {0}.OnEnable"
			, gameObject.name + "\\" + GetType().Name
			)
		);
	}

	/// <summary>
	/// Raises the disable event when Test2 goes out of
	/// scope.
	/// </summary>
	public void OnDisable()
	{
		Debug.Log
		(
			string.Format
			(
				"In {0}.OnDisable"
			, gameObject.name + "\\" + GetType().Name
			)
		);
	}

	/// <summary>
	/// Raises the destroy event when Test2 is about to be
	/// destroyed.
	/// </summary>
	public void OnDestroy()
	{
		Debug.Log
		(
			string.Format
			(
				"In {0}.OnDestroy"
			, gameObject.name + "\\" + GetType().Name
			)
		);
	}

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
	/// Initializes a new instance of the <see cref="OpenCog.Test2"/>
	/// class.  Generally, intitialization should occur in the Start or Awake
	/// functions, not here.
	/// </summary>
	public Test2()
	{
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class Test2

}// namespace OpenCog




