using UnityEngine;

/// <summary>
/// Utility manager for terrain operations in the hilly driving simulator.
/// Provides tools for flattening road areas and querying terrain heights.
/// Manual terrain sculpting using Unity Terrain Tools is recommended.
/// </summary>
[RequireComponent(typeof(Terrain))]
public class TerrainManager : MonoBehaviour
{
    [Header("Terrain Reference")]
    [SerializeField] private Terrain terrain;

    [Header("Road Flattening Tool")]
    [SerializeField] private float flattenRadius = 10f;
    [SerializeField] private float flattenBlendDistance = 5f;
    [SerializeField] private AnimationCurve flattenFalloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Procedural Generation (Optional)")]
    [SerializeField] private bool enableProceduralGeneration = false;
    [SerializeField] private int seed = 12345;
    [SerializeField] private float baseScale = 0.01f;
    [SerializeField] private float heightMultiplier = 0.4f;
    [SerializeField] private int octaves = 4;
    [SerializeField] private float persistence = 0.5f;
    [SerializeField] private float lacunarity = 2.0f;

    private TerrainData terrainData;

    private void Awake()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();

        if (terrain != null)
            terrainData = terrain.terrainData;
    }

    /// <summary>
    /// Optional: Generate hilly terrain procedurally using improved Perlin noise.
    /// Recommended: Use Unity's Terrain Tools for manual sculpting instead.
    /// </summary>
    [ContextMenu("Generate Hilly Terrain (Procedural)")]
    public void GenerateHillyTerrain()
    {
        if (!enableProceduralGeneration)
        {
            Debug.LogWarning("Procedural generation is disabled. Enable it in inspector or sculpt terrain manually with Unity Terrain Tools.");
            return;
        }

        if (terrainData == null)
        {
            Debug.LogError("Terrain data not found!");
            return;
        }

        int resolution = terrainData.heightmapResolution;
        float[,] heights = GenerateImprovedHeights(resolution);
        terrainData.SetHeights(0, 0, heights);

        Debug.Log("Hilly terrain generated! Consider refining with Unity Terrain Tools for better control.");
    }

    private float[,] GenerateImprovedHeights(int resolution)
    {
        float[,] heights = new float[resolution, resolution];
        System.Random prng = new System.Random(seed);
        float offsetX = prng.Next(-10000, 10000);
        float offsetY = prng.Next(-10000, 10000);

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float xCoord = (float)x / resolution;
                float yCoord = (float)y / resolution;

                // Multi-octave Perlin noise (Fractal Brownian Motion)
                float noiseValue = FractalBrownianMotion(
                    xCoord + offsetX,
                    yCoord + offsetY,
                    octaves,
                    persistence,
                    lacunarity
                );

                // Add some valleys using abs
                float valleys = Mathf.Abs(Mathf.PerlinNoise(
                    (xCoord + offsetX) * baseScale * 3f,
                    (yCoord + offsetY) * baseScale * 3f
                ) * 2f - 1f);

                // Combine for more dramatic terrain
                heights[x, y] = (noiseValue * 0.7f + valleys * 0.3f) * heightMultiplier;
            }
        }

        return heights;
    }

    private float FractalBrownianMotion(float x, float y, int octaves, float persistence, float lacunarity)
    {
        float total = 0f;
        float frequency = baseScale;
        float amplitude = 1f;
        float maxValue = 0f;

        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return total / maxValue;
    }

    /// <summary>
    /// Flatten a circular area of terrain for road placement.
    /// Call this at road waypoints to create smooth driving surfaces.
    /// </summary>
    public void FlattenRoadArea(Vector3 worldPosition, float customRadius = -1)
    {
        if (terrainData == null) return;

        float radius = customRadius > 0 ? customRadius : flattenRadius;

        // Convert world position to terrain local position
        Vector3 terrainPos = worldPosition - terrain.GetPosition();

        // Normalize to 0-1 range
        float normalizedX = terrainPos.x / terrainData.size.x;
        float normalizedZ = terrainPos.z / terrainData.size.z;

        // Convert to heightmap coordinates
        int heightmapX = Mathf.RoundToInt(normalizedX * (terrainData.heightmapResolution - 1));
        int heightmapZ = Mathf.RoundToInt(normalizedZ * (terrainData.heightmapResolution - 1));

        // Calculate radius in heightmap units
        int radiusInHeightmap = Mathf.RoundToInt((radius / terrainData.size.x) * terrainData.heightmapResolution);
        int blendRadiusInHeightmap = Mathf.RoundToInt((flattenBlendDistance / terrainData.size.x) * terrainData.heightmapResolution);
        int totalRadius = radiusInHeightmap + blendRadiusInHeightmap;

        // Get target height at center
        float targetHeight = terrainData.GetHeight(heightmapX, heightmapZ) / terrainData.size.y;

        // Get affected area
        int startX = Mathf.Max(0, heightmapX - totalRadius);
        int startZ = Mathf.Max(0, heightmapZ - totalRadius);
        int width = Mathf.Min(terrainData.heightmapResolution, heightmapX + totalRadius) - startX;
        int height = Mathf.Min(terrainData.heightmapResolution, heightmapZ + totalRadius) - startZ;

        float[,] heights = terrainData.GetHeights(startX, startZ, width, height);

        // Flatten with smooth falloff
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int actualX = startX + x;
                int actualZ = startZ + z;

                float distance = Vector2.Distance(
                    new Vector2(actualX, actualZ),
                    new Vector2(heightmapX, heightmapZ)
                );

                if (distance <= totalRadius)
                {
                    float blend = 1f;

                    if (distance > radiusInHeightmap)
                    {
                        // Apply falloff curve
                        float normalizedDist = (distance - radiusInHeightmap) / blendRadiusInHeightmap;
                        blend = flattenFalloff.Evaluate(normalizedDist);
                    }

                    heights[z, x] = Mathf.Lerp(heights[z, x], targetHeight, blend);
                }
            }
        }

        terrainData.SetHeights(startX, startZ, heights);
    }

    /// <summary>
    /// Flatten terrain along a path defined by multiple points.
    /// Useful for creating road beds that follow waypoints.
    /// </summary>
    public void FlattenRoadPath(Vector3[] pathPoints, float roadWidth)
    {
        if (pathPoints == null || pathPoints.Length < 2)
        {
            Debug.LogWarning("Need at least 2 path points to flatten a road path.");
            return;
        }

        foreach (Vector3 point in pathPoints)
        {
            FlattenRoadArea(point, roadWidth * 0.5f);
        }

        Debug.Log($"Flattened road path with {pathPoints.Length} points.");
    }

    /// <summary>
    /// Get terrain height at a world position.
    /// Useful for placing objects or conforming roads to terrain.
    /// </summary>
    public float GetHeightAtPosition(Vector3 worldPosition)
    {
        if (terrain == null) return 0f;
        return terrain.SampleHeight(worldPosition);
    }

    /// <summary>
    /// Get terrain normal at a world position.
    /// Useful for aligning objects to terrain slope.
    /// </summary>
    public Vector3 GetNormalAtPosition(Vector3 worldPosition)
    {
        if (terrainData == null) return Vector3.up;

        Vector3 terrainPos = worldPosition - terrain.GetPosition();
        float normalizedX = terrainPos.x / terrainData.size.x;
        float normalizedZ = terrainPos.z / terrainData.size.z;

        return terrainData.GetInterpolatedNormal(normalizedX, normalizedZ);
    }

    /// <summary>
    /// Get terrain steepness (angle) at a world position in degrees.
    /// </summary>
    public float GetSteepnessAtPosition(Vector3 worldPosition)
    {
        Vector3 normal = GetNormalAtPosition(worldPosition);
        return Vector3.Angle(normal, Vector3.up);
    }

    private void OnDrawGizmosSelected()
    {
        if (terrain == null) return;

        // Draw terrain bounds
        Gizmos.color = Color.green;
        Vector3 center = terrain.GetPosition() + terrainData.size * 0.5f;
        Gizmos.DrawWireCube(center, terrainData.size);
    }
}