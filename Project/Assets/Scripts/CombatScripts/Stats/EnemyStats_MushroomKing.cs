using UnityEngine;

public class EnemyStats_MushroomKing : EnemyStats
{
    void Start()
    {
        Skill[] skills = {
            new Skill
            {
                Name = "Mycelus attack",
                ScalingPercent = 1.5f,      // Escalado del 50% del ataque físico
                UseMagic = false,           // Es un ataque físico
                DiceType = "D6",            // Dado de 6 caras
                RequiredRoll = 4,           // Necesita 3 o más para acertar
                ManaCost = 0,               // No usa mana
                attackAnimationTrigger = "Attack 1"
},
            new Skill
            {
                Name = "Fungus Wrath",
                ScalingPercent = 2f,      // Escalado del 60% del poder mágico
                UseMagic = false,            // Usa magia
                DiceType = "D20",           // Dado de 20 caras
                RequiredRoll = 17,          // Necesita 15 o más para acertar
                ManaCost = 0,               // Usa 50 de mana
                attackAnimationTrigger = "Attack 1"
            }
        };
        // Inicializar estadísticas del enemigo
        InitializeStats(attack: 220, magicPower: 180, armor: 85, maxHealth: 3000, maxMana: 0, stamina: 100, speed: 40, skills);

        Debug.Log("Speed de " + gameObject.name + ": " + Speed);

    }


    // Métodos específicos del enemigo pueden ir aquí si es necesario


}

