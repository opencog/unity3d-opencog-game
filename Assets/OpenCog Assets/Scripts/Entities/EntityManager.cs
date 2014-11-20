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


#region Usings, Namespaces, and Pragmas

using System.Collections;
using OpenCog.Actions;
using UnityEngine;
using System;

#pragma warning disable 0414
#endregion

namespace OpenCog.Entities
{
	// This class is INCOMPLETE in its functionality and refactoring; it is concieved of as a top-level manager for Entities
	//
	// A note on what is going on with the EntityManager folder:
	// ----------------------------------
	// It is believed that EntityManager will eventually contain a large number of methods which can be grouped into subclasses. For example, there may be 20 ways
	// to send data for: loading a character, moving a character, artificially constructing a path, giving orders, etc. In order to neatly sort all of these methods,
	// keep them accessible to the outside, and yet prevent anyone else from instantiating the classes, we are doing the following:
	//
	//		1. Placing the method-group helper-sub-classes (ie: Load) into their own files (breaking them up for easy browsing)
	//		2. Keeping them nested in EntityManager as protected classes (so the outside world can't instantiate them)
	//		3. Using the 'partial class' keyword combo to satisfy points 1 and 2 (each of the files mentioned in part 1 will then be wrapped in `public partial class etc.`)
	//		4. Exposing the protected class's method prototypes to the outside world using a public interface (which can't be instantiated on its own)
	//
	// -----------------------------------

	/// <summary>
	/// <para>To make use of the methods in this class, reference: </para><para>GameManager.character.* </para><para></para>
	/// This is a top-level manager for in-game avatars and other characters. 
	/// It is to be owned and managed exclusively by GameManager. 
	/// </summary>
	///

	public partial class EntityManager:OCSingletonMonoBehaviour<EntityManager>
	{
		//-------------------------------------------------------------------------
		#region 						  Child Functionality
		//-------------------------------------------------------------------------
		//The methods for loading characters to the screen
		private _LoadMethods _loadMethods;
		public LoadMethods load {get{return (_loadMethods ?? _LoadMethods.New()) as LoadMethods;}}

	


		//A little clarity on the ?? operator:
		//The form return(first_expression ?? second_expression) implies that it will attempt to return 
		//the first expression unless it finds a null; else it will return the second expression.
		//Or that it will return its loadMethods or try to create a new one, addressing a situation
		//where .load is called before Awake();



		//---------------------------------------------------------------------------
		#endregion
		#region 				Singleton Stuff
		//---------------------------------------------------------------------------

		//Do not let anyone instantiate this (not that monobehaviors should be instantiated in the first place)
		protected EntityManager(){}

		/// <summary>This initialization function creates the submanagers, and will be called automatically by the SingletonMonoBehavior's Awake()</summary>
		protected override void Initialize()
		{
			//shouldn't be destroyed with scenes. 
			DontDestroyOnLoad(this);
			
			//Create a new set of load methods
			_loadMethods = _LoadMethods.New();

			//for easy catagorization!
			_loadMethods.gameObject.transform.parent = this.gameObject.transform;
			
		}

		//protected static EntityManager AsParentOf{get{return _instance;}}
		


		/// <summary>Used to instantiate this class. It should only be called once. It will supply a single instance, and then throw an error
		/// if it is called a second time.</summary>
		public static EntityManager New()
		{
			//This is not a true Singleton; we only want one ever built and then we want gameManager to throw errors if we failed
			if(_instance)
			{
				throw new OCException( "Two CharacterManagers exist and this is forbidden. Permit GameManager to handle EntityManager internally.");

			}

			//the Singleton pattern handles everything about instantiation for us, including searching
			//the game for a pre-existing object.
			EntityManager cm = GetInstance<EntityManager>();
			
			//this is the line that will prevent the gameObject from being destroyed between scenes.
			DontDestroyOnLoad(cm.gameObject);
			return cm;
		}
		#endregion
		
		//---------------------------------------------------------------------------

	}
}