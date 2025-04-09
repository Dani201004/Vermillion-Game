using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotEquip : InventorySlotBase, IDropHandler
{
    // Tipo de equipo permitido para este slot (Ej.: EquipoArma, EquipoAccesorio, EquipoArmadura).
    public ItemType allowedEquipmentType;

    public override bool CanPlaceItem(Item item)
    {
        // Se permite colocar el item solo si su tipo coincide con el permitido en este slot.
        return item.itemType == allowedEquipmentType;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggable = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (draggable == null)
            return;

        // Verificamos que el item que se arrastra sea del tipo permitido.
        if (!CanPlaceItem(draggable.item))
        {
            Debug.Log("El item " + draggable.item.Name + " no es del tipo permitido para este slot.");
            draggable.droppedSuccessful = false;
            return;
        }

        // Obtenemos el índice de este slot en el array de equipmentItems.
        int equipIndex = GetEquipSlotIndex();
        if (equipIndex < 0)
        {
            Debug.LogError("Equip slot index inválido.");
            draggable.droppedSuccessful = false;
            return;
        }

        // Si el slot ya tiene un item, se realiza el intercambio.
        if (transform.childCount > 0)
        {
            // Obtenemos el DraggableItem que ya está en este slot.
            DraggableItem existingDraggable = transform.GetChild(0).GetComponent<DraggableItem>();
            if (existingDraggable == null)
            {
                Debug.LogError("El item existente no tiene componente DraggableItem.");
                draggable.droppedSuccessful = false;
                return;
            }

            // Guardamos el slot de origen del item arrastrado (se debe haber asignado en OnBeginDrag).
            Transform sourceParent = draggable.originalParent;
            if (sourceParent == null)
            {
                Debug.LogError("El draggable no tiene originalParent asignado.");
                draggable.droppedSuccessful = false;
                return;
            }

            // Se intercambian los items en la UI:
            // 1. El item que estaba en el slot de equipo se mueve al slot original del draggable.
            existingDraggable.transform.SetParent(sourceParent);
            existingDraggable.transform.localPosition = Vector3.zero;

            // 2. El item arrastrado se coloca en este slot de equipo.
            draggable.transform.SetParent(this.transform);
            draggable.transform.localPosition = Vector3.zero;

            // Se actualizan los arrays de InventoryManager:
            // El slot de equipo (equipIndex) recibe el item arrastrado.
            Item temp = InventoryManager.currentEquipmentItems[equipIndex];
            InventoryManager.currentEquipmentItems[equipIndex] = draggable.item;

            // Se asume que el slot de origen está en el inventario.
            int sourceIndex = sourceParent.GetSiblingIndex();
            InventoryManager.inventoryItems[sourceIndex] = temp;

            draggable.droppedSuccessful = true;
        }
        else
        {
            // Si el slot de equipo está vacío, se coloca el item directamente.
            draggable.transform.SetParent(this.transform);
            draggable.transform.localPosition = Vector3.zero;
            InventoryManager.currentEquipmentItems[equipIndex] = draggable.item;

            // Se limpia el slot original (del inventario) del draggable.
            Transform sourceParent = draggable.originalParent;
            if (sourceParent != null)
            {
                int sourceIndex = sourceParent.GetSiblingIndex();
                InventoryManager.inventoryItems[sourceIndex] = null;
            }
            draggable.droppedSuccessful = true;
        }
    }

    /// <summary>
    /// Mapea el allowedEquipmentType al índice correspondiente en InventoryManager.equipmentItems.
    /// </summary>
    private int GetEquipSlotIndex()
    {
        switch (allowedEquipmentType)
        {
            case ItemType.EquipoArma:
                return 0;
            case ItemType.EquipoArmadura:
                return 1;
            case ItemType.EquipoAccesorio:
                return 2;
            default:
                return -1;
        }
    }
}
