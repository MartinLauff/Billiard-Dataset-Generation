using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Material_Randomizer : MonoBehaviour
{
    public Renderer floor;    // Assign the floor object
    public Renderer table;    // Assign the table object
    public Renderer cushions; // Assign the cushions object
    public Renderer[] walls;  // Assign all wall objects
    public Material[] wallMaterials;  // Assign different wall materials
    public Material[] floorMaterials; // Assign different floor materials
    public Material[] tableMaterials; // Assign different table materials

    public Material segmentationMaterial;
    public Material initialMaterial;
    public GameObject[] pocketObjects;
    private Dictionary<GameObject, Material[]> originalMaterials = new Dictionary<GameObject, Material[]>();

    // void Start()
    // {        
    //     if(pocketObjects.Length != 6) {
    //         Debug.LogError("Not enough pockets! " + pocketObjects.Length);
    //         return;
    //     }
    //     // Store original materials
    //     foreach (GameObject obj in pocketObjects)
    //     {
    //         Renderer renderer = obj.GetComponent<Renderer>();
    //         if (renderer != null)
    //         {
    //             originalMaterials[obj] = renderer.sharedMaterials;
    //         }
    //     }
    // }

    public void RandomizeMaterials()
    {
        // Randomly choose a material for the walls
        Material selectedWallMaterial = wallMaterials[Random.Range(0, wallMaterials.Length)];

        // Apply the selected material to all walls
        foreach (Renderer wall in walls)
        {
            wall.material = selectedWallMaterial;
        }

        // Randomly choose and apply a floor material
        Material selectedFloorMaterial = floorMaterials[Random.Range(0, floorMaterials.Length)];
        floor.material = selectedFloorMaterial;
        
        // Randomly choose and apply a table material
        Material selectedTableMaterial = tableMaterials[Random.Range(0, tableMaterials.Length)];
        table.material = selectedTableMaterial;
        cushions.material = selectedTableMaterial;
    }
    
    public void ApplySegmentationMaterial()
    {
        foreach (GameObject obj in pocketObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Store original materials (only once)
                if (!originalMaterials.ContainsKey(obj))
                {
                    originalMaterials[obj] = renderer.materials;
                }

                // Apply segmentation material to all submeshes
                Material[] segMats = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < segMats.Length; i++)
                    segMats[i] = segmentationMaterial;

                renderer.materials = segMats;
            }
        }
    }
    
    public void RestoreOriginalMaterials()
    {
        foreach (GameObject obj in pocketObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
           if (renderer != null)
            {
                 // Apply segmentation material to all submeshes
                Material[] initMats = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < initMats.Length; i++)
                    initMats[i] = initialMaterial;

                renderer.materials = initMats;
            }
        }
    }
    // public void RestoreOriginalMaterials()
    // {
    //     foreach (GameObject obj in pocketObjects)
    //     {
    //         Renderer renderer = obj.GetComponent<Renderer>();
    //        if (renderer != null && originalMaterials.ContainsKey(obj))
    //         {
    //             renderer.materials = originalMaterials[obj];
    //         }
    //     }
    // }
}
