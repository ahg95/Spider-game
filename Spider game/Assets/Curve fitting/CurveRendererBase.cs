using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CurveRendererBase : MonoBehaviour
{
    public CurveCalculator curveCalculator;
    [Range(0.1f, 10.0f)]
    public float DistanceBetweenRenderPoints;

    private void LateUpdate()
    {
        CalculateRenderPoints();
        RenderCurve();
    }

    protected void CalculateRenderPoints()
    {
        float linearInterpolationLength = curveCalculator.GetLinearInterpolationLength();
        float renderPointDistance = 0;
        int pointIndex = 0;

        // Set first point of the curve
        SetPosition(pointIndex++, curveCalculator.GetCurvePointAtLength(0));

        while (renderPointDistance < linearInterpolationLength)
        {
            SetPosition(pointIndex++, curveCalculator.GetCurvePointAtLength(renderPointDistance));
            renderPointDistance += DistanceBetweenRenderPoints;
        }
        // Set end point of the curve
        SetPosition(pointIndex++, curveCalculator.GetCurvePointAtLength(curveCalculator.GetLinearInterpolationLength()));

        TrimLineToNumberOfPoints(pointIndex);
    }



    protected abstract void SetPosition(int linePointIndex, Vector3 position);

    protected abstract void TrimLineToNumberOfPoints(int nrOfPoints);

    protected abstract void RenderCurve();
}
