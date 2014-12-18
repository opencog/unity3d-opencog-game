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
using OpenCog.BlockSet.BaseBlockSet; //OcBlock
using OpenCog.Utility; //vectorUtils

#pragma warning disable 0414
#endregion

namespace OpenCog.Worlds
{
	
	public partial class WorldManager:OCSingletonMonoBehaviour<WorldManager>
	{
		///<summary><para>Access this interface's methods using GameManager.world.voxel.*</para>
		///<para>This interface exposes a subcatagory of WorldManager methods, which should be used for exiting the voxel map.</para></summary>
		public interface VoxelMethods
		{

			OCBlock selectedBlock {get;set;}
			void AddSelectedVoxel(Vector3 location, Vector3 direction, OCBlock type);
			void DeleteSelectedVoxel(Vector3 location);
			//put classes to expose here

		}

		///<summary><para>Access this class's methods using GameManager.World.Voxel.*</para>
		/// <para>A subcatagory of WorldManager methods, which should be used for Voxeling characters. Nested in WorldManager, its functions are exposed to the outside through the VoxelMethods interface.</para></summary>
		protected class _VoxelMethods:OCSingletonMonoBehaviour<_VoxelMethods>, VoxelMethods
		{

			protected OCBlock _selectedBlock;
			public OCBlock selectedBlock{get{return _selectedBlock;}set{_selectedBlock = value;}}

			#region 										Singleton Stuff
			protected _VoxelMethods(){}

			/// <summary>This initialization function will be called automatically by the SingletonMonoBehavior's Awake().</summary>
			protected override void Initialize()
			{
				//shouldn't be destroyed with scenes. 
				DontDestroyOnLoad(this);

			}

			/// <summary>Used to instantiate this class. It should only be called once. It will supply a single instance, and then throw an error
			/// if it is called a second time.</summary>
			public static _VoxelMethods New()
			{
				//THERE CAN ONLY BE ONE (and it must be created by the WorldManager)
				if(_instance)
				{
					throw new OCException("Two WorldManager.VoxelMethods exist and this is forbidden.");

				}
				
				//the Singleton pattern handles everything about instantiation for us, including searching
				//the game for a pre-existing object.
				return GetInstance<_VoxelMethods>();


			}

			#endregion
			#region 									Functionality

			/// <summary>
			/// This function is responsible for adding a voxel to the map. Because our voxel engine is about to change, this function will take two approaches
			/// to reducing later refactoring, we try to pass only OCBlock (We would have had to written a lot of code to get around that) and
			/// we wrap a protected AddSelectedVoxelPixelland to mentally remind ourselves that we want to replace this in the future,
			/// and we should not be doing a lot of 'work' here.
			/// </summary>
			public void AddSelectedVoxel(Vector3 location, Vector3 direction, OCBlock type)
			{
				AddSelectedVoxelPixelland(location, direction, type);
			}

			protected void AddSelectedVoxelPixelland(Vector3 location, Vector3 direction, OCBlock type)
			{
				//create a block to fill with dataz
				OCBlockData block = OCBlockData.CreateInstance<OCBlockData>();

				//get the location in vector3i form
				Vector3i location3i = VectorUtil.Vector3ToVector3i(location);

				//initialize the block
				block.Init(type, location3i);

				//set its direction (which should be calculated using GetDirection on -transform.forward)
				block.SetDirection(direction);

				//and set the block
				map.SetBlockAndRecompute(block, location3i);
			}

			public void DeleteSelectedVoxel(Vector3 location)
			{
				AddSelectedVoxelPixelland(location, new Vector3(), null);
			}

			#endregion

		}
	}
}
