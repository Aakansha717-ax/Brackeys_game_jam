using UnityEngine;
using TMPro;
using System.Collections;

public class CameraInstructions : MonoBehaviour
{
    [Header("Instruction Text")]
    public TextMeshProUGUI instructionText;

    [Header("Messages")]
    [TextArea(2, 3)]
    public string[] instructions = new string[]
    {
        "Look away... things change...",
        "The stalker follows when you're not watching",
        "Find the hidden passages",
        "Reach the exit"
    };

    [Header("Settings")]
    public float displayTime = 4f;
    public float fadeSpeed = 2f;
    public float startDelay = 2f;

    [Header("Glow Effects")]
    public Color textColor = Color.white;
    public Color glowColor = Color.cyan;
    public float glowIntensity = 0.5f;  // Reduced for better look
    public float pulseSpeed = 2f;
    public float outlineWidth = 0.2f;
    public Color outlineColor = Color.black;

    private int currentIndex = 0;
    private CanvasGroup canvasGroup;
    private Material textMaterial;
    private float originalGlow;

    void Start()
    {
        // Get or add CanvasGroup
        canvasGroup = instructionText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = instructionText.gameObject.AddComponent<CanvasGroup>();

        // Setup TextMeshPro properties
        instructionText.color = textColor;
        instructionText.outlineWidth = outlineWidth;
        instructionText.outlineColor = outlineColor;

        // Setup glow through material
        SetupGlowMaterial();

        // Start hidden
        canvasGroup.alpha = 0f;

        // Start instruction sequence
        StartCoroutine(InstructionSequence());
    }

    void SetupGlowMaterial()
    {
        // Get the material
        textMaterial = instructionText.fontMaterial;

        if (textMaterial != null)
        {
            // Different shaders use different property names
            // Try common glow property names

            // For Standard shader
            if (textMaterial.HasProperty("_GlowPower"))
            {
                textMaterial.SetFloat("_GlowPower", glowIntensity);
                originalGlow = glowIntensity;
            }

            // For TextMeshPro shader
            if (textMaterial.HasProperty("_Glow"))
            {
                textMaterial.EnableKeyword("GLOW_ON");
                textMaterial.SetFloat("_Glow", glowIntensity);
                originalGlow = glowIntensity;
            }

            // For Face/Outline glow
            if (textMaterial.HasProperty("_OutlineGlow"))
            {
                textMaterial.SetFloat("_OutlineGlow", glowIntensity);
                originalGlow = glowIntensity;
            }

            // Set glow color if property exists
            if (textMaterial.HasProperty("_GlowColor"))
            {
                textMaterial.SetColor("_GlowColor", glowColor);
            }

            Debug.Log("Glow material setup complete");
        }
        else
        {
            Debug.LogWarning("No font material found! Creating one...");
            CreateGlowMaterial();
        }
    }

    void CreateGlowMaterial()
    {
        // Create a new material for glow
        textMaterial = new Material(Shader.Find("TextMeshPro/Distance Field"));
        instructionText.fontMaterial = textMaterial;

        textMaterial.EnableKeyword("GLOW_ON");
        textMaterial.SetFloat("_Glow", glowIntensity);
        textMaterial.SetColor("_GlowColor", glowColor);
        textMaterial.SetFloat("_GlowPower", glowIntensity);

        originalGlow = glowIntensity;
    }

    void Update()
    {
        // Pulse glow intensity when visible
        if (textMaterial != null && canvasGroup.alpha > 0.1f)
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.2f + 0.8f;
            float pulsedGlow = originalGlow * pulse;

            // Apply to various possible properties
            if (textMaterial.HasProperty("_GlowPower"))
                textMaterial.SetFloat("_GlowPower", pulsedGlow);

            if (textMaterial.HasProperty("_Glow"))
                textMaterial.SetFloat("_Glow", pulsedGlow);

            if (textMaterial.HasProperty("_OutlineGlow"))
                textMaterial.SetFloat("_OutlineGlow", pulsedGlow);
        }
    }

    IEnumerator InstructionSequence()
    {
        // Initial delay
        yield return new WaitForSeconds(startDelay);

        // Show each instruction
        while (currentIndex < instructions.Length)
        {
            // Set text
            instructionText.text = instructions[currentIndex];

            // Fade in
            yield return StartCoroutine(Fade(0f, 1f));

            // Wait
            yield return new WaitForSeconds(displayTime);

            // Fade out
            yield return StartCoroutine(Fade(1f, 0f));

            // Next instruction
            currentIndex++;

            // Small pause between
            yield return new WaitForSeconds(0.5f);
        }

        // All done - hide permanently
        canvasGroup.alpha = 0f;
    }

    IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeSpeed)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeSpeed);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}