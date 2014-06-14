using UnityEngine;
using System.Collections;
using UnityEditor;

using System.IO;

namespace Cubiquity
{
	public class MainMenuEntries : MonoBehaviour
	{		
		[MenuItem ("GameObject/Create Other/Terrain Volume")]
		static void CreateTerrainVolume()
		{
			int width = 128;
			int height = 32;
			int depth = 128;
			
			TerrainVolumeData data = TerrainVolumeData.CreateEmptyVolumeData(new Region(0, 0, 0, width-1, height-1, depth-1), VolumeData.Paths.StreamingAssets, VolumeData.GeneratePathToVoxelDatabase());
			
			// Create some ground in the terrain so it shows up in the editor.
			// Soil as a base (mat 1) and then a couple of layers of grass (mat 2).
			TerrainVolumeGenerator.GenerateFloor(data, 6, (uint)1, 8, (uint)2);
			
			// Now create the terrain game object from the data.
			GameObject terrain = TerrainVolume.CreateGameObject(data);
			
			// And select it, so the user can get straight on with editing.
			Selection.activeGameObject = terrain;
			
			// Set up our material	
			Material material = new Material(Shader.Find("TriplanarTexturing"));
			terrain.GetComponent<TerrainVolumeRenderer>().material = material;
			
			// Set up the default textures
			Texture2D rockTexture = Resources.Load("Textures/Rock") as Texture2D;
			Texture2D soilTexture = Resources.Load("Textures/Soil") as Texture2D;
			Texture2D grassTexture = Resources.Load("Textures/Grass") as Texture2D;
			
			// It's possible the textures won't actually be found, as they are just examples and the
			// user might have decided not to include them when importing Cubiquity. This doesn't
			// matter and just means the uer will have to set up their own textures.
			if(rockTexture != null && soilTexture != null && grassTexture != null)
			{
				material.SetTexture("_Tex0", rockTexture);
				material.SetTextureScale("_Tex0", new Vector2(0.125f, 0.125f));
				material.SetTexture("_Tex1", soilTexture);
				material.SetTextureScale("_Tex1", new Vector2(0.125f, 0.125f));			
				material.SetTexture("_Tex2", grassTexture);
				material.SetTextureScale("_Tex2", new Vector2(0.125f, 0.125f));
			}
			else
			{
				Debug.LogWarning("Failed to set up the default Cubiquity terrain textures. This is probably " +
					"because you chose not to import the examples when importing Cubiquity? It doesn't matter, " +
					"it just means you have to configure your own textures through the inspector.");
			}
		}
		
		[MenuItem ("GameObject/Create Other/Colored Cubes Volume")]
		static void CreateColoredCubesVolume()
		{
			int width = 256;
			int height = 32;
			int depth = 256;
			
			ColoredCubesVolumeData data = ColoredCubesVolumeData.CreateEmptyVolumeData(new Region(0, 0, 0, width-1, height-1, depth-1), VolumeData.Paths.StreamingAssets, VolumeData.GeneratePathToVoxelDatabase());
			
			GameObject coloredCubesGameObject = ColoredCubesVolume.CreateGameObject(data);
			
			// And select it, so the user can get straight on with editing.
			Selection.activeGameObject = coloredCubesGameObject;
			
			// Set up our material	
			Shader shader = Shader.Find("ColoredCubesVolume");
			Material material = new Material(shader);
			coloredCubesGameObject.GetComponent<ColoredCubesVolumeRenderer>().material = material;
			
			int floorThickness = 8;
			QuantizedColor floorColor = new QuantizedColor(192, 192, 192, 255);
			
			for(int z = 0; z <= depth-1; z++)
			{
				for(int y = 0; y < floorThickness; y++)
				{
					for(int x = 0; x <= width-1; x++)
					{
						data.SetVoxel(x, y, z, floorColor);
					}
				}
			}
		}
	}
}
