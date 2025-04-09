using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class InventorySlotBase : MonoBehaviour, IDropHandler
{
    // Imagen que muestra el icono del item en este slot.
    public Image icon;

    /// <summary>
    /// Actualiza la visualización del slot.
    /// </summary>


    // Método abstracto para validar si se puede colocar el item en este slot.
    public abstract bool CanPlaceItem(Item item);

    public abstract void OnDrop(PointerEventData eventData);
}
