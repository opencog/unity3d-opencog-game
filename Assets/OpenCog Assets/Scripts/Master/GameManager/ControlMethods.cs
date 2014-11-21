
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
			public void QuitWithError(int code = 1)
			{
				//do not accidentally update anything while quitting
				Time.timeScale = 0; 

				//if the application is the editor, we can't REALLY quit with an error code,
				//but we can print the error code to screen and terminate the game
				if( Application.isEditor)
				{
					//throw the exception
					throw new OCException(OCLogSymbol.FAIL + "Exiting with Error Code " + code.ToString());

					//quit application
					//Debug.Break();  <---- unreachable code detected
				}

				//this should be the unit tests
				if(UnityEditorInternal.InternalEditorUtility.inBatchMode || SystemInfo.graphicsDeviceID == 0)
				{
					//why do I bother with this? Answer: Unity WANTS to close in batch mode
					//by throwing a new OCException... IT doesn't want us to use kill
					//but if we're going to send another error code, we might as well try
					if(code == 1)
					{
						//according to the unity documentation, this will close things
						throw new OCException(OCLogSymbol.FAIL + "Exiting with Error Code 1");

						//return; <--- unreachable code detected; confirmation this should close
					}
					else
					{
						//System.Diagnostics.Process.
						Debug.Log (OCLogSymbol.FAIL + "Exiting with Error Code " + code);

						//TODO [UNTESTED]: This code works for some people and not for others.
						//It's conditions for operation seem to be largely that it needs to be on
						//the main thread
						System.Environment.Exit (code);

						//<Uh oh; the compiler does not consider this to be unreachable code!!
						//we'll leave it in in case Exit fails, so at least we're returning a failure.
						throw new OCException(OCLogSymbol.FAIL + "System.EnvironmentExit(" + code + ") failed.");  
					}
				}

				//this code should be for if we are just running happily in the player!

				//exceptions shouldn't close things
				throw new OCException(OCLogSymbol.FAIL + "Exiting! (The error Code would be " + code + " on the command line.)");

				#if !UNITY_EDITOR
				//this is the unity kosher quit. The internet has not guarenteed me its unnecessary.
				Application.Quit(); //<-- unreachable code detected; confirmation this should close
				#endif

			}
			public void QuitWithSuccess()
			{

			}


		}
	}
}