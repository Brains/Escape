//----------------------------------------------------------------------------
sampler2D Current : register(s0);
sampler2D Velocity;
sampler2D Advected;
sampler2D Reversed;
sampler2D Divergence;
sampler2D Pressure;
sampler2D Vorticity;


//----------------------------------------------------------------------------
// Dimensions of the textures making up the fluid stages
float Size;
float2 Shift;

float DT;
float PermanentVelocity;
float Diffusion;
float HalfCellSize;
float VorticityScale;

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
float4 LinearVerticalInterpolation (sampler2D field, float2 position)
{
	float y1 = floor (position.y * Size);
	float y2 = y1 + 1.0f;

	float4 tex1 = tex2D (field, float2 (position.x, y1 / Size)); 
	float4 tex2 = tex2D (field, float2 (position.x, y2 / Size));

	float rate = ((position.y * Size) - y1);

	return lerp (tex1, tex2, rate);
}

//----------------------------------------------------------------------------
// Adds the sources to Density and Velocity
float4 PSPermanentAdvection (float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - Shift;

    Pos -= float2 (0, PermanentVelocity) / Size;

	return LinearVerticalInterpolation (Current, Pos);
}

//----------------------------------------------------------------------------
// Advects Velocity and Density
float4 PSAdvection(float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - Shift;

    Pos -= tex2D (Velocity, Pos) * DT / Size;	
	float4 color = Diffusion * QuadLerp (Current, Pos);

    return color;
}

//----------------------------------------------------------------------------
float4 PSAdvectionMacCormack (float2 TexCoords : TEXCOORD0) : COLOR0
{
	//float2 Pos = TexCoords - Shift;	
	float2 Pos = TexCoords;	
	// float Offset = 1.0f / Size;
	

	float4 pn = tex2D(Current, Pos);
	
	float4 pnhat = tex2D(Reversed, Pos);

	float4 velocity = tex2D (Velocity, Pos);
	float2 position =  Pos * Size - velocity.xy * DT;
	position =  floor  (position + float2 (0.5f , 0.5f ));
	position /= Size;

	//float4 p1 = tex2D(Current, position);
	//float4 p2 = tex2D(Current, position + float2(Offset, 0));
	//float4 p3 = tex2D(Current, position + float2(0, Offset));
	//float4 p4 = tex2D(Current, position + float2(Offset, Offset));

	float Offset = 0.5f / Size;
	float4 p1 = tex2D(Current, position + float2(- Offset, - Offset));
	float4 p2 = tex2D(Current, position + float2(- Offset, + Offset));
	float4 p3 = tex2D(Current, position + float2(+ Offset, - Offset));
	float4 p4 = tex2D(Current, position + float2(+ Offset, + Offset));

	float4 pmin = min(min(p1, p2), min(p3, p4));
	float4 pmax = max(max(p1, p2), max(p3, p4));
	
	// In PDF was `nposTC` instead `position`
	float4 pn1hat = tex2D(Advected, position);
	
	float4 pn1 = pn1hat + 0.5 * (pn - pnhat);
	  //return pn1;

	return max(min(pn1, pmax), pmin);
}

//----------------------------------------------------------------------------
// Calculates and spits out a divergence texture
float4 PSDivergence(float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - Shift;
	float Offset = 1.0f/Size;	
	float4 left   = tex2D(Current, float2(Pos.x - Offset, Pos.y));
	float4 right  = tex2D(Current, float2(Pos.x + Offset, Pos.y));
	float4 top    = tex2D(Current, float2(Pos.x, Pos.y - Offset));
	float4 bottom = tex2D(Current, float2(Pos.x, Pos.y + Offset));

	return .5f * ((right.x - left.x) + (bottom.y - top.y));
}

//----------------------------------------------------------------------------
// Calculates and spits out the pressure texture over a series of iterations
float4 PSJacobi(float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - Shift;
	float Offset = 1.0f/Size;	
	float4 center = tex2D(Divergence, Pos);
	float4 left   = tex2D(Current, float2(Pos.x - Offset, Pos.y));
	float4 right  = tex2D(Current, float2(Pos.x + Offset, Pos.y));
	float4 top    = tex2D(Current, float2(Pos.x, Pos.y - Offset));
	float4 bottom = tex2D(Current, float2(Pos.x, Pos.y + Offset));

	return ((left + right + bottom + top) - center) * .25f;
}

