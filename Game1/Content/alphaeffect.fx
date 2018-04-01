texture alphaMask;
texture secretScene;
sampler alphaSampler = sampler_state { Texture = <alphaMask>; };
sampler s0;// = sampler_state { Texture = <secretScene>; };

float4 AlphaShader(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float4 myColor = tex2D(s0, coords);
	float4 alphaColor = tex2D(alphaSampler, coords); 	 					 	 				
	return float4(myColor.r, myColor.g, myColor.b, min(alphaColor.a, myColor.a));						
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 AlphaShader();
	}
}