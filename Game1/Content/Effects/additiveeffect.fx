sampler s0;

texture lightMask;
sampler lightSampler = sampler_state { Texture = <lightMask>; };

float4 PixelShaderLight(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	float4 lightColor = tex2D(lightSampler, coords);
	// return color * lightColor * color1;
	return clamp((color * color1) + lightColor, 0, 1);
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 PixelShaderLight();
	}
}