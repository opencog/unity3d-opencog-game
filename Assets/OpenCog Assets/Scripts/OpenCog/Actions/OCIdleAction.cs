
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
/// The OpenCog OCIdleAction.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
[ExecuteInEditMode]
#endregion
public class OCIdleAction : OCAction
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

	public OCIdleAction()
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

	public void Initialize()
	{
		HasAnimation = true;
		Animation = new OCAnimation();//ScriptableObject.CreateInstance<OCAnimation>();
		Animation.Initialize(gameObject, animation["idle"]);
		Animation.AnimationState_.wrapMode = WrapMode.Once;
		Animation.AnimationState_.layer = -1;
		Animation.OnStart = "IdleStart";
		Animation.OnEnd = "IdleEnd";

		DontDestroyOnLoad(this);
	}

	public override void Execute()
	{
		Animation.Play();
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

	public void IdleStart()
	{
		Animation.Start();
	}

	public void IdleEnd()
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

}// class OCIdleAction

}// namespace Actions

}// namespace OpenCog




