/* Variables */

float OffsetPower;
sampler textureMapSampler;
//texture TextureMap; // texture 0
// sampler2D displacementMapSampler = sampler_state
/*
sampler2D textureMapSampler = sampler_state
{
    Texture = (TextureMap);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};
*/



texture DisplacementMap; // texture 1
// sampler2D textureMapSampler = sampler_state

// sampler2D displacementMapSampler = sampler_state
sampler displacementMapSampler = sampler_state
{
    Texture = <DisplacementMap>;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

/* Vertex shader output structures */

struct VertexShaderOutput
{
    float4 Position : position0;
    float2 TexCoord : texcoord0;
};

/* Pixel shaders */

float angle;

// float4 PixelShader1(VertexShaderOutput pVertexOutput) : color0
float4 PixelShader1(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // float4 displacementColor = tex2D(displacementMapSampler, pVertexOutput.TexCoord);	
	
	float4 texColor = tex2D(textureMapSampler, coords);
	// float4 displacementColor = tex2D(displacementMapSampler, coords);
		
	// return texColor * displacementColor;
	
	
    // float offset = (displacementColor.g - displacementColor.r) * OffsetPower;	
	// float offset = (displacementColor.g - displacementColor.r) * OffsetPower;	
	// float offset = (displacementColor.g - displacementColor.r);	
	// float offset = 0.3f;
	// float offset = 0.4f;
	
	float offsetx = sin(angle + 5 * coords.x) * OffsetPower;	
	float offsety = cos(angle + 7 * coords.y) * OffsetPower;

    // float2 newTexCoord = float2(pVertexOutput.TexCoord.x + offset, pVertexOutput.TexCoord.y + offset);
	float2 newTexCoord = float2(coords.x + offsetx, coords.y + offsety);
    
	float4 newTexColor = tex2D(textureMapSampler, newTexCoord);
	return newTexColor * color1;
	
	//if (texColor.r <= 1)
	//	return displacementColor;
	//else
	//	return displacementColor;	
}

/* Techniques */

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 PixelShader1();
    }
}