//----------------------------------------------------------------------------
// Subtracts the pressure texture from the velocity texture
float4 PSSubtract(float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - Shift;
	float Offset = 1.0f/Size;
	float left   = tex2D(Pressure, float2(Pos.x - Offset, Pos.y)).x;
	float right  = tex2D(Pressure, float2(Pos.x + Offset, Pos.y)).x;
	float top    = tex2D(Pressure, float2(Pos.x, Pos.y - Offset)).y;
	float bottom = tex2D(Pressure, float2(Pos.x, Pos.y + Offset)).y;

	float2 grad = float2(right-left, bottom-top) * .5f;
	float4 v = tex2D(Current, Pos);
	v.xy -= grad;

	return v;
}

//----------------------------------------------------------------------------
float4 PSVorticity(float2 TexCoords : TEXCOORD0) : COLOR0 
{
	float2 Pos = TexCoords - Shift;	
	float Offset = 1.0f/Size;

	float4 uL	= tex2D (Velocity, float2(Pos.x - Offset, Pos.y));
	float4 uR	= tex2D (Velocity, float2(Pos.x + Offset, Pos.y));
	float4 uB	= tex2D (Velocity, float2(Pos.x, Pos.y - Offset));
	float4 uT	= tex2D (Velocity, float2(Pos.x, Pos.y + Offset));
	
	float vort = HalfCellSize * ((uR.y - uL.y) - (uT.x - uB.x));

	return float4 (vort, 0.0f, 0.0f, 0.0f);
} 

//----------------------------------------------------------------------------
float4 PSVorticityForce (float2 TexCoords : TEXCOORD0) : COLOR0 
{
	float2 Pos = TexCoords - Shift;	
	float Offset = 1.0f/Size;

	// Vorticity from adjacent pixels
	float vL	= tex2D (Vorticity, float2(Pos.x - Offset, Pos.y));
	float vR	= tex2D (Vorticity, float2(Pos.x + Offset, Pos.y));
	float vB	= tex2D (Vorticity, float2(Pos.x, Pos.y - Offset));
	float vT	= tex2D (Vorticity, float2(Pos.x, Pos.y + Offset));

	float vC	= tex2D (Vorticity, Pos);

	// Force
	float2 force = HalfCellSize * float2(abs(vT) - abs(vB), abs(vR) - abs(vL));

	// Safe normalize
	static const half EPSILON = 2.4414e-4; // 2^-12
	float magSqr = max(EPSILON, dot(force, force)); 
	force = force * rsqrt(magSqr); 

	// Scale
	force *= VorticityScale * vC * float2(1, -1);      

	// Apply to Velocity field
	float2 velocityNew = tex2D (Velocity, Pos);
	velocityNew += DT * force;

	return float4 (velocityNew.x, velocityNew.y, 0.0f, 0.0f);
} 









//----------------------------------------------------------------------------
technique PermanentAdvection
{
	pass PermanentAdvection
	{
		PixelShader = compile ps_2_0 PSPermanentAdvection();
	}
}

//----------------------------------------------------------------------------
technique DoAdvection
{
	pass DoAdvection
	{
		PixelShader = compile ps_2_0 PSAdvection();
	}
	//pass SetBounds
	//{
	//	PixelShader = compile ps_2_0 PSSetBoundsDouble();
	//}
}

//----------------------------------------------------------------------------
technique DoAdvectionMacCormack
{
	pass DoAdvectionMacCormack
	{
		PixelShader = compile ps_2_0 PSAdvectionMacCormack();
	}
	//pass SetBounds
	//{
	//	PixelShader = compile ps_2_0 PSSetBoundsDouble();
	//}
}

//----------------------------------------------------------------------------
technique DoDivergence
{
	pass DoDivergence
	{
		PixelShader = compile ps_2_0 PSDivergence();
	}
}

//----------------------------------------------------------------------------
Technique DoJacobi
{
	pass DoJacobi
	{
		PixelShader = compile ps_2_0 PSJacobi();
	}
	//pass SetBounds
	//{
	//	PixelShader = compile ps_2_0 PSArbitraryPressureBoundary();
	//}
}

//----------------------------------------------------------------------------
Technique Subtract
{
	pass Subtract
	{
		PixelShader = compile ps_2_0 PSSubtract();
	}
}

//----------------------------------------------------------------------------
Technique DoVorticity
{
	pass DoVorticity
	{
		PixelShader = compile ps_2_0 PSVorticity();
	}
}

//----------------------------------------------------------------------------
Technique DoVorticityForce
{
	pass DoVorticityForce
	{
		PixelShader = compile ps_2_0 PSVorticityForce();
	}
}