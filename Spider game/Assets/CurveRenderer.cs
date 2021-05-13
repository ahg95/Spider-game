using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CurveCalculator))]
public class CurveRenderer : MonoBehaviour
{
    public GameObject renderObject;
    public float DistanceBetweenRenderPoints;

    private CurveCalculator curveCalculator;
    private List<GameObject> renderObjects;

    private void Start()
    {
        curveCalculator = GetComponent<CurveCalculator>();
        renderObjects = new List<GameObject>();
    }

    private void Update()
    {
        RenderCurve();
    }

    private void RenderCurve()
    {
        float linearInterpolationLength = curveCalculator.GetLinearInterpolationLength();
        float renderPointDistance = 0;
        int pointIndex = 0;

        while (renderPointDistance < linearInterpolationLength)
        {
            Vector3 curvePoint = curveCalculator.GetCurvePoint(renderPointDistance / linearInterpolationLength);
            SetPosition(pointIndex, curvePoint);
            renderPointDistance += DistanceBetweenRenderPoints;
        }
        TrimLineToNumberOfPoints(pointIndex);
    }

    protected virtual void SetPosition(int linePointIndex, Vector3 position)
    {
        while (renderObjects.Count <= linePointIndex)
            renderObjects.Add(Instantiate(renderObject));

        renderObjects[linePointIndex].SetActive(true);
        renderObjects[linePointIndex].transform.position = position;
    }

    protected virtual void TrimLineToNumberOfPoints(int nrOfPoints)
    {
        for (int i = nrOfPoints; i < renderObjects.Count; i++)
        {
            renderObjects[i].SetActive(false);
        }
    }
}
