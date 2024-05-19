Shader "Custom/ShaderAim"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _AimTexture ("Aim Texture", 2D) = "white" {}
        _BlendFactor ("Blend Factor", Range(0, 1)) = 1
    }
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform sampler2D _AimTexture;
            uniform float _BlendFactor;
            float4 _MainTex_ST;
            float4 _AimTexture_ST;

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert_img(float4 pos : POSITION, float4 uv : TEXCOORD0)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(pos);
                o.uv = TRANSFORM_TEX(uv, _MainTex);
                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                half4 color = tex2D(_MainTex, i.uv);
                half4 aim = tex2D(_AimTexture, i.uv);

                float blackIntensity = 1.0 * dot(aim.rgb, float3(0.299, 0.587, 0.114));
                float alpha = blackIntensity * _BlendFactor;

                return lerp(color, aim, alpha);
            }
            ENDCG
        }
    }
}
