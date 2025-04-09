[System.Serializable]  // Asegúrate de que la clase sea serializable
public class ItemStats
{
    public int AttackBonus;
    public int MagicPowerBonus;
    public int ArmorBonus;
    public int MaxHealthBonus;
    public int MaxManaBonus;
    public int StaminaBonus;
    public int SpeedBonus;

    // Constructor para inicializar las bonificaciones de las estadísticas
    public ItemStats(int attackBonus = 0, int magicPowerBonus = 0, int armorBonus = 0,
                     int maxHealthBonus = 0, int maxManaBonus = 0, int staminaBonus = 0,
                     int speedBonus = 0)
    {
        AttackBonus = attackBonus;
        MagicPowerBonus = magicPowerBonus;
        ArmorBonus = armorBonus;
        MaxHealthBonus = maxHealthBonus;
        MaxManaBonus = maxManaBonus;
        StaminaBonus = staminaBonus;
        SpeedBonus = speedBonus;
    }
}
