using UnityEngine;
using System.Collections;
using UnityEditor;
using Substrate;
using Vector3 = UnityEngine.Vector3;

using System.IO;

namespace Cubiquity
{
	public class MainMenuEntries : MonoBehaviour
	{
		static string RandomString()
		{
			int randomVal = Random.Range(0, 1000000000);
			return randomVal.ToString();
		}
		
		[MenuItem ("GameObject/Create Other/Terrain Volume")]
		static void CreateTerrainVolume()
		{
			int width = 32;
			int height = 10;
			int depth = 32;
			
			TerrainVolumeData data = TerrainVolumeData.CreateEmptyVolumeData(new Region(0, 0, 0, width-1, height-1, depth-1));
			
			// Create some ground in the terrain so it shows up in the editor.
			// Soil as a base (mat 1) and then a couple of layers of grass (mat 2).
			TerrainVolumeGenerator.GenerateFloor(data, 6, (uint)1, 8, (uint)2);
			
			// Now create the terrain game object from the data.
			GameObject terrain = TerrainVolume.CreateGameObject(data);
			
			// Set up our textures in the appropriate material slots.
			terrain.GetComponent<TerrainVolume>().materials[0].diffuseMap = Resources.Load("Textures/Rock") as Texture2D;
			terrain.GetComponent<TerrainVolume>().materials[0].scale = new Vector3(16.0f, 16.0f, 16.0f);		
			terrain.GetComponent<TerrainVolume>().materials[1].diffuseMap = Resources.Load("Textures/Soil") as Texture2D;		
			terrain.GetComponent<TerrainVolume>().materials[2].diffuseMap = Resources.Load("Textures/Grass") as Texture2D;
			
			// It's possible the textures won't actually be found, as they are just examples and the
			// user might have decided not to include them when importing Cubiquity. This doesn't
			// matter and just means the uer will have to set up their own textures.
			if((terrain.GetComponent<TerrainVolume>().materials[0].diffuseMap == null) ||
				(terrain.GetComponent<TerrainVolume>().materials[1].diffuseMap == null) ||
				(terrain.GetComponent<TerrainVolume>().materials[2].diffuseMap == null))
			{
				Debug.LogWarning("Failed to set up the default Cubiquity terrain textures. This is probably " +
					"because you chose not to import the examples when importing Cubiquity? It doesn't matter, " +
					"it just means you have to configure your own textures through the inspector.");
			}
		}
		
        //[MenuItem ("GameObject/Create Other/Colored Cubes Volume")]
        //static void CreateColoredCubesVolume()
        //{
        //    AnvilWorld world = ColoredCubesVolume.World;
        //    ColoredCubesVolume.SetRowColomuns(world);
        //    int width = 16 * ColoredCubesVolume.NumberOfColumns;
        //    int height = 10;
        //    int depth = 16 * ColoredCubesVolume.NumberOfRows;
			
        //    ColoredCubesVolumeData data = ColoredCubesVolumeData.CreateEmptyVolumeData(new Region(0, 0, 0, width-1, height-1, depth-1));
        //    ColoredCubesVolume.CreateGameObject(data);
        //    ColoredCubesVolume.MCSubstrate(data);
        //}
	}
}
