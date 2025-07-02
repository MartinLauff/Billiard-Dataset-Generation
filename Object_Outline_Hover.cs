using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class Object_Outline_Hover : MonoBehaviour
{
    public Camera datasetCamera;
    public LayerMask ballLayer; // Assign a layer to the billiard balls in Unity
    private GameObject hoveredObject;
    private Rect borderRect; // Store the bounding box for OnGUI

    void Update()
    {
        hoveredObject = null; // Reset hovered object
        borderRect = new Rect(0, 0, 0, 0); // Reset bounding box

        // Get mouse position using the new Input System
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = datasetCamera.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ballLayer))
        {
            if (hit.collider != null)
            {
                hoveredObject = hit.collider.gameObject;
                CalculateBoundingBox();
            }
        }
    }

    void CalculateBoundingBox()
    {
        if (hoveredObject == null) return;

        // Get the object's mesh
        MeshFilter meshFilter = hoveredObject.GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null) return;

        // Initialize min/max screen coordinates
        Vector2 minScreen = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 maxScreen = new Vector2(float.MinValue, float.MinValue);

        // Loop through all vertices of the mesh
        foreach (Vector3 vertex in mesh.vertices)
        {
            // Convert world position to screen position
            Vector3 worldVertex = hoveredObject.transform.TransformPoint(vertex);
            Vector3 screenVertex = datasetCamera.WorldToScreenPoint(worldVertex);

            // Update min/max x and y coordinates
            if (screenVertex.z > 0) // Ignore vertices behind the camera
            {
                minScreen.x = Mathf.Min(minScreen.x, screenVertex.x);
                minScreen.y = Mathf.Min(minScreen.y, screenVertex.y);
                maxScreen.x = Mathf.Max(maxScreen.x, screenVertex.x);
                maxScreen.y = Mathf.Max(maxScreen.y, screenVertex.y);
            }
        }

        // Ensure valid bounding box
        if (minScreen.x < maxScreen.x && minScreen.y < maxScreen.y)
        {
            borderRect = new Rect(minScreen.x, Screen.height - maxScreen.y, maxScreen.x - minScreen.x, maxScreen.y - minScreen.y);
        }
    }

    void OnGUI()
    {
        if (hoveredObject != null && borderRect.width > 0 && borderRect.height > 0)
        {
            DrawBoundingBox(borderRect, Color.red, 2f);
        }
    }

    // Draws only the border using GUI
    void DrawBoundingBox(Rect rect, Color color, float thickness)
    {
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), Texture2D.whiteTexture); // Top
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture); // Bottom
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), Texture2D.whiteTexture); // Left
        GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), Texture2D.whiteTexture); // Right
    }
}
