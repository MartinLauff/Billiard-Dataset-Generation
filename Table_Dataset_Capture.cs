using UnityEngine;
using System.IO;
using System.Collections;

public class Table_Dataset_Capture : MonoBehaviour 
{
    public Camera datasetCamera;
    public Camera segmentationCamera;
    public RenderTexture segmentationTexture;
    public int numImages = 30000;
    private float timeBlocker = 0.2f;
    private string datasetPath;
    private string segmentationPath;

    IEnumerator  Start() 
    {
        datasetPath = Application.dataPath + "/Table_Dataset_Images/images/";
        segmentationPath = Application.dataPath + "/Table_Dataset_Images/seg_masks/";
        if (!Directory.Exists(datasetPath)) {
            Directory.CreateDirectory(datasetPath);
        }
        if (!Directory.Exists(segmentationPath)) {
            Directory.CreateDirectory(segmentationPath);
        }
        
        for (int i = 10000; i < numImages; i++) {
            // Reposition balls
            GetComponent<Ball_Randomizer>().RandomizeBalls();  
            // Random ball hiding
            GetComponent<Ball_Randomizer>().RandomlyHideBalls(0.8f);
            // Random material application
            GetComponent<Material_Randomizer>().RandomizeMaterials();
            // Reposition Camera
            GetComponent<Camera_Randomizer>().RandomizeCameraForTableView();
            yield return new WaitForSeconds(timeBlocker);
             

            // 1. Switch to segmentation material
            GetComponent<Material_Randomizer>().ApplySegmentationMaterial();
            yield return new WaitForSeconds(timeBlocker);

            // 2. Capture segmentation mask
            bool canContinue = CaptureSegmentation(i);

            // 3. Restore original materials
            GetComponent<Material_Randomizer>().RestoreOriginalMaterials();
            yield return new WaitForSeconds(timeBlocker);
            
            // Take screenshot
            if(canContinue) {
                CaptureImage(i);
                yield return new WaitForSeconds(timeBlocker);
            }
        }
        // for (int i = 0; i < numImages; i++) {
        //     // Reposition balls
        //     GetComponent<Ball_Randomizer>().RandomizeBalls();  
        //     // Random ball hiding
        //     GetComponent<Ball_Randomizer>().RandomlyHideBalls(0.8f);
        //     // Random material application
        //     GetComponent<Material_Randomizer>().RandomizeMaterials();
        //     // Reposition Camera
        //     GetComponent<Camera_Randomizer>().RandomizeCameraForTableView();
        //     yield return new WaitForSeconds(timeBlocker);
             
        //     // Take screenshot
        //     bool canContinue = CaptureImage(i);

        //     if(canContinue) {
        //     // 1. Switch to segmentation material
        //     GetComponent<Material_Randomizer>().ApplySegmentationMaterial();

        //     // 2. Capture segmentation mask
        //     CaptureSegmentation(i);

        //     // 3. Restore original materials
        //     GetComponent<Material_Randomizer>().RestoreOriginalMaterials();
        //     }
        //     yield return new WaitForSeconds(timeBlocker);
        // }
    }

    void CaptureImage(int index) 
    {
        if (datasetCamera == null)
        {
            Debug.LogError("Main camera is not set!");
            return;
        }
        string imagePath = datasetPath + $"table_{index}.png";

        ScreenCapture.CaptureScreenshot(imagePath);
        Debug.Log($"Saved Image: {imagePath}");
    }

    bool CaptureSegmentation(int index) {
        
        if (segmentationCamera == null || segmentationTexture == null)
        {
            Debug.LogError("Segmentation camera or texture is not set!");
            return false;
        }
        bool isAnnotated = GetComponent<Bounding_Box_Generator>().GenerateTableAnnotations(index);

        if(isAnnotated) {
            string segImagePath = segmentationPath + $"table_mask_{index}.png";
            segmentationCamera.targetTexture = segmentationTexture;
            segmentationCamera.Render();

            RenderTexture.active = segmentationTexture;
            Texture2D segImage = new Texture2D(segmentationTexture.width, segmentationTexture.height, TextureFormat.RGB24, false);
            segImage.ReadPixels(new Rect(0, 0, segmentationTexture.width, segmentationTexture.height), 0, 0);
            segImage.Apply();

            byte[] bytes = segImage.EncodeToPNG();
            File.WriteAllBytes(segImagePath, bytes);

            segmentationCamera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(segImage);

            Debug.Log($"Saved segmentation mask: {segImagePath}");
        }
        
        return isAnnotated;
    }
}
