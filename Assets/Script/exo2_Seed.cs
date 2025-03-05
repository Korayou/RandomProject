using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

//[ExecuteInEditMode]
public class NoiseSeed : MonoBehaviour
{
    public Vector2Int textureSize = new Vector2Int(250, 250);
    public NoiseGenerator noise;
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
                greyscale = noise.GetNoiseValueAtCoordinates(i, j);
                color.r = color.g = color.b = greyscale;
                noiseTexture.SetPixel(i, j, color);
            }
        }
        noiseTexture.Apply();
        noiseRenderer.material.mainTexture = noiseTexture;
    }
}

[CustomEditor(typeof(NoiseSeed))]
public class NoiseSeedEditor : Editor
{
    SerializedProperty textureSize;

    private NoiseSeed targetNoise;

    void OnEnable()
    {
        textureSize = serializedObject.FindProperty("textureSize");

        targetNoise = (NoiseSeed)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(textureSize);

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Random seed"))
        {
            targetNoise.noise.RandomizeSeed();
        }

        if (GUILayout.Button("Random Values"))
        {
            targetNoise.noise.PickRandomValues();
        }

        if (GUILayout.Button("Generate"))
        {
            targetNoise.ReloadTexture();
        }
    }
}