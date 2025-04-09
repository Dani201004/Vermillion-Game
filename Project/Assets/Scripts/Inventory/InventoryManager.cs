using System.Collections.Generic;  // Opcional según tu proyecto
using UnityEngine;

// Clase que gestiona el inventario y el equipamiento.
public static class InventoryManager
{
    // Tamaños configurables
    private static int InventorySize = 36;
    public static int EquipmentSize = 3;
    public static Item[] inventoryItems;
    public static Item[] equipmentItemsAdventurer;
    public static Item[] equipmentItemsWitch;
    public static Item[] equipmentItemsPaladin;
    public static Item[] equipmentItemsCleric;

    public static Item[] currentEquipmentItems;
    private static string equipmentID;


    static InventoryManager()
    {
        inventoryItems = new Item[InventorySize];
        equipmentItemsAdventurer = new Item[EquipmentSize];
        equipmentItemsWitch = new Item[EquipmentSize];
        equipmentItemsPaladin = new Item[EquipmentSize];
        equipmentItemsCleric = new Item[EquipmentSize];


        currentEquipmentItems = CloneEquipmentArray(equipmentItemsAdventurer);
        equipmentID = "Adventurer";
    }

    /// <summary>
    /// Agrega un ítem al inventario general.  
    /// Si el ítem es de equipo, se añade al inventario (no se equipa automáticamente).
    /// </summary>
    public static bool AddItem(Item newItem)
    {
        bool result = AddInventory(newItem);
        PrintInventoryItems();
        return result;
    }

    /// <summary>
    /// Agrega un ítem al inventario.
    /// Los ítems consumibles o de misión se acumulan; los de equipo requieren un slot vacío
    /// y no se permite duplicarlos (es decir, solo se puede tener una vez cada arma, armadura o accesorio).
    /// </summary>
    private static bool AddInventory(Item newItem)
    {
        if (newItem.IsEquipo())
        {
            if (HasEquipmentItem(newItem))
            {
                Debug.Log("Ya tienes este ítem de equipo. No se permiten duplicados.");
                return false;
            }
        }

        for (int i = 0; i < inventoryItems.Length; i++)
        {

            if (inventoryItems[i] == null)
            {
                inventoryItems[i] = newItem;
                return true;
            }
            else if (!newItem.IsEquipo() && inventoryItems[i].Name == newItem.Name)
            {
                inventoryItems[i].Quantity += newItem.Quantity;
                return true;
            }
        }
        Debug.Log("Inventario lleno.");
        return false;
    }

