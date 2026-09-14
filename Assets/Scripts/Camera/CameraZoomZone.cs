using UnityEngine;

public class CameraZoomZone : MonoBehaviour
{
    [Header("Zoom Settings")]
    [Tooltip("Camera Orthographic Size when the Hero is inside this zone.")]
    public float targetZoom = 5f;

    [Tooltip("How quickly the camera zooms in and out.")]
    public float zoomSpeed = 3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only react to the Hero/Player
        if (!other.CompareTag("Player"))
            return;

        // Find the camera zoom controller
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning("CameraZoomZone: Main Camera not found.");
            return;
        }

        CameraZoomController cameraController =
            mainCamera.GetComponent<CameraZoomController>();

        if (cameraController == null)
        {
            Debug.LogWarning(
                "CameraZoomZone: CameraZoomController is missing from the Main Camera."
            );
            return;
        }

        // Tell the camera to zoom in
        cameraController.SetTargetZoom(
            targetZoom,
            zoomSpeed
        );
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Only react to the Hero/Player
        if (!other.CompareTag("Player"))
            return;

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        CameraZoomController cameraController =
            mainCamera.GetComponent<CameraZoomController>();

        if (cameraController == null)
            return;

        // Return to the normal camera zoom
        cameraController.RemoveZoomZone(this);
    }
}