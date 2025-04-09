public interface ICharacterStats
{
    int Attack { get; }
    int MagicPower { get; }
    int Armor { get; }
    float MaxHealth { get; }
    float CurrentHealth { get; set; }
    float CurrentMana { get; set; }
    float MaxMana { get; set; }
    int Stamina { get; set; }
    int Speed { get; }

    void TakeDamage(int damage, string effectType = "");
}

