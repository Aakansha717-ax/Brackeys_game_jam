using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    [Header("Target Scene")]
    public string sceneToLoad = "Room_2";  // Set different for each wall
    public int sceneBuildIndex = 1;         // Alternative

    [Header("Settings")]
    public bool useBuildIndex = false;
    public Color wallGlowColor = Color.cyan;

    private bool isTransitioning = false;

    void Start()
    {
        GetComponent<Collider>().isTrigger = true;

        // Make wall glow with its specific color
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.SetColor("_EmissionColor", wallGlowColor * 0.5f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true;

            if (useBuildIndex)
                SceneManager.LoadScene(sceneBuildIndex);
            else
                SceneManager.LoadScene(sceneToLoad);
        }
    }
}