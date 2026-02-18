using UnityEngine;

public class StrangeGatherer : StrangeObjectBase
{
    public float gatherRadius = 5f;
    public float gatherSpeed = 2f;
    public Transform gatherPoint;

    private GameObject[] strangeObjects;
    private bool isGathering = false;

    public override void Start()
    {
        base.Start();
        // Find all strange objects
        strangeObjects = GameObject.FindGameObjectsWithTag("StrangeObject");
    }

    protected override void OnObserved()
    {
        // Stop gathering when watched
        isGathering = false;
        SetGlow(Color.blue, 1f);
    }

    protected override void OnUnobserved()
    {
        // Start gathering objects
        isGathering = true;
        SetGlow(Color.red, glowIntensity);
    }

    protected override void OnWaiting()
    {
        float pulse = Mathf.Sin(Time.time * 3f) * 0.5f + 0.5f;
        SetGlow(Color.red, glowIntensity * pulse * 0.3f);
    }

    void Update()
    {
        base.Update();

        if (isGathering)
        {
            foreach (GameObject obj in strangeObjects)
            {
                if (obj != null && obj != this.gameObject)
                {
                    float distance = Vector3.Distance(obj.transform.position, transform.position);

                    if (distance < gatherRadius)
                    {
                        // Pull object toward gather point
                        obj.transform.position = Vector3.MoveTowards(
                            obj.transform.position,
                            gatherPoint.position,
                            gatherSpeed * Time.deltaTime * (1f - distance / gatherRadius)
                        );
                    }
                }
            }
        }
    }
}