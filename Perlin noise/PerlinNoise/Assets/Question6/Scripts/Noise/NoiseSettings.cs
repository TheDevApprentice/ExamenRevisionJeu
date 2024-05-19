using UnityEngine;

[System.Serializable]
public struct NoiseSettings
{
    public float scale;
    public int octaves;
    [Range(0, 1)]
    public float persistence;
    public float lacunarity;
}
