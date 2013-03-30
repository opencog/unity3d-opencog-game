
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

using System;
using System.Collections;
using OpenCog.Attributes;
using ProtoBuf;
using UnityEngine;

namespace OpenCog
{

namespace Extensions
{            

/// <summary>
/// The OpenCog OCSingleton.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposeProperties]
[Serializable]
#endregion

public class OCSingleton<T> : OCMonoBehaviour where T : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

    #region Protected Member Data

	//---------------------------------------------------------------------------

	/// <summary>
	/// The global instance of this singleton.
	/// </summary>
	protected static T m_Instance;

	//---------------------------------------------------------------------------

    #endregion

	//---------------------------------------------------------------------------

    #region Accessors and Mutators

	//---------------------------------------------------------------------------

	//@TODO: Define static properties and methods for public variables and
	// methods that are used often from outside the class.
            
	//---------------------------------------------------------------------------

    #endregion

	//---------------------------------------------------------------------------    

    #region Constructors

	//---------------------------------------------------------------------------
        

	//---------------------------------------------------------------------------

    #endregion

	//---------------------------------------------------------------------------

    #region Public Member Functions

	//---------------------------------------------------------------------------

	/// <summary>
	/// Gets the instance of this singleton.
	/// </summary>
	/// <value>
	/// The instance of this singleton.
	/// </value>
	public static T Instance
	{
		get
		{
			if(m_Instance == null)
			{
				m_Instance = (T)FindObjectOfType(typeof(T));
		
				if(m_Instance == null)
				{
					Debug.LogError("An instance of " + typeof(T) +
		              " is needed in the scene, but there is none.");
				}
			}

			return m_Instance;
		}
	}

	//---------------------------------------------------------------------------

    #endregion

	//---------------------------------------------------------------------------

    #region Private Member Functions

	//---------------------------------------------------------------------------
            
    
            
	//---------------------------------------------------------------------------

    #endregion

	//---------------------------------------------------------------------------

    #region Member Classes

	//---------------------------------------------------------------------------        

	//---------------------------------------------------------------------------

    #endregion

	//---------------------------------------------------------------------------

}// class OCSingleton

}// namespace Extensions            

}// namespace OpenCog




