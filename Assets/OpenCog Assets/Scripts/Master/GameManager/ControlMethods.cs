
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
using OpenCog.Utilities.Logging;
using UnityEngine;

#region Usings, Namespaces, and Pragmas


#endregion

//The private field is assigned but its value is never used
#pragma warning disable 0414 




namespace OpenCog.Master
{

	public partial class GameManager:OCSingletonMonoBehaviour<GameManager>
	{
		public interface ControlMethods
		{
			void QuitWithError(int code);
			void QuitWithSuccess();
			void ping();
		}

		protected class _ControlMethods:OCSingletonMonoBehaviour<_ControlMethods>, ControlMethods
		{
			protected _ControlMethods(){}
			
			/// <summary>This initialization function will be called automatically by the SingletonMonoBehavior's Awake().</summary>
			protected override void Initialize()
			{
				//shouldn't be destroyed with scenes. 
				DontDestroyOnLoad(this);
				
			}
			
			/// <summary>Used to instantiate this class. It should only be called once. It will supply a single instance, and then throw an error
			/// if it is called a second time.</summary>
			public static _ControlMethods New()
			{
				//THERE CAN ONLY BE ONE (and it must be created by the EntityManager)
				if(_instance)
				{
					throw new OCException(OCLogSymbol.ERROR +  "Two GameManager.ControlMethods exist and this is forbidden.");
					
				}
				
				//the Singleton pattern handles everything about instantiation for us, including searching
				//the game for a pre-existing object.
				return GetInstance<_ControlMethods>();
				
			}

			/// <summary>
			/// Please Log.Error() messages before calling QuitWithError(), which simply determines
			/// how best to exit the game based on whether or not unit tests are running. 
			/// </summary>
			public void QuitWithError(int code)
			{
				//do not accidentally update anything while quitting
				//Time.timeScale = 0;  <-- cannot be done from exterior thread
										
				Debug.LogError (OCLogSymbol.FAIL + "Exiting with Error Code " + code);

				//TODO [UNTESTED]: This code works for some people and not for others.
				//It's conditions for operation seem to be largely that it needs to be on
				//the main thread

				#if UNITY_EDITOR
				
				//stop the player
				UnityEditor.EditorApplication.isPlaying = false;
				throw new OCException(OCLogSymbol.FAIL + "Exiting. Error code was: " + code);

				#else

				//try to thow this error code to make the batch people happy!
				if(SystemInfo.graphicsDeviceID == 0)
					System.Environment.Exit (code);

				Application.Quit();

				//If that didn't work or we aren't in batch mode, just attempt to throw an exception.
				throw new OCException(OCLogSymbol.FAIL + "System.Environment.Exit(" + code + ") failed."); 

				#endif


			}
			public void QuitWithSuccess()
			{
				//System.Diagnostics.Process.
				Debug.Log(OCLogSymbol.CLEARED + "Exiting Successfully.");

				#if UNITY_EDITOR

				Debug.Log(OCLogSymbol.DEBUG + "Exiting Editor.");
				UnityEditor.EditorApplication.isPlaying = false;

				#else

				Debug.Log(OCLogSymbol.DEBUG + "Exiting System.");

				//try to thow this successful code to make the batch people happy!
				if(SystemInfo.graphicsDeviceID == 0)
					System.Environment.Exit (0);

				//If that didn't work or we aren't in batch mode, just attempt to quit
				Application.Quit();

				#endif
			}


			public void ping()
			{
			}


		}
	}
}