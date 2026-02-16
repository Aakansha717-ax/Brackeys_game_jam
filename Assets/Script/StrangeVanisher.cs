using UnityEngine;

public class StrangeVanisher : StrangeObjectBase
{
    private MeshRenderer meshRenderer;
    private Collider objectCollider;

    public override void Start()
    {
        base.Start();
        meshRenderer = GetComponent<MeshRenderer>();
        objectCollider = GetComponent<Collider>();
    }

    protected override void OnObserved()
    {
        // Reappear when watched
        meshRenderer.enabled = true;
        objectCollider.enabled = true;
        objectRenderer.material.color = Color.white;
    }

    protected override void OnUnobserved()
    {
        // Vanish completely
        meshRenderer.enabled = false;
        objectCollider.enabled = false;
    }

    protected override void OnWaiting()
    {
        // Flicker in and out as warning
        if (Random.value > 0.5f)
        {
            meshRenderer.enabled = true;
            objectRenderer.material.color = new Color(1f, 1f, 1f, 0.3f);
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }
}