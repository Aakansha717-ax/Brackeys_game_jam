using UnityEngine;

public class StrangeMover : StrangeObjectBase
{
    [Header("Mover Settings")]
    public Transform[] hidingSpots;
    public float moveSpeed = 3f;

    private Vector3 normalPosition;
    private Vector3 targetPosition;
    private int currentHidingSpot = 0;

    public override void Start()
    {
        base.Start();
        normalPosition = transform.position;
        targetPosition = normalPosition;
    }

    protected override void OnObserved()
    {
        // Return to normal spot when watched
        targetPosition = normalPosition;
        objectRenderer.material.color = Color.green;
    }

    protected override void OnUnobserved()
    {
        // Move to a random hiding spot
        if (hidingSpots.Length > 0)
        {
            currentHidingSpot = Random.Range(0, hidingSpots.Length);
            targetPosition = hidingSpots[currentHidingSpot].position;
            objectRenderer.material.color = Color.red;
        }
    }

    protected override void OnWaiting()
    {
        // Pulse while waiting to move
        float pulse = Mathf.Sin(Time.time * 5f) * 0.2f + 0.8f;
        objectRenderer.material.color = Color.Lerp(Color.white, Color.red, pulse);
    }

    void Update()
    {
        base.Update();
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}