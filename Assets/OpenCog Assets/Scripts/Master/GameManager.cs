
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

#endregion

//The private field is assigned but its value is never used
#pragma warning disable 0414 




namespace OpenCog.Master
{
	public class GameManager:OCSingletonMonoBehaviour<GameManager>
	{
		//---------------------------------------------------------------------------
		#region 					  Private Member Data
		//---------------------------------------------------------------------------





		//---------------------------------------------------------------------------
		#endregion
		#region 				Singleton Accessors and Mutators
		//---------------------------------------------------------------------------


		//Do not let anyone instantiate this (not that monobehaviors should be instantiated in the first place)
		protected GameManager(){}

		//Singleton pattern Instance accessor!
		public static GameManager Instance
		{
			get
			{
				//the Singleton pattern handles everything about instantiation for us, including searching
				//the game for a pre-existing object.
				GameManager gm = OCSingletonMonoBehaviour<GameManager>.GetInstance() as GameManager;

				//this is the line that will prevent the gameObject from being destroyed between scenes.
				DontDestroyOnLoad(gm.gameObject);
				return gm;
			}

		}
		#endregion

		//---------------------------------------------------------------------------

	}
}
