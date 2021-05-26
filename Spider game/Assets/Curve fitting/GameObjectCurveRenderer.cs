using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectCurveRenderer : CurveRendererBase
{
    public GameObject renderObject;
    private List<GameObject> renderObjects;

    private void Start()
    {
        renderObjects = new List<GameObject>();
    }

    protected override void SetPosition(int linePointIndex, Vector3 position)
    {
        while (renderObjects.Count <= linePointIndex)
            renderObjects.Add(Instantiate(renderObject));

        renderObjects[linePointIndex].SetActive(true);
        renderObjects[linePointIndex].transform.position = position;
    }

    protected override void TrimLineToNumberOfPoints(int nrOfPoints)
    {
        for (int i = nrOfPoints; i < renderObjects.Count; i++)
        {
            renderObjects[i].SetActive(false);
        }
    }
}
