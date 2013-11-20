Shader "TerrainVolume" {
	Properties {
		_Tex0 ("Base (RGB)", 2D) = "white" {}
		_Tex1 ("Base (RGB)", 2D) = "white" {}
		_Tex2 ("Base (RGB)", 2D) = "white" {}
		_Tex3 ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0
		#pragma only_renderers d3d9
		#pragma multi_compile BRUSH_MARKER_ON BRUSH_MARKER_OFF

		sampler2D _Tex0;
		sampler2D _Tex1;
		sampler2D _Tex2;
		sampler2D _Tex3;
		
		float3 _TexInvScale0;
		float3 _TexInvScale1;
		float3 _TexInvScale2;
		float3 _TexInvScale3;
		
		float3 _TexOffset0;
		float3 _TexOffset1;
		float3 _TexOffset2;
		float3 _TexOffset3;
		
#if BRUSH_MARKER_ON
		float4 _BrushCenter;
		float4 _BrushSettings;
		float4 _BrushColor;
#endif

		struct Input
		{
			float4 color : COLOR;
			float3 worldNormal;
			float3 worldPos;
		};
		
		half4 texTriplanar(sampler2D tex, float3 coords, float3 dx, float3 dy, float3 triplanarBlendWeights)
		{						
			// Used to avoid sampling a texture unless it
			// signicantly contributes to the final color.
			float blendWeightThreshold = 0.01;
			
			// Sample the texture three times (once along each axis) and combine the results.
			half4 triplanarSample = 0.0;
			if(triplanarBlendWeights.z > blendWeightThreshold)
			{
				triplanarSample += tex2Dgrad(tex, coords.xy, dx.xy, dy.xy) * triplanarBlendWeights.z;
			}
			if(triplanarBlendWeights.x > blendWeightThreshold)
			{
				triplanarSample += tex2Dgrad(tex, coords.yz, dx.yz, dy.yz) * triplanarBlendWeights.x;
			}
			if(triplanarBlendWeights.y > blendWeightThreshold)
			{
				triplanarSample += tex2Dgrad(tex, coords.xz, dx.xz, dy.xz) * triplanarBlendWeights.y;
			}
					
			// Return the combined result.
			return triplanarSample;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			//half texScale = 8.0;
			//half invTexScale = 1.0 / texScale;
			
			// Interpolation can cause the normal vector to become denomalised.
			IN.worldNormal = normalize(IN.worldNormal);
			
			// Vertex colors coming out of Cubiquity don't actually sum to one
			// (roughly 0.5 as that's where the isosurface is). Make them sum
			// to one, though Cubiquity should probably be changed to do this.
			half4 materialStrengths = IN.color;
			half materialStrengthsSum = materialStrengths.x + materialStrengths.y + materialStrengths.z + materialStrengths.w;
			materialStrengths /= materialStrengthsSum;
			
			// Texture coordinates are calculated from the world
			// space position, scaled by a user-supplied factor.
			float3 texCoords = IN.worldPos.xyz; // * invTexScale;
			
			// Texture coordinate derivatives are explicitly calculated
			// so that we can sample textures inside conditional logic.
			float3 dx = ddx(texCoords);
			float3 dy = ddy(texCoords);
			
			// Squaring a normalized vector makes the components sum to one. It also seems
			// to give nicer transitions than simply dividing each component by the sum.
			float3 triplanarBlendWeights = IN.worldNormal * IN.worldNormal;	
			
			// Sample each of the four textures using triplanar texturing, and
			// additively blend the results using the factors in materialStrengths.
			half4 diffuse = 0.0;
			diffuse += texTriplanar(_Tex0, texCoords * _TexInvScale0 + _TexOffset0, dx * _TexInvScale0, dy * _TexInvScale0, triplanarBlendWeights * materialStrengths.r);
			diffuse += texTriplanar(_Tex1, texCoords * _TexInvScale1 + _TexOffset1, dx * _TexInvScale1, dy * _TexInvScale1, triplanarBlendWeights * materialStrengths.g);
			diffuse += texTriplanar(_Tex2, texCoords * _TexInvScale2 + _TexOffset2, dx * _TexInvScale2, dy * _TexInvScale2, triplanarBlendWeights * materialStrengths.b);
			diffuse += texTriplanar(_Tex3, texCoords * _TexInvScale3 + _TexOffset3, dx * _TexInvScale3, dy * _TexInvScale3, triplanarBlendWeights * materialStrengths.a);
			
#if BRUSH_MARKER_ON
			float brushStrength = 0.0f;
			
			float distToBrushCenter = length(IN.worldPos.xyz - _BrushCenter.xyz);
			if(distToBrushCenter < _BrushSettings.x)
			{
				brushStrength = 1.0;
			}
			else if(distToBrushCenter < _BrushSettings.y)
			{
				float lerpFactor = (distToBrushCenter - _BrushSettings.x) / (_BrushSettings.y - _BrushSettings.x);
				brushStrength = lerp(1.0f, 0.0f, lerpFactor);
		
				brushStrength = min(brushStrength, 1.0f);
				brushStrength = max(brushStrength, 0.0f);
				
				//brushStrength = 1.0 - lerpFactor;
			}
			
			_BrushColor.a = _BrushColor.a * brushStrength;
			
			o.Albedo = diffuse.rgb * (1.0 - _BrushColor.a) + _BrushColor.rgb * _BrushColor.a;
#else
			o.Albedo = diffuse.rgb;
#endif

			o.Alpha = 1.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
