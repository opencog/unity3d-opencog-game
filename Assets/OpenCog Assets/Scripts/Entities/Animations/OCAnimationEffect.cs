
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
[ExecuteInEditMode]	
	
#endregion
public class OCAnimationEffect : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------

	/// <summary>
	/// The target Unity game object to be animated.
	/// </summary>
	[SerializeField]
	private GameObject _Target;// = null;

	/// <summary>
	/// The Unity animation state that we're wrapping.
	/// </summary>
	[SerializeField]
	private AnimationState _State;// = null;
			
	[SerializeField]
	private WrapMode _Wrap;// = WrapMode.Default;
	
	[SerializeField]
	private int _Layer;// = -1;
			
	[SerializeField]
	private string _StateName;// = "";

	[SerializeField]
	private float _Speed = 1.0f;

	/// <summary>
	/// The iTween parameters for the wrapped animation state.
	/// </summary>
	[SerializeField]	
	private Hashtable _iTweenParams;

	[SerializeField]
	private Vector3 _Translation;

	[SerializeField]
	private Vector3 _Rotation;

	/// <summary>
	/// The length of the animation's cross fade.
	/// </summary>
	[SerializeField]
	private float _FadeLength;// = 0.5f;

//	private bool _initialized = false;

	[SerializeField]
	private bool _IsTranslationVsRotation = true;



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
		get{ return _State;}
		set{ _State = value;}
	}

	public string StateName
	{
		get{ return _StateName;}
		set{ _StateName = value;}
	}
			
	public WrapMode Wrap
	{
		get{ return _Wrap;}
		set{ _Wrap = value;}
	}
			
	public int Layer
	{
		get{ return _Layer;}
		set{ _Layer = value;}
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
		get{ return _FadeLength;}
		set{ _FadeLength = value;}
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
		get{ return _Target;}
		set{ _Target = value;}
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
	public float Delay
	{
		get{ return ValueOrDefault<float>(iT.MoveBy.delay);}
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

	#region Public Member Functions

	//---------------------------------------------------------------------------
			
	public void OnEnable()
	{
		Initialize();
	}

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
	public void Initialize()
	{
		if(_iTweenParams == null) _iTweenParams = new Hashtable();
		if(_Target != null)
		{
			_State = _Target.animation[_StateName];
			_State.speed = _Speed;
			Time = _State.length / _State.speed + 0.01f;
			_State.wrapMode = _Wrap;
			_State.layer = _Layer;		
		}
		
			
		EaseType = "linear";//iTween.EaseType.linear;
		Delay = 0.0f; 
		OnStart = "StartAnimationEffect";
		OnEnd = "EndAnimationEffect";
				
		_iTweenParams["OnStartTarget"] = gameObject;
		_iTweenParams["OnEndTarget"] = gameObject;


		//TODO [TASK]: Remove _Translation and _Rotation and find a way to serialize Hashtables directly.
		if(_IsTranslationVsRotation)
		{
			MoveByX = _Translation.x;
			MoveByY = _Translation.y;
			MoveByZ = _Translation.z;
		}
		else
		{
			RotateByX = _Rotation.x;
			RotateByY = _Rotation.y;
			RotateByZ = _Rotation.z;
		}
		//_initialized = true;
		//DontDestroyOnLoad(this);
	}

	/// <summary>
	/// Play this animation. 
	/// </summary>
	public void Play()
	{
		if(_IsTranslationVsRotation)
			iTween.MoveBy(_Target, _iTweenParams);
		else
			iTween.RotateBy(_Target, _iTweenParams);
	}

	public void Stop()
	{
		if(_Target.GetComponent<iTween>() != null)
			iTween.Stop(_Target);
	}

	public bool IsPlaying
	{
		get { return _Target.animation.IsPlaying(_State.name);}
	}

	public bool IsPlayingButNotThis
	{
		get { return _Target.animation.isPlaying && !IsPlaying;}
	}

	/// <summary>
	/// Call from the function which is the value of OnStart.
	/// </summary>
	public void StartAnimationEffect()
	{
		_Target.animation.CrossFade(_State.name, _FadeLength);
	}

	/// <summary>
	/// Call from the function which is the value of OnEnd.
	/// </summary>
	public void EndAnimationEffect()
	{
		OCCharacterMotor motor = _Target.GetComponent<OCCharacterMotor>();
		motor.enabled = true;

		if(_State.wrapMode != WrapMode.Loop)
		{
			_Target.animation.Stop();
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




