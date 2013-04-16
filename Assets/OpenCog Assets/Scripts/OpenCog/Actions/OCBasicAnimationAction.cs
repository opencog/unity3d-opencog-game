
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

namespace Actions
{

/// <summary>
/// The OpenCog OCBasicAnimationAction.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
#endregion
public abstract class OCBasicAnimationAction : OCAction
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------

	bool m_IsTranslation = false;
	bool m_IsRotation = false;

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------

	public bool IsRotation
	{
		get { return this.m_IsRotation;}
		set {	m_IsRotation = value;}
	}

	public bool IsTranslation {
		get {return this.m_IsTranslation;}
		set {m_IsTranslation = value;}
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

	public abstract void Initialize();

	public void Awake()
	{
		Initialize();
	}

	public void Start()
	{
		Initialize();
	}

	public void OnEnable()
	{
		Initialize();
	}

	public override void Execute()
	{
		if(m_IsTranslation)
			Animation.PlayAndTranslate();
		else if(m_IsRotation)
			Animation.PlayAndRotate();
	}

	public override bool IsExecuting()
	{
		return Animation.IsPlaying;
	}

	public override void Terminate()
	{
		Animation.Stop();
	}

	public override bool ShouldTerminate()
	{
		return Animation.IsPlayingButNotThis;
	}

	public void BasicAnimationStart()
	{
		Animation.Start();
	}

	public void BasicAnimationEnd()
	{
		Animation.End();
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

}// class OCBasicAnimationAction

}// namespace Actions

}// namespace OpenCog




