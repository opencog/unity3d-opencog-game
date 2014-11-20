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
using UnityEngine;
using System;


using OpenCog.Map;

#pragma warning disable 0414
#endregion

namespace OpenCog.Worlds
{
	// This class is INCOMPLETE in its functionality and refactoring; it is concieved of as a top-level manager for Entities
	//
	// A note on what is going on with the WorldManager folder:
	// ----------------------------------
	// It is believed that WorldManager will eventually contain a large number of methods which can be grouped into subclasses. For example, there may be 20 ways
	// to send data for: loading a map, saving a map, doing raycasting, editing a map, etc. In order to neatly sort all of these methods,
	// keep them accessible to the outside, and yet prevent anyone else from instantiating the classes, we are doing the following:
	//
	//		1. Placing the method-group helper-sub-classes (ie: Load) into their own files (breaking them up for easy browsing)
	//		2. Keeping them nested in WorldManager as protected classes (so the outside world can't instantiate them)
	//		3. Using the 'partial class' keyword combo to satisfy points 1 and 2 (each of the files mentioned in part 1 will then be wrapped in `public partial class etc.`)
	//		4. Exposing the protected class's method prototypes to the outside world using a public interface (which can't be instantiated on its own)
	//
	// -----------------------------------
	
	/// <summary>
	/// <para>To make use of the methods in this class, reference: </para><para>.* </para><para></para>
	/// This is a top-level manager for in-game avatars and other characters. 
	/// It is to be owned and managed exclusively by GameManager. 
	/// </summary>
	///
	
	public partial class WorldManager:OCSingletonMonoBehaviour<WorldManager>
	{
		//-------------------------------------------------------------------------
		#region 						  Child Functionality
		//-------------------------------------------------------------------------
		//The methods for manipulating blocks to the screen
		private _VoxelMethods _voxelMethods;
		public VoxelMethods voxels {get{return (_voxelMethods ?? _VoxelMethods.New()) as VoxelMethods;}}

		
		//A little clarity on the ?? operator:
		//The form return(first_expression ?? second_expression) implies that it will attempt to return 
		//the first expression unless it finds a null; else it will return the second expression.
		//Or that it will return its loadMethods or try to create a new one, addressing a situation
		//where .load is called before Awake();
		
		
		
		//---------------------------------------------------------------------------
		#endregion
		#region 				Protected Member Variables
		//---------------------------------------------------------------------------

		//reveal specifically, easily, and only to child classes (ie, we know they exist already)
		private OCMap _map;
		protected static OCMap map{get{return (_instance._map ?? _instance.GetMap()) as OCMap;}}




		//---------------------------------------------------------------------------
		#endregion
		#region 				Singleton Stuff
		//---------------------------------------------------------------------------
		
		//Do not let anyone instantiate this (not that monobehaviors should be instantiated in the first place)
		protected WorldManager(){}
		
		/// <summary>This initialization function creates the submanagers, and will be called automatically by the SingletonMonoBehavior's Awake()</summary>
		protected override void Initialize()
		{
			//shouldn't be destroyed with scenes. 
			DontDestroyOnLoad(this);

			GetMap();
			
			//Create a new set of  methods
			_voxelMethods = _VoxelMethods.New();

			//parenting
			_voxelMethods.gameObject.transform.parent = this.gameObject.transform;
			
		}
		protected OCMap GetMap()
		{
			//get the map instance!
			_map = OCMap.Instance;
			return _map;
		}

		//protected static WorldManager AsParentOf{get{return _instance;}}
		
		
		
		/// <summary>Used to instantiate this class. It should only be called once. It will supply a single instance, and then throw an error
		/// if it is called a second time.</summary>
		public static WorldManager New()
		{
			//This is not a true Singleton; we only want one ever built and then we want gameManager to throw errors if we failed
			if(_instance)
			{
				throw new OCException( "Two WorldManagers exist and this is forbidden. Permit GameManager to handle WorldManager internally.");
				
			}
			
			//the Singleton pattern handles everything about instantiation for us, including searching
			//the game for a pre-existing object.
			WorldManager wm = GetInstance<WorldManager>();
			
			//this is the line that will prevent the gameObject from being destroyed between scenes.
			DontDestroyOnLoad(wm.gameObject);
			return wm;
		}
		#endregion
		
		//---------------------------------------------------------------------------
		
	}
}