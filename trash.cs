// for bounding_box_generator.cs
// private class CornerData
//     {
//         public GameObject obj;
//         public Vector2 screenPos;
//     }
 // public void GenerateCornerAnnotations(int index)
    // {
    //     string annotationFile = cornerAnnotationPath + "billiard_corners_dataset.txt";
    //     List<string> annotations = new List<string>();

    //     // Project all corners to screen and collect data
    //     List<CornerData> visibleCorners = new List<CornerData>();
    //     foreach (GameObject corner in tableCorners)
    //     {
    //         if (!corner.activeInHierarchy) continue;

    //         Vector3 screenPoint = datasetCamera.WorldToScreenPoint(corner.transform.position);

    //         if (screenPoint.z > 0) // In front of camera
    //         {
    //             visibleCorners.Add(new CornerData
    //             {
    //                 obj = corner,
    //                 screenPos = new Vector2(screenPoint.x, screenPoint.y)
    //             });
    //         }
    //     }

    //     if (visibleCorners.Count < 4)
    //     {
    //         Debug.Log("⚠️ Not all corners are visible, skipping annotation.");
    //         return;
    //     }

    //     // Sort by Y descending (top to bottom), then X ascending (left to right)
    //     var sorted = visibleCorners.OrderByDescending(c => c.screenPos.y).ThenBy(c => c.screenPos.x).ToList();

    //     Dictionary<GameObject, string> labelMap = new Dictionary<GameObject, string>
    //     {
    //         { sorted[0].obj, "top_left" },
    //         { sorted[1].obj, "top_right" },
    //         { sorted[2].obj, "bottom_left" },
    //         { sorted[3].obj, "bottom_right" }
    //     };

    //     foreach (var pair in labelMap)
    //     {
    //         GameObject corner = pair.Key;
    //         string label = pair.Value;

    //         Rect bbox = CalculateBoundingBox(corner);
    //         float ratio = CalculateScreenOverlapRatio(bbox);

    //         if (ratio < 0.5f) continue;

    //         float x_center = (bbox.x + bbox.width / 2) / Screen.width;
    //         float y_center = (bbox.y + bbox.height / 2) / Screen.height;
    //         float bbox_width = bbox.width / Screen.width;
    //         float bbox_height = bbox.height / Screen.height;

    //         annotations.Add($"table_{index}.png {label} {x_center} {y_center} {bbox_width} {bbox_height}");
    //     }

    //     if (annotations.Count > 0)
    //     {
    //         File.AppendAllLines(annotationFile, annotations);
    //         Debug.Log($"✅ [{annotations.Count}] Corner annotations saved for Image {index}.");
    //     }
    //     else
    //     {
    //         Debug.Log($"⚠️ No corners were visible in Image {index}, skipping annotation.");
    //     }
    // }