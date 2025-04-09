using UnityEngine;

public class EnemyStats_SlimeKing : EnemyStats
{
    void Start()
    {
        Skill[] skills = {
            new Skill
            {
                Name = "Sticky attack",
                ScalingPercent = 1.2f,      // Escalado del 50% del ataque f�sico
                UseMagic = false,           // Es un ataque f�sico
                DiceType = "D6",            // Dado de 6 caras
                RequiredRoll = 2,           // Necesita 3 o m�s para acertar
                ManaCost = 0,               // No usa mana
                attackAnimationTrigger = "Attack 1"
},
            new Skill
            {
                Name = "Sovereign Splash",
                ScalingPercent = 1.4f,      // Escalado del 60% del poder m�gico
                UseMagic = false,            // Usa magia
                DiceType = "D20",           // Dado de 20 caras
                RequiredRoll = 17,          // Necesita 15 o m�s para acertar
                ManaCost = 0,               // Usa 50 de mana
                attackAnimationTrigger = "Attack 1"
            }
        };
        // Inicializar estad�sticas del enemigo
        InitializeStats(attack: 210, magicPower: 180, armor: 160, maxHealth: 3500, maxMana: 0, stamina: 100, speed: 75, skills);

        Debug.Log("Speed de " + gameObject.name + ": " + Speed);

    }


    // M�todos espec�ficos del enemigo pueden ir aqu� si es necesario


}
