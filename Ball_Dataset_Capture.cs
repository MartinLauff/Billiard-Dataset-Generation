using UnityEngine;
using System.IO;
using System.Collections;

public class Ball_Dataset_Capture : MonoBehaviour 
{
    public Camera datasetCamera;
    public int numImages = 37000;
    private float timeBlocker = 0.2f;
    private string datasetPath;

    IEnumerator  Start() 
    {
        datasetPath = Application.dataPath + "/Ball_Dataset_Images/";
        if (!Directory.Exists(datasetPath)) {
            Directory.CreateDirectory(datasetPath);
        }
        
        for (int i = 35000; i < numImages; i++) {
            // Reposition balls
            GetComponent<Ball_Randomizer>().RandomizeBalls();  
            // Random ball hiding
            // GetComponent<Ball_Randomizer>().RandomlyHideBalls();
            // Random material application
            GetComponent<Material_Randomizer>().RandomizeMaterials();
            // Reposition Camera
            GetComponent<Camera_Randomizer>().RandomizeCamera();
            yield return new WaitForSeconds(timeBlocker);
             
            // Take screenshot
            CaptureImage(i);
            yield return new WaitForSeconds(timeBlocker);
        }
    }

    void CaptureImage(int index) 
    {
        string imagePath = datasetPath + $"billiard_{index}.png";
        
        ScreenCapture.CaptureScreenshot(imagePath);
        Debug.Log($"Saved Image: {imagePath}");

        GetComponent<Bounding_Box_Generator>().GenerateAnnotations(index);
    }
}
