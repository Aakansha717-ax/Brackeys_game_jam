using UnityEngine;

// Base class for all strange objects
public abstract class StrangeObjectBase : MonoBehaviour
{
    [Header("Base Settings")]
    public float timeBeforeAct = 1.5f;
    public bool isBeingObserved = false;

    protected Camera mainCamera;
    protected Renderer objectRenderer;
    protected float unobservedTimer = 0f;
    protected Material originalMaterial;

    public virtual void Start()
    {
        mainCamera = Camera.main;
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
            originalMaterial = objectRenderer.material;
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

    // Abstract methods for different behaviors
    protected abstract void OnObserved();
    protected abstract void OnUnobserved();
    protected abstract void OnWaiting();
}