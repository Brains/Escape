//----------------------------------------------------------------------------
// Samplers, from DrawFluid
sampler2D buffer : register(s0);
sampler2D Velocity : register(s1);
sampler2D Density : register(s2);
sampler2D Divergence : register(s3);
sampler2D Pressure : register(s4);
sampler2D VelocitySources : register(s5);
sampler2D DensitySources : register(s6);
sampler2D Vorticity : register(s7);
sampler2D OffsetTable : register(s8);
sampler2D VelocityOffsets : register(s9);
sampler2D PressureOffsets : register(s10);
sampler2D Boundaries : register(s11);

//----------------------------------------------------------------------------
// Dimensions of the textures making up the fluid stages
float FluidSize = 256.0f;
float dT = 1.0f; // Delta time
float VelocityDiffusion = 1.0f;
float DensityDiffusion = 1.0f;
float4 Impulse;
float HalfCellSize = 0.5f;
float VorticityScale = 0.25f;

//----------------------------------------------------------------------------
// Struct for when we use two render targets
struct DoubleOutput
{
	float4 Vel : COLOR0;
	float4 Den : COLOR1;
};

//----------------------------------------------------------------------------
// Bilerps between the 4 closest texels
float4 QuadLerp(sampler2D samp, float2 s)
{
  float x0 = floor(s.x*FluidSize);
  float x2 = x0 + 1.f;
  float y0 = floor(s.y*FluidSize);
  float y1 = y0 + 1.f;
  
  float4 tex12 = tex2D(samp, float2(x0/FluidSize, y1/FluidSize)); 
  float4 tex22 = tex2D(samp, float2(x2/FluidSize, y1/FluidSize));   
  float4 tex11 = tex2D(samp, float2(x0/FluidSize, y0/FluidSize)); 
  float4 tex21 = tex2D(samp, float2(x2/FluidSize, y0/FluidSize)); 
  
  float fx = ((s.x*FluidSize) - x0);
  float fy = ((s.y*FluidSize) - y0);

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
// Sets the edges of the texture, clips everything else
float2 SetBounds(float2 v)
{
	float2 pos = v;
	float Offset = (1.0f/(FluidSize));
	float HalfPixel = (.5f/FluidSize);
	if(pos.x <  HalfPixel)
        pos.x += Offset;
    else if(pos.y <  HalfPixel)
        pos.y += Offset;
    else if(pos.x >=  (1.0f-HalfPixel)-Offset)
        pos.x -= Offset;
    else if(pos.y >=  (1.0f-HalfPixel)-Offset)
		pos.y -= Offset;		
	else
		clip(-1);
	return pos;
}

//----------------------------------------------------------------------------
float4 PSVelocityColorize(float2 TexCoords : TEXCOORD0) : COLOR0
{
	return (tex2D(buffer, TexCoords) * -Impulse) * FluidSize;
}

//----------------------------------------------------------------------------
// Adds the sources to Density and Velocity
DoubleOutput PSAddSources(float2 TexCoords : TEXCOORD0)
{
	DoubleOutput Output;	
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);
	float4 uv = tex2D(VelocitySources, Pos);	
	Output.Vel = max(-.8f, min(.8f, tex2D(Velocity, Pos) + uv));
	Output.Den = tex2D(Density, Pos) + (tex2D(DensitySources, Pos) / 16);	
	Output.Vel.w = 1.0f;
	Output.Den.w = 1.0f;

	return Output;
}

//----------------------------------------------------------------------------
// Sets the boundaries for Density and Velocity
DoubleOutput PSSetBoundsDouble(float2 TexCoords : TEXCOORD0)
{
	DoubleOutput Output;
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);
    Pos = SetBounds(Pos);
	Output.Vel = -tex2D(Velocity, Pos);
	Output.Den = tex2D(Density, Pos);

	return Output;
}

//----------------------------------------------------------------------------
// Sets the boundries for Pressure
float4 PSSetBoundsSingle(float2 TexCoords : TEXCOORD0) : COLOR0
{	
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);
    Pos = SetBounds(Pos);
	return tex2D(buffer, Pos);	
}

