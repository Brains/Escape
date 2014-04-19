sampler2D Current : register(s0);

//----------------------------------------------------------------------------
float Size;
float2 Shift;




//----------------------------------------------------------------------------
// Bilerps between the 4 closest texels
float4 QuadLerp(sampler2D samp, float2 s)
{
  float x0 = floor(s.x*Size);
  float x2 = x0 + 1.f;
  float y0 = floor(s.y*Size);
  float y1 = y0 + 1.f;
  
  float4 tex12 = tex2D(samp, float2(x0/Size, y1/Size)); 
  float4 tex22 = tex2D(samp, float2(x2/Size, y1/Size));   
  float4 tex11 = tex2D(samp, float2(x0/Size, y0/Size)); 
  float4 tex21 = tex2D(samp, float2(x2/Size, y0/Size)); 
  
  float fx = ((s.x*Size) - x0);
  float fy = ((s.y*Size) - y0);

  float4 l1 = lerp(tex11, tex21, fx);
  float4 l2 = lerp(tex12, tex22, fx);

  return lerp(l1, l2, fy);
}


//----------------------------------------------------------------------------
float4 PSInterpolation (float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - Shift;	
	
	float4 color = QuadLerp (Current, Pos);

	return color;
}






//----------------------------------------------------------------------------
Technique Interpolation
{
	pass Interpolation
	{
		PixelShader = compile ps_2_0 PSInterpolation();
	}
}