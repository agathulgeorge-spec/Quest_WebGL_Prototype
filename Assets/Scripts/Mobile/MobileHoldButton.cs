using UnityEngine;
using UnityEngine.EventSystems;

public class MobileHoldButton : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    public enum ButtonType
    {
        Left,
        Right,
        E
    }

    public ButtonType buttonType;

    public void OnPointerDown(PointerEventData eventData)
    {
        SetPressed(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetPressed(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetPressed(false);
    }

    private void SetPressed(bool pressed)
    {
        if (buttonType == ButtonType.Left)
        {
            MobileInput.SetLeft(pressed);
        }
        else if (buttonType == ButtonType.Right)
        {
            MobileInput.SetRight(pressed);
        }
        else if (buttonType == ButtonType.E)
        {
            MobileInput.SetE(pressed);
        }
    }

    private void OnDisable()
    {
        SetPressed(false);
    }
}