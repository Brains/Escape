float Size;
float2 Shift;
float4 Impulse;
float Fraction;

sampler2D Current : register(s0);
sampler2D NewVelocities;
sampler2D NewDensities;
sampler2D Velocity;
sampler2D Density;

//----------------------------------------------------------------------------
// Struct for when we use two render targets
struct DoubleOutput
{
	float4 Vel : COLOR0;
	float4 Den : COLOR1;
};










//----------------------------------------------------------------------------
// Adds the sources to Density and Velocity
DoubleOutput PSAddSources(float2 TexCoords : TEXCOORD0)
{
	DoubleOutput Output;	
	float2 Pos = TexCoords - Shift;
	
	// Velocity
	float limit = 1.5f;
	float4 source = tex2D (NewVelocities, Pos);
	Output.Vel = max (-limit, min (limit, tex2D (Velocity, Pos) + source));
	
	Output.Vel += float4 (0,  0.005f, 0, 0);
	//Output.Vel = tex2D(Velocity, Pos) + source;
	
	// Density
	Output.Den = tex2D (Density, Pos) + (tex2D (NewDensities, Pos) / Fraction);

	Output.Vel.w = 1.0f;
	Output.Den.w = 1.0f;

	return Output;
}

//----------------------------------------------------------------------------
float4 PSVelocityColorize(float2 TexCoords : TEXCOORD0) : COLOR0
{
	return (tex2D (Current, TexCoords) * -Impulse) * Size;
}








//----------------------------------------------------------------------------
technique VelocityColorize
{
	pass VelocityColorize
	{
		PixelShader = compile ps_2_0 PSVelocityColorize();
	}
}

//----------------------------------------------------------------------------
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

