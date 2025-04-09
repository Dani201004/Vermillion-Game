using UnityEngine;

public class EnemyStatsSlime : EnemyStats
{
    void Start()
    {
        Skill[] skills = {
            new Skill
            {
                Name = "Sticky attack",
                ScalingPercent = 1.1f,      // Escalado del 50% del ataque f�sico
                UseMagic = false,           // Es un ataque f�sico
                DiceType = "D6",            // Dado de 6 caras
                RequiredRoll = 2,           // Necesita 3 o m�s para acertar
                ManaCost = 0,               // No usa mana
                attackAnimationTrigger = "Attack 1"
},
            new Skill
            {
                Name = "Unknown gas",
                ScalingPercent = 1.5f,      // Escalado del 60% del poder m�gico
                UseMagic = true,            // Usa magia
                DiceType = "D20",           // Dado de 20 caras
                RequiredRoll = 17,          // Necesita 15 o m�s para acertar
                ManaCost = 0,               // Usa 50 de mana
                attackAnimationTrigger = "Attack 1"
            }
        };
        // Inicializar estad�sticas del enemigo
        InitializeStats(attack: 140, magicPower: 180, armor: 75, maxHealth: 700, maxMana: 0, stamina: 100, speed: 75, skills);

        Debug.Log("Speed de " + gameObject.name + ": " + Speed);

    }


    // M�todos espec�ficos del enemigo pueden ir aqu� si es necesario


}
