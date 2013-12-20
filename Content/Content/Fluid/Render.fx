sampler2D Current : register(s0);
sampler2D Map;
sampler2D Stencil;

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
float4 DisplayScalar (sampler2D input, float2 position, float scale, float bias)
{
	return bias + scale * QuadLerp (input, position).xxxx;
} 

//----------------------------------------------------------------------------
float4 DisplayVector (sampler2D input, float2 position, float scale, float bias)
{
	return bias + scale * QuadLerp (input, position);
} 

//----------------------------------------------------------------------------
float GetOpacity (float4 color, float factor)
{
	float opacity;
	
	opacity = color.w * max (color.x, max (color.y, color.z)) * factor;

	return opacity;
}


//----------------------------------------------------------------------------
float4 PSGradient (float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - Shift;	
	
	float value = tex2D (Current, Pos).x;
	
	// Scaling
	value = value * 4.0 + 0.0;
	
	float4 color = tex2D (Map, float2 (value, 0));
	color *= tex2D (Stencil, Pos);
	color.w = GetOpacity (color, 1);

	return color;
}

//----------------------------------------------------------------------------
float4 PSInterpolation (float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - Shift;	
	
	float4 color = QuadLerp (Current, Pos);
	color.w = GetOpacity (color, 1);

	return color;
}

//----------------------------------------------------------------------------
float4 PSDisplay(float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - Shift;	
	float4 color;

	color = DisplayVector (Current, TexCoords, 0.5, 0.5);
	//color = DisplayScalar (Current, TexCoords, 5, 0.5);

	color.w = GetOpacity (color, 2);
	//color.w = GetOpacity (color, 20);

	return color;
}



//----------------------------------------------------------------------------
Technique Gradient
{
	pass Gradient
	{
		PixelShader = compile ps_2_0 PSGradient();
	}
}

//----------------------------------------------------------------------------
Technique Interpolation
{
	pass Interpolation
	{
		PixelShader = compile ps_2_0 PSInterpolation();
	}
}

//----------------------------------------------------------------------------
Technique Display
{
	pass Display
	{
		PixelShader = compile ps_2_0 PSDisplay();
	}
}
