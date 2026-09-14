using UnityEngine;
using UnityEngine.EventSystems;

public class MobileBeaconJoystick : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [Header("Joystick")]
    public RectTransform joystickArea;
    public RectTransform joystickHandle;

    [Header("Settings")]
    public float radius = 60f;
    public float dragThreshold = 15f;

    private Vector2 startPosition;
    private bool isDragging = false;
    private bool pointerDown = false;


    private void Awake()
    {
        // The Interact button itself is the handle.
        if (joystickHandle == null)
        {
            joystickHandle = GetComponent<RectTransform>();
        }

        // Use the parent as the joystick area.
        if (joystickArea == null && transform.parent != null)
        {
            joystickArea = transform.parent.GetComponent<RectTransform>();
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
        isDragging = false;

        startPosition = joystickHandle.anchoredPosition;

        // E is being held.
        MobileInput.SetE(true);
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (!pointerDown || joystickArea == null)
            return;

        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickArea,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        Vector2 direction = localPoint;

        if (direction.magnitude > dragThreshold)
        {
            isDragging = true;
        }

        if (!isDragging)
            return;

        if (direction.magnitude > radius)
        {
            direction = direction.normalized * radius;
        }

        joystickHandle.anchoredPosition = direction;

        Vector2 normalizedDirection = direction / radius;

        normalizedDirection = Vector2.ClampMagnitude(
            normalizedDirection,
            1f
        );

        MobileInput.SetBeaconDirection(
            normalizedDirection
        );
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;

        MobileInput.SetE(false);
        MobileInput.ResetBeaconDirection();

        joystickHandle.anchoredPosition = startPosition;

        isDragging = false;
    }


    private void OnDisable()
    {
        pointerDown = false;
        isDragging = false;

        MobileInput.SetE(false);
        MobileInput.ResetBeaconDirection();

        if (joystickHandle != null)
        {
            joystickHandle.anchoredPosition = Vector2.zero;
        }
    }
}