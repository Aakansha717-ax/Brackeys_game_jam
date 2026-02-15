using UnityEngine;
using System.Collections.Generic;

public class StrangeFlickerer : StrangeObjectBase
{
    [Header("Flickerer Settings")]
    public Light[] lightsToControl;
    public float flickerSpeed = 10f;

    private Dictionary<Light, float> originalIntensities = new Dictionary<Light, float>();
    private bool lightsOn = true;

    public override void Start()
    {
        base.Start();

        foreach (Light light in lightsToControl)
        {
            if (light != null)
            {
                originalIntensities[light] = light.intensity;
            }
        }
    }

    protected override void OnObserved()
    {
        // Lights behave normally when watched
        lightsOn = true;
        foreach (Light light in lightsToControl)
        {
            if (light != null)
            {
                light.intensity = originalIntensities[light];
                light.enabled = true;
            }
        }
        objectRenderer.material.color = Color.yellow;
    }

    protected override void OnUnobserved()
    {
        // Crazy flickering when unobserved
        lightsOn = !lightsOn;

        foreach (Light light in lightsToControl)
        {
            if (light != null)
            {
                if (lightsOn)
                {
                    light.intensity = originalIntensities[light] * Random.Range(0.3f, 1.5f);
                }
                light.enabled = lightsOn;
            }
        }

        objectRenderer.material.color = Color.Lerp(Color.black, Color.yellow, Random.value);
    }

    protected override void OnWaiting()
    {
        // Subtle flicker as warning
        float flicker = Mathf.Sin(Time.time * flickerSpeed) > 0 ? 1f : 0.3f;
        objectRenderer.material.color = Color.Lerp(Color.white, Color.yellow, flicker);
    }
}