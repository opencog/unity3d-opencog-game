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

using System;
using System.Collections;
using UnityEngine;
using ProtoBuf;


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
	public class OCUnitTests : OCMonoBehaviour
	{
		#region 				MonoBehavior Stuff
		protected bool enabled = false;

		#endregion

		#region 				Constants for Testing

		public Vector3 embodimentSpawn;
		public GameObject embodimentPrefab;

		public Vector3 batteryPosition;

		/// <summary>Often -transform.forward (faces the player)</summary>
		public Vector3 batteryDirection; 
		public OCBlock batteryBlock;

		#endregion

		#region 					Unit Test Functions
		public IEnumerator Embodiment()
		{
			if(!enabled) yield break;
			yield return StartCoroutine(GameManager.entity.load.AtRunTime(embodimentSpawn, embodimentPrefab));

			//TODO: How do we sense whether the load was successful? Set it in LoadAgent
		}

		public IEnumerator SetBattery()
		{
			if(!enabled) yield break;

			//set the block
			GameManager.world.voxels.AddSelectedVoxel(batteryPosition, batteryDirection, batteryBlock);

			//TODO: How do we sense the embodiment can see it? Ask Console!
		}
		#endregion

		public void Start()
		{
			enabled = true;
		}
		public void Update()
		{
			//this is where Embodiment/SetBattery/etc should be called from. 
		}

		public void OnDestroy()
		{
			enabled = false;
		}

	}
}
