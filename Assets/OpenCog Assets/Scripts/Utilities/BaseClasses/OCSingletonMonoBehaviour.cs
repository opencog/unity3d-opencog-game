
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
using UnityEngine;
using OpenCog.Extensions;
using OpenCog.Utilities.Logging;

namespace OpenCog
{

	/// <summary>
	/// The OpenCog Singleton for MonoBehaviours.  Any class which inherits 
	/// from this will be a singleton, monobehaviour.
	/// </summary>
	public class OCSingletonMonoBehaviour<T> : OCMonoBehaviour
	where T : OCMonoBehaviour 
	{

		/// <summary>The singleton instance.</summary>
		protected static T _instance = null;

		//With Unity, there is more than one way to get that very first instance of the singleton. This accounts for the drag and drop onto a game object method.
		//In practice, it is possible that a Singleton could be dragged onto a game object and have a DoNotDestroyOnLoad on it.
		//Then when another scene is loaded with the same class dragged onto another game object, we could end up in a situation with 
		//a second instance coming awake. Uh oh! This will help us address that problem.

		//in the event that we expect attributes to be dragged onto new singleton scripts to modify in-game behavior,
		//it should be quite possible to override this Awake function in subclasses and load in 'new' attributes
		//to the old instance of the singleton before destroying the new/unnecessary one. 

		//This code was added later after many singletons already handled their own instancing/initialization (because it ought to have been here)
		//The game then ran fine with it. It's cleared to stay. 
		public void Awake()
		{
			//if the instance doesn't exist, good work, we're on target
			if(_instance == null) _instance = this as T;

			//Annihilate any accidentally created new instances.
			if(this != _instance)
			{
				Destroy(this.gameObject);
				Destroy(this);
			}

			//initialize any properly created instances
			else
			{
				this.Initialize ();
			}

			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is awake.");
		}

		/// <summary>Exists to be overwritten. Kept virtual to work as a fallback when a derived class has no necessary initialization.</summary>
		protected virtual void Initialize()
		{
		}

		/// <summary>An accessor function which can be used by derived classes only, that will attempt to instantiate the Singleton if it does not already exist, through Instantiate()</summary>
		protected static U GetInstance<U>() where U : T
		{
			//The AND (&&) property in C# uses lazy evaluation. This means
			//it will only called Instantiate if _instance == null. 
			// Logical OR (||) is also lazy. 
			if(_instance == null && !Instantiate<U>())
			{
				Debug.LogError( "In OCSingletonMonoBehaviour.Instance, an instance of singleton " + typeof(U) + " does not exist and could not be instantiated. Confusion abounds.");
			}

			//cast the instance to type U and send it on its way. 
			return (U)_instance;	
		}

		/// <summary> Instantiate this singleton instance. </summary>
		protected static bool Instantiate<U>() where U : T
		{
			//Assert that we're not already instantiated
			if(_instance != null)
			{
				throw new OCException("In OCSingletonMonoBehaviour.Instantiate does not have a null _instance.");
			}
				
			//Find one in the scene if we've dragged it to the screen. (The awake function should catch this,
			//but we'll give it a try anyway, particularly in case Instantiate somehow gets called before Awake)
			_instance = (T)FindObjectOfType(typeof(U));
				
			//Otherwise create a new object for our monobehaviour singleton.
			if(_instance == null)
			{
				//by the way, name it after our class so that we can identify it in the inspector
				GameObject gameObject = new GameObject(typeof(U).ToString(), typeof(U));

				//and nab out the instance
				_instance = gameObject.GetComponent<U>();		
			}

			//assuming no catastrophic failure, this should always be true.
			return _instance != null;
		}
						

	}// class OCSingletonMonoBehaviour
}// namespace OpenCog.Utility




