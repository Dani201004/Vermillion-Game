using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item item;
    public Image image;

    // Agrega esta variable para almacenar el parent original.
    [HideInInspector]
    public Transform originalParent;

    [HideInInspector]
    public bool droppedSuccessful = false;


    void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Guarda el parent original para luego poder intercambiarlo.
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;

        // Otras acciones que necesites...
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;

        if (!droppedSuccessful)
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
            // Otras acciones si el drop no fue exitoso...
        }

        droppedSuccessful = false;
    }
}
