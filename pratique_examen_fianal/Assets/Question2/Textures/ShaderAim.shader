Shader "Image Effect Shader/ShaderAim"
{
    Properties
    {
        _MainTex ("Base de mon (RGB)", 2D) = "white" {}
        _AimTexture ("Texture de visée", 2D) = "white" {}
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

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _AimTexture;

            v2f vert (appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 aimCol = tex2D(_AimTexture, i.uv);

                col.rgb = col.rgb - aimCol.rgb;
                return col;
            }
            ENDCG
        }
    }
}