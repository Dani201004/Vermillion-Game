using UnityEngine;

public class EnemyStats_Seta1 : EnemyStats
{
    void Start()
    {
        Skill[] skills = {
            new Skill
            {
                Name = "Headbutt",
                ScalingPercent = 1.2f,      // Escalado del 50% del ataque f�sico
                UseMagic = false,           // Es un ataque f�sico
                DiceType = "D6",            // Dado de 6 caras
                RequiredRoll = 1,           // Necesita 3 o m�s para acertar
                ManaCost = 0,               // No usa mana
                attackAnimationTrigger = "Attack 1"
},
            new Skill
            {
                Name = "Serious Headbutt",
                ScalingPercent = 1.3f,      // Escalado del 60% del poder m�gico
                UseMagic = false,            // Usa magia
                DiceType = "D20",           // Dado de 20 caras
                RequiredRoll = 10,          // Necesita 15 o m�s para acertar
                ManaCost = 0,               // Usa 50 de mana
                attackAnimationTrigger = "Attack 2"
            }
        };
        // Inicializar estad�sticas del enemigo
        InitializeStats(attack: 140, magicPower: 180, armor: 70, maxHealth: 500, maxMana: 0, stamina: 80, speed: 120, skills);

        Debug.Log("Speed de " + gameObject.name + ": " + Speed);

    }


    // M�todos espec�ficos del enemigo pueden ir aqu� si es necesario


}
