using UnityEngine;

public class EnemyStats_Seta2 : EnemyStats
{
    void Start()
    {
        Skill[] skills = {
            new Skill
            {
                Name = "Mushroom attack",
                ScalingPercent = 1.2f,      // Escalado del 50% del ataque físico
                UseMagic = false,           // Es un ataque físico
                DiceType = "D6",            // Dado de 6 caras
                RequiredRoll = 3,           // Necesita 3 o más para acertar
                ManaCost = 0,               // No usa mana
                attackAnimationTrigger = "Attack 1"
},
            new Skill
            {
                Name = "Mushroom trouble",
                ScalingPercent = 1.5f,      // Escalado del 60% del poder mágico
                UseMagic = false,            // Usa magia
                DiceType = "D20",           // Dado de 20 caras
                RequiredRoll = 8,          // Necesita 15 o más para acertar
                ManaCost = 0,               // Usa 50 de mana
                attackAnimationTrigger = "Attack 2"
            }
        };
        // Inicializar estadísticas del enemigo
        InitializeStats(attack: 150, magicPower: 185, armor: 80, maxHealth: 500, maxMana: 0, stamina: 90, speed: 100, skills);

        Debug.Log("Speed de " + gameObject.name + ": " + Speed);

    }


    // Métodos específicos del enemigo pueden ir aquí si es necesario


}