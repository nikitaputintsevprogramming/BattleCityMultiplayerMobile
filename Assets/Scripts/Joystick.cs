using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public Image Joystick_BG;
    public Image Joystick_Drag;
    private Vector2 inputVector; // полученные координаты джойстика

    void Start()
    {
        Joystick_BG = GetComponent<Image>();
        Joystick_Drag = GetComponent<Image>();
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        // Reset джойстика
        inputVector = Vector2.zero;
        Joystick_Drag.rectTransform.anchoredPosition = Vector2.zero;
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Joystick_BG.rectTransform, ped.position, ped.pressEventCamera, out pos));
        {
            // Выведем результаты джойстика относительно бэкграунда точки касания и удержания
            pos.x = (pos.x / Joystick_BG.rectTransform.sizeDelta.x);
            pos.y = (pos.y / Joystick_BG.rectTransform.sizeDelta.y);

            inputVector = new Vector2(pos.x * 2, pos.y * 2); // Установка точных координат из касания по формуле
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            Joystick_Drag.rectTransform.anchoredPosition = new Vector2(inputVector.x * (Joystick_BG.rectTransform.sizeDelta.x / 1.080f), inputVector.y * (Joystick_BG.rectTransform.sizeDelta.y / 1.920f));
        }
    }

    public float Horizontal()
    {
        if (inputVector.x != 0)
        {
            return inputVector.x;
        }
        else
        {
            return 0f;
        }
    }

    public float Vertical()
    {
        if (inputVector.y != 0)
        {
            return inputVector.y;
        }
        else
        {
            return 0f;
        }
    }
}
