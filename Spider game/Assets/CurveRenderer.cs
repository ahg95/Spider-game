using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveRenderer : MonoBehaviour
{
    public GameObject renderObject;

    private List<GameObject> renderObjects;

    private void Start()
    {
        renderObjects = new List<GameObject>();
    }

    private void Update()
    {
        Render();
    }

    private void Render()
    {

    }

    public void SetPosition(int linePointIndex, Vector3 position)
    {
        while (renderObjects.Count <= linePointIndex)
            renderObjects.Add(Instantiate(renderObject));

        renderObjects[linePointIndex].SetActive(true);
        renderObjects[linePointIndex].transform.position = position;
    }

    public void TrimLineToNumberOfPoints(int nrOfPoints)
    {
        for (int i = nrOfPoints; i < renderObjects.Count; i++)
        {
            renderObjects[i].SetActive(false);
        }
    }
}
