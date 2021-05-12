using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCurveCalculator : MonoBehaviour
{
    public float DistanceBetweenCurvePoints;
    public AnimationCurve SmoothingCurve;

    public void SetTransformsToModel(Transform[] transforms)
    {

    }

    public Vector3 GetCurvePositionAtInterpolationValue(float t)
    {
        return Vector3.zero;
    }

    public Vector3[] CalculateSmoothCurvePoints(Vector3[] curvePoints)
    {
        // Contains all calculated curve points in order
        List<Vector3> smoothCurvePointsList = new List<Vector3>();

        // The offset from the startPosition of the currently modeled curve section to the point that should be calculated.
        float curvePointOffset = 0;

        // We loop until i = dataPoints.Length - 1 because the last dataPoint has already been modeled as the endpoint of the curve
        for (int i = 0; i < curvePoints.Length - 1; i++)
        {
            Vector3 startPosition = curvePoints[i];
            Vector3 endPosition = curvePoints[i + 1];

            Vector3 startDirection;
            if (i == 0)
                startDirection = endPosition - startPosition;
            else
                startDirection = endPosition - curvePoints[i - 1];

            Vector3 endDirection;
            if (i == curvePoints.Length - 2) // This is true if the endPoint is the position of the last dataPoint
                endDirection = endPosition - startPosition;
            else
                endDirection = curvePoints[i + 2] - startPosition;

            float startToEndPointDistance = (endPosition - startPosition).magnitude;

            while (curvePointOffset < startToEndPointDistance)
            {
                Vector3 smoothCurvePoint = Vector3Utility.GetPointOfSmoothCurveConnectingTwoPoints(startPosition, startDirection, endPosition, endDirection, curvePointOffset / startToEndPointDistance, SmoothingCurve);

                smoothCurvePointsList.Add(smoothCurvePoint);

                curvePointOffset += DistanceBetweenCurvePoints;
            }
            curvePointOffset -= startToEndPointDistance;
        }

        return smoothCurvePointsList.ToArray();
    }
}