//----------------------------------------------------------------------------
// Advects Velocity and Density
DoubleOutput PSAdvection(float2 TexCoords : TEXCOORD0)
{
	DoubleOutput Output;
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);	
    Pos -= tex2D(Velocity, Pos) / FluidSize;	
	Output.Vel = VelocityDiffusion*QuadLerp(Velocity, Pos);
	Output.Den = DensityDiffusion*QuadLerp(Density, Pos);  

    return Output;
}

//----------------------------------------------------------------------------
// Calculates and spits out a divergence texture
float4 PSDivergence(float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);
	float Offset = 1.0f/FluidSize;	
	float4 left   = tex2D(buffer, float2(Pos.x - Offset, Pos.y));
	float4 right  = tex2D(buffer, float2(Pos.x + Offset, Pos.y));
	float4 top    = tex2D(buffer, float2(Pos.x, Pos.y - Offset));
	float4 bottom = tex2D(buffer, float2(Pos.x, Pos.y + Offset));

	return .5f * ((right.x - left.x) + (bottom.y - top.y));
}

//----------------------------------------------------------------------------
// Calculates and spits out the pressure texture over a series of iterations
float4 PSJacobi(float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);
	float Offset = 1.0f/FluidSize;	
	float4 center = tex2D(Divergence, Pos);
	float4 left   = tex2D(buffer, float2(Pos.x - Offset, Pos.y));
	float4 right  = tex2D(buffer, float2(Pos.x + Offset, Pos.y));
	float4 top    = tex2D(buffer, float2(Pos.x, Pos.y - Offset));
	float4 bottom = tex2D(buffer, float2(Pos.x, Pos.y + Offset));

	return ((left + right + bottom + top) - center) * .25f;
}

//----------------------------------------------------------------------------
// Subtracts the pressure texture from the velocity texture
float4 PSSubtract(float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);	
	float Offset = 1.0f/FluidSize;
	float left   = tex2D(Pressure, float2(Pos.x - Offset, Pos.y)).x;
	float right  = tex2D(Pressure, float2(Pos.x + Offset, Pos.y)).x;
	float top    = tex2D(Pressure, float2(Pos.x, Pos.y - Offset)).y;
	float bottom = tex2D(Pressure, float2(Pos.x, Pos.y + Offset)).y;

	float2 grad = float2(right-left, bottom-top) * .5f;
	float4 v = tex2D(buffer, Pos);
	v.xy -= grad;

	return v;
}

//----------------------------------------------------------------------------
float4 PSVorticity(float2 TexCoords : TEXCOORD0) : COLOR0 
{
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);	
	float Offset = 1.0f/FluidSize;

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
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);	
	float Offset = 1.0f/FluidSize;

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
	velocityNew += dT * force;

	return float4 (velocityNew.x, velocityNew.y, 0.0f, 0.0f);
} 

//----------------------------------------------------------------------------
float4 PSShapeObstacles (float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);	
	float4 color = tex2D (buffer, Pos);

	float opacity = color.w;
	//color *= color.w * 10;

	if (opacity > 0.2f)
	{
		color = float4(1, 1, 1, 1);
	}

	return color;
}

//----------------------------------------------------------------------------
float4 PSUpdateOffsets (float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);	
	float Offset = 1.0f/FluidSize;

    // Get neighboring boundary values (on or off)
	float bW	= tex2D (buffer, float2(Pos.x - Offset, Pos.y));
	float bE	= tex2D (buffer, float2(Pos.x + Offset, Pos.y));
	float bN	= tex2D (buffer, float2(Pos.x, Pos.y - Offset));
	float bS	= tex2D (buffer, float2(Pos.x, Pos.y + Offset));

	// Center cell
	float bC = tex2D (buffer, Pos);

	// Compute offset lookup index by adding neighbors.
	// The strange offsets ensure a unique index for each possible configuration
	float index = 3 * bN + bE + 5 * bS + 7 * bW + 17 * bC;

	// Get scale and offset = (uScale, uOffset, vScale, vOffset)
	//float integerConvertFactor = 128;
	return tex2D (OffsetTable, float2 (index / 34, 0)) ;
}

