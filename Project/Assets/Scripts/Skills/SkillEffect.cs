using UnityEngine;

[System.Serializable]
public class SkillEffect
{
    // Flags para determinar qué efectos aplica la habilidad.
    public bool applyDamageToAllEnemies;
    public bool applyHealToAllAllies;
    public bool applyBuffToAllAllies;
    public bool applyDefenseDebuffToEnemies;

    // Parámetros base de cada efecto.
    public int baseDamageAmount = 50;
    public int baseHealAmount = 30;
    public int baseBuffAmount = 10;
    public int defenseDebuffAmount = 5;
    public string statToBuff = "";
    // Escalado de los efectos (0.1 del stat relevante).
    private const float scalingFactor = 0.8f;
    private const float scalingFactorBuff = 0.1f;

    // Ejecuta los efectos basados en los flags.
    public void ExecuteEffects(CharacterStats caster)
    {
        if (applyDamageToAllEnemies)
        {
            DamageAllEnemies(caster);
        }
        if (applyHealToAllAllies)
        {
            HealAllAllies(caster);
        }
        if (applyBuffToAllAllies)
        {
            BuffAlliesStat(caster);
        }
        if (applyDefenseDebuffToEnemies)
        {
            DebuffAllEnemiesDefense();
        }
    }

    // Inflige daño a todos los enemigos con escalado basado en el ataque o poder mágico del caster.
    private void DamageAllEnemies(CharacterStats caster)
    {
        int scaledDamage = baseDamageAmount + Mathf.RoundToInt(caster.Attack * scalingFactor);
        EnemyStats[] enemies = GameObject.FindObjectsOfType<EnemyStats>();

        foreach (EnemyStats enemy in enemies)
        {
            enemy.TakeDamage(scaledDamage);
        }

        Debug.Log($"Se ha infligido {scaledDamage} de daño a todos los enemigos.");
    }

    // Cura a todos los aliados con escalado basado en el poder mágico del caster.
    private void HealAllAllies(CharacterStats caster)
    {
        int scaledHeal = baseHealAmount + Mathf.RoundToInt(caster.MagicPower * scalingFactor);
        PlayerStats[] allies = GameObject.FindObjectsOfType<PlayerStats>();

        foreach (PlayerStats ally in allies)
        {
            ally.CurrentHealth = Mathf.Min(ally.CurrentHealth + scaledHeal, ally.MaxHealth);
            ally.NotifyHealthChanged();

            HealthBar hb = ally.GetComponentInChildren<HealthBar>();
            if (hb != null)
            {
                hb.SendMessage("UpdateHealthBar", SendMessageOptions.DontRequireReceiver);
            }
        }

        Debug.Log($"Se han curado {scaledHeal} puntos de vida a todos los aliados.");
    }

    // Aumenta una estadística específica a todos los aliados con escalado basado en el ataque o poder mágico del caster.
    private void BuffAlliesStat(CharacterStats caster)
    {
        int scaledBuff = baseBuffAmount + Mathf.RoundToInt((caster.Attack + caster.MagicPower) * scalingFactorBuff);
        PlayerStats[] allies = GameObject.FindObjectsOfType<PlayerStats>();

        foreach (PlayerStats ally in allies)
        {
            switch (statToBuff.ToLower())
            {
                case "attack":
                    ally.Attack += scaledBuff;
                    break;
                case "magicpower":
                    ally.MagicPower += scaledBuff;
                    break;
                case "armor":
                    ally.Armor += scaledBuff;
                    break;
                default:
                    Debug.LogWarning($"Estadística '{statToBuff}' no reconocida.");
                    return;
            }
            Debug.Log($"{ally.gameObject.name} ha recibido un aumento de {scaledBuff} en {statToBuff}.");
        }
    }

    // Reduce la defensa de todos los enemigos.
    private void DebuffAllEnemiesDefense()
    {
        EnemyStats[] enemies = GameObject.FindObjectsOfType<EnemyStats>();

        foreach (EnemyStats enemy in enemies)
        {
            enemy.Armor = Mathf.Max(0, enemy.Armor - defenseDebuffAmount);
            Debug.Log($"{enemy.gameObject.name} ha recibido una reducción de {defenseDebuffAmount} en su defensa.");
        }
    }
}
