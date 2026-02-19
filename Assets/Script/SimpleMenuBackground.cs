// Just add this to your main menu camera
using UnityEngine;

public class SimpleMenuBackground : MonoBehaviour
{
    void Start()
    {
        Camera.main.backgroundColor = Color.black;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;

        // Optional subtle fog
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.02f, 0.02f, 0.03f);
        RenderSettings.fogDensity = 0.01f;
    }
}