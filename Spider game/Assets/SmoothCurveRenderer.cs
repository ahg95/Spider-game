using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SmoothCurveRenderer : MonoBehaviour
{
    public Transform[] CurvePoints;

    public GameObject renderObject;
    public List<GameObject> renderPoints;

    public SmoothCurveCalculator curveCalculator;

    public void RenderPointInCurve(Vector3 point, int indexOfPoint)
    {

    }
}
