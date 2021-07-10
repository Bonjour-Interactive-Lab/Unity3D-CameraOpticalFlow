Shader "Hidden/GaussianBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        CGINCLUDE
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

           
        ENDCG

        Pass
        {
            Name "LOW"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define TYPE 0

            #include "UnityCG.cginc"
            #include "utils/GaussianBlur.hlsl"

            fixed4 frag (v2f i) : SV_Target
            {
                return Gaussian(_MainTex, i.uv);
            }
            ENDCG
        }

        Pass
        {
            Name "MEDIUM"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define TYPE 1

            #include "UnityCG.cginc"
            #include "utils/GaussianBlur.hlsl"

            fixed4 frag (v2f i) : SV_Target
            {
                return Gaussian(_MainTex, i.uv);
            }
            ENDCG
        }

        Pass
        {
            Name "HIGH"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define TYPE 2

            #include "UnityCG.cginc"
            #include "utils/GaussianBlur.hlsl"

            fixed4 frag (v2f i) : SV_Target
            {
                return Gaussian(_MainTex, i.uv);
            }
            ENDCG
        }

        Pass
        {
            Name "ULTRA"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define TYPE 3

            #include "UnityCG.cginc"
            #include "utils/GaussianBlur.hlsl"

            fixed4 frag (v2f i) : SV_Target
            {
                return Gaussian(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
