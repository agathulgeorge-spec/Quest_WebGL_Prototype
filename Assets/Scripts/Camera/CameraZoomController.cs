using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    [Header("Default Camera")]
    public float defaultZoom = 8f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 3f;

    private Camera cam;
    private float targetZoom;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        targetZoom = defaultZoom;
    }

    private void Start()
    {
        // Start at the normal camera zoom
        cam.orthographicSize = defaultZoom;
    }

    private void Update()
    {
        // Smoothly move toward the target zoom
        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetZoom,
            zoomSpeed * Time.deltaTime
        );
    }

    // Called when entering a zoom zone
    public void SetTargetZoom(float newZoom, float newSpeed)
    {
        targetZoom = newZoom;
        zoomSpeed = newSpeed;
    }

    // Called when leaving a zoom zone
    public void RemoveZoomZone(CameraZoomZone zone)
    {
        targetZoom = defaultZoom;
    }
}