using UnityEngine;

public class EnemyStats_GoblinArcher : EnemyStats
{
    void Start()
    {
        Skill[] skills = {
            new Skill
            {
                Name = "Cleaver Strike",
                ScalingPercent = 1f,      // Escalado del 50% del ataque físico
                UseMagic = false,           // Es un ataque físico
                DiceType = "D6",            // Dado de 6 caras
                RequiredRoll = 2,           // Necesita 3 o más para acertar
                ManaCost = 0,               // No usa mana
                attackAnimationTrigger = "Attack 1"
},
            new Skill
            {
                Name = "Frenzy",
                ScalingPercent = 1.2f,      // Escalado del 60% del poder mágico
                UseMagic = false,            // Usa magia
                DiceType = "D20",           // Dado de 20 caras
                RequiredRoll = 10,          // Necesita 15 o más para acertar
                ManaCost = 0,               // Usa 50 de mana
                attackAnimationTrigger = "Attack 2"
            }
        };
        // Inicializar estadísticas del enemigo
        InitializeStats(attack: 155, magicPower: 180, armor: 15, maxHealth: 500, maxMana: 0, stamina: 100, speed: 90, skills);

        Debug.Log("Speed de " + gameObject.name + ": " + Speed);

    }


    // Métodos específicos del enemigo pueden ir aquí si es necesario


}

