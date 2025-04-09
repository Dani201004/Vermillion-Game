using System;
using UnityEngine;

public enum ItemType
{
    Consumible,
    Mision,
    EquipoArma,        // Tipo espec�fico para Armas
    EquipoAccesorio,   // Tipo espec�fico para Accesorios
    EquipoArmadura     // Tipo espec�fico para Armaduras
}

[Serializable]
public class Item
{
    public string Name;
    public int Quantity;
    public ItemType itemType;   // Esto determinar� el tipo de item
    public bool isEquipped = false;
    public Sprite iconSprite;

    // Aqu�, aseguramos que el campo stats sea visible en el Inspector
    [SerializeField]  // Esto hace que stats sea editable en el Inspector
    public ItemStats stats;

    // Constructor del �tem
    public Item(string name, int quantity, ItemType itemType, Sprite iconSprite = null, ItemStats stats = null)
    {
        Name = name;
        Quantity = quantity;
        this.itemType = itemType;
        this.iconSprite = iconSprite;
        this.stats = stats ?? new ItemStats();  // Si no se pasa un ItemStats, se crea uno vac�o
    }

    public string GetName()
    {
        return Name;
    }

    /// <summary>
    /// Verifica si el item es un equipo.
    /// </summary>
    public bool IsEquipo()
    {
        return itemType == ItemType.EquipoArma || itemType == ItemType.EquipoAccesorio || itemType == ItemType.EquipoArmadura;
    }
}