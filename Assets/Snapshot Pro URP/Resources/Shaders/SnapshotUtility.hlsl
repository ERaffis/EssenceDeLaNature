struct appdata
{
	float4 positionOS : Position;
	float2 uv : TEXCOORD0;
};

struct v2f
{
	float4 positionCS : SV_Position;
	float2 uv : TEXCOORD0;
};

v2f vert(appdata v)
{
	v2f o;
	o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
	o.uv = v.uv;
	return o;
}
