using UnityEngine;

public class StrangeMover : StrangeObjectBase
{
    [Header("Mover Settings")]
    public Transform[] hidingSpots;
    public float moveSpeed = 3f;

    private Vector3 normalPosition;
    private Vector3 targetPosition;
    private int currentHidingSpot = 0;
    private bool isMoving = false;

    public override void Start()
    {
        base.Start();
        normalPosition = transform.position;
        targetPosition = normalPosition;
    }

    protected override void OnObserved()
    {
        // When watched: return to normal spot
        targetPosition = normalPosition;
        isMoving = true;  // Start moving back
        objectRenderer.material.color = Color.green;
    }

    protected override void OnUnobserved()
    {
        // When unobserved long enough: move to a hiding spot
        if (hidingSpots.Length > 0 && !isMoving)
        {
            // Pick a random hiding spot
            currentHidingSpot = Random.Range(0, hidingSpots.Length);
            targetPosition = hidingSpots[currentHidingSpot].position;
            isMoving = true;  // Start moving to hide
            objectRenderer.material.color = Color.red;
        }
    }

    protected override void OnWaiting()
    {
        // While waiting to hide: pulse
        float pulse = Mathf.Sin(Time.time * 5f) * 0.2f + 0.8f;
        objectRenderer.material.color = Color.Lerp(Color.white, Color.red, pulse);
    }

    void Update()
    {
        // Always call base Update() first to update observation state
        base.Update();

        // Only move if we have a target and we're not at it yet
        if (isMoving)
        {
            // Move toward target position
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Check if we've arrived
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition;
                isMoving = false;

                // If we just finished moving to normal position, reset color
                if (targetPosition == normalPosition)
                {
                    objectRenderer.material.color = Color.green;
                }
            }
        }
    }

    // Visualize the hiding spots in the editor
    void OnDrawGizmosSelected()
    {
        if (hidingSpots != null)
        {
            Gizmos.color = Color.cyan;
            foreach (Transform spot in hidingSpots)
            {
                if (spot != null)
                {
                    Gizmos.DrawWireSphere(spot.position, 0.3f);
                    Gizmos.DrawLine(transform.position, spot.position);
                }
            }
        }
    }
}