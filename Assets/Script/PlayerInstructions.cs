using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerInstructions : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI instructionText;
    public CanvasGroup canvasGroup;

    [Header("Settings")]
    public float displayTime = 5f;
    public float fadeSpeed = 2f;

    [Header("Messages")]
    [TextArea(2, 3)]
    public string[] tutorialMessages;

    private int currentMessageIndex = 0;
    private bool isDisplaying = false;

    void Start()
    {
        // Hide at start
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        // Start first message after short delay
        Invoke("ShowNextMessage", 2f);
    }

    public void ShowMessage(string message, float duration)
    {
        if (instructionText != null && canvasGroup != null)
        {
            instructionText.text = message;
            StopAllCoroutines();
            StartCoroutine(FadeInOut(duration));
        }
    }

    public void ShowNextMessage()
    {
        if (currentMessageIndex < tutorialMessages.Length)
        {
            ShowMessage(tutorialMessages[currentMessageIndex], displayTime);
            currentMessageIndex++;
        }
    }

    IEnumerator FadeInOut(float duration)
    {
        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeSpeed)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Hold
        yield return new WaitForSeconds(duration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeSpeed)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;

        // Show next message after delay
        yield return new WaitForSeconds(2f);
        ShowNextMessage();
    }
}