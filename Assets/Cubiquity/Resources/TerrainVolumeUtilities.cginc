#ifndef TERRAIN_VOLUME_UTILITIES
#define TERRAIN_VOLUME_UTILITIES

half4 texTriplanar(sampler2D tex, float3 coords, float4 texST, float3 dx, float3 dy, float3 triplanarBlendWeights)
{						
	// Used to avoid sampling a texture unless it
	// signicantly contributes to the final color.
	float blendWeightThreshold = 0.01;
	
	// Sample the texture three times (once along each axis) and combine the results.
	half4 triplanarSample = 0.0;
	if(triplanarBlendWeights.z > blendWeightThreshold)
	{
		triplanarSample += tex2Dgrad(tex, coords.xy * texST.xy + texST.zw, dx.xy * texST.xy, dy.xy * texST.xy) * triplanarBlendWeights.z;
	}
	if(triplanarBlendWeights.x > blendWeightThreshold)
	{
		triplanarSample += tex2Dgrad(tex, coords.yz * texST.xy + texST.zw, dx.yz * texST.xy, dy.yz * texST.xy) * triplanarBlendWeights.x;
	}
	if(triplanarBlendWeights.y > blendWeightThreshold)
	{
		triplanarSample += tex2Dgrad(tex, coords.xz * texST.xy + texST.zw, dx.xz * texST.xy, dy.xz * texST.xy) * triplanarBlendWeights.y;
	}
			
	// Return the combined result.
	return triplanarSample;
}
		
#endif //TERRAIN_VOLUME_UTILITIES