    /// <summary>
    /// Revisa en el inventario y en el equipamiento actual si ya existe un ítem de equipo con el mismo nombre.
    /// </summary>
    private static bool HasEquipmentItem(Item newItem)
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] != null && inventoryItems[i].IsEquipo() && inventoryItems[i].Name == newItem.Name)
                return true;
        }

        for (int i = 0; i < currentEquipmentItems.Length; i++)
        {
            if (currentEquipmentItems[i] != null && currentEquipmentItems[i].IsEquipo() && currentEquipmentItems[i].Name == newItem.Name)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Equipa un ítem moviéndolo del inventario al slot correspondiente del equipamiento.
    /// Solo se pueden equipar ítems de tipo equipo.
    /// </summary>
    public static bool EquipItem(string name)
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] != null &&
                inventoryItems[i].Name == name &&
                inventoryItems[i].IsEquipo())
            {
                int slotIndex = GetEquipmentSlotIndex(inventoryItems[i].itemType);
                if (slotIndex < 0 || slotIndex >= currentEquipmentItems.Length)
                {
                    Debug.LogError("Tipo de equipo inválido.");
                    return false;
                }


                if (currentEquipmentItems[slotIndex] == null)
                {
                    currentEquipmentItems[slotIndex] = inventoryItems[i];
                    inventoryItems[i] = null;
                    PrintInventoryItems();
                    PrintEquipmentItems();
                    return true;
                }
                else
                {
                    Debug.Log("El slot de " + inventoryItems[i].itemType + " ya está ocupado.");
                    return false;
                }
            }
        }
        Debug.Log("No se encontró el ítem en el inventario o no es un ítem de equipo.");
        return false;
    }

    /// <summary>
    /// Desequipa un ítem moviéndolo del equipamiento de vuelta al inventario.
    /// </summary>
    public static bool UnequipItem(string name)
    {
        // Buscar en el equipamiento el ítem a desequipar.
        for (int i = 0; i < currentEquipmentItems.Length; i++)
        {
            if (currentEquipmentItems[i] != null && currentEquipmentItems[i].Name == name)
            {
                // Intentamos añadir al inventario.
                if (AddInventory(currentEquipmentItems[i]))
                {
                    currentEquipmentItems[i] = null;
                    PrintInventoryItems();
                    PrintEquipmentItems();
                    return true;
                }
                else
                {
                    Debug.Log("Inventario lleno, no se puede desequipar el ítem.");
                    return false;
                }
            }
        }
        Debug.Log("No se encontró el ítem equipado.");
        return false;
    }

    /// <summary>
    /// Elimina el primer ítem que se encuentre con el nombre indicado, ya sea del inventario o del equipamiento.
    /// </summary>
    public static void RemoveItem(string name)
    {
        // Buscar en el inventario.
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] != null && inventoryItems[i].Name == name)
            {
                inventoryItems[i] = null;
                PrintInventoryItems();
                return;
            }
        }
        // Buscar en el equipamiento.
        for (int i = 0; i < currentEquipmentItems.Length; i++)
        {
            if (currentEquipmentItems[i] != null && currentEquipmentItems[i].Name == name)
            {
                currentEquipmentItems[i] = null;
                PrintEquipmentItems();
                return;
            }
        }
    }

    /// <summary>
    /// Guarda el inventario y equipamiento en PlayerPrefs.
    /// </summary>
    public static void SaveInventory()
    {
        List<string> inventoryData = new List<string>();
        foreach (var item in inventoryItems)
        {
            if (item != null)
                inventoryData.Add(JsonUtility.ToJson(item));
        }
        PlayerPrefs.SetString("Inventory", string.Join("|", inventoryData));

        List<string> equipmentData = new List<string>();
        foreach (var item in currentEquipmentItems)
        {
            if (item != null)
                equipmentData.Add(JsonUtility.ToJson(item));
        }
        PlayerPrefs.SetString("Equipment", string.Join("|", equipmentData));

        PlayerPrefs.SetString("EquipmentID", equipmentID);

        PlayerPrefs.Save();
        Debug.Log("Inventario guardado.");
    }

    /// <summary>
    /// Carga el inventario y equipamiento desde PlayerPrefs.
    /// </summary>
    public static void LoadInventory()
    {
        if (PlayerPrefs.HasKey("Inventory"))
        {
            string[] inventoryData = PlayerPrefs.GetString("Inventory").Split('|');
            inventoryItems = new Item[InventorySize];

            for (int i = 0; i < inventoryData.Length; i++)
            {
                if (!string.IsNullOrEmpty(inventoryData[i]))
                    inventoryItems[i] = JsonUtility.FromJson<Item>(inventoryData[i]);
            }
        }

        if (PlayerPrefs.HasKey("Equipment"))
        {
            string[] equipmentData = PlayerPrefs.GetString("Equipment").Split('|');
            currentEquipmentItems = new Item[EquipmentSize];

            for (int i = 0; i < equipmentData.Length; i++)
            {
                if (!string.IsNullOrEmpty(equipmentData[i]))
                    currentEquipmentItems[i] = JsonUtility.FromJson<Item>(equipmentData[i]);
            }
        }

        if (PlayerPrefs.HasKey("EquipmentID"))
        {
            equipmentID = PlayerPrefs.GetString("EquipmentID");
        }
        else
        {
            equipmentID = "Adventurer";
        }

        Debug.Log("Inventario cargado.");
    }

    /// <summary>
    /// Permite modificar el inventario externamente usando JSON.
    /// </summary>
    public static void SetInventoryFromJson(string json)
    {
        PlayerPrefs.SetString("Inventory", json);
        PlayerPrefs.Save();
        LoadInventory();
        Debug.Log("Inventario modificado externamente.");
    }

    /// <summary>
    /// Mapea el tipo de ítem a un índice específico del array de equipamiento.
    /// Ajustado para que:
    /// - EquipoArma: índice 0
    /// - EquipoArmadura: índice 1
    /// - EquipoAccesorio: índice 2
    /// </summary>
    private static int GetEquipmentSlotIndex(ItemType itemType)
    {
        switch (itemType)
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

    // Métodos para obtener los arrays completos (opcional).
    public static Item[] GetInventoryItems() => inventoryItems;
    public static Item[] GetEquipmentItems() => currentEquipmentItems;

    /// <summary>
    /// Devuelve el ID del equipamiento actualmente activo.
    /// Esta función es necesaria para integrarlo en el guardado.
    /// </summary>
    public static string GetCurrentEquipmentID() => equipmentID;

    private static void PrintInventoryItems()
    {
        string invItems = "Inventario: ";
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] != null)
                invItems += $"[{i}: {inventoryItems[i].Name} x{inventoryItems[i].Quantity}] ";
            else
                invItems += $"[{i}: vacío] ";
        }
        Debug.Log(invItems);
    }

    private static void PrintEquipmentItems()
    {
        string equipItems = "Equipamiento: ";
        for (int i = 0; i < currentEquipmentItems.Length; i++)
        {
            if (currentEquipmentItems[i] != null)
                equipItems += $"[{i}: {currentEquipmentItems[i].Name}] ";
            else
                equipItems += $"[{i}: vacío] ";
        }
        Debug.Log(equipItems);
    }

    /// <summary>
    /// Cambia el personaje actual, guardando y asignando el equipamiento correspondiente.
    /// Se realiza una copia de los arrays para evitar compartir referencias y efectos secundarios.
    /// </summary>
    public static void ChangeCharacter(string characterName)
    {
        // Guardar el equipamiento actual copiando el contenido en el array persistente del personaje activo.
        switch (equipmentID)
        {
            case "Adventurer":
                CopyEquipment(currentEquipmentItems, equipmentItemsAdventurer);
                break;
            case "Witch":
                CopyEquipment(currentEquipmentItems, equipmentItemsWitch);
                break;
            case "Paladin":
                CopyEquipment(currentEquipmentItems, equipmentItemsPaladin);
                break;
            case "Cleric":
                CopyEquipment(currentEquipmentItems, equipmentItemsCleric);
                break;
            default:
                Debug.LogWarning("Personaje activo desconocido.");
                break;
        }

        equipmentID = characterName;

        // Asignar el array de equipamiento correspondiente (haciendo una copia para trabajar independientemente).
        switch (characterName)
        {
            case "Adventurer":
                currentEquipmentItems = CloneEquipmentArray(equipmentItemsAdventurer);
                InventoryUIController.Instance.UpdateUI();
                break;
            case "Witch":
                currentEquipmentItems = CloneEquipmentArray(equipmentItemsWitch);
                InventoryUIController.Instance.UpdateUI();
                break;
            case "Paladin":
                currentEquipmentItems = CloneEquipmentArray(equipmentItemsPaladin);
                InventoryUIController.Instance.UpdateUI();
                break;
            case "Cleric":
                currentEquipmentItems = CloneEquipmentArray(equipmentItemsCleric);
                InventoryUIController.Instance.UpdateUI();
                break;
            default:
                Debug.LogError("Personaje no reconocido.");
                break;
        }

        PrintEquipmentItems();
    }

    /// <summary>
    /// Copia el contenido del array source en el array target (asumiendo que tienen la misma longitud).
    /// </summary>
    private static void CopyEquipment(Item[] source, Item[] target)
    {
        for (int i = 0; i < source.Length; i++)
        {
            target[i] = source[i];
        }
    }

    /// <summary>
    /// Crea y devuelve una copia (nuevo array) del array de equipamiento.
    /// </summary>
    public static Item[] CloneEquipmentArray(Item[] arrayToClone)
    {
        Item[] newArray = new Item[arrayToClone.Length];
        for (int i = 0; i < arrayToClone.Length; i++)
        {
            newArray[i] = arrayToClone[i];
        }
        return newArray;
    }

    /// <summary>
    /// Elimina (o reduce en cantidad) un ítem del inventario guardado en PlayerPrefs.
    /// </summary>
    public static void RemoveItemFromSave(string itemName)
    {
        // Buscar en el inventario en memoria
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] != null && inventoryItems[i].Name == itemName)
            {
                if (inventoryItems[i].Quantity > 1)
                {
                    inventoryItems[i].Quantity--; // Reducir cantidad
                }
                else
                {
                    inventoryItems[i] = null; // Eliminar si la cantidad es 1
                }

                PrintInventoryItems(); // Actualizar la UI o logs
                return; // Salir tras encontrar el ítem
            }
        }

        // Buscar en el equipamiento
        for (int i = 0; i < currentEquipmentItems.Length; i++)
        {
            if (currentEquipmentItems[i] != null && currentEquipmentItems[i].Name == itemName)
            {
                if (currentEquipmentItems[i].Quantity > 1)
                {
                    currentEquipmentItems[i].Quantity--; // Reducir cantidad
                }
                else
                {
                    currentEquipmentItems[i] = null; // Eliminar si la cantidad es 1
                }

                PrintEquipmentItems(); // Actualizar la UI o logs
                return; // Salir tras encontrar el ítem
            }
        }

        Debug.LogWarning($"El ítem '{itemName}' no se encontró en el inventario ni en el equipamiento.");
    }

    /// <summary>
    /// Retorna una lista con todos los ítems consumibles (no de equipo), considerando su cantidad.
    /// </summary>
    public static List<Item> GetConsumableItemsList()
    {
        List<Item> consumables = new List<Item>();


        for (int i = 0; i < inventoryItems.Length; i++)
        {
            Item currentItem = inventoryItems[i];
            if (currentItem != null && !currentItem.IsEquipo())
            {

                for (int j = 0; j < currentItem.Quantity; j++)
                {
                    consumables.Add(currentItem);
                }
            }
        }

        return consumables;
    }
}
