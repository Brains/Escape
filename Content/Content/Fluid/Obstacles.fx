float Size;
float2 Shift;

sampler2D Current : register(s0);
sampler2D Boundaries;
sampler2D OffsetTable;
sampler2D VelocityOffsets;
sampler2D PressureOffsets;





//----------------------------------------------------------------------------
float4 PSShapeObstacles (float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - Shift;	
	float4 color = tex2D (Current, Pos);

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
	float2 Pos = TexCoords - Shift;	
	float Offset = 1.0f/Size;

    // Get neighboring boundary values (on or off)
	float bW	= tex2D (Current, float2(Pos.x - Offset, Pos.y));
	float bE	= tex2D (Current, float2(Pos.x + Offset, Pos.y));
	float bN	= tex2D (Current, float2(Pos.x, Pos.y - Offset));
	float bS	= tex2D (Current, float2(Pos.x, Pos.y + Offset));

	// Center cell
	float bC = tex2D (Current, Pos);

	// Compute offset lookup index by adding neighbors.
	// The strange offsets ensure a unique index for each possible configuration
	float index = 3 * bN + bE + 5 * bS + 7 * bW + 17 * bC;

	// Get scale and offset = (uScale, uOffset, vScale, vOffset)
	return tex2D (OffsetTable, float2 (index / 34.0f, 0)) ;
}

//----------------------------------------------------------------------------
float4 PSVelocity (float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - Shift;	

	// Get scale and offset = (uScale, uOffset, vScale, vOffset)
	float4 scaleoffset = tex2D (VelocityOffsets, Pos);

	float vertical = scaleoffset.y / Size;
	float horizontal = scaleoffset.w / Size;

	float4 uNew;
	uNew.x = scaleoffset.x * tex2D (Current, Pos + float2 (0, vertical)).x;
	uNew.y = scaleoffset.z * tex2D (Current, Pos + float2 (horizontal, 0)).y;
	uNew.zw = 0;

	return uNew;
}

//----------------------------------------------------------------------------
float4 PSDensity (float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - Shift;	

	// Invert Obstacles color
	float4 obstacles = 1.0 - tex2D (Boundaries, Pos);

	// Remove Density inside obstacles
	float4 color = tex2D (Current, Pos) * obstacles;

	return color;
}
	
//----------------------------------------------------------------------------
float4 PSPressure (float2 TexCoords : TEXCOORD0) : COLOR0
{
	float2 Pos = TexCoords - Shift;	

	// Get the two neighboring pressure offsets
	// They will be the same if this is N, E, W, or S, different if NE, SE, etc.
	float4 offset = tex2D (PressureOffsets, Pos) / Size;

	return 0.5f * (tex2D (Current, Pos + offset.xy) + tex2D (Current, Pos + offset.zw));
}








//----------------------------------------------------------------------------
Technique ShapeObstacles
{
	pass ShapeObstacles
	{
		PixelShader = compile ps_4_0 PSShapeObstacles();
	}
}

//----------------------------------------------------------------------------
Technique UpdateOffsets
{
	pass UpdateOffsets
	{
		PixelShader = compile ps_4_0 PSUpdateOffsets();
	}
}

//----------------------------------------------------------------------------
Technique Velocity
{
	pass Velocity
	{
		PixelShader = compile ps_4_0 PSVelocity();
	}
}

//----------------------------------------------------------------------------
Technique Density
{
	pass Density
	{
		PixelShader = compile ps_4_0 PSDensity();
	}
}

//----------------------------------------------------------------------------
Technique Pressure
{
	pass Pressure
	{
		PixelShader = compile ps_4_0 PSPressure();
	}
}
