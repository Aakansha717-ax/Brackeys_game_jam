using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Transform cameraTransform;
    private Vector3 originalPosition;
    private float shakeDuration = 0f;
    private float shakeIntensity = 0.3f;
    private float shakeReduction = 1f; // How fast shake diminishes

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }

        cameraTransform = Camera.main.transform;
        originalPosition = cameraTransform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            // More controlled shake using Perlin noise (smoother than random)
            float offsetX = Mathf.PerlinNoise(Time.time * 10f, 0) * 2 - 1;
            float offsetY = Mathf.PerlinNoise(0, Time.time * 10f) * 2 - 1;

            // Apply shake with intensity
            Vector3 shakeOffset = new Vector3(offsetX, offsetY, 0) * shakeIntensity;
            cameraTransform.localPosition = originalPosition + shakeOffset;

            // Reduce shake over time
            shakeDuration -= Time.deltaTime * shakeReduction;
        }
        else
        {
            // Smoothly return to original position
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                originalPosition,
                Time.deltaTime * 5f
            );
        }
    }

    // Call this method to shake the camera
    public void Shake(float duration, float intensity)
    {
        // Don't stack shakes - take the stronger one
        if (intensity > shakeIntensity)
        {
            shakeIntensity = intensity;
        }

        // Extend duration but don't make it too long
        shakeDuration = Mathf.Max(shakeDuration, duration);

        // Store original position
        originalPosition = cameraTransform.localPosition;
    }

    // Quick shakes for different events - now much gentler!
    public void LightShake() => Shake(0.15f, 0.05f);
    public void MediumShake() => Shake(0.25f, 0.1f);
    public void HeavyShake() => Shake(0.35f, 0.15f);
    public void StalkerNearShake() => Shake(0.2f, 0.08f);
}