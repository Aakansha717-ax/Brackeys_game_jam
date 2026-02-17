using UnityEngine;

public class StalkerBlock : MonoBehaviour
{
    [Header("Movement Settings")]
    public float followSpeed = 2f;
    public float stopDistance = 3f;        // How close it gets before stopping
    public float startDelay = 3f;           // Time before it starts following

    [Header("Appearance")]
    public Color idleColor = Color.gray;
    public Color activeColor = Color.red;
    public float pulseSpeed = 2f;

    private Transform player;
    private Vector3 targetPosition;
    private bool isActive = false;
    private float activeTimer = 0f;
    private Renderer blockRenderer;
    private Vector3 startPosition;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        blockRenderer = GetComponent<Renderer>();
        startPosition = transform.position;

        // Start idle
        blockRenderer.material.color = idleColor;
    }

    void Update()
    {
        // Countdown to activation
        if (!isActive)
        {
            activeTimer += Time.deltaTime;

            // Pulse while waiting
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.3f + 0.7f;
            blockRenderer.material.color = Color.Lerp(idleColor, Color.white, pulse);

            if (activeTimer >= startDelay)
            {
                isActive = true;
                blockRenderer.material.color = activeColor;
            }
            return;
        }

        // Active - follow player
        FollowPlayer();

        // Pulse menacingly
        float activePulse = Mathf.Sin(Time.time * (pulseSpeed * 2f)) * 0.2f + 0.8f;
        blockRenderer.material.color = Color.Lerp(activeColor, Color.black, activePulse * 0.5f);
    }

    void FollowPlayer()
    {
        // Calculate direction to player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; // Keep on ground

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Only move if farther than stop distance
        if (distanceToPlayer > stopDistance)
        {
            // Move toward player
            Vector3 moveAmount = directionToPlayer * followSpeed * Time.deltaTime;
            transform.position += moveAmount;
        }

        // Always face the player (creepy!)
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    // Visualize stop distance in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}