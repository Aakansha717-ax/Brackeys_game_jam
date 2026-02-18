using UnityEngine;

public class StrangeMover : StrangeObjectBase
{
    [Header("Mover Settings")]
    public Transform[] hidingSpots;
    public float moveSpeed = 3f;

    private Vector3 normalPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private int currentHidingSpot = 0;

    public override void Start()
    {
        base.Start();
        normalPosition = transform.position;
        targetPosition = normalPosition;
    }

    protected override void OnObserved()
    {
        targetPosition = normalPosition;
        isMoving = true;
        SetGlow(observedColor, 1f);
    }

    protected override void OnUnobserved()
    {
        if (hidingSpots.Length > 0)
        {
            currentHidingSpot = Random.Range(0, hidingSpots.Length);
            targetPosition = hidingSpots[currentHidingSpot].position;
            isMoving = true;
            SetGlow(unobservedColor, glowIntensity);
        }
    }

    protected override void OnWaiting()
    {
        float pulse = Mathf.Sin(Time.time * 5f) * 0.2f + 0.8f;
        SetGlow(Color.Lerp(Color.white, unobservedColor, pulse), glowIntensity * pulse);
    }

    void Update()
    {
        base.Update();

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

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