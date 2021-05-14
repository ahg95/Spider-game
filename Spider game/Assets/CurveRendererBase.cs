using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CurveCalculator))]
public abstract class CurveRendererBase : MonoBehaviour
{
    public float DistanceBetweenRenderPoints;
    public CurveCalculator curveCalculator;

    protected void 

    protected abstract void SetPosition(int linePointIndex, Vector3 position);

    protected abstract void TrimLineToNumberOfPoints(int nrOfPoints);

}
