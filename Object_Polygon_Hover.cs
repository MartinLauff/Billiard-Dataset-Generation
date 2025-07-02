using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Object_Polygon_Hover : MonoBehaviour
{
    public Camera datasetCamera;
    public LayerMask segPocket;
    private GameObject hoveredObject;
    public Material lineMaterial; // Use a simple unlit color material

    void Update()
    {
        hoveredObject = null;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = datasetCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, segPocket))
        {
            hoveredObject = hit.collider.gameObject;
        }
    }

    void OnRenderObject()
    {
        if (hoveredObject == null || lineMaterial == null) return;

        MeshFilter meshFilter = hoveredObject.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null) return;

        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Count edge occurrences
        var edgeCount = new Dictionary<(int, int), int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            AddEdge(edgeCount, triangles[i], triangles[i + 1]);
            AddEdge(edgeCount, triangles[i + 1], triangles[i + 2]);
            AddEdge(edgeCount, triangles[i + 2], triangles[i]);
        }

        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.MultMatrix(hoveredObject.transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(Color.red);

        foreach (var edge in edgeCount)
        {
            if (edge.Value == 1) // only draw edges that appear once
            {
                Vector3 v1 = vertices[edge.Key.Item1];
                Vector3 v2 = vertices[edge.Key.Item2];
                GL.Vertex(v1);
                GL.Vertex(v2);
            }
        }

        GL.End();
        GL.PopMatrix();
    }

    void AddEdge(Dictionary<(int, int), int> edgeDict, int a, int b)
    {
        var edge = (Mathf.Min(a, b), Mathf.Max(a, b));
        if (edgeDict.ContainsKey(edge)) edgeDict[edge]++;
        else edgeDict[edge] = 1;
    }


    void DrawEdge(Vector3 v1, Vector3 v2)
    {
        GL.Vertex(v1);
        GL.Vertex(v2);
    }
}
