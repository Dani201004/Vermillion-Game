using UnityEngine;

public class HealthPotionItem : MonoBehaviour
{
    [SerializeField] Sprite potionSprite;
    PlayerStats stats;
    public HealthPotionItem(string name, int quantity, ItemType itemType, Sprite iconSprite = null) //: base(name, quantity, itemType, iconSprite)
    {
        name = "Poción de curación";
        quantity = 1;
        itemType = ItemType.Consumible;
        iconSprite = potionSprite;
    }

    public void UsePotion()
    {
        stats.CurrentHealth = stats.CurrentHealth + 100;
    }

}
