sampler2D Current : register(s0);


//----------------------------------------------------------------------------
float4 PSTest (float2 TexCoords : TEXCOORD0) : COLOR0
{
	float4 color =  float4 (1, 0, 0, 1);
	color = tex2D (Current, TexCoords) * float4 (1, 0, 0, 1);
	//color = tex2D (Current, TexCoords);

	return color;
}

//----------------------------------------------------------------------------
Technique Test
{
	pass Test
	{
		PixelShader = compile ps_2_0 PSTest();
	}
}