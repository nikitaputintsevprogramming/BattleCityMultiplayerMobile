using UnityEngine;
using UnityEngine.EventSystems;

#pragma warning disable 0649

public class Joystick : MonoBehaviour, IDragHandler, IEndDragHandler
{

    [SerializeField] GameObject Handle;

    [SerializeField] float MoveRadius;

    static Joystick instance;

    private Vector3 offset;
    public Vector2 inputVector;

    public static Vector2 Position
    {
        get 
        {
            return (instance.Handle.transform.position - instance.gameObject.transform.position).normalized;
        }
    }


    private void Start()
    {
        instance = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 inputPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector3 offset = inputPosition - gameObject.transform.position;

        offset = new Vector3(offset.x, offset.y, 0);
        Handle.gameObject.transform.position = gameObject.transform.position + Vector3.ClampMagnitude(offset, MoveRadius);

        inputVector = new Vector2(offset.x, offset.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Handle.gameObject.transform.localPosition = Vector3.zero;
        inputVector = Vector2.zero;
    }

    public float Horizontal()
    {
        return inputVector.x;
    }

    public float Vertical()
    {
        return inputVector.y;
    }

}