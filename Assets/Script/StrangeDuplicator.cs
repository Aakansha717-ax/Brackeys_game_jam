using UnityEngine;
using System.Collections.Generic;

public class StrangeDuplicator : StrangeObjectBase
{
    [Header("Duplicator Settings")]
    public GameObject objectToDuplicate;
    public Transform[] possibleSpawnPoints;
    public int maxCopies = 3;
    public float duplicateInterval = 2f;

    private List<GameObject> copies = new List<GameObject>();
    private float duplicateTimer = 0f;

    protected override void OnObserved()
    {
        // When watched, copies fade away
        objectRenderer.material.color = Color.cyan;

        foreach (GameObject copy in copies)
        {
            if (copy != null)
            {
                copy.GetComponent<Renderer>().material.color = Color.Lerp(Color.cyan, Color.clear, 0.5f);
            }
        }
    }

    protected override void OnUnobserved()
    {
        duplicateTimer += Time.deltaTime;

        if (duplicateTimer >= duplicateInterval && copies.Count < maxCopies)
        {
            CreateDuplicate();
            duplicateTimer = 0f;
        }

        objectRenderer.material.color = Color.magenta;
    }

    protected override void OnWaiting()
    {
        // Subtle pulse
        float pulse = Mathf.Sin(Time.time * 3f) * 0.1f + 0.9f;
        objectRenderer.material.color = Color.Lerp(Color.white, Color.magenta, pulse);
    }

    void CreateDuplicate()
    {
        if (possibleSpawnPoints.Length > 0)
        {
            Transform spawnPoint = possibleSpawnPoints[Random.Range(0, possibleSpawnPoints.Length)];
            GameObject newCopy = Instantiate(objectToDuplicate, spawnPoint.position, spawnPoint.rotation);
            copies.Add(newCopy);

            // Make it semi-transparent
            Renderer copyRenderer = newCopy.GetComponent<Renderer>();
            if (copyRenderer != null)
            {
                Color copyColor = copyRenderer.material.color;
                copyColor.a = 0.6f;
                copyRenderer.material.color = copyColor;
            }
        }
    }
}