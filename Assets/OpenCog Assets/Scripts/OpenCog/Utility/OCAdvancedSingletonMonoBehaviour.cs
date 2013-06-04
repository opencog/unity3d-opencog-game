
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
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog
{

/// <summary>
/// The OpenCog OCAdvancedSingletonMonoBehaviour.
/// </summary>
#region Class Attributes
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCAdvancedSingletonMonoBehaviour<T> : UnityEngine.MonoBehaviour
	where T : UnityEngine.MonoBehaviour 
	{

		//---------------------------------------------------------------------------

	#region Private Member Data

		//---------------------------------------------------------------------------
	
		private static T _instance = null;
			
		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Accessors and Mutators

		//---------------------------------------------------------------------------
		
		public static T Instance {
			get {
				if (_instance == null)
				{
					UnityEngine.Debug.Log ("Instantiating advanced singleton of type: " + typeof(T) + "...");
					UnityEngine.GameObject go = new UnityEngine.GameObject();
					
					_instance = go.AddComponent<T>();
					go.name = "singleton";
				}
				
				return _instance;
			}
			
		}
			
		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Public Member Functions

		//---------------------------------------------------------------------------

	

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Private Member Functions

		//---------------------------------------------------------------------------
	
	
			
		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Other Members

		//---------------------------------------------------------------------------		

	

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	}// class OCAdvancedSingletonMonoBehaviour

}// namespace OpenCog




