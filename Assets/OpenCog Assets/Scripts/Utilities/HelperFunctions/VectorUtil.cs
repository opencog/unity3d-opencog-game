
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
using UnityEngine;

#region Usings, Namespaces, and Pragmas

using System.Collections;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using Math = System.Math;
using Mathf = UnityEngine.Mathf;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Utility
{

/// <summary>
/// The OpenCog VectorUtil.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class VectorUtil : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	

			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		

			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public static bool AreVectorsEqual(Vector3 left, Vector3 right)
	{
			Vector3 diffVector = right - left;
			return Math.Abs(diffVector.x) <= 0.5f && Math.Abs(diffVector.y) <= 0.5f && Math.Abs(diffVector.z) <= 0.5f;
	}

	public static double NormalizeAngle(double angle)
	{
		while(angle > Math.PI)
		{
			angle -= (Math.PI * 2);
		}
		while(angle < -Math.PI)
		{
			angle += (Math.PI * 2);
		}
		return angle;
	}
		
	/// <summary>
	/// Converts any absolute angle value from OAC-based coordinates
	/// to Unity-based ones.
	/// </summary>
	/// <param name="angle">
	/// angle to be converted
	/// </param>
	/// <returns>
	/// the converted absolute angle value
	/// </returns>
	public static double OAC2UnityConv(double angle)
	{
		double result = VectorUtil.NormalizeAngle(angle) + (Math.PI / 2);
		return VectorUtil.NormalizeAngle(result);
	}
		
	/// <summary>
	/// Converts any absolute angle value from Unity-based coordinates
	/// to OAC-based ones.
	/// </summary>
	/// <param name="angle">
	/// angle to be converted
	/// </param>
	/// <returns>
	/// the converted absolute angle value
	/// </returns>
	public static double Unity2OACConv(double angle)
	{
		double result = VectorUtil.NormalizeAngle(angle) - (Math.PI / 2);
		return VectorUtil.NormalizeAngle(result);
	}

	/// <summary>
	/// Convert a position vector from Unity (x, y, z) coordinate to OpenCog coordinate (x, z, y).
	/// </summary>
	public static UnityEngine.Vector3 ConvertToOpenCogCoord(UnityEngine.Vector3 unityCoord)
	{
		return new UnityEngine.Vector3(unityCoord.x, unityCoord.z, unityCoord.y);
	}

	/// <summary>
	/// OpenCog takes the center point of one object as its physical coordinates.
	/// However, sometimes we get the bottom left point vector from unity. In that situation,
	/// we need to calculate its central point manually by its size.
	/// </summary>
	public static UnityEngine.Vector3 ConvertToCentralCoord(UnityEngine.Vector3 unityCoord, UnityEngine.Vector3 size)
	{
		UnityEngine.Vector3 ocVec = ConvertToOpenCogCoord(unityCoord);
		return ocVec + 0.5f * size;
	}

	/// <summary>
	/// Converts a Vector3 to a Vector3i.
	/// </summary>
	/// <returns>
	/// The vector3i.
	/// </returns>
	/// <param name='inputVector'>
	/// Input Vector3.
	/// </param>
	public static Vector3i Vector3ToVector3i(UnityEngine.Vector3 inputVector)
	{
		int iX, iY, iZ;

		iX = (int)Mathf.Round(inputVector.x);
		iY = (int)Mathf.Round(inputVector.y);
		iZ = (int)Mathf.Round(inputVector.z);
		return new Vector3i(iX, iY, iZ);
	}

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

}// class VectorUtil

}// namespace OpenCog




