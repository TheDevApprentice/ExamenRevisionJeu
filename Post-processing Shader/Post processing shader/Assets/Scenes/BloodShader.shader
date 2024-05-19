Shader "Custom/BloodShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BloodTex ("Blood Texture", 2D) = "white" {}
        _BlendFactor ("Blend Factor", Range(0, 1)) = 1
        _OffsetY ("Offset Y", Range(-1, 1)) = 0
        _Fade ("Fade", Range(0, 1)) = 1
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
            uniform sampler2D _BloodTex;
            uniform float _BlendFactor;
            uniform float _OffsetY;
            uniform float _Fade;
            float4 _MainTex_ST;
            float4 _BloodTex_ST;

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
                o.uv.y += _OffsetY; // Apply vertical offset
                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                half4 color = tex2D(_MainTex, i.uv);
                half4 blood = tex2D(_BloodTex, i.uv);

                // Calculate the intensity of the black color to make black transparent
                float blackIntensity = dot(blood.rgb, float3(0.299, 0.587, 0.114));
                float alpha = (1.0 - blackIntensity) * _BlendFactor * _Fade; // Apply fade

                return lerp(color, blood, alpha);
            }
            ENDCG
        }
    }
}
