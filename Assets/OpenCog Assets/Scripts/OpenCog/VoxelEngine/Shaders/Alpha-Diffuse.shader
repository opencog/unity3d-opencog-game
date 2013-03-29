Shader "VoxelEngine/Alpha-Diffuse" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="TransparentCutout" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		
		CGPROGRAM
      	#pragma surface surf Lambert noambient
      	#include "Lighting.inc"
      	
		struct Input {
        	float2 uv_MainTex;
        	float4 color : COLOR;
      	};
		sampler2D _MainTex;
		
      	void surf (Input IN, inout SurfaceOutput o) {
      		fixed4 color = tex2D (_MainTex, IN.uv_MainTex);
        	o.Albedo = color.rgb;
        	o.Alpha = color.a;
			clip(color.a-0.01f);
			
			o.Emission = Lighting(color, IN.color);
     	}
		
		ENDCG
	}
	
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 80
		
		Pass {
			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			
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
