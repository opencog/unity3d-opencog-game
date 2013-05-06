
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

namespace Extensions
{

/// <summary>
/// The OpenCog Animation Effect.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
#endregion
public class OCAnimationEffect
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------

	/// <summary>
	/// The target Unity game object to be animated.
	/// </summary>
	private GameObject _target = null;

	/// <summary>
	/// The Unity animation state that we're wrapping.
	/// </summary>
	private AnimationState _state = null;

	/// <summary>
	/// The iTween parameters for the wrapped animation state.
	/// </summary>
	private Hashtable _iTweenParams;

	/// <summary>
	/// The length of the animation's cross fade.
	/// </summary>
	private float _fadeLength = 0.5f;

//	private bool _initialized = false;



	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------

	/// <summary>
	/// Gets or sets the Unity animation state that we're wrapping.
	/// </summary>
	/// <value>
	/// The Unity animation state that we're wrapping.
	/// </value>
	[OCTooltip("The Unity animation state that we're wrapping.")]
	public AnimationState State
	{
		get{ return _state;}
		set{ _state = value;}
	}

	/// <summary>
	/// Gets or sets the length of the animation's cross fade.
	/// </summary>
	/// <value>
	/// The length of the animation's cross fade.
	/// </value>
	[OCTooltip("The length of the animation's cross fade.")]
	public float FadeLength
	{
		get{ return this._fadeLength;}
		set{ _fadeLength = value;}
	}

	/// <summary>
	/// Gets or sets the target Unity game object to be animated.
	/// </summary>
	/// <value>
	/// The target Unity game object to be animated.
	/// </value>
	[OCTooltip("The target Unity game object to be animated.")]
	public GameObject Target
	{
		get{ return this._target;}
		set{ _target = value;}
	}

//	public bool IsInitialized
//	{
//		get { return _initialized;}
//		set { _initialized = value;}
//	}

	/// <summary>
	/// Gets or sets the time in seconds that the animation will take to
	/// complete.
	/// </summary>
	/// <value>
	/// The time in seconds.
	/// </value>
	[OCTooltip("The time in seconds that the animation will take to complete.")]
	public float Time
	{
		get{ return ValueOrDefault<float>(iT.MoveBy.time);}
		set{ _iTweenParams[iT.MoveBy.time] = value;}
	}

	/// <summary>
	/// Gets or sets the shape of the easing curve applied to the animation.
	/// </summary>
	/// <value>
	/// A string representing the type of the easing curge.
	/// e.g. "linear", "punch", "spring", etc.
	/// </value>
	[OCTooltip("The shape of the easing curve applied to the animation.")]
	public string EaseType
	{
		get{ return ValueOrDefault<string>(iT.MoveBy.easetype);}
		set{ _iTweenParams[iT.MoveBy.easetype] = value;}
	}

	/// <summary>
	/// Gets or sets the time in seconds that the animation will wait before
	/// beginning.
	/// </summary>
	/// <value>
	/// The delay in seconds.
	/// </value>
	[	OCTooltip
		("The time in seconds that the animation will wait before beginning.") ]
	public int Delay
	{
		get{ return ValueOrDefault<int>(iT.MoveBy.delay);}
		set{ _iTweenParams[iT.MoveBy.delay] = value;}
	}

	/// <summary>
	/// Gets or sets the name of the function to call at the start of the
	/// animation.
	/// </summary>
	/// <value>
	/// A string specifying name of the function.
	/// </value>
	[ OCTooltip
		("The name of the function to call at the start of the animation.") ]
	public string OnStart
	{
		get{ return ValueOrDefault<string>(iT.MoveBy.onstart);}
		set{ _iTweenParams[iT.MoveBy.onstart] = value;}
	}

	/// <summary>
	/// Gets or sets the name of the function to call at the end of the
	/// animation.
	/// </summary>
	/// <value>
	/// A string specifying name of the function.
	/// </value>
	[ OCTooltip
		("The name of the function to call at the end of the animation.") ]
	public string OnEnd
	{
		get{ return ValueOrDefault<string>(iT.MoveBy.oncomplete);}
		set{ _iTweenParams[iT.MoveBy.oncomplete] = value;}
	}

	//@TODO: Don't move the characters directly through the animation.

	/// <summary>
	/// Gets or sets the distance to move by in the x-axis as part of the
	/// animation.
	/// </summary>
	/// <value>
	/// The move by x.
	/// </value>
	[ OCTooltip
		("The distance to move by in the x-axis as part of the animation.") ]
	public float MoveByX
	{
		get{ return ValueOrDefault<float>(iT.MoveBy.x);}
		set{ _iTweenParams[iT.MoveBy.x] = value;}
	}

