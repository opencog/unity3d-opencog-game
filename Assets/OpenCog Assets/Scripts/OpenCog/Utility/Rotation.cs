
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
using ProtoBuf;
using Serializable = System.SerializableAttribute;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Utility
{

/// <summary>
/// The OpenCog Rotation.
/// </summary>
#region Class Attributes


[OCExposePropertyFields]
[Serializable]
[ProtoContract]
#endregion
public class Rotation 
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	private float pitch;

	private float roll;

	private float yaw;
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		
	[ProtoMember(1, IsRequired=true)]
	public float Pitch
	{
		get { return this.pitch; }
		set { this.pitch = value; }
	}

	[ProtoMember(2, IsRequired = true)]
	public float Roll
	{
		get { return this.roll; }
		set { this.roll = value; }
	}

	[ProtoMember(3, IsRequired = true)]
	public float Yaw
	{
		get { return this.yaw; }
		set { this.yaw = value; }
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	public bool Equals(Rotation other)
	{
		// Decrease the impact the errors from float

		return System.Math.Abs(pitch - other.Pitch) < 0.01f &&
                   System.Math.Abs(roll - other.Roll) < 0.01f &&
                   System.Math.Abs(yaw - other.Yaw) < 0.01f;
	}

	public override int GetHashCode()
	{
		return (int)(pitch + roll + yaw);
	}

	public override string ToString()
	{
		return "(pitch:" + pitch + ", roll:" + roll + ", yaw:" + yaw + ")";
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

	public Rotation()
	{
		pitch = 0.0f;
		roll = 0.0f;
		yaw = 0.0f;
	}

	// Constructor
	public Rotation(float pitch, float roll, float yaw)
	{
		this.pitch = pitch;
		this.roll = roll;
		this.yaw = yaw;
	}

	public Rotation(UnityEngine.Quaternion orientation)
	{
		UnityEngine.Vector3 eulerAngle = orientation.eulerAngles;
		this.pitch = (float)System.Math.PI * (eulerAngle.z / 180);
		this.roll = (float)System.Math.PI * (eulerAngle.x / 180);
            
		// We make some rotation here for unity3d's default direction is facing towards 
		// Z-axis(equals to the Y-axis in OpenCog), while OpenCog's default 
		// direction is facing towards X-axis.
		this.yaw = (float)System.Math.PI * (0.5f - eulerAngle.y / 180);
		if(this.yaw > 2 * (float)System.Math.PI)
		{
			this.yaw -= 2 * (float)System.Math.PI;
		}
		else
		if(this.yaw < 0)
		{
			this.yaw += 2 * (float)System.Math.PI;
		}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class Rotation

}// namespace OpenCog




