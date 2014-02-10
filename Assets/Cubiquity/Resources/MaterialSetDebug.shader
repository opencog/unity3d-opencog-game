Shader "MaterialSetDebug"
{
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert addshadow

		struct Input
		{
			float4 color : COLOR;
			float3 worldPos : POSITION;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			// Vertex colors coming out of Cubiquity don't actually sum to one
			// (roughly 0.5 as that's where the isosurface is). Make them sum
			// to one, though Cubiquity should probably be changed to do this.
			half4 materialStrengths = IN.color;
			half materialStrengthsSum = materialStrengths.x + materialStrengths.y + materialStrengths.z + materialStrengths.w;
			materialStrengths /= materialStrengthsSum;
			
			o.Albedo  = half4(1.0, 0.0, 0.0, 1.0) * materialStrengths.x;
			o.Albedo += half4(0.0, 1.0, 0.0, 1.0) * materialStrengths.y;
			o.Albedo += half4(0.0, 0.0, 1.0, 1.0) * materialStrengths.z;
			o.Albedo += half4(0.0, 1.0, 1.0, 1.0) * materialStrengths.w;

			o.Alpha = 1.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
