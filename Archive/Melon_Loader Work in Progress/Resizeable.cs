
// Resizeable.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class Resizeable : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public RectTransform target;
    private Vector2 originalSize;
    private Vector2 originalMousePosition;

    public void OnPointerDown(PointerEventData eventData)
    {
        originalSize = target.sizeDelta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position, eventData.pressEventCamera, out originalMousePosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localMousePosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position, eventData.pressEventCamera, out localMousePosition))
        {
            Vector2 delta = localMousePosition - originalMousePosition;
            target.sizeDelta = originalSize + new Vector2(delta.x, -delta.y);
        }
    }
}
