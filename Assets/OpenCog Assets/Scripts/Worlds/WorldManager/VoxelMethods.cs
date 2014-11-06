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
using OpenCog.Actions;
using UnityEngine;
using System;

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
			//put classes to expose here

		}

		///<summary><para>Access this class's methods using GameManager.World.Voxel.*</para>
		/// <para>A subcatagory of WorldManager methods, which should be used for Voxeling characters. Nested in WorldManager, its functions are exposed to the outside through the VoxelMethods interface.</para></summary>
		protected class _VoxelMethods:OCSingletonMonoBehaviour<_VoxelMethods>, VoxelMethods
		{
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
					throw new OCException( "Two WorldManager.VoxelMethods exist and this is forbidden.");

				}
				
				//the Singleton pattern handles everything about instantiation for us, including searching
				//the game for a pre-existing object.
				return GetInstance<_VoxelMethods>();


			}

			/// <summary>
			/// This function is responsible for adding a voxel to the map. Because our voxel engine is about to change, this function will take two approaches
			/// to reducing later refactoring. 1) this function relies on the idea that there is a selected block type (so it does not need to be *passed* the
			/// block type) and 2) this public function will wrap a private function that handles any pixelland-dependent code
			/// </summary>
			public bool AddSelectedVoxel(Vector3 location)
			{
				return AddSelectedVoxelPixelland(location);
			}
			protected bool AddSelectedVoxelPixelland(Vector3 location)
			{
				OpenCog.Map.OCBlockData block = OCBlockData.CreateInstance<OCBlockData>().Init(_selectedBlock, OpenCog.Utility.VectorUtil.Vector3ToVector3i(point.Value));
				block.SetDirection(GetDirection(-transform.forward));
				_map.SetBlockAndRecompute(block, point.Value);
			}



		}
	}
}
