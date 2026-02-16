using UnityEngine;

public class StrangeSlidingWall : StrangeObjectBase
{
    [Header("Wall Settings")]
    public Transform closedPosition;
    public Transform openPosition;
    public float slideSpeed = 2f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    public override void Start()
    {
        base.Start();

        // Set starting position
        if (closedPosition != null)
            transform.position = closedPosition.position;

        targetPosition = transform.position;
    }

    protected override void OnObserved()
    {
        // When watched, wall stays closed (or closes if open)
        targetPosition = closedPosition.position;
        isMoving = true;
        objectRenderer.material.color = Color.gray;
    }

    protected override void OnUnobserved()
    {
        // When unobserved, wall slides open
        targetPosition = openPosition.position;
        isMoving = true;
        objectRenderer.material.color = new Color(0.5f, 0.5f, 0.5f, 0.8f); // Slightly transparent
    }

    protected override void OnWaiting()
    {
        // Subtle vibration while waiting to move
        float shakeX = Random.Range(-0.02f, 0.02f);
        float shakeZ = Random.Range(-0.02f, 0.02f);
        transform.position += new Vector3(shakeX, 0, shakeZ);

        // Pulse color
        float pulse = Mathf.Sin(Time.time * 3f) * 0.2f + 0.8f;
        objectRenderer.material.color = Color.Lerp(Color.gray, Color.white, pulse);
    }

    void Update()
    {
        base.Update();

        if (isMoving)
        {
            // Slide toward target position
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                slideSpeed * Time.deltaTime
            );

            // Check if arrived
            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    // Visualize in editor
    void OnDrawGizmosSelected()
    {
        if (closedPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(closedPosition.position, transform.localScale);
        }

        if (openPosition != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(openPosition.position, transform.localScale);
            Gizmos.DrawLine(closedPosition.position, openPosition.position);
        }
    }
}