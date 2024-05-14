Shader "Unlit/LavaShader"
{
    Properties
    {
        _LavaTexture ("Texture de lave", 2D) = "white" {}
        _RockTexture ("Texture de roche", 2D) = "white" {}
        _DispFactor ("Facteur de deplacement", Float) = 1.0
        _DispTexture ("Texture de deplacement", 2D) = "white" {}
        _ScrollSpeedX ("Vitesse de defilement X", Float) = 0.1
        _ScrollSpeedY ("Vitesse de defilement Y", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uvDisp : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _LavaTexture;
            sampler2D _RockTexture;
            sampler2D _DispTexture;
            float _DispFactor;
            float _ScrollSpeedX;
            float _ScrollSpeedY;

            v2f vert (appdata v)
            {
                v2f o;

                // Scroll UV coordinates
                float2 scrollUV = v.uv + float2(_ScrollSpeedX, _ScrollSpeedY) * _Time.y;
                o.uv = scrollUV;
                o.uvDisp = v.uv;

                // displacement calculation
                float displacement = tex2Dlod(_DispTexture, float4(o.uvDisp, 0, 0)).r * _DispFactor;
                float3 displacedVertex = v.vertex.xyz + v.normal * displacement;

                o.vertex = UnityObjectToClipPos(float4(displacedVertex, 1.0));

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the lava and rock textures
                fixed4 lavaCol = tex2D(_LavaTexture, i.uv);
                fixed4 rockCol = tex2D(_RockTexture, i.uv);

                // Compute the mask from the rock texture luminance
                float rockLuminance = dot(rockCol.rgb, float3(1, 0.59, 0.11));
                float mask = 4.0 - smoothstep(0.1, 0.1, rockLuminance);

                // Blend lava and rock textures based on the mask
                fixed4 col = (0.7 * lavaCol) - (5*  rockCol + 0.1);
                return col;
            }
            ENDCG
        }
    }
}
