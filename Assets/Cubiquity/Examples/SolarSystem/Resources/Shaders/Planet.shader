Shader "Planet"
{
	Properties
	{
		_Tex0 ("Base (RGB)", CUBE) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert addshadow
		#pragma target 3.0
		#pragma only_renderers d3d9
		
		#include "Assets/Cubiquity/Resources/TerrainVolumeUtilities.cginc"
		
		samplerCUBE _Tex0;
		
		sampler2D _Tex1;
		sampler2D _Tex2;
		sampler2D _Tex3;
		
		float4 _Tex1_ST;
		float4 _Tex2_ST;
		float4 _Tex3_ST;
		
		float4x4 _World2Volume;

		struct Input
		{
			float4 color : COLOR;
			float3 worldPos : POSITION;
			float3 volumeNormal;
			float4 volumePos;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			
			// Volume-space positions and normals are used for triplanar texturing
			float4 worldPos = mul(_Object2World, v.vertex);
			o.volumePos =  mul(_World2Volume, worldPos);
			o.volumeNormal = v.normal;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			IN.volumeNormal = normalize(IN.volumeNormal);
			
			// Unity's cubemap functionality is intended for reflection maps rather than wrapping 
			// textures around a sphere. As a result they show up backwards uness we invert them here.
			float3 cubemapSampleDir = IN.volumeNormal;
			cubemapSampleDir.x = -cubemapSampleDir.x;
			
			// Vertex colors coming out of Cubiquity don't actually sum to one
			// (roughly 0.5 as that's where the isosurface is). Make them sum
			// to one, though Cubiquity should probably be changed to do this.
			half4 materialStrengths = IN.color;
			half materialStrengthsSum = materialStrengths.x + materialStrengths.y + materialStrengths.z + materialStrengths.w;
			materialStrengths /= materialStrengthsSum;
			
			// Texture coordinates are calculated from the model
			// space position, scaled by a user-supplied factor.
			float3 texCoords = IN.volumePos.xyz; // * invTexScale;
			
			// Texture coordinate derivatives are explicitly calculated
			// so that we can sample textures inside conditional logic.
			float3 dx = ddx(texCoords);
			float3 dy = ddy(texCoords);
			
			// Squaring a normalized vector makes the components sum to one. It also seems
			// to give nicer transitions than simply dividing each component by the sum.
			float3 triplanarBlendWeights = IN.volumeNormal * IN.volumeNormal;
			
			half4 diffuse = 0.0;
			
			diffuse += texCUBE(_Tex0, cubemapSampleDir) * materialStrengths.r;
			
			// Sample each of the three textures using triplanar texturing, and
			// additively blend the results using the factors in materialStrengths.			
			diffuse += texTriplanar(_Tex1, texCoords, _Tex1_ST, dx, dy, triplanarBlendWeights * materialStrengths.g);
			diffuse += texTriplanar(_Tex2, texCoords, _Tex2_ST, dx, dy, triplanarBlendWeights * materialStrengths.b);
			diffuse += texTriplanar(_Tex3, texCoords, _Tex3_ST, dx, dy, triplanarBlendWeights * materialStrengths.a);
			
			
			o.Albedo = diffuse;

			o.Alpha = 1.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
