Shader "Custom/MovingDotLine"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _FlowDirection("Flow Direction", Vector) = (1, 0, 0, 0)
        [Float] _Speed("Speed", Float) = 1.0
        [Float] _DashCount("Dash Count", Float) = 100.0
        _DashFill("Dash Fill", Range(0, 1)) = 0.5
        _EdgeSoftness("Edge Softness", Range(0, 1)) = 0.02
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _FlowDirection;
                float _Speed;
                float _DashCount;
                float _DashFill;
                float _EdgeSoftness;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 flow = _FlowDirection.xy;
                float2 along = dot(flow, flow) > 1e-6 ? normalize(flow) : float2(1, 0);

                float travel = dot(IN.uv, along) * _DashCount - _Time.y * _Speed;
                float dashCenter = abs(frac(travel) - .5) * 2;

                float edge = max(fwidth(travel) * 2, _EdgeSoftness);
                half mask = 1 - smoothstep(_DashFill - edge, _DashFill + edge, dashCenter);

                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                color.a *= mask;

                return color;
            }
            ENDHLSL
        }
    }
}
