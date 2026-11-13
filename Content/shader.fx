#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define NUM_NODES 30

float2 Positions[NUM_NODES];
float4 Colors[NUM_NODES];
float Deviation;
float DeviationMult[NUM_NODES];


struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	output.Position = input.Position;
	output.TexCoord = input.TexCoord;

	return output;
}


float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 c = (float4)0;
	float x = (input.TexCoord.x - .5f) * 2;
	float y = (input.TexCoord.y - .5f) * 2;

	for (int i = 0; i < NUM_NODES; i++)
	{
		float a = Positions[i].x;
		float b = Positions[i].y;
		float t = exp(-pow((x-a) / (Deviation * DeviationMult[i]), 2) - pow((y-b) / (Deviation * DeviationMult[i]), 2));
		c += Colors[i] * t;
	}
	return c;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};