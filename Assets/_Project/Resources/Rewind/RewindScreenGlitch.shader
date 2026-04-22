Shader "Custom/RewindScreenGlitch"
{
    Properties
    {
        [HideInInspector] _Intensity ("Intensity", Range(0, 1)) = 0
        [HideInInspector] _RewindIntensity ("Rewind Intensity", Range(0, 1)) = 0
        [HideInInspector] _TintColor ("Tint Color", Color) = (0.92, 0.96, 1.08, 1)
        [HideInInspector] _Desaturate ("Desaturate", Range(0, 1)) = 0.28
        [HideInInspector] _Distortion ("Distortion Pixels", Float) = 1.4
        [HideInInspector] _RgbSplit ("RGB Split Pixels", Float) = 1
        [HideInInspector] _Jitter ("Horizontal Jitter Pixels", Float) = 1.75
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }

        Cull Off
        ZWrite Off
        Blend One Zero

        Pass
        {
            Name "RewindFullscreenPass"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _Intensity;
            float _RewindIntensity;
            float4 _TintColor;
            float _Desaturate;
            float _Distortion;
            float _RgbSplit;
            float _Jitter;

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float2 SnapUvToPixels(float2 uv)
            {
                float2 screenSize = _BlitTexture_TexelSize.zw;
                return (floor(uv * screenSize) + 0.5) / screenSize;
            }

            half3 SampleScene(float2 uv)
            {
                return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, SnapUvToPixels(uv)).rgb;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float intensity = saturate(max(_Intensity, _RewindIntensity));
                float2 uv = input.texcoord.xy;
                float2 texelSize = _BlitTexture_TexelSize.xy;
                float2 screenSize = _BlitTexture_TexelSize.zw;

                if (intensity <= 0.0001)
                {
                    return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv);
                }

                float timeStep = floor(_Time.y * 24.0);
                float rowIndex = floor(uv.y * screenSize.y);
                float bandIndex = floor(rowIndex * 0.25);

                float jitterNoise = Hash21(float2(bandIndex, timeStep));
                float jitterPixels = round((jitterNoise - 0.5) * 2.0 * _Jitter * intensity);

                float distortNoise = Hash21(float2(floor(uv.x * 32.0) + timeStep * 0.71, bandIndex + 3.17)) - 0.5;
                float distortPixels = round(distortNoise * 2.0 * _Distortion * intensity);

                float2 baseUv = SnapUvToPixels(uv + float2((jitterPixels + distortPixels) * texelSize.x, 0.0));

                half3 original = SampleScene(baseUv);
                half3 glitched = original;

                float splitPixels = round(_RgbSplit * intensity);
                if (splitPixels > 0.0)
                {
                    float2 split = float2(splitPixels * texelSize.x, 0.0);
                    glitched.r = SampleScene(baseUv + split).r;
                    glitched.b = SampleScene(baseUv - split).b;
                }

                half splitBlend = saturate(0.65h * intensity);
                half3 color = lerp(original, glitched, splitBlend);

                half luminance = dot(color, half3(0.299h, 0.587h, 0.114h));
                color = lerp(color, luminance.xxx, saturate(_Desaturate) * intensity);

                half tintStrength = 0.16h * intensity;
                color = lerp(color, saturate(color * _TintColor.rgb), tintStrength);

                return half4(color, 1.0h);
            }
            ENDHLSL
        }
    }
}
