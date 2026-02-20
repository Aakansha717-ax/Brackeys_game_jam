using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StalkerBlock : MonoBehaviour
{
    [Header("Movement Settings")]
    public float followSpeed = 2f;
    public float stopDistance = 3f;
    public float startDelay = 3f;
    public bool followsWhenWatched = false;  // Set to FALSE so it only moves when unobserved

    [Header("Appearance")]
    public Color idleColor = Color.gray;
    public Color activeColor = Color.red;
    public float pulseSpeed = 2f;

    [Header("Camera Shake")]
    public bool enableShake = true;
    public float shakeDistance = 5f;
    public float closeShakeIntensity = 0.1f;
    public float farShakeIntensity = 0.03f;
    public float shakeInterval = 0.5f;

    [Header("Death Settings")]
    public int hitsToKill = 3;
    public float deathDelay = 1f;
    public string deathSceneName = "MainMenu"; // Scene to load after death
    public AudioClip deathSound;
    public AudioClip hitSound;
    public Color hitColor = Color.white;

    private Transform player;
    private bool isActive = false;
    private float activeTimer = 0f;
    private Renderer blockRenderer;
    private Vector3 startPosition;
    private CameraShake cameraShake;
    private float nextShakeTime = 0f;

    // Death tracking
    private int hitCount = 0;
    private bool isDying = false;
    private AudioSource audioSource;
    private Camera playerCamera;
    private StrangeObjectBase strangeBase;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCamera = Camera.main;
        blockRenderer = GetComponent<Renderer>();
        startPosition = transform.position;

        // Get or add audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (deathSound != null || hitSound != null))
            audioSource = gameObject.AddComponent<AudioSource>();

        // Find camera shake
        cameraShake = FindObjectOfType<CameraShake>();
        if (cameraShake == null)
            Debug.LogWarning("CameraShake not found in scene!");

        // Try to get strange object base for observation checking
        strangeBase = GetComponent<StrangeObjectBase>();

        blockRenderer.material.color = idleColor;
    }

    void Update()
    {
        if (isDying) return;

        if (!isActive)
        {
            activeTimer += Time.deltaTime;

            float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.3f + 0.7f;
            blockRenderer.material.color = Color.Lerp(idleColor, Color.white, pulse);

            if (activeTimer >= startDelay)
            {
                isActive = true;
                blockRenderer.material.color = activeColor;
            }
            return;
        }

        // Check if player is watching
        bool playerWatching = IsPlayerWatching();

        // Only follow if conditions are met
        if (shouldFollow(playerWatching))
        {
            FollowPlayer();
        }

        // Pulse effect
        float activePulse = Mathf.Sin(Time.time * (pulseSpeed * 2f)) * 0.2f + 0.8f;
        blockRenderer.material.color = Color.Lerp(activeColor, Color.black, activePulse * 0.5f);

        // Camera shake (only when close and active)
        HandleCameraShake();
    }

    bool IsPlayerWatching()
    {
        if (playerCamera == null) return false;

        // Check if stalker is in camera view
        Vector3 viewportPoint = playerCamera.WorldToViewportPoint(transform.position);
        bool onScreen = viewportPoint.x > 0 && viewportPoint.x < 1 &&
                       viewportPoint.y > 0 && viewportPoint.y < 1 &&
                       viewportPoint.z > 0;

        if (!onScreen) return false;

        // Check line of sight
        RaycastHit hit;
        Vector3 directionToStalker = transform.position - playerCamera.transform.position;

        if (Physics.Raycast(playerCamera.transform.position, directionToStalker, out hit))
        {
            return hit.transform == this.transform;
        }

        return false;
    }

    bool shouldFollow(bool playerWatching)
    {
        // If followsWhenWatched is true, always follow
        // If false, only follow when player is NOT watching
        return followsWhenWatched ? true : !playerWatching;
    }

    void FollowPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance)
        {
            Vector3 moveAmount = directionToPlayer * followSpeed * Time.deltaTime;
            transform.position += moveAmount;
        }

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    void HandleCameraShake()
    {
        if (!enableShake || cameraShake == null || !isActive) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < shakeDistance && Time.time > nextShakeTime)
        {
            float t = 1f - (distanceToPlayer / shakeDistance);
            float intensity = Mathf.Lerp(farShakeIntensity, closeShakeIntensity, t);

            cameraShake.Shake(0.2f, intensity);
            nextShakeTime = Time.time + shakeInterval;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDying) return;

        if (other.CompareTag("Player"))
        {
            hitCount++;
            Debug.Log($"Stalker hit player! Hit {hitCount}/{hitsToKill}");

            // Play hit sound
            if (audioSource != null && hitSound != null)
                audioSource.PlayOneShot(hitSound);

            // Visual feedback
            StartCoroutine(HitFeedback());

            // Camera shake on hit
            if (cameraShake != null)
                cameraShake.HeavyShake();

            // Check if player dies
            if (hitCount >= hitsToKill)
            {
                StartCoroutine(PlayerDeath());
            }
            else
            {
                // Push player back slightly
                Vector3 pushDirection = (other.transform.position - transform.position).normalized;
                other.transform.position += pushDirection * 2f;
            }
        }
    }

    IEnumerator HitFeedback()
    {
        Color originalColor = blockRenderer.material.color;
        blockRenderer.material.color = hitColor;
        yield return new WaitForSeconds(0.2f);
        blockRenderer.material.color = originalColor;
    }

    IEnumerator PlayerDeath()
    {
        isDying = true;

        // Play death sound
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // Flash red/white several times
        for (int i = 0; i < 5; i++)
        {
            blockRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            blockRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
        }

        // Heavy camera shake
        if (cameraShake != null)
            cameraShake.HeavyShake();

        // Find player camera for fade effect
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            // Simple fade to black effect
            float elapsed = 0f;
            Color originalColor = mainCam.backgroundColor;
            while (elapsed < deathDelay)
            {
                elapsed += Time.deltaTime;
                mainCam.backgroundColor = Color.Lerp(originalColor, Color.black, elapsed / deathDelay);
                yield return null;
            }
        }

        // Load death/restart scene
        SceneManager.LoadScene(deathSceneName);
    }

    // Visualize everything in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shakeDistance);

        // Show follow behavior text
#if UNITY_EDITOR
        string behavior = followsWhenWatched ? "Always follows" : "Only follows when unobserved";
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, behavior);
#endif
    }
}