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
