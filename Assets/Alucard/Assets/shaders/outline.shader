Shader "Universal Render Pipeline/2D/SpriteOuterOutlineSmooth"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Range(1, 10)) = 1
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
            };

            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _OutlineColor;
                float _OutlineSize;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color * _Color;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float4 c = _MainTex.Sample(sampler_MainTex, input.uv) * input.color;
                
                float2 texelSize = _MainTex_TexelSize.xy * _OutlineSize;
                
                float alphaUp = _MainTex.Sample(sampler_MainTex, input.uv + float2(0, texelSize.y)).a;
                float alphaDown = _MainTex.Sample(sampler_MainTex, input.uv - float2(0, texelSize.y)).a;
                float alphaRight = _MainTex.Sample(sampler_MainTex, input.uv + float2(texelSize.x, 0)).a;
                float alphaLeft = _MainTex.Sample(sampler_MainTex, input.uv - float2(texelSize.x, 0)).a;

                float dOffset = 0.7071 * _OutlineSize;
                float2 dTexel = _MainTex_TexelSize.xy * dOffset;

                float alphaUpRight = _MainTex.Sample(sampler_MainTex, input.uv + float2(dTexel.x, dTexel.y)).a;
                float alphaUpLeft = _MainTex.Sample(sampler_MainTex, input.uv + float2(-dTexel.x, dTexel.y)).a;
                float alphaDownRight = _MainTex.Sample(sampler_MainTex, input.uv + float2(dTexel.x, -dTexel.y)).a;
                float alphaDownLeft = _MainTex.Sample(sampler_MainTex, input.uv + float2(-dTexel.x, -dTexel.y)).a;

                float maxAlpha = max(alphaUp, max(alphaDown, max(alphaLeft, max(alphaRight, max(alphaUpRight, max(alphaUpLeft, max(alphaDownRight, alphaDownLeft)))))));

                if (c.a == 0 && maxAlpha > 0)
                {
                    return _OutlineColor;
                }

                c.rgb *= c.a;
                return c;
            }
            ENDHLSL
        }
    }
}