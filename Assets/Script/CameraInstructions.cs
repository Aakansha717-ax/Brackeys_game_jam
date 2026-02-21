using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class CameraInstructions : MonoBehaviour
{
    [Header("Instruction Text")]
    public TextMeshProUGUI instructionText;

    [Header("Scene-Specific Messages")]
    public MessageSet[] sceneMessages;

    [Header("Settings")]
    public float displayTime = 4f;
    public float fadeSpeed = 2f;
    public float startDelay = 2f;

    [Header("Glow Effects")]
    public Color textColor = Color.white;
    public Color glowColor = Color.cyan;
    public float glowIntensity = 0.5f;
    public float pulseSpeed = 2f;
    public float outlineWidth = 0.2f;
    public Color outlineColor = Color.black;

    [System.Serializable]
    public class MessageSet
    {
        public string sceneName;           // Which scene these messages are for
        [TextArea(2, 3)]
        public string[] messages;          // Messages for that scene
        public Color sceneGlowColor = Color.cyan;  // Optional: different glow per scene
    }

    private int currentIndex = 0;
    private CanvasGroup canvasGroup;
    private Material textMaterial;
    private float originalGlow;
    private string currentScene;
    private string[] activeMessages;

    void Start()
    {
        // Get current scene name
        currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"CameraInstructions starting in scene: {currentScene}");

        // Get or add CanvasGroup
        canvasGroup = instructionText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = instructionText.gameObject.AddComponent<CanvasGroup>();

        // Setup TextMeshPro properties
        instructionText.color = textColor;
        instructionText.outlineWidth = outlineWidth;
        instructionText.outlineColor = outlineColor;

        // Load messages for this scene
        LoadMessagesForCurrentScene();

        // Setup glow through material
        SetupGlowMaterial();

        // Start hidden
        canvasGroup.alpha = 0f;

        // Start instruction sequence if there are messages
        if (activeMessages != null && activeMessages.Length > 0)
        {
            StartCoroutine(InstructionSequence());
        }
        else
        {
            Debug.LogWarning($"No messages found for scene: {currentScene}");
        }
    }

    void LoadMessagesForCurrentScene()
    {
        // Find messages for current scene
        foreach (MessageSet set in sceneMessages)
        {
            if (set.sceneName == currentScene)
            {
                activeMessages = set.messages;
                if (set.sceneGlowColor != glowColor)
                {
                    glowColor = set.sceneGlowColor; // Use scene-specific glow
                }
                Debug.Log($"Loaded {activeMessages.Length} messages for {currentScene}");
                return;
            }
        }

        // If no scene-specific messages found, try to use first set as default
        if (sceneMessages.Length > 0)
        {
            activeMessages = sceneMessages[0].messages;
            Debug.Log($"No specific messages for {currentScene}, using default");
        }
        else
        {
            Debug.LogError("No message sets configured in Inspector!");
        }
    }

    void SetupGlowMaterial()
    {
        // Get the material
        textMaterial = instructionText.fontMaterial;

        if (textMaterial != null)
        {
            // Try common glow property names
            if (textMaterial.HasProperty("_GlowPower"))
            {
                textMaterial.SetFloat("_GlowPower", glowIntensity);
                originalGlow = glowIntensity;
            }

            if (textMaterial.HasProperty("_Glow"))
            {
                textMaterial.EnableKeyword("GLOW_ON");
                textMaterial.SetFloat("_Glow", glowIntensity);
                originalGlow = glowIntensity;
            }

            if (textMaterial.HasProperty("_OutlineGlow"))
            {
                textMaterial.SetFloat("_OutlineGlow", glowIntensity);
                originalGlow = glowIntensity;
            }

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
        while (currentIndex < activeMessages.Length)
        {
            instructionText.text = activeMessages[currentIndex];

            // Fade in
            yield return StartCoroutine(Fade(0f, 1f));

            // Wait
            yield return new WaitForSeconds(displayTime);

            // Fade out
            yield return StartCoroutine(Fade(1f, 0f));

            currentIndex++;
            yield return new WaitForSeconds(0.5f);
        }

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