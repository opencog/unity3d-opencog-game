
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
/// The OpenCog OCSingletonScriptableObject.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
#endregion
public class OCSingletonScriptableObject<T> : OCScriptableObject
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
		get
		{
			return m_ExamplePrivateVar;
		}

		set
		{
			m_ExamplePrivateVar = value;
		}
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------	

	#region Constructors

	//---------------------------------------------------------------------------
		
	/// <summary>
	/// Initializes a new instance of the <see cref="OpenCog.OCSingletonScriptableObject"/> class.
	/// Generally, intitialization should occur in the Start function.
	/// </summary>
	public OCSingletonScriptableObject()
	{
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
	/// Raises the enable event when OCSingletonScriptableObject is loaded.
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
	/// Raises the disable event when OCSingletonScriptableObject goes out of
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
	/// Raises the destroy event when OCSingletonScriptableObject is about to be
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

	#region Member Classes

	//---------------------------------------------------------------------------		

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCSingletonScriptableObject

}// namespace OpenCog




