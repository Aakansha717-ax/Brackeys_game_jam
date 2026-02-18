using UnityEngine;

public class StrangeWatcher : StrangeObjectBase
{
    public float rotationSpeed = 45f;
    public Transform[] watchTargets;

    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private int currentTarget = 0;

    public override void Start()
    {
        base.Start();
        originalRotation = transform.rotation;
        targetRotation = originalRotation;
    }

    protected override void OnObserved()
    {
        // When watched, look away
        targetRotation = Quaternion.LookRotation(-transform.forward);
        SetGlow(Color.blue, 1f);
    }

    protected override void OnUnobserved()
    {
        // When not watched, stare at player or targets
        if (watchTargets.Length > 0)
        {
            Transform target = watchTargets[Random.Range(0, watchTargets.Length)];
            targetRotation = Quaternion.LookRotation(target.position - transform.position);
        }
        SetGlow(Color.red, glowIntensity);
    }

    protected override void OnWaiting()
    {
        // Twitch slightly
        float randomTwitch = Random.Range(-5f, 5f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);
    }

    void Update()
    {
        base.Update();
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}