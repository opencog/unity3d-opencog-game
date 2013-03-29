Shader "VoxelEngine/Water" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Alpha ("Alpha", float) = 0.8
	}
	
	SubShader {
		Tags { "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		Cull off
		
		CGPROGRAM
      	#pragma surface surf Lambert noambient vertex:vert
      	
      	struct Input {
        	float2 uv_MainTex;
        	float4 color : COLOR;
      	};
      	sampler2D _MainTex;
      	float _Alpha;
      	
      	
      	void vert (inout appdata_full v) {
        	float4 wpos = mul( UNITY_MATRIX_MV, v.vertex );
        	float4 pos = v.vertex;
			float sx = sin(wpos.x + _Time*30);
			float sz = sin(wpos.z + _Time*30);
        	pos.y += (sx + sz) * 0.05f - 0.1f;
        	v.vertex = pos;
      	}
		
      	void surf(Input IN, inout SurfaceOutput o) {
      		o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
      		o.Alpha = 0.5;
      	
      		float3 light = IN.color.rgb;
      		float sun = IN.color.a;
      		float3 ambient = UNITY_LIGHTMODEL_AMBIENT * 2 * sun;
      		ambient = max(ambient, 0.0666);
      		ambient = max(ambient, light);
        	o.Emission = o.Albedo * ambient;
     	}
		
		ENDCG
	}
	
	FallBack "Unlit/Texture"
}
