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
using OpenCog.Map;
using OpenCog.Utilities.Logging;

#region Usings, Namespaces, and Pragmas

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

using OpenCog;
using OpenCog.Attributes;
using OpenCog.Master;
using OpenCog.Extensions;
using OpenCog.BlockSet.BaseBlockSet;

//The private field is assigned but its value is never used
#pragma warning disable 0414
#endregion

namespace OpenCog.Interfaces.Game
{
	
	/// <summary>
	/// <para>Current Unit tests:</para>
	/// <para>UNITTEST_EMBODIMENT</para>
	/// <para>UNITTEST_BLOCK</para>
	/// <para>UNITTEST_PLAN</para>
	/// <para>UNITTEST_SECONDPLAN</para>
	/// <para></para>
	/// 
	/// </summary>
	#region Class Attributes
		
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	[OCExposePropertyFields]
	[Serializable]
	#endregion





	public class OCUnitTests : OpenCog.OCSingletonMonoBehaviour<OCUnitTests>
	{


		#region 						Singleton Stuff
		//---------------------------------------------------------------------------
			
			
		//Do not let anyone instantiate this (not that monobehaviors should be instantiated in the first place)
		//By the way, typically speaking, don't initialize in constructors for monobehaviors
		protected OCUnitTests()
		{
		}
			
		/// <summary>This initialization function creates the submanagers, and will be called automatically by the SingletonMonoBehavior's Awake()</summary>
		protected override void Initialize()
		{
			testFunctionsInit();
		}
			
		/// <summary>Singleton pattern Instance accessor!</summary>
		public static OCUnitTests Instance
		{
			get
			{
				//the Singleton pattern handles everything about instantiation for us, including searching
				//the game for a pre-existing object.
				return GetInstance<OCUnitTests>();
					
			}
				
		}
			
		//---------------------------------------------------------------------------
		#endregion
		#region 					Constants For Tests

		//the number of types
		public const uint numTests = 4;
		protected int selectedTests = 0;

		//the enumerated type which represents all the tests
		protected enum TestTypes: int
		{
			EMBODIMENT,
			BATTERY, 
			PLAN,
			SECONDPLAN
		}

		protected const string testConfigRunAll = "UNITTEST_ALL";

		//the names of the config variables corresponding to the enumerated type
		protected string[] testConfigNames = 
			{
				"UNITTEST_EMBODIMENT",
				"UNITTEST_BATTERY",
				"UNITTEST_PLAN",
				"UNITTEST_SECONDPLAN"
			};

		//These function prototypes promise the following functions will exist
		//virtual public IEnumerator TestEmbodiment(){}

		//the set of functions to call corresponding to the enumerated type
		protected List<Func<IEnumerator>> tests;

		protected void testFunctionsInit()
		{
			tests = new List<Func<IEnumerator>>();
			tests.Add(Instance.TestEmbodiment);
			tests.Add(Instance.TestBattery);
			tests.Add(Instance.TestPlan);
			tests.Add(Instance.TestSecondPlan);
					

		}

		#endregion
		#region 					Configuration

		protected bool[] configurations = new bool[numTests];
		public bool editorSaysToRun = false;

		protected void GetConfiguration()
		{
			//get the configuration as this handy-dandy variable
			OCConfig config = OCConfig.Instance;

			//is there any configuration information about whether we should exit on completing these tests?
			if((config.get (exitOnCompleteConfig)).ToString ().CompareTo("neutral") != 0)
			{
				Debug.Log (OCLogSymbol.DEBUG + "Configuration says to exit Unit Tests on complete: " + config.get (exitOnCompleteConfig).ToString());

				//then extract it in Boolean form!
				exitOnComplete = config.getBool(exitOnCompleteConfig);
			}

			//get the configuration  for UNITTEST_ALL & whether or not the editor box is checked.
			bool runAll = config.getBool(testConfigRunAll) || this.editorSaysToRun;

			//zero out tests selected
			selectedTests = 0;



			//iterate through all the configurations and read them in!
			for(uint i = 0; i < numTests; i++)
			{
				configurations[i] = config.getBool(testConfigNames[i]) || runAll;
				if(configurations[i] == true)
					selectedTests++;
			}

		
		}


