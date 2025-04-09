using UnityEngine;

public class EnemyStats_Seta3 : EnemyStats
{
    void Start()
    {
        Skill[] skills = {
            new Skill
            {
                Name = "Vicious Headbutt",
                ScalingPercent = 1.3f,      // Escalado del 50% del ataque físico
                UseMagic = false,           // Es un ataque físico
                DiceType = "D6",            // Dado de 6 caras
                RequiredRoll = 3,           // Necesita 3 o más para acertar
                ManaCost = 0,               // No usa mana
                attackAnimationTrigger = "Attack 1"
},
            new Skill
            {
                Name = "Amaenita Caesara",
                ScalingPercent = 1.6f,      // Escalado del 60% del poder mágico
                UseMagic = true,            // Usa magia
                DiceType = "D20",           // Dado de 20 caras
                RequiredRoll = 14,          // Necesita 15 o más para acertar
                ManaCost = 0,               // Usa 50 de mana
                attackAnimationTrigger = "Attack 2"
            }
        };
        // Inicializar estadísticas del enemigo
        InitializeStats(attack: 160, magicPower: 190, armor: 20, maxHealth: 700, maxMana: 100, stamina: 80, speed: 60, skills);

        Debug.Log("Speed de " + gameObject.name + ": " + Speed);

    }


    // Métodos específicos del enemigo pueden ir aquí si es necesario


}
