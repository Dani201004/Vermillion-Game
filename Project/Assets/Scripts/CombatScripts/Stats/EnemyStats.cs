using UnityEngine;

public class EnemyStats : CharacterStats
{
    [SerializeField] private int experienceReward;
    public bool isMagic { get; private set; }
    void Start()
    {


        Skill[] skills = {
            new Skill
            {
                Name = "Fury Strike",
                ScalingPercent = 1f,      // Escalado del 50% del ataque físico
                UseMagic = false,           // Es un ataque físico
                DiceType = "D6",            // Dado de 6 caras
                RequiredRoll = 3,           // Necesita 3 o más para acertar
                ManaCost = 0,                // No usa mana
                attackAnimationTrigger = "Attack 1"
            },
            new Skill
            {
                Name = "Frenzy",
                ScalingPercent = 0.6f,      // Escalado del 60% del poder mágico
                UseMagic = false,            // Usa magia
                DiceType = "D20",           // Dado de 20 caras
                RequiredRoll = 15,          // Necesita 15 o más para acertar
                ManaCost = 0,               // Usa 50 de mana
                attackAnimationTrigger = "Attack 2"
            }
        };
        // Inicializar estadísticas del enemigo
        InitializeStats(attack: 170, magicPower: 180, armor: 10, maxHealth: 300, maxMana: 300, stamina: 0, speed: 90, skills);

        Debug.Log("Speed de " + gameObject.name + ": " + Speed);

    }



    public void EnemyDie()
    {
        // Encontrar a todos los jugadores en la escena
        PlayerStats[] players = FindObjectsOfType<PlayerStats>();

        if (players.Length > 0)
        {
            int baseExp = experienceReward / players.Length; // Exp base para todos

            foreach (PlayerStats player in players)
            {
                player.HandleExperienceChange(baseExp); // Exp equitativa
            }

            // Elegir un jugador aleatorio para el bono de experiencia
            PlayerStats randomPlayer = players[Random.Range(0, players.Length)];
            int bonusExp = experienceReward / 4; // 25% extra para el jugador elegido aleatoriamente
            randomPlayer.HandleExperienceChange(bonusExp);
            Debug.Log($"{randomPlayer.name} recibió un bonus de {bonusExp} de experiencia.");
        }

        Debug.Log($"El enemigo ha muerto y se repartió {experienceReward} de experiencia.");
    }





    // Métodos específicos del enemigo pueden ir aquí si es necesario
}