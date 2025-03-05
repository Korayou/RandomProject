using System.Drawing;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    public NoiseGenerator generationGroundNoise;
    public NoiseGenerator generationPropsNoise;
    [SerializeField] private float xSize;
    [SerializeField] private float ySize;
    [SerializeField] private GameObject sandPrefab;
    [SerializeField] private GameObject grassPrefab;
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private GameObject boatPrefab;
    [SerializeField] private GameObject buildingPrefab;

    private void Awake()
    {
        generationGroundNoise = new();
        generationPropsNoise = new();
        GenerateField();
    }

    public void Randomize()
    {
        generationGroundNoise.PickRandomValues();
        generationGroundNoise.resultPow = 1;

        generationPropsNoise.PickRandomValues();
        generationPropsNoise.resultPow = 1;
    }
    public void GenerateField()
    {
        CleanChilds();

        float waterThreshold = 0.4f;
        float treeThreshold = 0.5f;
        float grassThreshold = 0.7f;

        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                switch (generationGroundNoise.GetNoiseValueAtCoordinates(i, j))
                {
                    case float g when g <= waterThreshold:
                        InstanciateElement(waterPrefab, i, j);
                        if (generationPropsNoise.GetNoiseValueAtCoordinates(i,j) < 0.2f)
                            InstanciateElement(boatPrefab, i, j);
                        break;
                    case float g when g <= treeThreshold && g > waterThreshold:
                        InstanciateElement(sandPrefab, i, j);
                        break;
                    case float g when g > treeThreshold && g <= grassThreshold:
                        InstanciateElement(treePrefab, i, j);
                        break;
                    case float g when g > grassThreshold:
                        InstanciateElement(grassPrefab, i, j);
                        if (generationPropsNoise.GetNoiseValueAtCoordinates(i, j) < 0.3f)
                            InstanciateElement(buildingPrefab, i, j);
                        break;
                    default:
                        InstanciateElement(waterPrefab, i, j);
                        break;
                }
            }
        }
    }

    private void CleanChilds()
    {
        for (int i = 0;i < transform.childCount; i++)
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

[CustomEditor(typeof(FieldGenerator))]
public class FieldGeneratorEditor : Editor
{
    SerializedProperty xSize;
    SerializedProperty ySize;
    SerializedProperty sandPrefab;
    SerializedProperty grassPrefab;
    SerializedProperty treePrefab;
    SerializedProperty waterPrefab;
    SerializedProperty boatPrefab;
    SerializedProperty buildingPrefab;

    FieldGenerator targetgo;

    void OnEnable()
    {
        xSize = serializedObject.FindProperty("xSize");
        ySize = serializedObject.FindProperty("ySize");
        sandPrefab = serializedObject.FindProperty("sandPrefab");
        grassPrefab = serializedObject.FindProperty("grassPrefab");
        treePrefab = serializedObject.FindProperty("treePrefab");
        waterPrefab = serializedObject.FindProperty("waterPrefab");
        boatPrefab = serializedObject.FindProperty("boatPrefab");
        buildingPrefab = serializedObject.FindProperty("buildingPrefab");

        targetgo = (FieldGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(xSize);
        EditorGUILayout.PropertyField(ySize);
        EditorGUILayout.PropertyField(sandPrefab);
        EditorGUILayout.PropertyField(grassPrefab);
        EditorGUILayout.PropertyField(treePrefab);
        EditorGUILayout.PropertyField(waterPrefab);
        EditorGUILayout.PropertyField(boatPrefab);
        EditorGUILayout.PropertyField(buildingPrefab);

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