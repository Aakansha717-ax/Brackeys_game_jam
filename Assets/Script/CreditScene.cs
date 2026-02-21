using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditScene : MonoBehaviour
{
    [Header("Credit Text")]
    public TextMeshProUGUI creditText;

    [Header("Strange Place Object")]
    public GameObject strangePlaceObject;
    public float objectAppearDelay = 1f;
    public float objectFadeTime = 2f;

    [Header("Buttons")]
    public Button mainMenuButton;    // Appears at end
    public Button skipButton;        // Appears during credits

    [Header("Credit Content")]
    [TextArea(3, 5)]
    public string[] creditLines = new string[]
    {
        "",
        "STRANGE PLACES",
        "",
        "A Game By",
        "═══════════════",
        "",
        "AAKANSHA",
        "",
        "",
        "SOUND FX",
        "═══════════",
        "",
        "PIXABAY",
        "(Royalty Free)",
        "",
        "",
        "Thank you for playing!",
        "",
        "Made with ♥ for",
        "Brackeys Game Jam 2025",
        "",
        "",
        "☠"
    };

    [Header("Settings")]
    public float scrollSpeed = 30f;
    public float startDelay = 1f;
    public float endWaitTime = 3f;
    public float skipButtonDelay = 2f; // Skip button appears after this many seconds

    [Header("Audio")]
    public AudioClip creditMusic;
    public AudioClip appearSound;
    public AudioClip skipSound;

    private RectTransform textRect;
    private float startY;
    private float endY;
    private bool isScrolling = true;
    private bool objectShown = false;
    private bool isSkipping = false;

    private CanvasGroup mainMenuCanvasGroup;
    private CanvasGroup skipCanvasGroup;
    private CanvasGroup objectCanvasGroup;

    void Start()
    {
        Debug.Log("=== CREDIT SCENE STARTED ===");

        // Setup credit text
        if (creditText != null)
        {
            creditText.text = string.Join("\n", creditLines);
            textRect = creditText.GetComponent<RectTransform>();
            startY = textRect.anchoredPosition.y;
            float textHeight = creditText.preferredHeight;
            endY = startY + textHeight + 500;
            Debug.Log($"Credit scroll: from {startY} to {endY}");
        }

        // SETUP MAIN MENU BUTTON - HIDDEN
        SetupButton(mainMenuButton, ref mainMenuCanvasGroup, false);

        // SETUP SKIP BUTTON - SHOW AFTER DELAY
        SetupButton(skipButton, ref skipCanvasGroup, true);
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipCredits);

            // Start hidden
            if (skipCanvasGroup != null)
            {
                skipCanvasGroup.alpha = 0f;
                skipCanvasGroup.interactable = false;
                skipCanvasGroup.blocksRaycasts = false;
            }

            // Show skip button after delay
            StartCoroutine(ShowSkipButton());
        }

        // SETUP STRANGE OBJECT - COMPLETELY HIDDEN
        if (strangePlaceObject != null)
        {
            strangePlaceObject.SetActive(false);
            objectCanvasGroup = strangePlaceObject.GetComponent<CanvasGroup>();
            if (objectCanvasGroup == null)
                objectCanvasGroup = strangePlaceObject.AddComponent<CanvasGroup>();

            Debug.Log($"Strange object {strangePlaceObject.name} disabled at start");
        }

        // Play music
        if (AudioManager.Instance != null && creditMusic != null)
        {
            AudioManager.Instance.PlayMusic(creditMusic);
        }

        // Start credit sequence
        StartCoroutine(CreditSequence());
    }

    void SetupButton(Button button, ref CanvasGroup canvasGroup, bool interactive)
    {
        if (button == null) return;

        canvasGroup = button.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = button.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = interactive ? 1f : 0f;
        canvasGroup.interactable = interactive;
        canvasGroup.blocksRaycasts = interactive;
    }

    IEnumerator ShowSkipButton()
    {
        yield return new WaitForSeconds(skipButtonDelay);

        if (skipButton != null && skipCanvasGroup != null && !isSkipping)
        {
            Debug.Log("Showing skip button");
            skipCanvasGroup.alpha = 1f;
            skipCanvasGroup.interactable = true;
            skipCanvasGroup.blocksRaycasts = true;

            // Pulse animation
            StartCoroutine(PulseButton(skipButton));
        }
    }

    IEnumerator PulseButton(Button button)
    {
        while (!isSkipping && button != null)
        {
            float pulse = Mathf.Sin(Time.time * 3f) * 0.1f + 0.9f;
            button.transform.localScale = Vector3.one * pulse;
            yield return null;
        }
        button.transform.localScale = Vector3.one;
    }

    void Update()
    {
        if (isScrolling && textRect != null && !isSkipping)
        {
            Vector2 pos = textRect.anchoredPosition;
            pos.y += scrollSpeed * Time.deltaTime;
            textRect.anchoredPosition = pos;

            if (pos.y >= endY && !objectShown)
            {
                isScrolling = false;
                Debug.Log("Credit scroll finished");
                StartCoroutine(ShowStrangePlace());
            }
        }
    }

    IEnumerator CreditSequence()
    {
        yield return new WaitForSeconds(startDelay);
        Debug.Log("Credit sequence started");
    }

    public void SkipCredits()
    {
        if (isSkipping) return;

        isSkipping = true;
        Debug.Log("Skipping credits...");

        // Play skip sound
        if (AudioManager.Instance != null && skipSound != null)
        {
            AudioManager.Instance.PlaySFX(skipSound);
        }

        // Hide skip button
        if (skipCanvasGroup != null)
        {
            skipCanvasGroup.alpha = 0f;
            skipCanvasGroup.interactable = false;
        }

        // Immediately go to main menu
        StopAllCoroutines();
        GoToMainMenu();
    }

    IEnumerator ShowStrangePlace()
    {
        objectShown = true;
        yield return new WaitForSeconds(objectAppearDelay);

        if (strangePlaceObject != null && !isSkipping)
        {
            Debug.Log("✨ STRANGE PLACE APPEARS!");

            strangePlaceObject.SetActive(true);

            if (objectCanvasGroup != null)
                objectCanvasGroup.alpha = 0f;

            if (AudioManager.Instance != null && appearSound != null)
            {
                AudioManager.Instance.PlaySFX(appearSound);
            }

            // Fade in
            float elapsed = 0f;
            while (elapsed < objectFadeTime && !isSkipping)
            {
                elapsed += Time.deltaTime;
                if (objectCanvasGroup != null)
                {
                    objectCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / objectFadeTime);
                }

                float scale = 1f + Mathf.Sin(elapsed * 10f) * 0.05f;
                strangePlaceObject.transform.localScale = Vector3.one * scale;

                yield return null;
            }

            if (!isSkipping)
            {
                if (objectCanvasGroup != null)
                    objectCanvasGroup.alpha = 1f;

                Debug.Log("Strange Place now visible");
                yield return new WaitForSeconds(endWaitTime);

                if (!isSkipping)
                    StartCoroutine(ShowMainMenuButton());
            }
        }
    }

    IEnumerator ShowMainMenuButton()
    {
        if (mainMenuButton != null && mainMenuCanvasGroup != null && !isSkipping)
        {
            Debug.Log("Showing main menu button");

            mainMenuCanvasGroup.interactable = true;
            mainMenuCanvasGroup.blocksRaycasts = true;

            float elapsed = 0f;
            while (elapsed < 1f && !isSkipping)
            {
                elapsed += Time.deltaTime;
                mainMenuCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed);
                yield return null;
            }

            if (!isSkipping)
                mainMenuCanvasGroup.alpha = 1f;
        }
    }

    public void GoToMainMenu()
    {
        Debug.Log("Going to Main Menu");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.transitionSound);
        }

        SceneManager.LoadScene("Main_menu");
    }
}