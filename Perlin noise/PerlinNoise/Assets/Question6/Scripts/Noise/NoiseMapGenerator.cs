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
