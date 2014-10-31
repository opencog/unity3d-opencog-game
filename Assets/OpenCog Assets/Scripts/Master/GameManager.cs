
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

using OpenCog.Character;

#endregion

//The private field is assigned but its value is never used
#pragma warning disable 0414 




namespace OpenCog.Master
{
	public class GameManager:OCSingletonMonoBehaviour<GameManager>
	{
		//---------------------------------------------------------------------------
		#region 					  Protected Member Data
		//---------------------------------------------------------------------------

		protected CharacterManager _characterManager; 
		public static CharacterManager character{get{return Instance._characterManager;}}




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
			DontDestroyOnLoad(this);

			//Create the character manager!
			_characterManager = CharacterManager.New ();

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
