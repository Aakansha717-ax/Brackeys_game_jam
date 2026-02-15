using UnityEngine;

public class StrangeWhisperer : StrangeObjectBase
{
    [Header("Whisperer Settings")]
    public float moveSpeed = 2f;
    public float stopDistance = 2f;
    public float whisperVolume = 0.3f;

    private Vector3 originalPosition;
    private AudioSource audioSource;
    private bool isMoving = false;

    public override void Start()
    {
        base.Start();
        originalPosition = transform.position;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.volume = 0f;

        // Load a whisper sound or generate noise
        // audioSource.clip = someWhisperSound;
    }

    protected override void OnObserved()
    {
        // Return to original spot when watched
        isMoving = true;
        objectRenderer.material.color = Color.blue;
        audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime * 5f);
    }

    protected override void OnUnobserved()
    {
        // Move toward player
        isMoving = true;
        objectRenderer.material.color = new Color(0.5f, 0f, 0.5f); // Dark purple
        audioSource.volume = Mathf.Lerp(audioSource.volume, whisperVolume, Time.deltaTime * 2f);
    }

    protected override void OnWaiting()
    {
        // Subtle hint of movement
        float pulse = Mathf.Sin(Time.time * 4f) * 0.3f;
        transform.localScale = Vector3.one * (1f + pulse * 0.1f);
    }

    void Update()
    {
        base.Update();

        if (!isBeingObserved && unobservedTimer >= timeBeforeAct)
        {
            // Move toward player
            Vector3 directionToPlayer = (mainCamera.transform.position - transform.position).normalized;
            directionToPlayer.y = 0; // Keep on ground

            float distanceToPlayer = Vector3.Distance(transform.position, mainCamera.transform.position);

            if (distanceToPlayer > stopDistance)
            {
                transform.position += directionToPlayer * moveSpeed * Time.deltaTime;
            }
        }
        else if (isBeingObserved)
        {
            // Return to original
            transform.position = Vector3.Lerp(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }
}