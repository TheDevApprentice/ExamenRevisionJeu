# Question 6 - Perlin noise

Vous devez générer des bancs de poissons et les faire bouger en utilisant le Perlin noise.

[Question6_start.unitypackage](https://s3-us-west-2.amazonaws.com/secure.notion-static.com/15b135cb-7f07-4e53-9b88-97042b805520/Question6_start.unitypackage)

1. Dans `FishGenerator`.
    1. Complétez la méthode `GenerateFishes` pour générer des poissons en utilisant le Perlin noise.
        1. Utilisez `NoiseMapGenerator`.
        2. Assurez-vous que `DeleteFishes` fonctionne.
        3. Modifiez les paramètres dans l’éditeur pour obtenir plusieurs bancs de poissons en créant des objets avec le prefab `Fish`.
    2. Dans `MovementGenerator`.
        1. Complétez la méthode `Update`.
            1. Utilisez `NoiseMapGenerator`.
            2. Utilisez le RigidBody2D pour appliquer une force.
            3. Méthodes/opérateurs potentiellement utiles : `Math.Abs`, `Math.Round`, `%`.
            4. Toutes les 5 secondes, change la direction des bancs de poissons.

# Question 6 - Perlin noise - Resultat

```c#
using UnityEngine;

public class Fish : MonoBehaviour {}


```

```c#
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

```

```c#
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FishGenerator))]
public class FishGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FishGenerator fishGen = (FishGenerator)target;

        if (DrawDefaultInspector())
        {
            if (fishGen.AutoUpdate)
            {
                fishGen.GenerateFishes();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            fishGen.GenerateFishes();
        }
    }
}


```

```c#

using UnityEngine;

public class MovementGenerator : MonoBehaviour
{
    [SerializeField] private FishGenerator fishGenerator;

    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private NoiseSettings noiseSettings;
    [SerializeField] private bool autoUpdate;
    [SerializeField] private int seed;
    [SerializeField] private Vector2 offset;

    [SerializeField] private float force = 3f;

    [SerializeField] private float timeBeforeUpdate = 5f;
    private float remainingTimeBeforeUpdate = 0;

    private Vector2[,] movementDirections;

    private void Awake()
    {
        // Initialisation des directions de mouvement avec des valeurs aléatoires
        GenerateMovementDirections();
    }

    private void FixedUpdate()
    {
        foreach (Fish fish in fishGenerator.Fishes)
        {
            // Applique une force pour le mouvement
            ApplyRandomForce(fish);
        }

        remainingTimeBeforeUpdate -= Time.fixedDeltaTime;
        if (remainingTimeBeforeUpdate <= float.Epsilon)
        {
            // Change la direction des bancs de poissons
            GenerateMovementDirections();
            remainingTimeBeforeUpdate = timeBeforeUpdate;
        }
    }

    private void GenerateMovementDirections()
    {
        movementDirections = new Vector2[mapSize.x, mapSize.y];

        // Génère une carte de bruit en utilisant NoiseMapGenerator
        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(mapSize, seed, noiseSettings, offset);

        // Affecte des directions de mouvement aléatoires basées sur la carte de bruit
        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                movementDirections[x, y] = new Vector2(noiseMap[x, y] * 2f - 1f, noiseMap[x, y] * 2f - 1f);
            }
        }
    }

    private void ApplyRandomForce(Fish fish)
    {
        // Obtient la position du poisson dans la grille
        int x = Mathf.RoundToInt(fish.transform.position.x);
        int y = Mathf.RoundToInt(fish.transform.position.z);

        // Assure que la position reste à l'intérieur des limites de la carte
        x = Mathf.Clamp(x, 0, mapSize.x - 1);
        y = Mathf.Clamp(y, 0, mapSize.y - 1);

        // Obtient la direction de mouvement à cette position
        Vector2 movementDirection = movementDirections[x, y];

        // Applique une force aléatoire basée sur la direction de mouvement
        fish.GetComponent<Rigidbody2D>().AddForce(movementDirection.normalized * force);
    }

    private void OnValidate()
    {
        if (mapSize.y < 1) { mapSize.y = 1; }
        if (mapSize.x < 1) { mapSize.x = 1; }
        if (noiseSettings.lacunarity < 1) { noiseSettings.lacunarity = 1; }
        if (noiseSettings.octaves < 0) { noiseSettings.octaves = 0; }
    }
}


```

```c#
using UnityEngine;

public class NoiseMapGenerator
{
    public static float[,] GenerateNoiseMap(Vector2Int mapSize, int seed, NoiseSettings settings, Vector2 offset)
    {
        if (settings.scale <= 0) { settings.scale = 0.0001f; }
        float[,] noiseMap = new float[mapSize.x, mapSize.y];

        Vector2[] octaveOffsets = GenerateOctaveOffsets(seed, settings, offset);

        // Makes the zoom centered.
        // Only useful because we can change the zoom on the fly
        Vector2 halfMapSize = new Vector2(mapSize.x / 2f, mapSize.y / 2f);

        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                noiseMap[x, y] = GenerateNoiseValueAtCoord(
                    new Vector2(x - halfMapSize.x, y - halfMapSize.y),
                    settings,
                    octaveOffsets
                );
            }
        }

        return noiseMap;
    }

    private static Vector2[] GenerateOctaveOffsets(int seed, NoiseSettings settings, Vector2 offset)
    {
        System.Random random = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        for (int i = 0; i < settings.octaves; ++i)
        {
            octaveOffsets[i] = new Vector2(
                random.Next(-100000, 100000) + offset.x,
                random.Next(-100000, 100000) + offset.y
            );
        }

        return octaveOffsets;
    }

    private static float GenerateNoiseValueAtCoord(Vector2 coord, NoiseSettings settings, Vector2[] octaveOffsets)
    {
        // Values for the first octave
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;
        float amplitudeSum = 0;

        for (int i = 0; i < settings.octaves; ++i)
        {
            // Coord to get from the Perlin noise
            Vector2 sample = new Vector2(
                coord.x / settings.scale * frequency + octaveOffsets[i].x * frequency,
                coord.y / settings.scale * frequency + octaveOffsets[i].y * frequency
            );

            // Get the value from the Perlin noise
            float perlinValue = Mathf.PerlinNoise(sample.x, sample.y);
            // Apply the current amplitude to the Perlin value
            noiseHeight += perlinValue * amplitude;

            // Keep track of the total amplitude to be able to normalize the result
            amplitudeSum += amplitude;

            // Calculate the amplitude and frequency for the next octave
            amplitude *= settings.persistence;
            frequency *= settings.lacunarity;
        }

        // Normalize result in the 0 to 1 range
        return noiseHeight / amplitudeSum;
    }
}


```


```c#
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


```
