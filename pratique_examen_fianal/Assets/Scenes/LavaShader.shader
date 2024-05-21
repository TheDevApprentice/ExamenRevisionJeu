Shader "Unlit/LavaShader"
{
    Properties
    {
        
        _LavaTexture ("Texture de lave", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

         
            sampler2D _LavaTexture;
            float4 _LavaTexture_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _LavaTexture);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv += _Time.x * 0.1f + _Time.y * 0.1f;
                fixed4 col = tex2D(_LavaTexture, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