	/// <summary>
	/// Gets or sets the distance to move by in the y-axis as part of the
	/// animation.
	/// </summary>
	/// <value>
	/// The move by y.
	/// </value>
	[ OCTooltip
		("The distance to move by in the y-axis as part of the animation.") ]
	public float MoveByY
	{
		get{ return ValueOrDefault<float>(iT.MoveBy.y);}
		set{ _iTweenParams[iT.MoveBy.y] = value;}
	}


	/// <summary>
	/// Gets or sets the distance to move by in the z-axis as part of the
	/// animation.
	/// </summary>
	/// <value>
	/// The move by z.
	/// </value>
	[ OCTooltip
		("The distance to move by in the z-axis as part of the animation.") ]
	public float MoveByZ
	{
		get{ return ValueOrDefault<float>(iT.MoveBy.z);}
		set{ _iTweenParams[iT.MoveBy.z] = value;}
	}

	/// <summary>
	/// Gets or sets the distance to rotate by in the x-axis as part of the
	/// animation.
	/// </summary>
	/// <value>
	/// The rotation by x.
	/// </value>
	[ OCTooltip
		("The distance to rotate by in the x-axis as part of the animation.") ]
	public float RotateByX
	{
		get{ return ValueOrDefault<float>(iT.RotateBy.x);}
		set{ _iTweenParams[iT.RotateBy.x] = value;}
	}

	/// <summary>
	/// Gets or sets the distance to rotate by in the y-axis as part of the
	/// animation.
	/// </summary>
	/// <value>
	/// The rotation by y.
	/// </value>
	[ OCTooltip
		("The distance to rotate by in the y-axis as part of the animation.") ]
	public float RotateByY
	{
		get{ return ValueOrDefault<float>(iT.RotateBy.y);}
		set{ _iTweenParams[iT.RotateBy.y] = value;}
	}


	/// <summary>
	/// Gets or sets the distance to rotate by in the z-axis as part of the
	/// animation.
	/// </summary>
	/// <value>
	/// The rotation by z.
	/// </value>
	[ OCTooltip
		("The distance to rotate by in the z-axis as part of the animation.") ]
	public float RotateByZ
	{
		get{ return ValueOrDefault<float>(iT.RotateBy.z);}
		set{ _iTweenParams[iT.RotateBy.z] = value;}
	}

	public Vector3 Position
	{
		get{ return ValueOrDefault<Vector3>("position");}
		set{ _iTweenParams["position"] = value;}
	}

			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------	

	#region Constructors

	//---------------------------------------------------------------------------

	/// <summary>
	/// Initializes a new instance of the
	/// <see cref="OpenCog.Extensions.OCAnimationEffect"/> class.
	/// </summary>

	public OCAnimationEffect()
	{
	}

	public OCAnimationEffect(OCAnimationEffect anim)
	{
		FadeLength = anim.FadeLength;
		Target = anim.Target;
		State = anim.State;
		_iTweenParams = anim._iTweenParams;
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	/// <summary>
	/// Initialize the OpenCog Animation with the specified target and
	/// animationState.
	/// </summary>
	/// <param name='target'>
	/// The target object to animate.
	/// </param>
	/// <param name='animationState'>
	/// The Unity animation state that corresponds to this OpenCog animation.
	/// </param>
	public void Initialize(GameObject target, AnimationState animationState)
	{

		Target = target;
		State = animationState;
		_iTweenParams = new Hashtable();
		Time = State.length / State.speed + 0.01f;
		EaseType = "linear";//iTween.EaseType.linear;
		Delay = 0;

		//_initialized = true;
		//DontDestroyOnLoad(this);
	}

	/// <summary>
	/// Play this animation.
	/// </summary>
	public void PlayAndTranslate()
	{
		iTween.MoveBy(_target, _iTweenParams);
	}

	public void PlayAndRotate()
	{
		iTween.RotateBy(_target, _iTweenParams);
	}

	public void Stop()
	{
		iTween.Stop(_target);
	}

	public bool IsPlaying
	{
		get { return _target.animation.IsPlaying(State.name);}
	}

	public bool IsPlayingButNotThis
	{
		get { return _target.animation.isPlaying && !IsPlaying;}
	}

	/// <summary>
	/// Call from the function which is the value of OnStart.
	/// </summary>
	public void Start()
	{
		_target.animation.CrossFade(_state.name, _fadeLength);
	}

	/// <summary>
	/// Call from the function which is the value of OnEnd.
	/// </summary>
	public void End()
	{
		if(_state.wrapMode != WrapMode.Loop)
		{
			_target.animation.Stop();
		}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
			
	private T ValueOrDefault<T>(string key)
	{
		if(_iTweenParams.Contains(key))
			return (T)_iTweenParams[key];
		else
			return default(T);
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Member Classes

	//---------------------------------------------------------------------------		

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCAnimation

}// namespace Extensions

}// namespace OpenCog




