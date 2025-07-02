using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Material_Randomizer : MonoBehaviour
{
    public Renderer floor;
    public Renderer table;
    public Renderer cushions;
    public Renderer[] walls;
    public Material[] wallMaterials;
    public Material[] floorMaterials;
    public Material[] tableMaterials;

    public Material segmentationMaterial;
    public Material initialMaterial;
    public GameObject[] pocketObjects;
    private Dictionary<GameObject, Material[]> originalMaterials = new Dictionary<GameObject, Material[]>();

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
}
