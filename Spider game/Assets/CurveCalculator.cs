using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveCalculator : MonoBehaviour
{
    public List<Transform> PointsToFitCurveTo;
    public AnimationCurve SmoothingCurve;

    float linearInterpolationLength;
    bool linearInterpolationLengthIsDirty = true;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetCurvePoint(float t)
    {
        float startPositionDistance;
        int startPositionIndex = FindStartPositionIndexForInterpolationValue(t, out startPositionDistance);

        Vector3 startPosition = PointsToFitCurveTo[startPositionIndex].position;
        Vector3 endPosition = PointsToFitCurveTo[startPositionIndex + 1].position;

        Vector3 startDirection;
        if (startPositionIndex == 0)
            startDirection = endPosition - startPosition;
        else
            startDirection = endPosition - PointsToFitCurveTo[startPositionIndex - 1].position;

        Vector3 endDirection;
        if (startPositionIndex == PointsToFitCurveTo.Count - 2) // This is true if the endPoint is the position of the last dataPoint
            endDirection = endPosition - startPosition;
        else
            endDirection = PointsToFitCurveTo[startPositionIndex + 2].position - startPosition;


        float max = startPositionDistance + (endPosition - startPosition).magnitude;
        float curveSectionInterpolation = TransformValueFromIntervalToUnitInterval(t, startPositionDistance, max);

        Vector3 curvePoint = Vector3Utility.GetPointOfSmoothCurveConnectingTwoPoints(startPosition, startDirection, endPosition, endDirection, curveSectionInterpolation, SmoothingCurve);

        return curvePoint;
    }

    private float TransformValueFromIntervalToUnitInterval(float value, float intervalMinimum, float intervalMaximum)
    {
        return (value - intervalMinimum) / (intervalMaximum - intervalMinimum);
    }

    private int FindStartPositionIndexForInterpolationValue(float t, out float startPositionDistance)
    {
        t = Mathf.Clamp(t, 0, 1);
        startPositionDistance = 0;
        int startPositionIndex = 0;

        while (startPositionDistance / GetLinearInterpolationLength() < t)
        {
            startPositionDistance += (PointsToFitCurveTo[startPositionIndex + 1].position - PointsToFitCurveTo[startPositionIndex].position).magnitude;
            startPositionIndex++;
        }

        return startPositionIndex;
    }

    private void Update()
    {
        linearInterpolationLengthIsDirty = true;
    }

    public float GetLinearInterpolationLength()
    {
        if (linearInterpolationLengthIsDirty)
        {
            CalculateLinearInterpolationLength();
        }

        return linearInterpolationLength;
    }

    private void CalculateLinearInterpolationLength()
    {
        linearInterpolationLength = 0;
        for (int i = 0; i < PointsToFitCurveTo.Count - 1; i++)
        {
            linearInterpolationLength += (PointsToFitCurveTo[i + 1].position - PointsToFitCurveTo[i].position).magnitude;
        }
    }
}