		#endregion
		#region 				  Variables for Unit Tests (Configure In Inspector)


		//FOR THE EMBODIMENT TEST
		public Vector3 embodimentSpawn;
		public GameObject embodimentPrefab;

		//FOR THE BATTERY TEST
		public Vector3 batteryPosition;
		/// <summary>Often -transform.forward (faces the player)</summary>
		public Vector3 batteryDirection; 
		public Vector2 batteryBlockOnHUD;
		private OCBlock batteryBlock;

		//FOR THE PLANNING TEST
		public float planMinutesToSucceed = 1.0f;

		//FOR THE SECOND PLANNING TEST
		//<none>

		protected const string exitOnCompleteConfig = "UNITTEST_EXIT";
		public bool exitOnComplete = true;

		//TEST THE INITIALIZATION OF THESE VARIABLES
		public bool TestInitialization()
		{
			//OTHER EMBODIMENT STUFF
			//<none>
				
			//OTHER BATTERY STUFF
			//calculate the place of the battery
			int hudPos = (int)this.batteryBlockOnHUD.x + (int)this.batteryBlockOnHUD.y * 8;
			batteryBlock = OCMap.Instance.GetBlockSet().GetBlock(hudPos);
			
			//create the fun iterator which will make coding this easy
			List<object>[] needsInit = new List<object>[numTests];

			//btw we have to init this too...
			for(int i = 0; i < numTests; i++)
			{
				needsInit[i] = new List<object>();
			}

			//EMBODIMENT test stuff
			needsInit[(int)TestTypes.EMBODIMENT].Add(embodimentPrefab);

			//BATTERY test stuff
			needsInit[(int)TestTypes.BATTERY].Add(batteryBlock);

			//FOR THE PLAN
			//<none>

			//FOR THE SECOND PLAN
			//<none>

			bool initResult = true;
			//now iterate through every test (represented above in the List by [])
			for(uint i = 0; i < numTests; i++)
			{
				//checkout each mandatory object we added to the list
				foreach(object o in needsInit[i])
				{
					//if it's null, complain loudly
					if(o == null)
					{
						Debug.LogError("Disabling test " + testConfigNames[i] + "owed to null parameters. Check the inspector. Unit test will automatically fail."); 

						//turn off the test so we don't hurt ourselves
						configurations[i] = false;

						//and automatically fail the unit test, cause something be borked.
						results[i] = false;
						result = false;

						//since result could have been set strangely prior to this, we'll be returning a kosher variable instead
						initResult = false;
					}
				}
			}

			return initResult;
		}
		
		//TO MAKE SURE CERTAIN CLASSES ARE THROWING DATA WE'LL BE SNIFFING FOR
		protected void SolicitTestData()
		{

		
			//ask whether the battery data has been sent (as part of a MapInfo) after the finishTerrain message
			OCConnectorSingleton.Instance.dispatchFlags[(int)OCConnectorSingleton.DispatchTypes.finishTerrain] = true;
			OCConnectorSingleton.Instance.dispatchFlags[(int)OCConnectorSingleton.DispatchTypes.mapInfo] = true;

			//ask about the plan - did we get it?
			OCConnectorSingleton.Instance.receptFlags[(int)OCConnectorSingleton.ReceptTypes.robotHasPlan] = true;

			//ask if the plan has succeeded
			OCConnectorSingleton.Instance.dispatchFlags[(int)OCConnectorSingleton.DispatchTypes.actionPlanFailed] = true;
			OCConnectorSingleton.Instance.dispatchFlags[(int)OCConnectorSingleton.DispatchTypes.actionPlanSucceeded] = true;
		}
		

		#endregion
		#region  					Unit Test Results
		protected bool[] results = new bool[numTests];
		protected bool result = true;
		protected bool hasConcluded = false;

