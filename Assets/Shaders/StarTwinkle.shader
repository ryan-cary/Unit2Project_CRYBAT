Shader "CRYBAT/StarTwinkle"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        _TwinkleAmount ("Twinkle Amount", Range(0, 1)) = 0.55
        _TwinkleSpeed ("Twinkle Speed", Range(0.2, 3)) = 1.15
        _GlintStrength ("Glint Strength", Range(0, 1)) = 0.7
        _GlowSpread ("Glow Spread", Range(0, 2)) = 1.1
        [HideInInspector] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        [MaterialToggle] _ZWrite ("ZWrite", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite [_ZWrite]

        Pass
        {
            Name "StarTwinkleUnlit"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            struct Attributes
            {
                COMMON_2D_INPUTS
                half4 color : COLOR;
                UNITY_SKINNED_VERTEX_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_OUTPUTS
                half4 color : COLOR;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/2DCommon.hlsl"

            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY SKINNED_SPRITE

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _TwinkleAmount;
                float _TwinkleSpeed;
                float _GlintStrength;
                float _GlowSpread;
            CBUFFER_END

            float4 _MainTex_TexelSize;

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            void StarPulse(float2 texelId, out float breathe, out float glint)
            {
                float h = Hash21(texelId);
                float h2 = Hash21(texelId + 17.13);
                float speed = _TwinkleSpeed * lerp(0.35, 1.45, h2);
                float t = _TimeParameters.x * speed + h * 6.2831853;
                float wave = sin(t);
                float slow = sin(t * 0.37 + h2 * 9.1);
                breathe = saturate(0.5 + 0.5 * (wave * 0.7 + slow * 0.3));
                glint = pow(saturate(wave * 0.5 + 0.5), lerp(7.0, 16.0, h));
            }

            Varyings UnlitVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);

                Varyings o = CommonUnlitVertex(input);
                o.color = input.color * _Color * unity_SpriteColor;
                return o;
            }

            half4 UnlitFragment(Varyings input) : SV_Target
            {
                float2 texelSize = _MainTex_TexelSize.xy;
                float2 texDim = _MainTex_TexelSize.zw;
                if (texDim.x < 2.0)
                {
                    texDim = float2(4096.0, 4096.0);
                    texelSize = 1.0 / texDim;
                }

                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.color;
                float2 centerId = floor(input.uv * texDim);
                float breathe, glint;
                StarPulse(centerId, breathe, glint);

                float lum = max(max(c.r, c.g), c.b);
                float starMask = smoothstep(0.02, 0.12, lum);

                float dimMul = lerp(1.0 - _TwinkleAmount * 0.75, 1.0 + _TwinkleAmount * 0.35, breathe);
                c.rgb *= lerp(1.0, dimMul, starMask);
                c.rgb += c.rgb * glint * _GlintStrength * 1.8 * starMask;

                float glow = 0.0;
                float2 nUV;
                half4 nSample;
                float nLum, nBreathe, nGlint;

                nUV = input.uv + float2(-texelSize.x, 0);
                nSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, nUV);
                nLum = max(max(nSample.r, nSample.g), nSample.b);
                StarPulse(floor(nUV * texDim), nBreathe, nGlint);
                glow += nLum * nGlint * nBreathe;

                nUV = input.uv + float2(texelSize.x, 0);
                nSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, nUV);
                nLum = max(max(nSample.r, nSample.g), nSample.b);
                StarPulse(floor(nUV * texDim), nBreathe, nGlint);
                glow += nLum * nGlint * nBreathe;

                nUV = input.uv + float2(0, -texelSize.y);
                nSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, nUV);
                nLum = max(max(nSample.r, nSample.g), nSample.b);
                StarPulse(floor(nUV * texDim), nBreathe, nGlint);
                glow += nLum * nGlint * nBreathe;

                nUV = input.uv + float2(0, texelSize.y);
                nSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, nUV);
                nLum = max(max(nSample.r, nSample.g), nSample.b);
                StarPulse(floor(nUV * texDim), nBreathe, nGlint);
                glow += nLum * nGlint * nBreathe;

                glow *= _GlowSpread * 0.55;
                c.rgb += glow;
                c.a = max(c.a, saturate(glow * 1.4));

#if defined(DEBUG_DISPLAY)
                SurfaceData2D surfaceData;
                InputData2D inputData;
                half4 debugColor = 0;
                InitializeSurfaceData(c.rgb, c.a, surfaceData);
                InitializeInputData(input.uv, inputData);
                SETUP_DEBUG_TEXTURE_DATA_2D_NO_TS(inputData, input.positionWS, input.positionCS, _MainTex);
                if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                    return debugColor;
#endif
                return c;
            }
            ENDHLSL
        }
    }
}
