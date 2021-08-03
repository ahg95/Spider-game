using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Given a model of a smooth curve, this class renders a tube-like mesh that follows the smooth curve.
/// </summary>
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class MeshCurveRenderer : CurveRendererBase
{
    [Range(3, 64)]
    [Tooltip("The mesh is made up of individual rings of vertices, and this number specifies how many vertices each ring has.")]
    public int NumberOfVerticesPerRing;

    [Range(0.1f, 10f)]
    [Tooltip("The mesh is made up of individual rings of vertices, and this is their radius.")]
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

    /// <summary>
    /// This function completely upodates the mesh based on the points to render.
    /// </summary>
    void UpdateMeshFromRenderPositions()
    {
        GetMesh().Clear();

        GetMesh().vertices = CalculateVertices();

        GetMesh().triangles = CalculateTriangles();

        GetMesh().RecalculateNormals();

        GetMesh().Optimize();
    }


    /// <summary>
    /// Calculates and returns the positions of all vertices for the mesh that follows the smooth curve.
    /// </summary>
    /// <returns></returns>
    private Vector3[] CalculateVertices()
    {
        List<Vector3> vertices = new List<Vector3>();

        if (1 < renderPositions.Count)
        {
            vertices.Capacity = CalculateTotalNumberOfVertices();

            // Add the cap vertex at the start of the curve
            vertices.Add(renderPositions[0]);

            // Calculate all vertices that are part of a ring
            for (int i = 0; i < renderPositions.Count; i++)
            {
                Vector3 ringDirection = CalculateDirectionOfRingAtIndex(i);

                vertices.AddRange(CalculateVerticesForRingAtPositionInDirection(renderPositions[i], ringDirection));
            }

            // Add the cap vertex at the end of the curve
            vertices.Add(renderPositions[renderPositions.Count - 1]);
        }

        return vertices.ToArray();
    }

    private int[] CalculateTriangles()
    {
        List<int> triangles = new List<int>();

        if (1 < renderPositions.Count)
        {
            // Each triangle consists of 3 integers
            triangles.Capacity = CalculateTotalNumberOfTriangles() * 3;

            // Connect the cap vertex at the start of the curve with the first vertex ring.
            triangles.AddRange(CalculateTrianglesBetweenRingAndVertex(0, 0, true));

            // Connect all vertex rings with each other
            for (int i = 0; i < renderPositions.Count - 1; i++)
                triangles.AddRange(CalculateTrianglesBetweenRingsAtIndex(i, i + 1));

            // Connect the cap vertex at the end of the curve with the last vertex ring.
            triangles.AddRange(CalculateTrianglesBetweenRingAndVertex(renderPositions.Count - 1, CalculateTotalNumberOfVertices() - 1, false));
        }

        return triangles.ToArray();
    }

    private int CalculateTotalNumberOfVertices()
    {
        int totalNumberOfVertices = 0;

        // There are renderPositions.Count rings, and each one has NumberOfVerticesPerRing vertices.
        // In addition, there are two vertices for the end caps of the curve.
        if (1 < renderPositions.Count)
            totalNumberOfVertices = (renderPositions.Count * NumberOfVerticesPerRing + 2);

        return totalNumberOfVertices;
    }

    /// <summary>
    /// Returns in which direction the ring of vertices at the render position at the given <paramref name="index"/> should face.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    Vector3 CalculateDirectionOfRingAtIndex(int index)
    {
        Vector3 ringDirection;

        if (index == 0)
            ringDirection = renderPositions[1] - renderPositions[0];
        else if (index == renderPositions.Count - 1)
            ringDirection = renderPositions[renderPositions.Count - 1] - renderPositions[renderPositions.Count - 2];
        else
            ringDirection = renderPositions[index + 1] - renderPositions[index - 1];

        return ringDirection;
    }

    /// <summary>
    /// Given a <paramref name="position"/> and a <paramref name="direction"/>, this function returns a list of vertex positions 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private List<Vector3> CalculateVerticesForRingAtPositionInDirection(Vector3 position, Vector3 direction)
    {
        List<Vector3> ringVertices = new List<Vector3>();
        ringVertices.Capacity = NumberOfVerticesPerRing;

        // Calculate a vector perpendicular to the direction the ring faces and the direction of the sky. This vector faces to the right if you look in the given direction.
        Vector3 vertexPositionOffset = Vector3.Cross(direction, Vector3.up).normalized * RingRadius;

        float angularOffsetPerVertex = 360 / NumberOfVerticesPerRing;

        for (int i = 0; i < NumberOfVerticesPerRing; i++)
        {
            ringVertices.Add(position + vertexPositionOffset);

            // Rotate the offset vector for the next vertex
            vertexPositionOffset = Quaternion.AngleAxis(-angularOffsetPerVertex, direction) * vertexPositionOffset;
        }

        return ringVertices;
    }

    private int CalculateTotalNumberOfTriangles()
    {
        int totalNumberOfTriangles = 0;

        // At each end of the rope we have NumberOfVerticesPerRing triangles that connect the first and last ring
        // with the first and last cap vertex respectively. In addition, each ring segment has
        // NumberOfVerticesPerRing * 2 triangles, and we have (renderPositions.Count - 1) ring segments. This
        // Calculation can be simplified to the following term:
        if (1 < renderPositions.Count)
            totalNumberOfTriangles = (renderPositions.Count * NumberOfVerticesPerRing * 2);

        return totalNumberOfTriangles;
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
            }
            else
            {
                triangles.Add(GetIndexOfRingVertexInMesh(ringIndex, i + 1));
                triangles.Add(GetIndexOfRingVertexInMesh(ringIndex, i));
            }

        }

        return triangles;
    }

    /// <summary>
    /// Given two positions specified by two given indeces for render positions, this function calculates the triangles between the vertex rings at these positions and returns them.
    /// </summary>
    /// <param name="ringIndexA"> The index for a render position where the first vertex ring lies that should be connected with the other vertex ring. </param>
    /// <param name="ringIndexB"> The index for a render position where the second vertex ring lies that should be connected with the other vertex ring. </param>
    /// <returns></returns>
    private List<int> CalculateTrianglesBetweenRingsAtIndex(int ringIndexA, int ringIndexB)
    {
        List<int> triangles = new List<int>();

        // Each vertex in a ring is connected with 2 different triangles, and each triangle is made up of 3 vertices.
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
