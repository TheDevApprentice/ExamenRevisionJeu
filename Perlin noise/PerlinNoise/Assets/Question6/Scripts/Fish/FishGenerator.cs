using System.Collections.Generic;
using UnityEngine;

public class FishGenerator : MonoBehaviour
{
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private NoiseSettings noiseSettings;
    [SerializeField] private bool autoUpdate;
    [SerializeField] private int seed;
    [SerializeField] private Vector2 offset;

    [SerializeField] private int maxNumberOfFishes = 100;
    [SerializeField] private Fish fishPrefab;

    private List<Fish> fishes = new List<Fish>();
    public List<Fish> Fishes { get => fishes; private set => fishes = value; }
    public bool AutoUpdate { get => autoUpdate; }

    private void Awake()
    {
        fishes.Clear();
        foreach (var fish in FindObjectsOfType<Fish>())
        {
            fishes.Add(fish);
        }
    }

    public void GenerateFishes()
    {
        // Supprime tous les poissons existants
        DeleteFishes();

        // Génère une carte de bruit en utilisant NoiseMapGenerator
        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(mapSize, seed, noiseSettings, offset);

        // Pour chaque position sur la carte de bruit, générez un poisson si la valeur de bruit est supérieure à un seuil donné
        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                // Exemple de seuil à ajuster selon vos besoins
                float threshold = 0.5f;

                // Si la valeur de bruit est supérieure au seuil, génère un poisson à cette position
                if (noiseMap[x, y] > threshold && Fishes.Count < maxNumberOfFishes)
                {
                    Vector3 spawnPosition = new Vector3(x, 0, y); // Ajustez la hauteur si nécessaire
                    Fish newFish = Instantiate(fishPrefab, spawnPosition, Quaternion.identity);
                    Fishes.Add(newFish);
                }
            }
        }
    }
    public void DeleteFishes()
    {
        foreach (Fish fish in fishes)
        {
            if (fish == null) { continue; }
            DestroyImmediate(fish.gameObject);
        }
        fishes.Clear();
    }

    private void OnValidate()
    {
        if (mapSize.y < 1) { mapSize.y = 1; }
        if (mapSize.x < 1) { mapSize.x = 1; }
        if (noiseSettings.lacunarity < 1) { noiseSettings.lacunarity = 1; }
        if (noiseSettings.octaves < 0) { noiseSettings.octaves = 0; }
    }
}