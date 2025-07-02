using System.Collections;
using System.IO;
using UnityEngine;

public class Camera_Randomizer : MonoBehaviour
{
    public Camera datasetCamera;
    public Camera segmentationCamera;
    public Transform tableCenter;

    // Ball detection range
    // public Vector3 ballViewMin = new Vector3(-1.3f, 0f, -1f);
    // public Vector3 ballViewMax  = new Vector3(1.3f, 1f, 1f);
    public Vector3 ballViewMin = new Vector3(-1f, 0.1f, -0.5f);
    public Vector3 ballViewMax  = new Vector3(1f, 0.3f, 0.5f);

    // Table view range
    public Vector3 tableViewMin = new Vector3(-0.5f, 1.8f, -0.5f);
    public Vector3 tableViewMax = new Vector3(0.5f, 2.3f, 0.5f);

    // Ball-focused camera positioning
    public void RandomizeCamera()
    {
        // Set random position
        datasetCamera.transform.position = new Vector3(
            Random.Range(ballViewMin.x, ballViewMax.x),
            Random.Range(ballViewMin.y, ballViewMax.y),
            Random.Range(ballViewMin.z, ballViewMax.z)
        );

        // Make the camera always look at the table center
        datasetCamera.transform.LookAt(tableCenter);
    }

    // Table-focused camera positioning
    public void RandomizeCameraForTableView()
    {
        // Randomize only once
        Vector3 newPosition = new Vector3(
            Random.Range(ballViewMin.x, ballViewMax.x),
            Random.Range(ballViewMin.y, ballViewMax.y),
            Random.Range(ballViewMin.z, ballViewMax.z)
        );
        // Vector3 newPosition = new Vector3(
        //     Random.Range(tableViewMin.x, tableViewMax.x),
        //     Random.Range(tableViewMin.y, tableViewMax.y),
        //     Random.Range(tableViewMin.z, tableViewMax.z)
        // );

        // Apply same transform to both cameras
        datasetCamera.transform.position = newPosition;
        segmentationCamera.transform.position = newPosition;

        // Make the camera always look at the table center
        datasetCamera.transform.LookAt(tableCenter);
        segmentationCamera.transform.LookAt(tableCenter);
    }
}
