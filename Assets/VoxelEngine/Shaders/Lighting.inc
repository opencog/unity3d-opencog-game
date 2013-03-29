
//uniform float3 _SunDir;

float3 Lighting(float3 color, float4 lightData) {
	float3 light = lightData.rgb;
    float sun = lightData.a;
    float3 ambient = UNITY_LIGHTMODEL_AMBIENT * 2 * sun;
      		
    //half rim = saturate( dot(normalize(_SunDir), o.Normal) );
    //rim = 0.7 + rim * 0.3;
    //ambient *= rim;
      		
    ambient = max(ambient, 0.0666);
    ambient = max(ambient, light);
    return color * ambient;
}