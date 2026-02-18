using UnityEngine;

// Base class for all strange objects
public abstract class StrangeObjectBase : MonoBehaviour
{
    [Header("Base Settings")]
    public float timeBeforeAct = 1.5f;
    public bool isBeingObserved = false;

    [Header("Glow Settings")]
    public Color observedColor = Color.white;
    public Color unobservedColor = Color.red;
    public float glowIntensity = 3f;

    protected Camera mainCamera;
    protected Renderer objectRenderer;
    protected float unobservedTimer = 0f;
    protected Material originalMaterial;

    private Material objectMaterial;
    private Light objectLight;

    public virtual void Start()
    {
        mainCamera = Camera.main;
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
            objectMaterial = objectRenderer.material;

            // Enable emission
            objectMaterial.EnableKeyword("_EMISSION");
        }

        objectLight = GetComponentInChildren<Light>();
    }

    public virtual void Update()
    {
        CheckIfObserved();

        if (isBeingObserved)
        {
            unobservedTimer = 0f;
            OnObserved();
        }
        else
        {
            unobservedTimer += Time.deltaTime;

            if (unobservedTimer >= timeBeforeAct)
            {
                OnUnobserved();
            }
            else
            {
                OnWaiting();
            }
        }
    }

    void CheckIfObserved()
    {
        if (mainCamera == null) return;

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
        bool onScreen = viewportPoint.x > 0 && viewportPoint.x < 1 &&
                       viewportPoint.y > 0 && viewportPoint.y < 1 &&
                       viewportPoint.z > 0;

        if (!onScreen)
        {
            isBeingObserved = false;
            return;
        }

        RaycastHit hit;
        Vector3 directionToObject = transform.position - mainCamera.transform.position;

        if (Physics.Raycast(mainCamera.transform.position, directionToObject, out hit))
        {
            isBeingObserved = (hit.transform == this.transform);
        }
        else
        {
            isBeingObserved = false;
        }
    }

    // Glow methods
    protected void SetGlow(Color color, float intensity)
    {
        if (objectMaterial != null)
        {
            objectMaterial.SetColor("_EmissionColor", color * intensity);
        }

        if (objectLight != null)
        {
            objectLight.color = color;
            objectLight.intensity = intensity * 0.5f;
        }
    }

    // Abstract methods
    protected abstract void OnObserved();
    protected abstract void OnUnobserved();
    protected abstract void OnWaiting();
}