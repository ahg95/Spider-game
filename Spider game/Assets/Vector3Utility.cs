using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Utility
{
    /// <summary>
    /// Models a smooth curve between two given points and returns the point on this curve defined by the provided interpolation value.
    /// </summary>
    /// <param name="startPoint">The point where the smooth curve should start.</param>
    /// <param name="startPointDirection">The direction in which the smooth curve should start.</param>
    /// <param name="endPoint">The point where the smooth curve should end.</param>
    /// <param name="endPointDirection">The direction in which the smooth curve should end.</param>
    /// <param name="t">The interpolation value. If t<=0 then startPoint is returned, if 1<=t then endPoint is returned, and if 0 < t < 1 then an intermediate value on the smooth curve is returned.</param>
    /// <returns>The point of the defined smooth curve at the given interpolation value.</returns>
    public static Vector3 GetPointOfSmoothCurveConnectingTwoPoints(Vector3 startPoint, Vector3 startPointDirection, Vector3 endPoint, Vector3 endPointDirection, float t, AnimationCurve smoothingCurve = null)
    {
        Vector3 smoothCurvePoint;

        if (t <= 0)
            smoothCurvePoint = startPoint;
        else if (1 <= t)
            smoothCurvePoint = endPoint;
        else // 0 < t < 1 holds
        {
            float distanceBetweenPoints = (endPoint - startPoint).magnitude;

            startPointDirection = startPointDirection.normalized * distanceBetweenPoints;
            Vector3 startPointExtension = startPoint + startPointDirection * t;

            endPointDirection = endPointDirection.normalized * distanceBetweenPoints;
            Vector3 endPointBackwardExtension = endPoint - endPointDirection * (1 - t);

            smoothCurvePoint = Vector3.Lerp(startPointExtension, endPointBackwardExtension, smoothingCurve.Evaluate(t));
        }

        return smoothCurvePoint;
    }
}
