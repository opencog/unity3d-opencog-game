#ifndef COLORED_CUBES_VOLUME_UTILITIES
#define COLORED_CUBES_VOLUME_UTILITIES

float positionBasedNoise(float4 positionAndStrength)
{
	//'floor' is more widely supported than 'round'. Offset consists of:
	//  - An integer to push us away from the origin (divide by zero causes a ringing artifact
	//    at one point in the world, and we want to pushthis somewhere it won't be seen.)
	//  - 0.5 to perform the rounding
	//  - A tiny offset to prevent sparkes as faces are exactly on rounding boundary.
	float3 roundedPos = floor(positionAndStrength.xyz + vec3(1000.501));
	
	//Our noise function generate banding for high inputs, so wrap them
	roundedPos = fmod(roundedPos, float3(17.0, 19.0, 23.0));
	
	//Large number is arbitrary, but smaller number lead to banding. '+ 1.0' prevents divide-by-zero
	float noise = 100000000.0 / (dot(roundedPos, roundedPos) + 1.0);
	noise = fract(noise);
	
	//Scale the noise
	float halfNoiseStrength = positionAndStrength.w * 0.5;
	noise = -halfNoiseStrength + positionAndStrength.w * noise; //http://www.opengl.org/wiki/GLSL_Optimizations#Get_MAD
	
	return noise;
}
		
#endif //COLORED_CUBES_VOLUME_UTILITIES