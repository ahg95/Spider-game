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

    protected override void RenderCurve()
    {
        UpdateMeshFromRenderPositions();
    }

    void UpdateMeshFromRenderPositions()
    {
        GetMesh().Clear();

        GetMesh().vertices = CalculateVertices();

        GetMesh().triangles = CalculateTriangles();

        GetMesh().Optimize();
    }

    private Vector3[] CalculateVertices()
    {
        List<Vector3> vertices = new List<Vector3>();

        if (1 < renderPositions.Count )
        {
            vertices.Capacity = CalculateTotalNumberOfVertices();

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

    private int[] CalculateTriangles()
    {
        List<int> triangles = new List<int>();

        triangles.Capacity = CalculateTotalNumberOfTriangles() * 3;

        triangles.AddRange(CalculateTrianglesBetweenRingAndVertex(0, 0, true));

        // Connect all rings with each other
        for (int i = 0; i < renderPositions.Count - 1; i++)
            triangles.AddRange(CalculateTrianglesBetweenRings(i, i + 1));

        triangles.AddRange(CalculateTrianglesBetweenRingAndVertex(renderPositions.Count - 1, CalculateTotalNumberOfVertices() - 1, false));

        return triangles.ToArray();
    }

    private int CalculateTotalNumberOfVertices() {
        // There are renderPositions.Count rings, and each one has NumberOfVerticesPerRing vertices.
        // In addition, there are two vertices for the end caps of the curve.
        return (renderPositions.Count * NumberOfVerticesPerRing + 2);
    }

    private int CalculateTotalNumberOfTriangles()
    {
        // At each end of the rope we have NumberOfVerticesPerRing triangles that connect the first and last ring
        // with the first and last cap vertex respectively. In addition, each ring segment has
        // NumberOfVerticesPerRing * 2 triangles, and we have (renderPositions.Count - 1) ring segments. This
        // Calculation can be simplified to the following term:
        return (renderPositions.Count * NumberOfVerticesPerRing *2);
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

            vertexPositionOffset = Quaternion.AngleAxis(-angleOffset, direction) * vertexPositionOffset;
        }

        return ringVertices;
    }

    private List<int> CalculateTrianglesBetweenRings(int ringIndexA, int ringIndexB)
    {
        List<int> triangles = new List<int>();
        triangles.Capacity = NumberOfVerticesPerRing * 2 * 3;

        // Calculate all triangles that point towards the second ring
        for (int i = 0; i < NumberOfVerticesPerRing; i++)
        {
            // Triangle pointing from ring A to ring B
            triangles.Add(GetIndexOfRingVertexInMesh(ringIndexA, i));
            triangles.Add(GetIndexOfRingVertexInMesh(ringIndexB, i));
            triangles.Add(GetIndexOfRingVertexInMesh(ringIndexA, i+1));

            // Triangle pointing from ring B to ring A
            triangles.Add(GetIndexOfRingVertexInMesh(ringIndexB, i));
            triangles.Add(GetIndexOfRingVertexInMesh(ringIndexB, i+1));
            triangles.Add(GetIndexOfRingVertexInMesh(ringIndexA, i+1));
        }

        return triangles;
    }

    private List<int> CalculateTrianglesBetweenRingAndVertex(int ringIndex, int vertexIndex, bool calculateClockwise)
    {
        List<int> triangles = new List<int>();
        triangles.Capacity = NumberOfVerticesPerRing * 3;

        for (int i = 0; i < NumberOfVerticesPerRing; i++)
        {
            triangles.Add(vertexIndex);

            if (calculateClockwise)
            {
                triangles.Add(GetIndexOfRingVertexInMesh(ringIndex, i));
                triangles.Add(GetIndexOfRingVertexInMesh(ringIndex, i + 1));
            } else
            {
                triangles.Add(GetIndexOfRingVertexInMesh(ringIndex, i + 1));
                triangles.Add(GetIndexOfRingVertexInMesh(ringIndex, i));
            }

        }

        return triangles;
    }

    /// <summary>
    /// Given the index of a ring and the index of a vertex in that ring, the function returns the index of that vertex in the vertices array of the mesh. If
    /// the index of the vertex in that ring is out of bounds then it performs a modulo operation to keep it inside the bounds. Same applies to the ringIndex.
    /// </summary>
    /// <param name="ringIndex"> </param>
    /// <param name="vertexIndex"></param>
    /// <returns></returns>
    private int GetIndexOfRingVertexInMesh(int ringIndex, int vertexIndex)
    {
        ringIndex %= renderPositions.Count;
        vertexIndex %= NumberOfVerticesPerRing;

        return (1 + ringIndex * NumberOfVerticesPerRing + vertexIndex);
    }

    private void PrintTrianglePositionsInConsole()
    {
        for (int i = 0; i < GetMesh().triangles.Length / 3; i++)
        {
            Vector3 v1 = GetMesh().vertices[GetMesh().triangles[i * 3]];
            Vector3 v2 = GetMesh().vertices[GetMesh().triangles[i * 3 + 1]];
            Vector3 v3 = GetMesh().vertices[GetMesh().triangles[i * 3 + 2]];

            Debug.Log(i + ". Triangle: " + v1 + ", " + v2 + ", " + v3);
        }
    }
}
