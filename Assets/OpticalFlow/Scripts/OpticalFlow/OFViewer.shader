Shader "Hidden/OFViewer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OF("Optical Flow", 2D) = "white" {}
        _ColsRows("Cols Rows", Vector) = (100, 100, 0, 0)
        _Opacity("Cam Opacity", Range(0, 1)) = 0.5
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
            float4 _MainTex_TexelSize;
            sampler2D _OF;
            float2 _ColsRows;
            float _Opacity;

            //From Inigo Quilez
            float sdSegment( in float2 p, in float2 a, in float2 b )
            {
                float2 pa = p-a, ba = b-a;
                float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
                return length( pa - ba*h );
            }

            float sdCircle(float2 p, float r )
            {
                return length(p) - r;
            }

            float2 opUnite(float d1, float d2){
               return min(d1,d2);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 src          = tex2D(_MainTex, i.uv);
                fixed4 of           = abs(tex2D(_OF, i.uv));

                float aspect        = _MainTex_TexelSize.z / _MainTex_TexelSize.w;

                float2 colsrows     = _ColsRows.xy;
                colsrows.y          /= aspect; 

                float2 nuv          = i.uv * colsrows;
                float2 fuv          = frac(nuv);
                float2 iuv          = floor(nuv);
                float2 offset       = float2(1.0, 1.0) / colsrows;

                float2 uvsampleOF   = (iuv + offset * 1.0) / colsrows;

                float2 sampleOF     = tex2D(_OF, uvsampleOF).rg;

                fuv             = fuv * 2.0 - 1.0;
                float direction = 0.0;
                if(length(sampleOF) > 0.1){
                    //top line
                    direction       = sdSegment(fuv, float2(0, 0), sampleOF * 5.0);
                    float dirPoint  = sdCircle(fuv, 0.1);
                    direction       = opUnite(direction, dirPoint);
                    direction       = 1.0 - (step(0.1, direction));
                }
                
                return float4(src.rgb * _Opacity + smoothstep(0.1, 1.0, abs(of)) * (1.0 - _Opacity) + direction, 1);
            }
            ENDCG
        }
    }
}
