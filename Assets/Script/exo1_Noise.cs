using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

//[ExecuteInEditMode]
public class Noise : MonoBehaviour
{
    public Vector2Int textureSize = new Vector2Int(250, 250);
    public NoiseType noiseType;
    public Vector2Int noiseOffset;
    public float noiseScale;
    public float resultPow;

    private Texture2D noiseTexture;
    private MeshRenderer noiseRenderer;

    public void Start()
    {
        noiseRenderer = GetComponent<MeshRenderer>();
        noiseTexture = new(textureSize.x, textureSize.y);
    }

    public void ReloadTexture()
    {
        Color color = Color.white;
        float greyscale = 0;
        for (int i = 0; i < textureSize.x; i++)
        {
            for (int j = 0; j < textureSize.y; j++)
            {
                greyscale = GetNoiseValueAtCoordinates(i + noiseOffset.x, j + noiseOffset.y); 
                color.r = color.g = color.b = greyscale;
                noiseTexture.SetPixel(i, j, color);
            }
        }
        noiseTexture.Apply();
        noiseRenderer.material.mainTexture = noiseTexture;
    }

    public float GetNoiseValueAtCoordinates(int x, int y)
    {
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

[CustomEditor(typeof(Noise))]
public class NoiseEditor : Editor
{
    SerializedProperty textureSize;
    SerializedProperty noiseType;
    SerializedProperty noiseOffset;
    SerializedProperty noiseScale;
    SerializedProperty resultPow;

    private Noise targetNoise;

    void OnEnable()
    {
        textureSize = serializedObject.FindProperty("textureSize");
        noiseType = serializedObject.FindProperty("noiseType");
        noiseOffset = serializedObject.FindProperty("noiseOffset");
        noiseScale = serializedObject.FindProperty("noiseScale");
        resultPow = serializedObject.FindProperty("resultPow");

        targetNoise = (Noise)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(textureSize);
        EditorGUILayout.PropertyField(noiseType);
        EditorGUILayout.PropertyField(noiseOffset);
        EditorGUILayout.PropertyField(noiseScale);
        EditorGUILayout.PropertyField(resultPow);

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Apply"))
        {
            targetNoise.ReloadTexture();
        }
    }
}