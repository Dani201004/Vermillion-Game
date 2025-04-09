using UnityEngine;

public class EnemyStats_GoblinWarrior : EnemyStats
{
    void Start()
    {
        Skill[] skills = {
            new Skill
            {
                Name = "Cleaver Strike",
                ScalingPercent = 1.1f,      // Escalado del 50% del ataque físico
                UseMagic = false,           // Es un ataque físico
                DiceType = "D6",            // Dado de 6 caras
                RequiredRoll = 2,           // Necesita 3 o más para acertar
                ManaCost = 0,                // No usa mana
                attackAnimationTrigger = "Attack 1"
            },
            new Skill
            {
                Name = "Frenzy",
                ScalingPercent = 1.4f,      // Escalado del 60% del poder mágico
                UseMagic = false,            // Usa magia
                DiceType = "D20",           // Dado de 20 caras
                RequiredRoll = 13,          // Necesita 15 o más para acertar
                ManaCost = 0,               // Usa 50 de mana
                attackAnimationTrigger = "Attack 2"
            }
        };
        // Inicializar estadísticas del enemigo
        InitializeStats(attack: 160, magicPower: 160, armor: 200, maxHealth: 600, maxMana: 0, stamina: 100, speed: 70, skills);

        Debug.Log("Speed de " + gameObject.name + ": " + Speed);

    }


    // Métodos específicos del enemigo pueden ir aquí si es necesario

}
