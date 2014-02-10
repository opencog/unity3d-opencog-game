using UnityEngine;
using System.Collections;
using System.IO;

namespace Cubiquity
{
	public static class TerrainVolumeGenerator
	{
		public static void GenerateFloor(TerrainVolumeData volumeData, int lowerLayerHeight, uint lowerLayerMaterial, int upperLayerHeight, uint upperLayerMaterial)
		{
			CubiquityDLL.GenerateFloor(volumeData.volumeHandle.Value, lowerLayerHeight, lowerLayerMaterial, upperLayerHeight, upperLayerMaterial);
		}
		
		public static void GeneratePlanet(TerrainVolumeData volumeData, float planetRadius, float mat1Radius, float mat2Radius, float mat3Radius)
		{
			Region volumeBounds = volumeData.enclosingRegion;
			MaterialSet materialSet = new MaterialSet();
			
			for(int z = volumeBounds.lowerCorner.z; z <= volumeBounds.upperCorner.z; z++)
			{
				for(int y = volumeBounds.lowerCorner.y; y <= volumeBounds.upperCorner.y; y++)
				{
					for(int x = volumeBounds.lowerCorner.x; x <= volumeBounds.upperCorner.x; x++)
					{
						// We are going to compute our density value based on the distance of a voxel from the center of our planet.
						// This is a function which (by definition) is zero at the center of the planet and has a smoothly increasing
						// value as we move away from the center.
						//
						// Note: For efficiency we could probably adapt this to work with squared distances (thereby eliminating
						// the square root operation), but we'd like to keep this example as intuitive as possible.
						float distFromCenter = Mathf.Sqrt(x * x + y * y + z * z);
						
						// We actually want our volume to have high values in the center and low values as we move out, because our
						// eath should be a solid sphere surrounded by empty space. If we invert the distance then this is a step in
						// the right direction. We still have zero in the center, but lower (negative) values as we move out.
						float density = -distFromCenter;
						
						// By adding the 'planetRadius' we now have a function which starts at 'planetRadius' and still decreases as it
						// moves out. The function passes through zero at a distance of 'planetRadius' and then continues do decrease
						// as it gets even further out.
						density += planetRadius;
						
						// Ideally we would like our final density value to be '255' for voxels inside the planet and '0' for voxels
						// outside the planet. At the surface there should be a transition but this should occur not too quickly and
						// not too slowly, as both of these will result in a jagged appearance to the mesh.
						//
						// We probably want the transition to occur over a few voxels, whereas it currently occurs over 255 voxels
						// because it was derived from the distance. By scaling the density field we effectivly compress the rate
						// at which it changes at the surface. We also make the center much too high and the outside very low, but
						// we will clamp these to the corect range later.
						//
						// Note: You can try commenting out or changing the value on this line to see the effect it has.
						density *= 50;
						
						// Until now we've been defining our density field as if the threshold was at zero, with positive densities
						// being solid and negative densities being empty. But actually Cubiquity operates on the range 0 to 255, and
						// uses a threashold of 127 to decide where to place the generated surface.  Therefore we shift and clamp our
						// density value and store it in a byte.
						density += 127;						
						byte densityAsByte = (byte)(Mathf.Clamp(density, 0, 255));
						
						if(distFromCenter < mat3Radius)
						{
							materialSet.weights[0] = 0;
							materialSet.weights[1] = 0;
							materialSet.weights[2] = 0;
							materialSet.weights[3] = densityAsByte;
						}
						else if(distFromCenter < mat2Radius)
						{
							materialSet.weights[0] = 0;
							materialSet.weights[1] = 0;
							materialSet.weights[2] = densityAsByte;
							materialSet.weights[3] = 0;
						}
						else if(distFromCenter < mat1Radius)
						{
							materialSet.weights[0] = 0;
							materialSet.weights[1] = densityAsByte;
							materialSet.weights[2] = 0;
							materialSet.weights[3] = 0;
						}
						else //Surface material
						{
							materialSet.weights[0] = densityAsByte;
							materialSet.weights[1] = 0;
							materialSet.weights[2] = 0;
							materialSet.weights[3] = 0;
						}
						
						volumeData.SetVoxel(x, y, z, materialSet);
						
					}
				}
			}
		}
	}
}