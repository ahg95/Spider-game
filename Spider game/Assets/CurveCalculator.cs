using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveCalculator : MonoBehaviour
{
    public List<Transform> PointsToFitCurveTo;
    public float DistanceBetweenCurvePoints;
    public AnimationCurve SmoothingCurve;
    public CurveRenderer curveRenderer;

    private void Update()
    {
        RenderCurve();
    }

    public void RenderCurve()
    {
        // The offset from the startPosition of the currently modeled curve section to the point that should be calculated.
        float curvePointOffset = 0;

        // The index of the next point to render.
        int pointIndexToRender = 0;

        // We loop until i = dataPoints.Length - 1 because the last dataPoint has already been modeled as the endpoint of the curve
        for (int i = 0; i < PointsToFitCurveTo.Count - 1; i++)
        {
            Vector3 startPosition = PointsToFitCurveTo[i].position;
            Vector3 endPosition = PointsToFitCurveTo[i + 1].position;

            Vector3 startDirection;
            if (i == 0)
                startDirection = endPosition - startPosition;
            else
                startDirection = endPosition - PointsToFitCurveTo[i - 1].position;

            Vector3 endDirection;
            if (i == PointsToFitCurveTo.Count - 2) // This is true if the endPoint is the position of the last dataPoint
                endDirection = endPosition - startPosition;
            else
                endDirection = PointsToFitCurveTo[i + 2].position - startPosition;

            float startToEndPointDistance = (endPosition - startPosition).magnitude;

            while (curvePointOffset < startToEndPointDistance)
            {
                Vector3 smoothCurvePoint = Vector3Utility.GetPointOfSmoothCurveConnectingTwoPoints(startPosition, startDirection, endPosition, endDirection, curvePointOffset / startToEndPointDistance, SmoothingCurve);

                curveRenderer.SetPosition(pointIndexToRender, smoothCurvePoint);

                pointIndexToRender++;
                curvePointOffset += DistanceBetweenCurvePoints;
            }
            curvePointOffset -= startToEndPointDistance;
        }

        curveRenderer.TrimLineToNumberOfPoints(pointIndexToRender-1);
    }
}
