using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotInventory : InventorySlotBase, IDropHandler
{
    public override bool CanPlaceItem(Item item)
    {
        return true; // En los slots del inventario, cualquier item puede colocarse.
    }

    public override void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggable = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (draggable != null)
        {
            // Detectar si el item viene de un slot de equipamiento.
            if (draggable.originalParent.GetComponent<InventorySlotEquip>() != null)
            {
                // Llamar a la función de desequipar para moverlo del equipamiento al inventario.
                bool unequipped = InventoryManager.UnequipItem(draggable.item.Name);
                if (!unequipped)
                {
                    Debug.Log("No se pudo desequipar el item (tal vez el inventario esté lleno).");
                    draggable.droppedSuccessful = false;
                    return;
                }
                // Si se desequipó correctamente, se actualiza la UI
                // (Aquí podrías, por ejemplo, asignar este slot como nuevo padre).
            }

            // Suponiendo que el orden de los slots en la UI coincide con el array de InventoryManager,
            // usamos el índice de hermano para determinar el índice en InventoryManager.
            int targetIndex = transform.GetSiblingIndex();
            Transform originalParent = draggable.originalParent;
            int sourceIndex = originalParent.GetSiblingIndex();

            // Si el slot destino ya tiene un item, realizamos el intercambio.
            if (transform.childCount > 0)
            {
                // Obtenemos el item existente en el slot destino.
                Transform existingItemTransform = transform.GetChild(0);
                DraggableItem existingDraggable = existingItemTransform.GetComponent<DraggableItem>();

                // Mueve el item que ya estaba en el slot al slot origen del item arrastrado.
                existingDraggable.transform.SetParent(originalParent);
                existingDraggable.transform.localPosition = Vector3.zero;

                // Coloca el item arrastrado en el slot destino.
                draggable.transform.SetParent(this.transform);
                draggable.transform.localPosition = Vector3.zero;

                // Actualiza el array del InventoryManager.
                InventoryManager.inventoryItems[targetIndex] = draggable.item;
                InventoryManager.inventoryItems[sourceIndex] = existingDraggable.item;

                draggable.droppedSuccessful = true;
            }
            else
            {
                // Si el slot destino está vacío, simplemente coloca el item.
                draggable.transform.SetParent(this.transform);
                draggable.transform.localPosition = Vector3.zero;
                draggable.droppedSuccessful = true;

                // Actualiza el array del InventoryManager.
                InventoryManager.inventoryItems[targetIndex] = draggable.item;
                InventoryManager.inventoryItems[sourceIndex] = null;
            }
        }
    }
}
