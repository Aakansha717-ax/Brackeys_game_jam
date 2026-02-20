using UnityEngine;

public class AudioListenerFix : MonoBehaviour
{
    void Start()
    {
        // Find all audio listeners
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();

        if (listeners.Length > 1)
        {
            Debug.Log($"Found {listeners.Length} audio listeners. Keeping one...");

            // Keep the first one, disable others
            for (int i = 1; i < listeners.Length; i++)
            {
                listeners[i].enabled = false;
                Debug.Log($"Disabled AudioListener on {listeners[i].gameObject.name}");
            }
        }
    }
}