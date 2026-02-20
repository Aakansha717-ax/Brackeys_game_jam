using UnityEngine;
using System.Collections.Generic;

public class ObjectCombiner : MonoBehaviour
{
    [Header("Combine Settings")]
    public bool combineOnStart = true;
    public Material targetMaterial;

    void Start()
    {
        if (combineOnStart)
            CombineObjects();
    }

    [ContextMenu("Combine Objects")]
    void CombineObjects()
    {
        // Get all child renderers
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<Material> materials = new List<Material>();

        // Create combine instances
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            // Use target material or original
            if (targetMaterial != null)
                meshFilters[i].GetComponent<Renderer>().material = targetMaterial;
        }

        // Create combined mesh
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        // Add to this object
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshFilter.mesh = combinedMesh;

        // Add renderer
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer == null)
            renderer = gameObject.AddComponent<MeshRenderer>();

        renderer.material = targetMaterial ?? meshFilters[0].GetComponent<Renderer>().sharedMaterial;

        // Disable original objects
        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.gameObject != gameObject)
                mf.gameObject.SetActive(false);
        }

        Debug.Log($"Combined {meshFilters.Length} objects into 1!");
    }
}