		public void RefreshResults()
		{
			//the tests are not over! Therefore, don't trust 'result'
			hasConcluded = false;

			//the default value of a bool in C++ is false
			for(uint i = 0; i < numTests; i++)
			{
				results[i] = true;
			}

			//innocent until proven guilty ;)
			result = true;

		}

		#endregion
		#region 					RunTests()
		public IEnumerator RunTests()
		{

			//wait until the game is running such that we can be pretty sure everything's 'start' already run (Since this coroutine is 
			//initialized in a Start()) By this point, things like OcWorldGenerator, etc, should already all be initialized, and so
			//a legit OCConfig file should be loaded.
			yield return new UnityEngine.WaitForFixedUpdate();
			
			//a good place to help us debug connection/initialization errors
			//yield return new UnityEngine.WaitForSeconds(20.0f);

			//INITIALIZE!
			//-----------------------------------
			//get our own configurations

			GetConfiguration();

			if(selectedTests != 0)
			{
				Debug.Log (OCLogSymbol.RUNNING + "UnitTests.RunTests() is about to run " + selectedTests + " selected tests.", this);
			}
			else
			{
				Debug.Log (OCLogSymbol.RUNNING + "UnitTests.RunTests() has no unit tests configured to run.", this);
			}

			//providing information to batch users
			//and attempting to minimize errors that can occur when the developer needs different settings from his user and forgets to set them.
			//note: exitOnComplete is not congruent with batch mode. It may also be very desirable in edit mode by the developer, or for hands-free demo purposes in
			//player mode.

			//TODO [TASK]: in the future it will be more sensible to let the developer enable/disable different options for batch, player, and editor modes!
			if(SystemInfo.graphicsDeviceID == 0)
			{
				if(!exitOnComplete)
				{
					Debug.LogWarning(OCLogSymbol.WARN + "UnitTests is running in Batch Mode but exitOnComplete is false. This is most likely not your intention. Setting exitOnComplete to true, to avoid hanging. You do not have to fix this warning.");
					exitOnComplete = true;
				}
				if(editorSaysToRun)
				{
					Debug.LogWarning(OCLogSymbol.WARN + "UnitTests is running in Batch Mode with 'editorSaysToRun' enabled. This will always enable all UnitTests, regardless of configuration files or command line arguments. This may or may not be your intention. Nevertheless, running as ordered.");
				}
				if(selectedTests == 0)
				{
					Debug.LogWarning (OCLogSymbol.WARN + "UnitTests is running in Batch Mode with no unit tests configured. This is most likely not your intention. Nevertheless, running as ordered.");
				}
			}

			//providing similar information to editor users & standalone players
			else
			{
				if(selectedTests == 0 && exitOnComplete)
				{
					Debug.LogWarning(OCLogSymbol.WARN + "UnitTests is running outside of Batch Mode with no tests enabled and exitOnComplete set to true. This is most likely not your intention. Setting exitOnComplete to false, to avoid premature shutdown. You do not have to fix this warning.");
					exitOnComplete = false;
				}
			}
		

			//make sure all the bools are set to true and haasConcluded = false
			RefreshResults();

			//make sure we have everything dragged into the ditor properly.
			TestInitialization();

			//set a bunch of variables elsewhere to tell the game to throw some flags/data up we'll be testing for
			SolicitTestData();

			//unpause game to get the tests up and running (the manager will do all it's own work of ensuring it exists, as it ought to have been dragged to stage; 
			//I don't need to worry about it; and it must be initialized by this time so I don't have to test for it.)
			OCGameStateManager.IsPlaying = true;



			//RUN THE TESTS!
			//-----------------------------------

			//run the tests in order

			for(int i = 0; i < numTests; i++)
			{
				if(configurations[i])
				{
					yield return StartCoroutine(tests[i]());
				}
			}

			if(!result)
			{
				Debug.Log(OCLogSymbol.FAIL + "One or more of the Unit Tests Failed.");
				GameManager.control.QuitWithError(1);
			}
			else
			{
				if(selectedTests != 0)
					Debug.Log(OCLogSymbol.PASS + "The " + selectedTests + " selected Unit Tests all Passed.");
				else
					Debug.Log(OCLogSymbol.PASS + "No Unit Tests were run; None succeeded and none failed.");

				if(exitOnComplete)
					GameManager.control.QuitWithSuccess();
			}

			this.hasConcluded = true;

		}
			
