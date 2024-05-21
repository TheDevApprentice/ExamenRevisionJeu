Shader "Unlit/LavaShader"
{
    Properties
    {
        _LavaTexture ("Texture de lave", 2D) = "white" {}
        _RockTexture ("Texture de roche", 2D) = "white" {}
        _DispFactor ("Facteur de deplacement", Range(0, 10)) = 1
        _DispTexture ("Texture de deplacement", 2D) = "white" {}
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _LavaTexture;
            sampler2D _RockTexture;
            float4 _LavaTexture_ST;
            float4 _RockTexture_ST;
            float _DispFactor;
            sampler2D _DispTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float d = tex2Dlod(_DispTexture, float4(v.uv.xy,0,0)).r * _DispFactor;
                o.vertex.xyz += v.normal * d; 
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
               
                float2 lavaUV = i.uv + _Time.x * 0.1f + _Time.y * 0.1f;
                float2 rockUV = i.uv;
                
                
                fixed4 lavaColor = tex2D(_LavaTexture, lavaUV);
                fixed4 rockColor = tex2D(_RockTexture, rockUV);
               
                fixed4 finalColor = lavaColor - rockColor * 2;
                
                return finalColor;
            }
            ENDCG
        }
    }
}
