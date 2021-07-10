Shader "Hidden/CustomRenderTexture/Trail"
{
    Properties
    {
        _NewFrame ("New Frame to add", 2D) = "white" {}
        _TrailForce("Trail Force", Range(.9, 1)) = .9
    }
    CGINCLUDE
    #include "UnityCustomRenderTexture.cginc"
    sampler2D _NewFrame;
    float _TrailForce;

    float4 frag(v2f_customrendertexture i) : SV_Target
    {
        float2 uv           = i.globalTexcoord;
        float4 current      = tex2D(_SelfTexture2D, uv) * _TrailForce;
        float4 newFrame     = tex2D(_NewFrame, uv);

        float4 trail        = newFrame;
        if(length(trail) > 0.0){
            current += trail;
        }

        
        return clamp(current, -1, 1);
    }

    ENDCG

    SubShader
    {
         Cull Off ZWrite Off ZTest Always
        Pass
        {
            Name "Update"
            CGPROGRAM
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            ENDCG
        }
    }
}
