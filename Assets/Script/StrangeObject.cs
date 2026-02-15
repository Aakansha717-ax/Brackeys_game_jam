using System.Collections;
using UnityEngine;

public class StrangeObject : MonoBehaviour
{
    [Header("Settings")]
    public Transform hiddenPosition;      // Where it goes when unobserved
    public float timeBeforeMove = 1.5f;    // How long unobserved before moving
    public float moveSpeed = 3f;

    [Header("Debug")]
    public bool isBeingObserved = false;

    private Vector3 normalPosition;
    private Vector3 targetPosition;
    private float unobservedTimer = 0f;
    private Camera mainCamera;
    private Renderer objectRenderer;

    public AudioClip moveSound;
    private AudioSource audioSource;

    void Start()
    {
        mainCamera = Camera.main;
        objectRenderer = GetComponent<Renderer>();
        normalPosition = transform.position;
        targetPosition = normalPosition;
        audioSource = GetComponent<AudioSource>();

    }

    void Update()
    {
        // Check if camera can see this object
        CheckIfObserved();

        // Handle the "unobserved" timer
        if (isBeingObserved)
        {
            unobservedTimer = 0f;
            targetPosition = normalPosition;  // Return to normal spot

            // Visual feedback - normal color
            objectRenderer.material.color = Color.red;
        }
        else
        {
            unobservedTimer += Time.deltaTime;

            // If unobserved long enough, target to hidden spot
            if (unobservedTimer >= timeBeforeMove)
            {
                targetPosition = hiddenPosition.position;

                // Visual feedback - fading/dark when moving
                objectRenderer.material.color = Color.Lerp(Color.red, Color.black, 0.5f);
            }
        }

        // Smoothly move to target position
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (targetPosition != transform.position && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(moveSound);
        }
    }

    void CheckIfObserved()
    {
        // Is the object on screen?
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
        bool onScreen = viewportPoint.x > 0 && viewportPoint.x < 1 &&
                       viewportPoint.y > 0 && viewportPoint.y < 1 &&
                       viewportPoint.z > 0;

        if (!onScreen)
        {
            isBeingObserved = false;
            return;
        }

        // Check line of sight (no walls in between)
        RaycastHit hit;
        Vector3 directionToObject = transform.position - mainCamera.transform.position;

        if (Physics.Raycast(mainCamera.transform.position, directionToObject, out hit))
        {
            isBeingObserved = (hit.transform == this.transform);
        }
        else
        {
            isBeingObserved = false;
        }
    }

    // Optional: Draw debug lines in Scene view
    void OnDrawGizmos()
    {
        if (hiddenPosition != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, hiddenPosition.position);
            Gizmos.DrawWireSphere(hiddenPosition.position, 0.3f);
        }
    }
}
