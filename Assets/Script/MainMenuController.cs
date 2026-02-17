using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public Button startButton;
    public Button optionsButton;
    public Button quitButton;
    public TextMeshProUGUI titleText;

    [Header("Effects")]
    public float glowSpeed = 2f;
    public Color normalColor = Color.white;
    public Color glowColor = Color.red;

    [Header("Background Objects")]
    public GameObject[] floatingObjects; // Your strange objects

    private void Start()
    {
        // Add button listeners
        startButton.onClick.AddListener(StartGame);
        optionsButton.onClick.AddListener(OpenOptions);
        quitButton.onClick.AddListener(QuitGame);

        // Hide cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        // Make title glow
        float glow = Mathf.Sin(Time.time * glowSpeed) * 0.5f + 0.5f;
        titleText.color = Color.Lerp(normalColor, glowColor, glow);

        // Make floating objects move slightly
        foreach (GameObject obj in floatingObjects)
        {
            if (obj != null)
            {
                float hover = Mathf.Sin(Time.time * 2f + obj.GetInstanceID()) * 0.1f;
                obj.transform.localPosition += new Vector3(0, hover * Time.deltaTime, 0);
            }
        }
    }

    public void StartGame()
    {
        // Add fade out effect here
        SceneManager.LoadScene("Room_1");
    }

    public void OpenOptions()
    {
        Debug.Log("Options - to be implemented");
        // You can create an options panel
    }

    public void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }

    // Button hover effects
    public void OnButtonHover(Button button)
    {
        button.transform.localScale = new Vector3(1.1f, 1.1f, 1f);
    }

    public void OnButtonExit(Button button)
    {
        button.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}