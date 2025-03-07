using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GenerationTerrain : MonoBehaviour
{
    [SerializeField] private float xSize;
    [SerializeField] private float ySize;
    [SerializeField] private int seed;

    [SerializeField] private List<ScriptableTerrain> biomes;
    [SerializeField] private GameObject filler;

    private Dictionary<NoiseStruct, NoiseGenerator> generationNoise;


    private void Awake()
    {
        generationNoise = new();
        foreach (ScriptableTerrain terrain in biomes)
        {
            foreach (NoiseStruct noiseSettings in terrain.noises)
            {
                generationNoise.Add(noiseSettings, new NoiseGenerator(noiseSettings.type, seed, noiseSettings.offset, noiseSettings.scaleMinMax));
            }
        }

        Randomize();
        GenerateField();
    }

    public void Randomize()
    {
        foreach (KeyValuePair<NoiseStruct, NoiseGenerator> noisegenerator in generationNoise)
        {
            noisegenerator.Value.PickRandomValues();
            noisegenerator.Value.resultPow = 1;
        }
    }
    public void GenerateField()
    {
        CleanChilds();

        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                PlaceTerrain(i, j);
            }
        }
    }

    private void PlaceTerrain(int i, int j)
    {
        foreach (ScriptableTerrain biome in biomes)
        {
            ShuffleBag<GameObject> shuffleBag = new(biome.prefabs, 5, 0.9f);
            foreach (NoiseStruct noiseSettings in biome.noises)
            {
                if (generationNoise[noiseSettings].GetNoiseValueAtCoordinates(i, j) > 0.5)
                {
                    InstanciateElement(shuffleBag.Pick(), i, j);
                    return;
                }
            }
        }
        InstanciateElement(filler, i, j);
    }
    private void CleanChilds()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void InstanciateElement(GameObject prefab, float x, float z)
    {
        Vector3 position = new Vector3(x * 0.85f, 0, x % 2 == 0 ? z : z + 0.5f);
        GameObject prefabInstance = Instantiate(prefab, transform.position + position, new Quaternion(0, 0.7071f, 0, 0.7071f));
        prefabInstance.transform.SetParent(transform);
    }

}

[CustomEditor(typeof(GenerationTerrain))]
public class GenerationTerrainEditor : Editor
{
    SerializedProperty xSize;
    SerializedProperty ySize;
    SerializedProperty seed;
    SerializedProperty biomes;
    SerializedProperty filler;

    GenerationTerrain targetgo;

    void OnEnable()
    {
        xSize = serializedObject.FindProperty("xSize");
        ySize = serializedObject.FindProperty("ySize");
        seed = serializedObject.FindProperty("seed");
        biomes = serializedObject.FindProperty("biomes");
        filler = serializedObject.FindProperty("filler");

        targetgo = (GenerationTerrain)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(xSize);
        EditorGUILayout.PropertyField(ySize);
        EditorGUILayout.PropertyField(seed);
        EditorGUILayout.PropertyField(biomes);
        EditorGUILayout.PropertyField(filler);

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Generate"))
        {
            targetgo.GenerateField();
        }

        if (GUILayout.Button("Random generate"))
        {
            targetgo.Randomize();
            targetgo.GenerateField();
        }
    }
}
