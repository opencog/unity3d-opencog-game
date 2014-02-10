using UnityEngine;
using System.Collections;

using Cubiquity;

//FIXME - Should check the .Net rules regarding how the naming of namespaces corresponds to the naming of .dlls.
namespace CubiquityExamples
{
	public class Earth : MonoBehaviour
	{
		public float earthOrbitSpeed = 1.0f;
		public float earthRotationSpeed = -5.0f;
		
		GameObject earthOrbitPoint;
		
		void Start()
		{
			earthOrbitPoint = transform.parent.gameObject;
			
			TerrainVolume volume = GetComponent<TerrainVolume>();
			TerrainVolumeRenderer volumeRenderer = GetComponent<TerrainVolumeRenderer>();
			
			Material material = new Material(Shader.Find("Planet"));
			volumeRenderer.material = material;
			
			Cubemap earthSurfaceTexture = Resources.Load("Textures/EarthSurface") as Cubemap;
			Texture2D rockTexture = Resources.Load("Textures/Rock") as Texture2D;
			Texture2D lavaTexture = Resources.Load("Textures/Lava") as Texture2D;
			Texture2D coreTexture = Resources.Load("Textures/Core") as Texture2D;
			
			material.SetTexture("_Tex0", earthSurfaceTexture);
			material.SetTexture("_Tex1", rockTexture);
			material.SetTextureScale("_Tex1", new Vector2(0.125f, 0.125f));
			material.SetTexture("_Tex2", lavaTexture);
			material.SetTextureScale("_Tex2", new Vector2(0.125f, 0.125f));
			material.SetTexture("_Tex3", coreTexture);
			
			int earthRadius = 60;
			Region volumeBounds = new Region(-earthRadius, -earthRadius, -earthRadius, earthRadius, earthRadius, earthRadius);		
			TerrainVolumeData result = TerrainVolumeData.CreateEmptyVolumeData(volumeBounds, VolumeData.Paths.TemporaryCache, VolumeData.GeneratePathToVoxelDatabase());
			
			TerrainVolumeGenerator.GeneratePlanet(result, 60, 59, 50, 25);
			
			volume.data = result;
		}
		
		void Update()
		{
			earthOrbitPoint.transform.Rotate(new Vector3(0,1,0), Time.deltaTime * earthOrbitSpeed);
			transform.Rotate(new Vector3(0,1,0), Time.deltaTime * earthRotationSpeed);
		}
	}
}
