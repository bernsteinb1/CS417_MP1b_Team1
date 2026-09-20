// Hidden-message shader: invisible unless the UV flashlight's beam is on it.
// Use a texture with a WHITE letter on a transparent (or black) background.
Shader "Custom/UVReveal"
{
    Properties
    {
        _BaseMap ("Letter Texture", 2D) = "white" {}
        _Color ("Glow Color", Color) = (0.75, 0.35, 1, 1)
        _Intensity ("Glow Intensity", Float) = 2
        _EdgeSoftness ("Beam Edge Softness", Range(0.001, 0.2)) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _Color;
                float _Intensity;
                float _EdgeSoftness;
            CBUFFER_END

            // Set every frame by UVFlashlight.cs
            float4 _UVLightPos;   // xyz = position, w = range
            float4 _UVLightDir;   // xyz = direction, w = cos(half spot angle)
            float _UVLightOn;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                float letterMask = tex.a * dot(tex.rgb, float3(0.333, 0.333, 0.333));

                float3 toPoint = IN.positionWS - _UVLightPos.xyz;
                float dist = length(toPoint);
                float cosAngle = dot(toPoint / max(dist, 0.0001), normalize(_UVLightDir.xyz));

                float inCone = smoothstep(_UVLightDir.w, _UVLightDir.w + _EdgeSoftness, cosAngle);
                float inRange = 1.0 - smoothstep(_UVLightPos.w * 0.8, _UVLightPos.w, dist);
                float reveal = inCone * inRange * _UVLightOn;

                return half4(_Color.rgb * _Intensity, letterMask * reveal);
            }
            ENDHLSL
        }
    }
}
