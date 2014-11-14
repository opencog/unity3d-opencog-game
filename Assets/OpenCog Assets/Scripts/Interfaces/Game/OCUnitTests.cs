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

	//the enumerated type which represents all the tests
	protected enum TestTypes: int
	{
		EMBODIMENT,
		BATTERY, 
		PLAN,
		SECONDPLAN
	}

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

		//iterate through all the configurations and read them in!
		for(uint i = 0; i < numTests; i++)
		{
			configurations[i] = config.getBool(testConfigNames[i]);
		}
	}

	//set everything to true!
	protected void FakeConfiguration()
	{
		//get the configuration as this handy-dandy variable
		OCConfig config = OCConfig.Instance;
			
		//iterate through all the configurations and read them in!
		for(uint i = 0; i < numTests; i++)
		{
			configurations[i] = true;
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
	//<none>

	//FOR THE SECOND PLANNING TEST
	//<none>

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
		Debug.Log("Running Tests from Start");

		//wait until the game is running such that we can be pretty sure everything's 'start' already run (Since this coroutine is 
		//initialized in a Start()) By this point, things like OcWorldGenerator, etc, should already all be initialized, and so
		//a legit OCConfig file should be loaded.
		yield return new UnityEngine.WaitForFixedUpdate();

		yield return new UnityEngine.WaitForSeconds(20.0f);

		Debug.Log("Yielding on FixedUpdate()");

		//INITIALIZE!
		//-----------------------------------
		//get our own configurations

		if(this.editorSaysToRun)
		{
			FakeConfiguration();
		} else
		{
			GetConfiguration();
		}

		//make sure all the bools are set to true and haasConcluded = false
		RefreshResults();

		//make sure we have everything dragged into the ditor properly.
		TestInitialization();

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
			Debug.Log("One or more of the Unit Tests Failed");
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

		yield return StartCoroutine(GameManager.entity.load.AtRunTime(embodimentSpawn, embodimentPrefab, "", masterName, masterId, report));

		if(didConnect == false)
		{
			Debug.Log("Testing the Embodiment, Failed");
			results[(int)TestTypes.EMBODIMENT] = false;
			result = false;
		} else
		{
			Debug.Log("Testing the Embodiment, Succeeded");
		}
		yield break;
		
	}
	protected IEnumerator TestBattery()
	{
		//set the block
		GameManager.world.voxels.AddSelectedVoxel(batteryPosition, batteryDirection, batteryBlock);

		yield return 0;
	}

	protected IEnumerator TestPlan()
	{
		yield return 0;
	}

	protected IEnumerator TestSecondPlan()
	{
		yield return 0;
	}

		#endregion
		#region 						Unity Monobehavior Functions


	public void Start()
	{
		
		Debug.Log("StartingCoroutine(RunTests())");

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
