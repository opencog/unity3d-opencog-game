using UnityEngine;
using System.Collections;

using Cubiquity;

//FIXME - Should check the .Net rules regarding how the naming of namespaces corresponds to the naming of .dlls.
namespace CubiquityExamples
{
	public class Moon : MonoBehaviour
	{
		public float moonOrbitSpeed = 3.0f;
		public float moonRotationSpeed = -10.0f;
		
		GameObject moonOrbitPoint;
		
		void Start()
		{
			moonOrbitPoint = transform.parent.gameObject;
			
			TerrainVolume volume = GetComponent<TerrainVolume>();
			TerrainVolumeRenderer volumeRenderer = GetComponent<TerrainVolumeRenderer>();
			
			Material material = new Material(Shader.Find("Planet"));
			volumeRenderer.material = material;
			
			Cubemap earthSurfaceTexture = Resources.Load("Textures/MoonSurface") as Cubemap;			
			
			material.SetTexture("_Tex0", earthSurfaceTexture);
			
			int earthRadius = 15;
			Region volumeBounds = new Region(-earthRadius, -earthRadius, -earthRadius, earthRadius, earthRadius, earthRadius);		
			TerrainVolumeData result = TerrainVolumeData.CreateEmptyVolumeData(volumeBounds, VolumeData.Paths.TemporaryCache, VolumeData.GeneratePathToVoxelDatabase());
			
			TerrainVolumeGenerator.GeneratePlanet(result, 15, 14, 0, 0);
			
			volume.data = result;
		}
		
		void Update()
		{
			moonOrbitPoint.transform.Rotate(new Vector3(0,1,0), Time.deltaTime * moonOrbitSpeed);
			transform.Rotate(new Vector3(0,1,0), Time.deltaTime * moonRotationSpeed);
		}
	}
}
