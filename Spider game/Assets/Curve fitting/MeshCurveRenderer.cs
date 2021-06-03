using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class MeshCurveRenderer : CurveRendererBase
{
    [Range(3, 64)]
    public int NumberOfVerticesPerRing;

    [Range(0.1f, 10f)]
    public float RingRadius;

    private List<Vector3> renderPositions = new List<Vector3>();
    private MeshFilter meshFilter;

    public MeshFilter GetMeshFilter()
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        return meshFilter;
    }

    public Mesh GetMesh()
    {
        if (GetMeshFilter().mesh == null)
            GetMeshFilter().mesh = new Mesh();

        return GetMeshFilter().mesh;
    }

    protected override void SetPosition(int linePointIndex, Vector3 position)
    {
        if (renderPositions.Count <= linePointIndex)
        {
            renderPositions.Capacity = linePointIndex + 1;

            while (renderPositions.Count <= linePointIndex)
            {
                renderPositions.Add(Vector3.zero);
            }
        }

        renderPositions[linePointIndex] = position;
    }

    protected override void TrimLineToNumberOfPoints(int nrOfPoints)
    {
        if (nrOfPoints < renderPositions.Count) 
            renderPositions.RemoveRange(nrOfPoints, renderPositions.Count - nrOfPoints);
    }

    private void UpdateMeshFilterMeshFromRenderPositions()
    {
        GetMesh().Clear();

        GetMesh().vertices = CalculateVertices();
        GetMesh().triangles = CalculateTriangles();
    }

    private Vector3[] CalculateVertices()
    {
        List<Vector3> vertices = new List<Vector3>();

        if (1 < renderPositions.Count )
        {
            vertices.Capacity = CalculateNumberOfVertices();

            // Add the cap at the start of the curve
            vertices.Add(renderPositions[0]);

            for (int i = 0; i < renderPositions.Count; i++)
            {
                Vector3 ringDirection;

                if (i == 0)
                    ringDirection = renderPositions[1] - renderPositions[0];
                else if (i == renderPositions.Count - 1)
                    ringDirection = renderPositions[renderPositions.Count - 1] - renderPositions[renderPositions.Count - 2];
                else
                    ringDirection = renderPositions[i + 1] - renderPositions[i - 1];

                vertices.AddRange(CalculateVerticesForRingAtPositionInDirection(renderPositions[i], ringDirection));
            }

            // Add the cap at the end of the curve
            vertices.Add(renderPositions[renderPositions.Count - 1]);
        }

        return vertices.ToArray();
    }

    private int CalculateNumberOfVertices() {
        // There are renderPositions.Count rings, and each one has NumberOfVerticesPerRing vertices.
        // In addition, there are two vertices for the end caps of the curve.
        return renderPositions.Count * NumberOfVerticesPerRing +2;
    }

    private List<Vector3> CalculateVerticesForRingAtPositionInDirection(Vector3 position, Vector3 direction)
    {
        List<Vector3> ringVertices = new List<Vector3>();
        ringVertices.Capacity = NumberOfVerticesPerRing;

        Vector3 vertexPositionOffset = Vector3.Cross(direction, Vector3.up).normalized * RingRadius;
        float angleOffset = 360 / NumberOfVerticesPerRing;

        for (int i = 0; i < NumberOfVerticesPerRing; i++)
        {
            ringVertices.Add(position + vertexPositionOffset);

            vertexPositionOffset = Quaternion.AngleAxis(angleOffset, direction) * vertexPositionOffset;
        }

        return ringVertices;
    }

    private int[] CalculateTriangles()
    {
        List<int> triangles = new List<int>();

        // At each end of the rope we have NumberOfVerticesPerRing triangles that connect the first and last ring
        // with the first and last cap vertex respectively. In addition, each ring segment has
        // NumberOfVerticesPerRing * 2 triangles, and we have (renderPositions.Count - 1) ring segments. This
        // Calculation can be simplified to the following term:
        int numberOfTriangles = renderPositions.Count * NumberOfVerticesPerRing * 2;
        triangles.Capacity = numberOfTriangles * 3;

        // Connect the first cap vertex with the first ring
        for (int i = 0; i < NumberOfVerticesPerRing; i++)
        {
            triangles.Add(0);
            triangles.Add(1 + i);
            triangles.Add(2 + i);
        }

        // Connect all rings with each other
        for (int i = 0; i < renderPositions.Count - 1; i++)
        {
            triangles.AddRange(CalculateTrianglesForRingSegment(i));
        }

        int numberOfVertices = CalculateNumberOfVertices();

        // Connect the last cap vertex with the last ring
        for (int i = 0; i < NumberOfVerticesPerRing; i++)
        {
            triangles.Add(numberOfVertices - 1);
            triangles.Add(numberOfVertices - 2 - i);
            triangles.Add(numberOfVertices - 3 - i);
        }

        return triangles.ToArray();
    }

    private List<int> CalculateTrianglesForRingSegment(int ringSegmentindex)
    {
        List<int> triangles = new List<int>();
        triangles.Capacity = NumberOfVerticesPerRing * 2 * 3;

        int vertexIndexOffset = 1 + NumberOfVerticesPerRing * ringSegmentindex;

        // Calculate all triangles that point towards the second ring
        for (int i = 0; i < NumberOfVerticesPerRing; i++)
        {
            triangles.Add(vertexIndexOffset + i);
            triangles.Add(vertexIndexOffset + i + NumberOfVerticesPerRing);
            triangles.Add(vertexIndexOffset + i + 1);
        }

        // Calculate all triangles that point towards the first ring
        for (int i = 0; i < NumberOfVerticesPerRing; i++)
        {
            triangles.Add(vertexIndexOffset + NumberOfVerticesPerRing + i);
            triangles.Add(vertexIndexOffset + NumberOfVerticesPerRing + i + 1);
            triangles.Add(vertexIndexOffset + i + 1);
        }

        return triangles;
    }

    protected override void RenderCurve()
    {
        UpdateMeshFilterMeshFromRenderPositions();
    }
}
