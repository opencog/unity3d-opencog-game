
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

using OpenCog.Entities;
using OpenCog.Worlds;

#endregion

//The private field is assigned but its value is never used
#pragma warning disable 0414 




namespace OpenCog.Master
{
	// This class is INCOMPLETE in its functionality and refactoring; it is concieved of as a top-level manager for the entire game. It is concieved
	// of as owning many subordinate managers.

	//This class should eventually consume the GameStateManager.

	/// <summary>
	/// A top level class and singleton for triggering top-level tasks such as saving and loading the various components of the game. 
	/// </summary>
	public partial class GameManager:OCSingletonMonoBehaviour<GameManager>
	{
		//---------------------------------------------------------------------------
		#region 					  Sub-Managers
		//---------------------------------------------------------------------------

		protected EntityManager _entityManager; 
		public static EntityManager entity{get{return _instance._entityManager;}}
		protected WorldManager _worldManager;
		public static WorldManager world{get{return _instance._worldManager;}}
		protected _ControlMethods _controlMethods;
		public static ControlMethods control{get{return _instance._controlMethods;}}




		//---------------------------------------------------------------------------
		#endregion
		#region 						Singleton Stuff
		//---------------------------------------------------------------------------


		//Do not let anyone instantiate this (not that monobehaviors should be instantiated in the first place)
		//By the way, typically speaking, don't initialize in constructors for monobehaviors
		protected GameManager(){}
	
		/// <summary>This initialization function creates the submanagers, and will be called automatically by the SingletonMonoBehavior's Awake()</summary>
		protected override void Initialize()
		{
			if(this != Instance)
			{
				Destroy(this.gameObject);
				throw new OCException("Game was improperly prepared with more than one GameManager");
			}

			DontDestroyOnLoad(this);

			//Create the child managers!
			_entityManager = EntityManager.New ();
			_worldManager = WorldManager.New ();
			_controlMethods = _ControlMethods.New ();

			//parenting!
			_entityManager.gameObject.transform.parent = this.gameObject.transform;
			_worldManager.gameObject.transform.parent = this.gameObject.transform;
			_controlMethods.gameObject.transform.parent = this.gameObject.transform;

		}

		/// <summary>Singleton pattern Instance accessor!</summary>
		public static GameManager Instance
		{
			get
			{
				//the Singleton pattern handles everything about instantiation for us, including searching
				//the game for a pre-existing object.
				return GetInstance<GameManager>() ;

			}

		}

		//---------------------------------------------------------------------------
		#endregion
		#region 				Temporary
		//---------------------------------------------------------------------------

		//guarenteed to be called after 'awake'
		public void Start()
		{

		}


		#endregion

		//---------------------------------------------------------------------------

	}
}
