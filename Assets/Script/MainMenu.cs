using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public AudioSource menuAudio;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    void Start()
    {
        // Show main menu, hide options
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);

        // Play menu music
        if (menuAudio != null && !menuAudio.isPlaying)
            menuAudio.Play();
    }

    public void StartGame()
    {
        PlayClick();
        SceneManager.LoadScene("Room_1"); // Load your first game scene
    }

    public void OpenOptions()
    {
        PlayClick();
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void BackToMain()
    {
        PlayClick();
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        PlayClick();
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    // Audio feedback
    public void PlayHover()
    {
        if (hoverSound != null)
            menuAudio.PlayOneShot(hoverSound);
    }

    public void PlayClick()
    {
        if (clickSound != null)
            menuAudio.PlayOneShot(clickSound);
    }
}
