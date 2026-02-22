Shader "Custom/PassthroughOcclusion"
{
    SubShader
    {
        Tags { "Queue"="Geometry-1" "RenderType"="Opaque" }
        ColorMask 0
        ZWrite On

        Pass {}
    }
}