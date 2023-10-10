Shader "Unlit/Ouline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineWidth("Outline Width",Range(0,0.03)) = 0
        _OutlineColor("Outline Color",Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 normal : TEXCOORD1;
                float4 vertex : SV_POSITION;
                
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _OutlineWidth;
            fixed4 _OutlineColor;
            v2f vert (appdata v)
            {
                v.vertex += v.normal * _OutlineWidth;
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}
