sampler uImage0 : register(s0);
sampler uImage1 : register(s1); // Automatically Images/Misc/Perlin via Force Shader testing option
sampler uImage2 : register(s2); // Automatically Images/Misc/noise via Force Shader testing option
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 mid = float2(0.5, 0.5);
    float4 orange = float4(1, 0.64, 0, 1);
    
    float lerpAmt = distance(coords, mid) * uIntensity;
    
    color.rgba += orange * lerpAmt * 10 * 0.4;
    return color;
}

technique Technique1
{
    pass OrangeVignette
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}