using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NoiseStruct
{
    public NoiseType type;
    public Vector2Int scaleMinMax;
    public Vector2Int offset;
}

[CreateAssetMenu(menuName = "Terrain", fileName = "Terrain")]
public class ScriptableTerrain : ScriptableObject
{
    public List<GameObject> prefabs;
    public List<NoiseStruct> noises;
}
