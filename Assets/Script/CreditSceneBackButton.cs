using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditSceneBackButton : MonoBehaviour
{
    [Header("Button Settings")]
    public Button backButton;
    public string mainMenuSceneName = "MainMenu";

    [Header("Audio")]
    public AudioClip clickSound;

    [Header("Effects")]
    public float delayBeforeLoad = 0.2f;

    private void Start()
    {
        // Find button if not assigned
        if (backButton == null)
            backButton = GetComponent<Button>();

        if (backButton == null)
        {
            Debug.LogError("Back button not found! Please assign in Inspector.");
            return;
        }

        // Add click listener
        backButton.onClick.AddListener(OnBackButtonClicked);

        Debug.Log("Back button setup complete - will return to: " + mainMenuSceneName);
    }

    private void OnBackButtonClicked()
    {
        Debug.Log("Back button clicked - returning to main menu");

        // Play click sound if available
        if (clickSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickSound);
        }

        // Small delay for audio/visual feedback
        Invoke(nameof(LoadMainMenu), delayBeforeLoad);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Optional: Hover effect
    public void OnButtonHover()
    {
        // Scale up slightly
        if (backButton != null)
            backButton.transform.localScale = new Vector3(1.1f, 1.1f, 1f);
    }

    public void OnButtonExit()
    {
        // Scale back to normal
        if (backButton != null)
            backButton.transform.localScale = Vector3.one;
    }
}