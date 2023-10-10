Shader "Unlit/toonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowStrength("ShadowStrength",Float) = 1
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            
			
			
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                
                float4 vertex : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ShadowStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.positionWS = TransformObjectToWorld(v.vertex);
                o.normalWS   = TransformObjectToWorldNormal(v.normal);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // sample the texture
                DirectionalLightData dirLight;
                float3 lightDir = -dirLight.forward;
                float3 normalWs = SafeNormalize(i.normalWS);
                float diffuseLightFalloff = saturate( dot(lightDir,normalWs) );
                diffuseLightFalloff = step(diffuseLightFalloff,_ShadowStrength);

                half4 diffuseLight = half4(diffuseLightFalloff * dirLight.color,1);
                return diffuseLight;
            }
            ENDHLSL
        }
    }
}
