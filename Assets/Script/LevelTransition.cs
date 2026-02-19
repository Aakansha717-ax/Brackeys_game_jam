using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public string nextLevelName = "Room_2";  // Name of next scene
    public bool useLevelIndex = false;
    public int nextLevelIndex = 1;           // Build index (if not using name)

    [Header("Effects")]
    public float fadeOutTime = 1f;
    public float fadeInTime = 1f;
    public Color fadeColor = Color.black;

    [Header("Audio")]
    public AudioClip transitionSound;
    public float soundVolume = 0.5f;

    [Header("Visual Feedback")]
    public Color glowColor = Color.cyan;
    public float glowSpeed = 2f;

    private bool isTransitioning = false;
    private AudioSource audioSource;
    private Renderer wallRenderer;
    private Material originalMaterial;

    void Start()
    {
        // Get components
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && transitionSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();

        wallRenderer = GetComponent<Renderer>();
        if (wallRenderer != null)
        {
            originalMaterial = wallRenderer.material;
        }

        // Make sure collider is a trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
        else
            Debug.LogError("LevelTransition needs a Collider component!");
    }

    void Update()
    {
        // Make the wall pulse slightly to indicate it's special
        if (wallRenderer != null && !isTransitioning)
        {
            float pulse = Mathf.Sin(Time.time * glowSpeed) * 0.3f + 0.7f;
            wallRenderer.material.SetColor("_EmissionColor", glowColor * pulse);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            StartCoroutine(TransitionToNextLevel());
        }
    }

    IEnumerator TransitionToNextLevel()
    {
        isTransitioning = true;

        // Play sound
        if (audioSource != null && transitionSound != null)
        {
            audioSource.PlayOneShot(transitionSound, soundVolume);
        }

        // Visual feedback - wall glows bright
        if (wallRenderer != null)
        {
            wallRenderer.material.SetColor("_EmissionColor", glowColor * 5f);
        }

        // Try to find SceneFader - but don't require it
        GameObject faderObject = GameObject.Find("SceneFader");
        if (faderObject != null)
        {
            // If you have a SceneFader script, call it here
            // For now, just do simple fade
        }

        // Simple fade out
        yield return StartCoroutine(SimpleFadeOut());

        // Load next level
        if (useLevelIndex)
        {
            SceneManager.LoadScene(nextLevelIndex);
        }
        else
        {
            SceneManager.LoadScene(nextLevelName);
        }
    }

    IEnumerator SimpleFadeOut()
    {
        // Create a temporary black screen
        GameObject fadeCanvas = new GameObject("FadeCanvas");
        Canvas canvas = fadeCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        GameObject fadeImage = new GameObject("FadeImage");
        fadeImage.transform.parent = fadeCanvas.transform;

        UnityEngine.UI.Image image = fadeImage.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
        image.rectTransform.anchorMin = Vector2.zero;
        image.rectTransform.anchorMax = Vector2.one;
        image.rectTransform.sizeDelta = Vector2.zero;

        // Fade to black
        float elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsed / fadeOutTime);
            image.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        image.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1);
    }

    void OnDrawGizmos()
    {
        // Visualize transition wall in editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);

        // Add arrow pointing to next level
        Gizmos.color = Color.yellow;
        Vector3 arrowStart = transform.position;
        Vector3 arrowEnd = transform.position + transform.forward * 2f;
        Gizmos.DrawLine(arrowStart, arrowEnd);

        // Draw arrow head
        Vector3 right = Quaternion.LookRotation(arrowEnd - arrowStart) * Quaternion.Euler(0, 180 + 20, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(arrowEnd - arrowStart) * Quaternion.Euler(0, 180 - 20, 0) * Vector3.forward;
        Gizmos.DrawLine(arrowEnd, arrowEnd + right * 0.5f);
        Gizmos.DrawLine(arrowEnd, arrowEnd + left * 0.5f);
    }
}