using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class UIPushButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityAction onButtonPressed;
    public UnityAction onButtonReleased;

    public void OnPointerDown(PointerEventData eventData)
    {
        onButtonPressed?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onButtonReleased?.Invoke();
    }
}