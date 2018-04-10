/* Variables */
float OffsetPower;
float lighten;
float dirx, diry;
// sampler textureMapSampler;

sampler2D textureMapSampler = sampler_state
{
    Texture = (TextureMap);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 GaussianPixelShader(float2 TextureCoordinate : TEXCOORD0) : COLOR
{
	int kernelSize = 25;
	float Pixels[25] =
	{
		-12,
		-11,
		-10,
		-9,
		-8,
		-7,
	   -6,
	   -5,
	   -4,
	   -3,
	   -2,
	   -1,
		0,
		1,
		2,
		3,
		4,
		5,
		6,
		7,
		8,
		9,
		10,
		11,
		12
	};	

	float BlurWeights[25] =	{0.004571f,0.00723f,0.010989f,0.016048f,0.022521f,0.03037f,0.039354f,0.049003f,0.058632f,0.067411f,0.074476f,0.079066f,0.080657f,0.079066f,0.074476f,0.067411f,0.058632f,0.049003f,0.039354f,0.03037f,0.022521f,0.016048f,0.010989f,0.00723f,0.004571f};


    // Pixel width
	float TextureWidth = 2560;
    float pixelWidth = 1.0f / TextureWidth;	
	// float pixelWidth = 0.004f;

    float4 color = {0, 0, 0, 0};
    float2 blur;    

    for (int i = 0; i < kernelSize; i++) 
    {        
		blur.x = TextureCoordinate.x + Pixels[i] * pixelWidth * dirx;
		blur.y = TextureCoordinate.y + Pixels[i] * pixelWidth * diry;
        color += tex2D(textureMapSampler, blur) * lighten * BlurWeights[i];
		// color += tex2D(textureMapSampler, blur) * (lighten / (kernelSize));		
    }  

    return color;
}

/* Pixel shaders */
float4 PixelShader1(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float4 texColor = tex2D(textureMapSampler, coords);	
	// return float4(1,1,1,1);
	// return GaussianPixelShader(coords);
	if (texColor.a <= 1) {		
		return GaussianPixelShader(coords) * color1;
	}
	else {
		return texColor * color1;		
	}	
}

float4 Blur(int radius) {

}

/* Techniques */

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 PixelShader1();
    }
	
}






