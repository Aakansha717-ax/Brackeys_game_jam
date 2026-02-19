using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuCursorBlock : MonoBehaviour
{
    [Header("Block Settings")]
    public float followSpeed = 5f;
    public float rotationSpeed = 2f;
    public float scaleAmount = 0.5f;

    [Header("Color Settings")]
    public Color idleColor = Color.white;
    public Color hoverColor = Color.red;
    public Color clickColor = Color.blue;
    public float colorChangeSpeed = 3f;

    [Header("UI References")]
    public Button[] menuButtons;
    public TextMeshProUGUI[] menuTexts;

    private RectTransform rectTransform;
    private Camera mainCamera;
    private Vector3 targetPosition;
    private Color currentColor;
    private Renderer blockRenderer;
    private bool isClicked = false;
    private float clickTimer = 0f;

    void Start()
    {
        // Get components
        rectTransform = GetComponent<RectTransform>();
        blockRenderer = GetComponent<Renderer>();
        mainCamera = Camera.main;

        // Start position (center of screen in world space)
        targetPosition = GetWorldPositionFromMouse();
        transform.position = targetPosition;

        // Set initial color
        currentColor = idleColor;
        blockRenderer.material.color = currentColor;
        blockRenderer.material.EnableKeyword("_EMISSION");

        Debug.Log("MenuCursorBlock started - Move your mouse!");
    }

    void Update()
    {
        // Get mouse position in world space
        targetPosition = GetWorldPositionFromMouse();

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Gentle rotation
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime * 0.5f);

        // Scale pulsing
        float scale = 1f + Mathf.Sin(Time.time * 3f) * 0.1f;
        transform.localScale = Vector3.one * scale;

        // Handle color changes based on input
        HandleColorChanges();

        // Apply color with glow
        blockRenderer.material.SetColor("_EmissionColor", currentColor * 2f);
        blockRenderer.material.color = Color.Lerp(blockRenderer.material.color, currentColor, Time.deltaTime * colorChangeSpeed);

        // Check button hover
        CheckButtonHover();
    }

    Vector3 GetWorldPositionFromMouse()
    {
        // Convert mouse position to world position
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // Distance from camera
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    void HandleColorChanges()
    {
        // Change color based on mouse input
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            currentColor = clickColor;
            isClicked = true;
            clickTimer = 0.5f;

            // Scale pop on click
            transform.localScale = Vector3.one * 1.3f;
        }
        else if (Input.GetMouseButton(1)) // Right click
        {
            currentColor = Color.magenta;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            currentColor = Color.cyan;
        }
        else if (!isClicked)
        {
            // Cycle colors based on mouse position
            float x = Input.mousePosition.x / Screen.width;
            float y = Input.mousePosition.y / Screen.height;

            Color rainbowColor = new Color(
                Mathf.Sin(Time.time * 2f) * 0.5f + 0.5f,
                Mathf.Sin(Time.time * 2f + 2f) * 0.5f + 0.5f,
                Mathf.Sin(Time.time * 2f + 4f) * 0.5f + 0.5f
            );

            currentColor = Color.Lerp(idleColor, rainbowColor, x);
        }

        // Reset click color after delay
        if (isClicked)
        {
            clickTimer -= Time.deltaTime;
            if (clickTimer <= 0)
            {
                isClicked = false;
                transform.localScale = Vector3.one;
            }
        }
    }

    void CheckButtonHover()
    {
        foreach (Button button in menuButtons)
        {
            if (button != null)
            {
                Vector3 buttonPos = button.transform.position;
                float distance = Vector3.Distance(transform.position, buttonPos);

                if (distance < 2f)
                {
                    // Glow when near buttons
                    currentColor = Color.Lerp(currentColor, hoverColor, Time.deltaTime * 2f);

                    // Make button react (optional)
                    button.transform.localScale = Vector3.Lerp(
                        button.transform.localScale,
                        Vector3.one * 1.1f,
                        Time.deltaTime * 5f
                    );
                }
                else
                {
                    button.transform.localScale = Vector3.Lerp(
                        button.transform.localScale,
                        Vector3.one,
                        Time.deltaTime * 5f
                    );
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        // Visualize in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}