# Question 2 - Post-processing Shader

```c#
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


```
using UnityEngine;

[ExecuteInEditMode]
public class AimPostProcessing : MonoBehaviour
{
    public Shader shader;
    private Material material;

    public Texture2D aimTexture;
    [Range(0, 1)]
    public float blendFactor = 1.0f;

    void Start()
    {
        if (shader == null)
        {
            Debug.LogError("Shader not set.");
            enabled = false;
            return;
        }

        if (!shader.isSupported)
        {
            enabled = false;
            return;
        }

        material = new Material(shader);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material == null)
        {
            Graphics.Blit(src, dest);
            return;
        }

        material.SetTexture("_AimTexture", aimTexture);
        material.SetFloat("_BlendFactor", blendFactor);
        Graphics.Blit(src, dest, material);
    }

    void OnDestroy()
    {
        if (material != null)
        {
            DestroyImmediate(material);
        }
    }
}

```c#
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


```

```c#
using UnityEngine;

[ExecuteInEditMode]
public class BloodPostProcessing : MonoBehaviour
{
    public Shader bloodShader; // Référence au shader de sang
    private Material bloodMaterial; // Matériel pour le shader de sang

    public Texture2D bloodTexture; // Texture de sang
    [Range(0, 1)]
    public float blendFactor = 1.0f; // Facteur de mélange initial pour le shader de sang

    private bool isBloodActive = false; // Indicateur pour savoir si le shader de sang est actif
    private float fadeDuration = 1.0f; // Durée du fade-out en secondes
    private float elapsedTime = 0.0f; // Temps écoulé depuis le début de l'effet

    void Start()
    {
        if (bloodShader == null)
        {
            Debug.LogError("Shader not set.");
            enabled = false;
            return;
        }

        if (!bloodShader.isSupported)
        {
            enabled = false;
            return;
        }

        bloodMaterial = new Material(bloodShader);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartBloodEffect();
        }

        if (isBloodActive)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration); // Interpolation temporelle
            blendFactor = Mathf.Lerp(1.0f, 0.0f, t); // Interpolation linéaire de blendFactor de 1 à 0

            if (elapsedTime >= fadeDuration)
            {
                isBloodActive = false; // Désactive l'effet de sang après le fade-out
            }
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (bloodMaterial == null || !isBloodActive)
        {
            Graphics.Blit(src, dest);
            return;
        }

        bloodMaterial.SetTexture("_BloodTex", bloodTexture);
        bloodMaterial.SetFloat("_BlendFactor", blendFactor);
        Graphics.Blit(src, dest, bloodMaterial);
    }

    void StartBloodEffect()
    {
        isBloodActive = true;
        elapsedTime = 0.0f;
        blendFactor = 1.0f; // Réinitialise blendFactor au début de l'effet
    }

    void OnDestroy()
    {
        if (bloodMaterial != null)
        {
            DestroyImmediate(bloodMaterial);
        }
    }
}


```
# Question 2 - Post-processing Shader - Resultat

```c#


```