		#endregion
		#region      				Unit Test Functions

		protected IEnumerator TestEmbodiment()
		{

			//we're going to break the action out because we don't care about memory management right here and we 
			//want to clearly see what we're doing
			bool didConnect = false;
			
			System.Action<string> report = x => didConnect = (String.Compare(x, "true") == 0);
						
			//get the player object, if it exists
			UnityEngine.GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
			string masterId = playerObject.GetInstanceID().ToString();
			string masterName = playerObject.name;


			//This was the original way I did it until I enumerated manually.
			yield return StartCoroutine(GameManager.entity.load.AtRunTime(embodimentSpawn, embodimentPrefab, "", masterName, masterId, report));

			//note, this should handle everything appropriately even if the last yield return i tells the game to wait 20 minutes or what have you for the OAC to connect. 
			//because it doesn't rely on the end value to determine connection. 
			if(didConnect)
			{
				Debug.Log(OCLogSymbol.PASS + "Testing the Embodiment, Succeeded");
			}
			else
			{
				Debug.Log(OCLogSymbol.FAIL + "Testing the Embodiment, Failed");
				results[(int)TestTypes.EMBODIMENT] = false;
				result = false;
			}
			yield break;
			
		}
		protected IEnumerator TestBattery()
		{

			//FIXME [RACE]: This exists because a race condition seems to happen in which the OAC has an internal error SOMETIMES if we create the block too soon. 
				// One seconds seems enough to fix things; 0.1 second was not. 
			yield return new UnityEngine.WaitForSeconds(1.0f);

			//set the block
			GameManager.world.voxels.AddSelectedVoxel(batteryPosition, batteryDirection, batteryBlock);
			

			//if the first test failed, so did this one.
			if(results[(int)TestTypes.EMBODIMENT] == false)
			{
				Debug.Log(OCLogSymbol.DETAILEDINFO + "Battery Test Not Run.");
				results[(int)TestTypes.BATTERY] = false;
				yield break;
			}


			//determine when to time out
			DateTime end = System.DateTime.Now.AddMinutes((double)0.25);
			
			bool terrainSent = false;
			long checkTerrain = 0;
			long checkMapInfo = 0;

			//note on CompareTo (why don't they just have this in its summary?)
			//  instance.CompareTo(value)
			//	if return is < 0, instance is EARLIER than value
			//  if return is > 0, instance is LATER than value
			//  if return is = 0, they are the same (when does that ever happen, lol)

			//we need to poll what OCConnectorSingleton knows about sent messages 
			//since we can't really ask it about recieved ones
			while(System.DateTime.Now.CompareTo(end) < 0)
			{
				//ask it if it's finished percieving terrain
				checkTerrain = OCConnectorSingleton.Instance.DispatchTimes
						[(int)OCConnectorSingleton.DispatchTypes.finishTerrain];
				
				//ask it if it's finished sending mapinfo
				checkMapInfo = OCConnectorSingleton.Instance.DispatchTimes
						[(int)OCConnectorSingleton.DispatchTypes.mapInfo];
				
				//give us some extra time if terrain comes in.
				if((!terrainSent) && checkTerrain > 0)
				{
					terrainSent = true;
					end = System.DateTime.Now.AddMinutes((double)0.25);
				}
				
				//both finished?
				if(checkTerrain > 0 && checkMapInfo > 0)
				{
					//if this is true, then percieve terrain finished BEFORE the map Info was sent,
					//and odds are the battery was successfully reported. If it happened in the
					//inverse, an error happend (mapInfo should never be sent before terrain info!)
					if(checkTerrain < checkMapInfo)
					{
						Debug.Log(OCLogSymbol.PASS + "Testing the Battery, Succeeded");
					} 
					else
					{
						Debug.Log(OCLogSymbol.FAIL + "Testing the Battery, Failed");
						System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "MapInfo (such as the battery) was sent too early.");
						results[(int)TestTypes.BATTERY] = false;
						result = false;	
					}
					yield break;
				}
				
				//yield this coroutine every frame, and keep checking
				yield return 0;
			}
			
