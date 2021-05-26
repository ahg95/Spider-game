using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveCalculator : MonoBehaviour
{
    public List<Transform> PointsToFitCurveTo;
    public AnimationCurve SmoothingCurve;

    float linearInterpolationLength;
    bool linearInterpolationLengthIsOutdated = true;

    /// <summary>
    /// Returns the point where the smooth curve would end if it was as long as length.
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public Vector3 GetCurvePointAtLength(float length)
    {
        float lengthAtStartPosition;
        int startPositionIndex = FindStartPositionIndexForLength(length, out lengthAtStartPosition);

        Vector3 startPosition = PointsToFitCurveTo[startPositionIndex].position;
        Vector3 endPosition = PointsToFitCurveTo[startPositionIndex + 1].position;

        Vector3 startDirection;
        if (startPositionIndex == 0)
            startDirection = endPosition - startPosition;
        else
            startDirection = endPosition - PointsToFitCurveTo[startPositionIndex - 1].position;

        Vector3 endDirection;
        if (startPositionIndex + 1 == PointsToFitCurveTo.Count - 1) // This is true if the endPoint is the position of the last dataPoint
            endDirection = endPosition - startPosition;
        else
            endDirection = PointsToFitCurveTo[startPositionIndex + 2].position - startPosition;

        float t = (length - lengthAtStartPosition) / (PointsToFitCurveTo[startPositionIndex + 1].position - PointsToFitCurveTo[startPositionIndex].position).magnitude;

        Vector3 curvePoint = Vector3Utility.GetPointOfSmoothCurveConnectingTwoPoints(startPosition, startDirection, endPosition, endDirection, t, SmoothingCurve);

        return curvePoint;
    }

    private int FindStartPositionIndexForLength(float length, out float lengthAtStartPosition)
    {
        if (GetLinearInterpolationLength() < length)
            length = GetLinearInterpolationLength();

        int endPositionIndex = 0;
        float lengthAtEndPosition = 0;

        do
        {
            lengthAtStartPosition = lengthAtEndPosition;
            endPositionIndex++;
            lengthAtEndPosition += (PointsToFitCurveTo[endPositionIndex].position - PointsToFitCurveTo[endPositionIndex - 1].position).magnitude;
        } while (lengthAtEndPosition < length);

        return endPositionIndex - 1;
    }

    private void Update()
    {
        linearInterpolationLengthIsOutdated = true;
    }

    public float GetLinearInterpolationLength()
    {
        if (linearInterpolationLengthIsOutdated)
            CalculateLinearInterpolationLength();

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