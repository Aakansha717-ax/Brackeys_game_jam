using UnityEngine;

public class StalkerBlock : MonoBehaviour
{
    [Header("Movement Settings")]
    public float followSpeed = 2f;
    public float stopDistance = 3f;
    public float startDelay = 3f;

    [Header("Appearance")]
    public Color idleColor = Color.gray;
    public Color activeColor = Color.red;
    public float pulseSpeed = 2f;

    [Header("Camera Shake - ONLY STALKER")]
    public bool enableShake = true;
    public float shakeDistance = 5f;           // Distance to start shaking
    public float closeShakeIntensity = 0.1f;   // Intensity when very close
    public float farShakeIntensity = 0.03f;    // Intensity when farther
    public float shakeInterval = 0.5f;         // Seconds between shakes

    private Transform player;
    private bool isActive = false;
    private float activeTimer = 0f;
    private Renderer blockRenderer;
    private Vector3 startPosition;
    private CameraShake cameraShake;
    private float nextShakeTime = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        blockRenderer = GetComponent<Renderer>();
        startPosition = transform.position;

        // Find camera shake
        cameraShake = FindObjectOfType<CameraShake>();
        if (cameraShake == null)
            Debug.LogWarning("CameraShake not found in scene!");

        blockRenderer.material.color = idleColor;
    }

    void Update()
    {
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

        FollowPlayer();

        float activePulse = Mathf.Sin(Time.time * (pulseSpeed * 2f)) * 0.2f + 0.8f;
        blockRenderer.material.color = Color.Lerp(activeColor, Color.black, activePulse * 0.5f);
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

        // ONLY STALKER SHAKES CAMERA - based on distance
        if (enableShake && cameraShake != null && isActive)
        {
            if (distanceToPlayer < shakeDistance && Time.time > nextShakeTime)
            {
                // Calculate intensity based on distance (closer = stronger)
                float t = 1f - (distanceToPlayer / shakeDistance); // 0 to 1
                float intensity = Mathf.Lerp(farShakeIntensity, closeShakeIntensity, t);

                // Shake with short duration
                cameraShake.Shake(0.2f, intensity);

                // Set next shake time
                nextShakeTime = Time.time + shakeInterval;

                Debug.Log("Stalker shake! Distance: " + distanceToPlayer.ToString("F1") +
                         ", Intensity: " + intensity.ToString("F2"));
            }
        }

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shakeDistance);
    }
}