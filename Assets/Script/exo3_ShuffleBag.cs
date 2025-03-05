using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class exo3_ShuffleBag : MonoBehaviour
{
    public GameObject visualisationObject;
    public Vector2Int range;
    private List<GameObject> visualisationList;
    public int bagLevel = 1;
    public float refillPoint = 0f;
    private ShuffleBag<int> bag;

    void Start()
    {
        List<int> values = new List<int>();
        for (int i = range.x; i < range.y; i++)
            values.Add(i);
        bag = new(values, bagLevel, refillPoint);
        visualisationList = new List<GameObject>();
        GameObject visualisationInstance;

        for (int i = range.x; i < range.y; i++)
        {
            visualisationInstance = Instantiate(visualisationObject);
            visualisationList.Add(visualisationInstance);
            visualisationInstance.transform.SetParent(gameObject.transform);
            visualisationInstance.transform.position += new Vector3(visualisationInstance.GetComponent<Renderer>().bounds.size.x * i, 0, 0);
        }
    }

    void Update()
    {
        int value = bag.Pick();
        visualisationList[value - range.x].transform.localScale += new Vector3(0, 1, 0);
        visualisationList[value - range.x].transform.position += new Vector3(0, 0.5f, 0);
    }
}

public class ShuffleBag<T>
{

    private List<T> values;
    private List<T> bag;
    private int bagSize = 0;
    private int bagLevel;
    private float refillPoint; 

    public ShuffleBag(List<T> values, int bagLevel, float refillPoint)
    {
        bag = new List<T>();
        this.bagLevel = bagLevel;
        this.refillPoint = refillPoint;
        this.values = values;
    }

    public void FillBag()
    {

        for (int j = 0; j < bagLevel; j++)
        {
            for (int i = 0; i < values.Count; i++)
            {
                bag.Add(values[i]);
            }
        }
        if (bagSize <= 0)
            bagSize = bag.Count;
    }

    public T Pick()
    {
        if (bag.Count <= bagSize * refillPoint)
            FillBag();

        int index = UnityEngine.Random.Range(0, bag.Count);
        T value = bag[index];
        bag.RemoveAt(index);
        return value;
    }
}
