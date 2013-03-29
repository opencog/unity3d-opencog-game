Shader "VoxelEngine/Diffuse" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
      	#pragma surface surf Lambert noambient
      	#include "Lighting.inc"
      	
      	struct Input {
        	float2 uv_MainTex;
        	float4 color : COLOR;
      	};
      	sampler2D _MainTex;
		
      	void surf(Input IN, inout SurfaceOutput o) {
      		o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
      		
        	o.Emission = Lighting(o.Albedo, IN.color);
     	}
		
		ENDCG
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 80
		
		Pass {
			Lighting Off
			
			Material {
				Diffuse (1,1,1,1)
				Ambient (1,1,1,1)
			}
			
			BindChannels {
				Bind "Vertex", vertex
   				Bind "texcoord", texcoord
   				Bind "Color", color
			}
			
			SetTexture [_MainTex] {
				Combine texture * primary
			} 
		}
	}
	FallBack "Unlit/Texture"
}
