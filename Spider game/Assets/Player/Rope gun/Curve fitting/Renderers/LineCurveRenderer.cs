using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineCurveRenderer : CurveRendererBase
{
    LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    protected override void SetPosition(int linePointIndex, Vector3 position)
    {
        if (lineRenderer.positionCount <= linePointIndex)
            lineRenderer.positionCount = linePointIndex + 1;

        lineRenderer.SetPosition(linePointIndex, position);
    }

    protected override void TrimLineToNumberOfPoints(int nrOfPoints)
    {
        lineRenderer.positionCount = nrOfPoints;
    }

    protected override void RenderCurve()
    {
        // The lineRenderer already renders the curve automatically.
    }
}
