Shader "ColoredCubesVolume"
{	
    SubShader
    {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert vertex:vert addshadow
      #pragma target 3.0
      #pragma only_renderers d3d9
      
      struct Input
      {
          float4 color : COLOR;
          float4 modelPos;
      };
      
      #include "ColoredCubesVolumeUtilities.cginc"

		
		
      
      void vert (inout appdata_full v, out Input o)
      {
      	UNITY_INITIALIZE_OUTPUT(Input,o);
      	
      	// Unity can't cope with the idea that we're peforming lighting without having per-vertex
      	// normals. We specify dummy ones here to avoid having to use up vertex buffer space for them.
      	v.normal = float3 (0.0f, 0.0f, 1.0f);
      	v.tangent = float4 (1.0f, 0.0f, 0.0f, 1.0f);     
        
        // Model-space position is use for adding noise.
        o.modelPos = v.vertex;
      }
      
      void surf (Input IN, inout SurfaceOutput o)
      {
      	// Compute the surface normal in the fragment shader.
      	float3 surfaceNormal = normalize(cross(ddx(IN.modelPos.xyz), ddy(IN.modelPos.xyz)));
      	
	    //Add noise - we use model space to prevent noise scrolling if the volume moves.
	    float noise = positionBasedNoise(float4(IN.modelPos.xyz, 0.1));
        
        o.Albedo = IN.color + noise;
        o.Normal = surfaceNormal;
      }
      ENDCG
    }
    Fallback "Diffuse"
  }
  