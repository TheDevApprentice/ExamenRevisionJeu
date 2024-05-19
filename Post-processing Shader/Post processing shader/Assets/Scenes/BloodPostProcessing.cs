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
