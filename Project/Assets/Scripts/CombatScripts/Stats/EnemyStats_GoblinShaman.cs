using UnityEngine;

public class EnemyStats_GoblinShaman : EnemyStats
{
    void Start()
    {
        Skill[] skills = {
            new Skill
            {
                Name = "Goblin bolt",
                ScalingPercent = 1.3f,      // Escalado del 50% del ataque f�sico
                UseMagic = true,           // Es un ataque f�sico
                DiceType = "D4",            // Dado de 6 caras
                RequiredRoll = 2,           // Necesita 3 o m�s para acertar
                ManaCost = 0,                // No usa mana
                attackAnimationTrigger = "Attack 1"
            },
            new Skill
            {
                Name = "Fireball",
                ScalingPercent = 1.7f,      // Escalado del 60% del poder m�gico
                UseMagic = true,            // Usa magia
                DiceType = "D20",           // Dado de 20 caras
                RequiredRoll = 8,          // Necesita 15 o m�s para acertar
                ManaCost = 6,               // Usa 50 de mana
                attackAnimationTrigger = "Attack 2"
            }
        };
        // Inicializar estad�sticas del enemigo
        InitializeStats(attack: 140, magicPower: 195, armor: 120, maxHealth: 350, maxMana: 200, stamina: 0, speed: 30, skills);

        Debug.Log("Speed de " + gameObject.name + ": " + Speed);

    }


    // M�todos espec�ficos del enemigo pueden ir aqu� si es necesario

}
