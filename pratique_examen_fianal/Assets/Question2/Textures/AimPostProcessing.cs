using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AimPostProcessing : MonoBehaviour {
    private Material _material;
    public Shader shader;
    private void Start()
    {
       Material material = new Material(shader);
        material.GetTexture("Aim");
    }
void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_material == null)
        {
            _material = new Material(shader);
            _material.color = Color.black;
            _material.hideFlags = HideFlags.HideAndDontSave;
        }

        Graphics.Blit(source, destination, _material);
        
    }

    void OnDisable()
    {
        if (_material != null)
        {
            DestroyImmediate(_material);
        }
    }
}