using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CurveCalculator))]
public abstract class CurveRendererBase : MonoBehaviour
{
    public float DistanceBetweenRenderPoints;

    CurveCalculator curveCalculator;

    /*
    private void OnEnable()
    {
        curveCalculator = GetComponent<CurveCalculator>();
    }

    protected void RenderCurve()
    {
        float linearInterpolationLength = curveCalculator.GetLinearInterpolationLength();
        float renderPointDistance = 0;
        int pointIndex = 0;

        while (renderPointDistance < linearInterpolationLength)
        {
            Vector3 curvePoint = curveCalculator.GetCurvePointAtLength(renderPointDistance);
            SetPosition(pointIndex, curvePoint);
            renderPointDistance += DistanceBetweenRenderPoints;
            pointIndex++;
        }
        TrimLineToNumberOfPoints(pointIndex);
    }
    */

    protected abstract void SetPosition(int linePointIndex, Vector3 position);

    protected abstract void TrimLineToNumberOfPoints(int nrOfPoints);
}
