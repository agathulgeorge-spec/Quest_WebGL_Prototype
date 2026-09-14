using UnityEngine;
using UnityEngine.EventSystems;

public class BeaconTouchPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public static bool isPressed;
    public static Vector2 aimDirection;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        UpdateAim(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateAim(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        aimDirection = Vector2.zero;
    }

    void UpdateAim(PointerEventData eventData)
    {
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        aimDirection = localPoint.normalized;
    }
}