//----------------------------------------------------------------------------
float4 PSArbitraryVelocityBoundary (float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);	

	// Get scale and offset = (uScale, uOffset, vScale, vOffset)
	float4 scaleoffset = tex2D (VelocityOffsets, Pos);

	float vertical = scaleoffset.y / FluidSize;
	float horizontal = scaleoffset.w / FluidSize;

	float4 uNew;
	uNew.x = scaleoffset.x * tex2D (buffer, Pos + float2 (0, vertical)).x;
	uNew.y = scaleoffset.z * tex2D (buffer, Pos + float2 (horizontal, 0)).y;
	uNew.zw = 0;

	return uNew;
}

//----------------------------------------------------------------------------
float4 PSArbitraryDensityBoundary (float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);	

	// Invert Obstacles color
	float4 obstacles = 1.0 - tex2D (Boundaries, Pos);

	// Remove Density inside obstacles
	float4 color = tex2D (buffer, Pos) * obstacles;

	return color;
}

//----------------------------------------------------------------------------
float4 PSArbitraryPressureBoundary (float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);	

	// Get the two neighboring pressure offsets
	// They will be the same if this is N, E, W, or S, different if NE, SE, etc.
	float4 offset = tex2D (PressureOffsets, Pos) / FluidSize;

	return 0.5f * (tex2D (buffer, Pos + offset.xy) + tex2D (buffer, Pos + offset.zw));
}

//----------------------------------------------------------------------------
float4 PSFinal(float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - float2(.5f/FluidSize, .5f/FluidSize);	
	
	//return DisplayVector (buffer, TexCoords, 0.5, 0.5);
	//return QuadLerp(buffer, TexCoords - float2(.5f/FluidSize, .5f/FluidSize));
	float4 output = QuadLerp(buffer, Pos);
	//float4 output = DisplayVector(buffer, Pos, 0.5, 0.5);
	
	output.w *= max (output.x, max (output.y, output.z));
	//output.w = 1;

	return output;
}







//----------------------------------------------------------------------------
// Techniques and passes
Technique VelocityColorize
{
	pass VelocityColorize
	{
		PixelShader = compile ps_2_0 PSVelocityColorize();
	}
}

technique DoAddSources
{
	pass DoAddSources
	{
		PixelShader = compile ps_2_0 PSAddSources();
	}
	//pass SetBounds
	//{
	//	PixelShader = compile ps_2_0 PSSetBoundsDouble();
	//}
}

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

technique DoDivergence
{
	pass DoDivergence
	{
		PixelShader = compile ps_2_0 PSDivergence();
	}
}

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

Technique Subtract
{
	pass Subtract
	{
		PixelShader = compile ps_2_0 PSSubtract();
	}
}

Technique DoVorticity
{
	pass DoVorticity
	{
		PixelShader = compile ps_2_0 PSVorticity();
	}
}

Technique DoVorticityForce
{
	pass DoVorticityForce
	{
		PixelShader = compile ps_2_0 PSVorticityForce();
	}
}

Technique ShapeObstacles
{
	pass ShapeObstacles
	{
		PixelShader = compile ps_2_0 PSShapeObstacles();
	}
}

Technique UpdateOffsets
{
	pass UpdateOffsets
	{
		PixelShader = compile ps_2_0 PSUpdateOffsets();
	}
}

Technique ArbitraryVelocityBoundary
{
	pass ArbitraryVelocityBoundary
	{
		PixelShader = compile ps_2_0 PSArbitraryVelocityBoundary();
	}
}

Technique ArbitraryDensityBoundary
{
	pass ArbitraryDensityBoundary
	{
		PixelShader = compile ps_2_0 PSArbitraryDensityBoundary();
	}
}

Technique ArbitraryPressureBoundary
{
	pass ArbitraryPressureBoundary
	{
		PixelShader = compile ps_2_0 PSArbitraryPressureBoundary();
	}
}

Technique Final
{
	pass Final
	{
		PixelShader = compile ps_2_0 PSFinal();
	}
}