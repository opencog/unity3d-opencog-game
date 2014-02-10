using UnityEngine;
using System.Collections;

using Cubiquity;

[ExecuteInEditMode]
public class ColoredCubeMazeFromImage : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		// We will load our texture from the supplied maze image. If you wish to supply your own image then please
		// note that in Unity 4 you have to set the 'Read/Write Enabled flag' in the texture import properties.
		// To do this, find the texture in your project view, go to the import settings in the inspector, change
		// the 'Texture Type' to 'Advanced' and then check the 'Read/Write Enabled' checkbox.
		Texture2D mazeImage = Resources.Load("Images/Maze") as Texture2D;
		
		// The size of the volume we will generate. Note that our source image cn be considered
		// to have x and y axes,  but we map these to x and z because in Unity3D the y axis is up.
		int width = mazeImage.width;
		int height = 32;
		int depth = mazeImage.height;
		
		// Start with some empty volume data and we'll write our maze into this..
		ColoredCubesVolumeData data = ColoredCubesVolumeData.CreateEmptyVolumeData(new Region(0, 0, 0, width-1, height-1, depth-1));
		
		//Get the main volume component
		ColoredCubesVolume coloredCubesVolume = gameObject.GetComponent<ColoredCubesVolume>();
		
		// Attactch the empty data we created previously
		coloredCubesVolume.data = data;		
		
		// At this point we have a volume created and can now start writting our maze data into it.
		
		// It's best to create these outside of the loop.
		QuantizedColor red = new QuantizedColor(255, 0, 0, 255);
		QuantizedColor blue = new QuantizedColor(0, 0, 255, 255);
		QuantizedColor gray = new QuantizedColor(127, 127, 127, 255);
		QuantizedColor white = new QuantizedColor(255, 255, 255, 255);
		
		// Iterate over every pixel of our maze image.
		for(int z = 0; z < depth; z++)
		{
			for(int x = 0; x < width; x++)			
			{
				// The exact logic here isn't important for the purpose of the example, but basically we decide which
				// tile a voxel is part of based on it's position. You can tweak the values to get an dea of what they do.
				QuantizedColor tileColor;
				int tileSize = 4;
				int tileXOffset = 2;
				int tileZOffset = 2;
				int tileXPos = (x + tileXOffset) / tileSize;
				int tileZPos = (z + tileZOffset) / tileSize;
				if((tileXPos + tileZPos) % 2 == 1)
				{
					tileColor = blue;
				}
				else
				{
					tileColor = white;
				}
					
				// For each pixel of the maze image determine whether it is a wall or empty space.
				bool isWall = mazeImage.GetPixel(x, z).r < 0.5; // A black pixel represents a wall	
				
				// Height of the wall and floor in our maze.
				int floorHeight = 5;
				int wallHeight = 20;
				
				// Iterate over every voxel in the current column.
				for(int y = height-1; y > 0; y--)
				{
					// If the current column is a wall then we set every voxel
					// to gray except for the top one (which we set to red).
					if(isWall)
					{
						if(y < wallHeight)
						{
							data.SetVoxel(x, y, z, gray);
						}
						else if(y == wallHeight)
						{
							data.SetVoxel(x, y, z, red);
						}
					}
					else // Floor is also gray underneath but the top voxel is set to the tile color.
					{
						if(y < floorHeight)
						{
							data.SetVoxel(x, y, z, gray);
						}
						else if(y == floorHeight)
						{
							data.SetVoxel(x, y, z, tileColor);
						}
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
