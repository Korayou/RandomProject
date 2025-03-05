using Unity.Mathematics;
using UnityEngine;

public enum NoiseType
{
    Perlin,
    Simplex,
    Worley
}

public class NoiseGenerator
{
    public NoiseType noiseType;
    [Range(0.1f, 2f)] public float resultPow = 1;
    public int seed;
    public Vector2Int noiseOffset;
    private Vector2 noiseScaleMinMax = new(0, 100);
    [Range(0f, 100f)] public float noiseScale = 1;

    public NoiseGenerator(NoiseType noiseType, int seed, Vector2Int noiseOffset, Vector2Int noiseScaleMinMax)
    {
        this.noiseType = noiseType;
        this.seed = seed;
        this.noiseOffset = noiseOffset;
        this.noiseScaleMinMax = noiseScaleMinMax;
    }

    public NoiseGenerator() { }

    public void RandomizeSeed()
    {
        seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        UnityEngine.Random.InitState(seed);
    }

    public void PickRandomValues()
    {
        noiseOffset = new Vector2Int(UnityEngine.Random.Range(0, 10000000), UnityEngine.Random.Range(0, 10000000));
        noiseScale = UnityEngine.Random.Range(noiseScaleMinMax.x, noiseScaleMinMax.y);
        resultPow = UnityEngine.Random.Range(0.1f, 2f);
    }

    public float GetNoiseValueAtCoordinates(int x, int y)
    {
        x += noiseOffset.x;
        y += noiseOffset.y;

        switch (noiseType)
        {
            case NoiseType.Worley:
                return math.pow(noise.cellular(new float2(x, y) / noiseScale).x, resultPow);
            case NoiseType.Perlin:
                return (math.pow(noise.cnoise(new float2(x, y) / noiseScale), resultPow) + 1) / 2;
            case NoiseType.Simplex:
                return (math.pow(noise.snoise(new float2(x, y) / noiseScale), resultPow) + 1) / 2;
        }

        return 0;
    }
}