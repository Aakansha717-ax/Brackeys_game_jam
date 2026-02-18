using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangeMimic : StrangeObjectBase
{
    public float copyDelay = 1f;
    public float maxDistance = 10f;

    private Transform player;
    private Vector3[] positionHistory;
    private float[] timeHistory;
    private int historyIndex = 0;
    private int historySize = 100;

    public override void Start()
    {
        base.Start();
        player = Camera.main.transform;

        // Create history arrays
        positionHistory = new Vector3[historySize];
        timeHistory = new float[historySize];
    }

    protected override void OnObserved()
    {
        // Stop copying when watched
        SetGlow(Color.cyan, 1f);
    }

    protected override void OnUnobserved()
    {
        // Record player positions
        positionHistory[historyIndex] = player.position;
        timeHistory[historyIndex] = Time.time;
        historyIndex = (historyIndex + 1) % historySize;

        // Move to older position
        int pastIndex = (historyIndex - Mathf.RoundToInt(copyDelay / Time.deltaTime) + historySize) % historySize;

        if (Vector3.Distance(player.position, transform.position) < maxDistance)
        {
            transform.position = Vector3.Lerp(transform.position, positionHistory[pastIndex], Time.deltaTime);
        }

        SetGlow(Color.red, glowIntensity);
    }

    protected override void OnWaiting()
    {
        float pulse = Mathf.Sin(Time.time * 4f) * 0.5f + 0.5f;
        SetGlow(Color.cyan, glowIntensity * pulse * 0.3f);
    }
}