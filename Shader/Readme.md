# Question 1 - Shader

[Question1_start.unitypackage](https://s3-us-west-2.amazonaws.com/secure.notion-static.com/c38e3bcb-d119-4702-b4a7-99de7c81cbf9/Question1_start.unitypackage)

1. Créez un plan dans la scène.
2. Créez un `Unlit Shader` nommé `LavaShader`.
3. Retirez tout le code concernant le `fog`.

```c#
Shader "Unlit/LavaShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
```

4. Ajoutez/modifiez le nécessaire pour que votre shader ait les [propriétés](https://docs.unity3d.com/Manual/SL-Properties.html) suivantes :
    1. Texture nommée `_LavaTexture`.
        1. Le texte affiché dans l’Inspector doit être : `Texture de lave`.
        2. Doit être utilisée comme couleur principale.
        3. Doit scroll (défiler) en X et Y.
    2. Texture nommée `_RockTexture`.
        1. Le texte affiché dans l’Inspector doit être : `Texture de roche`.
        2. Utilisez cette texture en combinaison avec `_LavaTexture` pour obtenir un résultat similaire à l’image ci-dessous.
    3. Float nommée `_DispFactor`.
        1. Le texte affiché dans l’Inspector doit être : `Facteur de deplacement`.
        2. Doit avoir une valeur entre 0 et 10.
    4. Texture nommée `_DispTexture`.
        1. Le texte affiché dans l’Inspector doit être : `Texture de deplacement`.
        2. Faites le nécessaire pour déplacer les sommets à partir de cette texture.
            1. Les sommets doivent se déplacer le long de leur [normale](https://docs.unity3d.com/Manual/SL-VertexProgramInputs.html).
            2. Les sommets se déplacent proportionnellement à la valeur de la couleur et à la valeur de `_DispFactor`.
```c#
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

                // Blend lava and rock textures
                fixed4 col = lerp(rockCol, lavaCol, lavaCol.a);

                return col;
            }
            ENDCG
        }
    }
}

```
5. Créez un matériel utilisant votre shader.
6. Applique le matériel à votre plan.

# Question 1 - Shader - Resultat

```c#
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
                fixed4 col = (0.1 + lavaCol) - ( rockCol) * mask;

                return col;
            }
            ENDCG
        }
    }
}

```