using UnityEngine;

public class UIFollowCamera : MonoBehaviour
{
    public float distanceFromCamera = 2f;
    public float heightOffset = -0.5f;
    public bool faceCamera = true;

    private Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (playerCamera == null) return;

        // Position the canvas in front of camera
        Vector3 targetPosition = playerCamera.position +
                                 playerCamera.forward * distanceFromCamera +
                                 Vector3.up * heightOffset;

        transform.position = targetPosition;

        // Make canvas face the camera
        if (faceCamera)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);
        }
    }
}