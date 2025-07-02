using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class Bounding_Box_Generator : MonoBehaviour
{
    public Camera datasetCamera;
    public GameObject[] billiardBalls;
    public GameObject[] tableCorners;
    private string ballAnnotationPath;
    private string TableAnnotationPath;

    void Start()
    {
        // create directory for ball annotations
        ballAnnotationPath = Application.dataPath + "/Ball_Annotations/";
        if (!Directory.Exists(ballAnnotationPath))
        {
            Directory.CreateDirectory(ballAnnotationPath);
        }

        // create directory for table annotations
        TableAnnotationPath = Application.dataPath + "/Table_Annotations/";
        if (!Directory.Exists(TableAnnotationPath))
        {
            Directory.CreateDirectory(TableAnnotationPath);
        }
    }

    public void GenerateAnnotations(int index)
    {
        string annotationFile = ballAnnotationPath + "billiard_dataset.txt";
        List<string> annotations = new List<string>();

        foreach (GameObject ball in billiardBalls)
        {
            // ✅ Skip balls that are hidden (inactive)
            if (!ball.activeInHierarchy) continue;

           // Calculate precise bounding box using mesh vertices
            Rect borderRect = CalculateBoundingBox(ball);

            // Ensure valid bounding box
            if (borderRect.width > 0 && borderRect.height > 0)
            {
                float visibleRatio = CalculateScreenOverlapRatio(borderRect);
    
                if (visibleRatio < 0.5f)
                {
                    Debug.Log($"⚠️ Skipping {ball.name} – only {Mathf.RoundToInt(visibleRatio * 100)}% of bbox is on-screen.");
                    continue;
                }

                // Convert bounding box from screen space to normalized YOLO format
                float x_center = (borderRect.x + borderRect.width / 2) / Screen.width;
                float y_center = (borderRect.y + borderRect.height / 2) / Screen.height;
                float bbox_width = borderRect.width / Screen.width;
                float bbox_height = borderRect.height / Screen.height;

                // Format: filename class x_center y_center width height
                annotations.Add($"billiard_{index}.png {ball.name} {x_center} {y_center} {bbox_width} {bbox_height}");
            }
        }

         // Append annotations only if at least one ball was visible
        if (annotations.Count > 0)
        {
            File.AppendAllLines(annotationFile, annotations);
            Debug.Log($"✅ [{annotations.Count}] Annotations saved for Image {index}.");
        }
        else
        {
            Debug.Log($"⚠️ [{annotations.Count}] balls were visible in Image {index}, skipping annotation.");
            // Debug.Log($"⚠️ No balls were visible in Image {index}, skipping annotation.");
        }
    }

    public bool GenerateTableAnnotations(int index)
    {
        string annotationFile = TableAnnotationPath + "table_dataset.txt";
        List<string> annotations = new List<string>();

        foreach (GameObject corner in tableCorners)
        {
            if (!corner.activeInHierarchy) continue;

            Rect bbox = CalculateBoundingBox(corner);

            if (bbox.width > 0 && bbox.height > 0) // In front of camera
            {
                float ratio = CalculateScreenOverlapRatio(bbox);
                
                if (ratio < 0.3f) continue;

                float x_center = (bbox.x + bbox.width / 2) / Screen.width;
                float y_center = (bbox.y + bbox.height / 2) / Screen.height;
                float bbox_width = bbox.width / Screen.width;
                float bbox_height = bbox.height / Screen.height;

                annotations.Add($"table_{index}.png {corner.name} {x_center} {y_center} {bbox_width} {bbox_height}");
            }
        }

        if (annotations.Count > 1)
        {
            // Identify all pocket center points
            // List<Vector2> pocketCenters = new List<Vector2>();

            // foreach (var ann in annotations)
            // {
            //     var parts = ann.Split(' ');
            //     if (parts.Length < 6) continue;

            //     string label = parts[1].ToLower();
            //     if (label.Contains("1")) // Assuming label "1" is for pockets
            //     {
            //         float xc = float.Parse(parts[2]);
            //         float yc = float.Parse(parts[3]);
            //         pocketCenters.Add(new Vector2(xc, yc));
            //     }
            // }

            // if (pocketCenters.Count == 6) // Only if all 6 pockets are present
            // {
            //     float minX = pocketCenters.Min(p => p.x);
            //     float maxX = pocketCenters.Max(p => p.x);
            //     float minY = pocketCenters.Min(p => p.y);
            //     float maxY = pocketCenters.Max(p => p.y);

            //     float tableX = (minX + maxX) / 2f;
            //     float tableY = (minY + maxY) / 2f;
            //     float tableW = maxX - minX;
            //     float tableH = maxY - minY;

            //     annotations.Insert(0, $"table_{index}.png 0 {tableX} {tableY} {tableW} {tableH}");
            //     Debug.Log($"✅ Table surface bbox added using pocket centers.");
            // }

            File.AppendAllLines(annotationFile, annotations);
            Debug.Log($"✅ [{annotations.Count}] Corner annotations saved for Image {index}.");
            return true;
        }
        else
        {
            Debug.Log($"⚠️ [{annotations.Count}] corners were visible in Image {index}, skipping annotation.");
            return false;
        }
    }

    // 🔹 Calculate accurate bounding box from the object's MeshFilter
    private Rect CalculateBoundingBox(GameObject obj)
    {
        if (obj == null) return new Rect(0, 0, 0, 0);

        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null) return new Rect(0, 0, 0, 0);

        Mesh mesh = meshFilter.sharedMesh;

        // Initialize min/max screen coordinates
        Vector2 minScreen = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 maxScreen = new Vector2(float.MinValue, float.MinValue);

        // Loop through all vertices of the mesh
        foreach (Vector3 vertex in mesh.vertices)
        {
            // Convert world position to screen position
            Vector3 worldVertex = obj.transform.TransformPoint(vertex);
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
            return new Rect(minScreen.x, Screen.height - maxScreen.y, maxScreen.x - minScreen.x, maxScreen.y - minScreen.y);
        }

        return new Rect(0, 0, 0, 0); // Return empty rectangle if invalid
    }

    private float CalculateScreenOverlapRatio(Rect bbox)
    {
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        
        float xOverlap = Mathf.Max(0, Mathf.Min(bbox.xMax, screenRect.xMax) - Mathf.Max(bbox.xMin, screenRect.xMin));
        float yOverlap = Mathf.Max(0, Mathf.Min(bbox.yMax, screenRect.yMax) - Mathf.Max(bbox.yMin, screenRect.yMin));
        
        float overlapArea = xOverlap * yOverlap;
        float bboxArea = bbox.width * bbox.height;

        if (bboxArea == 0) return 0f;
        return overlapArea / bboxArea;
    }

}
