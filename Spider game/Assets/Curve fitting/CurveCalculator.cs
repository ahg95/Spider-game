using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class models a smooth curve based on a list of points to fit the curve to. For an explanation on how the algorithm works, please see <see cref="https://www.linkedin.com/pulse/simple-curve-fitting-algorithm-games-ansgar-glup/?trackingId=bjEHvl1HFJ1L7gFsZFsUbw%3D%3D"/>, 
/// </summary>
public class CurveCalculator : MonoBehaviour
{
    public AnimationCurve SmoothingCurve;

    public List<Transform> PointsToFitCurveTo = new List<Transform>();
    float linearInterpolationLength;
    bool linearInterpolationLengthIsOutdated = true;

    public void AddCurvePoint(Transform curvePoint) => PointsToFitCurveTo.Add(curvePoint);

    public void RemoveCurvePoint(Transform curvePoint) => PointsToFitCurveTo.Remove(curvePoint);

    public int GetNumberOfCurvePoints() => PointsToFitCurveTo.Count;

    public float GetLinearInterpolationLength()
    {
        if (linearInterpolationLengthIsOutdated)
            CalculateLinearInterpolationLength();

        return linearInterpolationLength;
    }

    void CalculateLinearInterpolationLength()
    {
        linearInterpolationLength = 0;
        for (int i = 0; i < PointsToFitCurveTo.Count - 1; i++)
        {
            linearInterpolationLength += (PointsToFitCurveTo[i + 1].position - PointsToFitCurveTo[i].position).magnitude;
        }
    }


    /// <summary>
    /// Returns the point where the smooth curve would end if it was as long as length.
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public Vector3 GetCurvePointAtLength(float length)
    {
        Vector3 desiredCurvePoint;

        if (GetNumberOfCurvePoints() == 0)
            desiredCurvePoint = Vector3.zero;
        else if (GetNumberOfCurvePoints() == 1)
            desiredCurvePoint = GetPointToFitCurveTo(0);
        else
            desiredCurvePoint = CalculateCurvePoint(length);

        return desiredCurvePoint;
    }

    Vector3 GetPointToFitCurveTo(int index) => PointsToFitCurveTo[index].position;

    Vector3 CalculateCurvePoint(float length)
    {
        Vector3 pointToCalculate = Vector3.zero;

        float sumOfDistancesBetweenCurvePoints = 0;

        for (int i = 0; i < GetNumberOfCurvePoints() - 1; i++)
        {
            float distance = Vector3.Distance(GetPointToFitCurveTo(i), GetPointToFitCurveTo(i + 1));

            sumOfDistancesBetweenCurvePoints += distance;

            if (length < sumOfDistancesBetweenCurvePoints)
            {
                Vector3 aPos = GetPointToFitCurveTo(i);
                Vector3 aDir = CalculateCurvePointDirectionForIndex(i);

                Vector3 bPos = GetPointToFitCurveTo(i + 1);
                Vector3 bDir = CalculateCurvePointDirectionForIndex(i + 1);

                pointToCalculate = GetCurvePointBetweenTwoPoints(aPos, aDir, bPos, bDir, length - (sumOfDistancesBetweenCurvePoints - distance));
                break;
            }
            // If i is the index of the second to last curve point to fit
            else if (i == GetNumberOfCurvePoints() - 2)
                pointToCalculate = GetPointToFitCurveTo(i + 1);
        }

        return pointToCalculate;
    }

    Vector3 CalculateCurvePointDirectionForIndex(int index)
    {
        Vector3 direction = Vector3.zero;

        if (1 < GetNumberOfCurvePoints() && 0 <= index && index < GetNumberOfCurvePoints())
        {
            if (index == 0)
                direction = GetPointToFitCurveTo(1) - GetPointToFitCurveTo(0);
            else if (index == GetNumberOfCurvePoints() - 1)
                direction = GetPointToFitCurveTo(GetNumberOfCurvePoints() - 1) - GetPointToFitCurveTo(GetNumberOfCurvePoints() - 2);
            else
                direction = GetPointToFitCurveTo(index + 1) - GetPointToFitCurveTo(index - 1);
        }
        else
            Debug.LogError("Index out of range.");

        return direction.normalized;
    }

    Vector3 GetCurvePointBetweenTwoPoints(Vector3 aPos, Vector3 aDir, Vector3 bPos, Vector3 bDir, float distance)
    {
        Vector3 curvePointBetweenTwoPoints;

        distance = Mathf.Clamp(distance, 0, Vector3.Distance(aPos, bPos));

        Vector3 aExtrapolationPoint = aPos + aDir * distance;
        Vector3 bExtrapolationPoint = bPos - bDir * (Vector3.Distance(aPos, bPos) - distance);

        float interpolationValue = SmoothingCurve.Evaluate(distance / Vector3.Distance(aPos, bPos));

        curvePointBetweenTwoPoints = Vector3.Lerp(aExtrapolationPoint, bExtrapolationPoint, interpolationValue);

        return curvePointBetweenTwoPoints;
    }

    void FixedUpdate()
    {
        linearInterpolationLengthIsOutdated = true;
    }
}
