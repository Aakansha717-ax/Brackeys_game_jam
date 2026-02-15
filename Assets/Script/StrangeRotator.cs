using UnityEngine;

public class StrangeRotator : StrangeObjectBase
{
    [Header("Rotator Settings")]
    public Transform[] rotatingElements;
    public float rotationSpeed = 90f;
    public Vector3[] hiddenRotations;

    private Quaternion[] originalRotations;
    private Quaternion[] targetRotations;

    public override void Start()
    {
        base.Start();

        if (rotatingElements.Length > 0)
        {
            originalRotations = new Quaternion[rotatingElements.Length];
            targetRotations = new Quaternion[rotatingElements.Length];

            for (int i = 0; i < rotatingElements.Length; i++)
            {
                if (rotatingElements[i] != null)
                {
                    originalRotations[i] = rotatingElements[i].rotation;
                    targetRotations[i] = originalRotations[i];
                }
            }
        }
    }

    protected override void OnObserved()
    {
        // Return to normal orientation
        for (int i = 0; i < rotatingElements.Length; i++)
        {
            if (rotatingElements[i] != null)
            {
                targetRotations[i] = originalRotations[i];
            }
        }
        objectRenderer.material.color = Color.white;
    }

    protected override void OnUnobserved()
    {
        // Rotate to strange angles
        for (int i = 0; i < rotatingElements.Length; i++)
        {
            if (rotatingElements[i] != null && hiddenRotations.Length > i)
            {
                targetRotations[i] = Quaternion.Euler(hiddenRotations[i]);
            }
        }
        objectRenderer.material.color = new Color(1f, 0.5f, 0f); // Orange
    }

    protected override void OnWaiting()
    {
        // Rotate back and forth slightly
        float wobble = Mathf.Sin(Time.time * 3f) * 10f;
        for (int i = 0; i < rotatingElements.Length; i++)
        {
            if (rotatingElements[i] != null && i < targetRotations.Length)
            {
                rotatingElements[i].rotation = Quaternion.RotateTowards(
                    rotatingElements[i].rotation,
                    targetRotations[i],
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }

    void Update()
    {
        base.Update();

        // Smooth rotation
        for (int i = 0; i < rotatingElements.Length; i++)
        {
            if (rotatingElements[i] != null && i < targetRotations.Length)
            {
                rotatingElements[i].rotation = Quaternion.RotateTowards(
                    rotatingElements[i].rotation,
                    targetRotations[i],
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}