			//if we made it out of this loop, it's cause our wait period came to an end. Which
			//means the test failed. 
			Debug.Log(OCLogSymbol.FAIL + "Testing the Battery, Failed");
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "Timed out waiting for MapInfo to send.");
			results[(int)TestTypes.BATTERY] = false;
			result = false;	

		}

		protected IEnumerator TestPlan()
		{
			//if the first test failed, so did this one.
			if(results[(int)TestTypes.BATTERY] == false)
			{
				Debug.Log(OCLogSymbol.DETAILEDINFO + "Plan Test Not Run.");
				results[(int)TestTypes.PLAN] = false;
				yield break;
			}


			//We need to test these things:
			//1) that the plan was made <---- most important
			//2) that the plan was carried out at all (the robot performed some action)
			//3) that the battery was eaten 
			//4) that the plan was finished <---- most important

			//determine when to time out
			DateTime end = System.DateTime.Now.AddMinutes((double)0.50);

			// -------------------
			// STEP ONE!
			// CHECK IF THE PLAN IS MADE
			// -------------------
			long checkPlanMade = 0;
			
			//we need to poll what OCConnectorSingleton knows about recieved messages 
			while(System.DateTime.Now.CompareTo(end) < 0 && checkPlanMade == 0)
			{
				//ask it if it makes the plan
				checkPlanMade = OCConnectorSingleton.Instance.ReceptTimes
					[(int)OCConnectorSingleton.ReceptTypes.robotHasPlan];
				yield return 0;
			}

			//see if we timed out
			if(checkPlanMade == 0)
			{
				Debug.Log(OCLogSymbol.FAIL + "Testing the Plan, Failed");
				System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "Timed out while waiting for the plan to be sent from the OAC.");
				results[(int)TestTypes.PLAN] = false;
				result = false;	

				yield break;
			}

			Debug.Log(OCLogSymbol.CLEARED + "Testing the Plan Part 1: The Plan is detected.");

			// -------------------
			// STEP TWO!
			// IS ANYTHING HAPPENING!?
			// -------------------
			//we won't bother implementing this just yet

			//keep going!
			/*end = end.AddMinutes((double)0.125);

			*/

			// -------------------
			// STEP THREE!
			// WAS THE BATTERY EATEN?
			// -------------------

			//We are going to have to get EVERY DAMN OCDestroyBlockEffect there is @.@!
			//No! Wait! When a block is changed, we should end up sending MapInfo right?
			//Oh no nevermind, that'll also happen when we're building blocks...
			OCDestroyBlockEffect[] destructors = UnityEngine.Object.FindObjectsOfType(typeof(OCDestroyBlockEffect)) as OCDestroyBlockEffect[];

			//we want to see if any blocks get destroyed in the meanwhile; so we'll do it by counting up how many destroyed they already have
			int startDestroyedBlocks = 0;
			int endDestroyedBlocks = 0;
			foreach(OCDestroyBlockEffect d in destructors)
			{
				startDestroyedBlocks += d.BlocksDestroyed;
			}

			//otherwise keep going!
			end = end.AddMinutes((double)planMinutesToSucceed);
			
			//we need to poll what OCConnectorSingleton knows about recieved messages 
			while(System.DateTime.Now.CompareTo(end) < 0)
			{
				//first, enumerate all the destroyed blocks
				foreach(OCDestroyBlockEffect d in destructors)
				{
					endDestroyedBlocks += d.BlocksDestroyed;
				}

				if(startDestroyedBlocks < endDestroyedBlocks)
					break;

				//iterate once per frame.
				yield return 0;
			}

			if(startDestroyedBlocks >= endDestroyedBlocks)
			{
				Debug.Log(OCLogSymbol.FAIL + "Testing the Plan, Failed");
				System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "Timed out while waiting for blocks to be destroyed.");
				results[(int)TestTypes.PLAN] = false;
				result = false;	
				
				yield break;
			}

			Debug.Log(OCLogSymbol.CLEARED + "Testing the Plan Part 3 (2 is not yet implemented): The Battery was eaten!");

			// -------------------
			// STEP FOUR!
			// IS THE PLAN MARKED FINISHED?
			// -------------------


			long checkPlanSucceeded = 0;
			long checkPlanFailed = 0;

			//otherwise keep going!
			end = end.AddMinutes((double)0.125);
			
			//we need to see if the robot is doing anything at all
			while(System.DateTime.Now.CompareTo(end) < 0)
			{
				//ask it if it makes the plan
				checkPlanSucceeded = OCConnectorSingleton.Instance.DispatchTimes
					[(int)OCConnectorSingleton.DispatchTypes.actionPlanSucceeded];
				checkPlanFailed = OCConnectorSingleton.Instance.DispatchTimes
					[(int)OCConnectorSingleton.DispatchTypes.actionPlanFailed];

				//handle JUST in case something strange happens and Failed is marked right before or right after
				//succeeded on the same update cycle (Could totally happen if there was a second plan to eat the same exact battery that failed
				//rapidly in between us originally eating the battery and succeeding and then starting up the second plan. 
				if(checkPlanSucceeded > 0 && checkPlanSucceeded > checkPlanFailed)
					break;

				//check if the plan failed.
				if(checkPlanFailed > 0)
				{
					Debug.Log(OCLogSymbol.FAIL + "Testing the Plan, Failed");
					System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "The plan reported failure to complete.");
					results[(int)TestTypes.PLAN] = false;
					result = false;	
					
					yield break;
				}

				yield return 0;
			}
	
			//make sure we didn't time out
			if(checkPlanSucceeded == 0)
			{
				Debug.Log(OCLogSymbol.FAIL + "Testing the Plan, Failed");
				System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "Timed out while waiting for the plan to succeed after "+ planMinutesToSucceed+" minutes.");
				results[(int)TestTypes.PLAN] = false;
				result = false;	
				
				yield break;
			}
					
			//we got through the test!
			Debug.Log(OCLogSymbol.CLEARED + "Testing the Plan Part 3: The Plan is marked completed!");
			Debug.Log(OCLogSymbol.PASS + "Testing the Plan, Succeeded");

		}

		protected IEnumerator TestSecondPlan()
		{
			//if the first test failed, so did this one.
			if(results[(int)TestTypes.PLAN] == false)
			{
				Debug.Log(OCLogSymbol.DETAILEDINFO + "Second Plan Test Not Run.");
				results[(int)TestTypes.SECONDPLAN] = false;
				yield break;
			}


			Debug.LogWarning(OCLogSymbol.PASS + "Second Plan Test not Implemented; It is understood the Planner cannot cope with block removal at this time.");
			yield return 0;
		}

		#endregion
		#region 						Unity Monobehavior Functions


		public void Start()
		{

			//RunTests must trickle up a chain of yield StartCoroutine(Function(x))'s up to something
			//with yieldInstructions like yield return new UnityEngine.WaitForSeconds(3f); or a simple 
			//yield return 0 in order to supply this Coroutine chain with the instructions it
			//requires for knowing when to fire up its next iteration.
			//We will use for loops and time outs farther up the chain to 'sense' for things...
			//and we can also set variables inside nexted coroutines that we can checkout.
			StartCoroutine(RunTests());
		}
		public void Update()
		{
			//this is where Embodiment/SetBattery/etc should be called from.
			if(!enabled)
			{
				return;
			}


		}

		public void OnDestroy()
		{

		}

		#endregion

	}
}
