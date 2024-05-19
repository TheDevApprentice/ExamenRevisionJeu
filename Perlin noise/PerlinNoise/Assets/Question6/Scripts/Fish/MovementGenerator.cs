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
