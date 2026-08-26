Shader "Custom/BlastWave"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, .82, .45, .55)
        _EdgeColor("Edge Color", Color) = (1, 1, .9, 1)
        _Progress("Progress", Range(0, 1)) = 0
        _RingWidth("Ring Width", Range(.001, 1)) = .18
        _RingSharpness("Ring Sharpness", Range(.25, 8)) = 2.0
        _InnerFill("Inner Fill", Range(0, 1)) = .22
        _WobbleAmplitude("Wobble Amplitude", Range(0, .5)) = .045
        _WobbleCount("Wobble Count", Float) = 7
        _FadeStart("Fade Start", Range(0, 1)) = .45
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            ZTest LEqual
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

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _EdgeColor;
                float _Progress;
                float _RingWidth;
                float _RingSharpness;
                float _InnerFill;
                float _WobbleAmplitude;
                float _WobbleCount;
                float _FadeStart;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 centered = IN.uv * 2 - 1;
                float radius = length(centered);
                float angle = atan2(centered.y, centered.x);

                float wobble = 1 + _WobbleAmplitude * (sin(angle * _WobbleCount) * .6 + sin(angle * _WobbleCount * 2.3 + 1.7) * .4);
                float front = _Progress * wobble;

                float ring = 1 - smoothstep(0, _RingWidth, abs(radius - front));
                ring = pow(ring, _RingSharpness);

                float inner = (1 - smoothstep(front - _RingWidth, front, radius)) * _InnerFill * (1 - _Progress);
                float fade = 1 - smoothstep(_FadeStart, 1, _Progress);
                float bounds = 1 - smoothstep(.94, 1, radius);

                half4 color = lerp(_BaseColor, _EdgeColor, ring);
                color.a = saturate(ring * _EdgeColor.a + inner * _BaseColor.a) * fade * bounds;

                return color;
            }
            ENDHLSL
        }
    }
}
