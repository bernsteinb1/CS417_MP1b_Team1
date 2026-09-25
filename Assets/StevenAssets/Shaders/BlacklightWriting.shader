Shader "Steven/BlacklightWriting"
{
    Properties
    {
        _MainTex ("Writing", 2D) = "white" {}
        _Tint ("Tint", Color) = (0.45, 0.9, 1.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
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
                float3 positionWS : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Tint;
            CBUFFER_END

            float4 _StevenBlacklightPos;
            float4 _StevenBlacklightDir;
            float _StevenBlacklightRange;
            float _StevenBlacklightConeDot;

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs pos = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = pos.positionCS;
                output.positionWS = pos.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 toFrag = input.positionWS - _StevenBlacklightPos.xyz;
                float dist = length(toFrag);
                float3 dir = dist > 0.0001 ? toFrag / dist : float3(0,0,1);
                float cone = dot(normalize(_StevenBlacklightDir.xyz), dir);
                float visible = step(dist, _StevenBlacklightRange) * step(_StevenBlacklightConeDot, cone);

                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half alpha = tex.a * visible;
                clip(alpha - 0.02h);
                return half4(_Tint.rgb * tex.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
