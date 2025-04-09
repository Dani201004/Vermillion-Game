using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private EnemyStats enemyStats;
    private CharacterStats[] allPlayers;
    private TurnManager turnManager;

    [SerializeField] private Animator npcAnimator;

    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        allPlayers = playerObjects.Select(playerObj => playerObj.GetComponent<CharacterStats>()).ToArray();
        turnManager = FindFirstObjectByType<TurnManager>();
    }

    public void ExecuteEnemyTurn()
    {
        if (enemyStats.CurrentHealth <= 0) return;

        HandleEnemyAction(); // Llamada directa en vez de Coroutine
    }

    private void HandleEnemyAction()
    {
        int skillCount = enemyStats.GetSkillCount();
        bool useSkill = skillCount > 0 && Random.Range(0, 2) == 0;

        int totalDamage;
        string effectType;

        if (useSkill)
        {
            int randomSkillIndex = Random.Range(0, skillCount);
            Skill selectedSkill = enemyStats.skills[randomSkillIndex];
            totalDamage = enemyStats.UseSkill(randomSkillIndex, enemyStats.isMagic);
            effectType = GetEffectType(selectedSkill.Name);

            if (npcAnimator != null)
            {
                npcAnimator.SetTrigger(selectedSkill.attackAnimationTrigger);
            }
        }
        else
        {
            bool useMagic = enemyStats.isMagic;
            totalDamage = enemyStats.CalculateBasicAttackDamage(useMagic);
            effectType = useMagic ? "magic" : "physical";
        }

        // 🔹 Aplica el daño inmediatamente
        CharacterStats targetPlayer = GetRandomTargetPlayer();
        if (targetPlayer != null)
        {
            targetPlayer.TakeDamage(totalDamage, effectType);
            Debug.Log($"{targetPlayer.gameObject.name} recibió {totalDamage} de daño con efecto {effectType}.");
        }

        turnManager.EndTurn(); // Asegurar que el turno del enemigo finaliza correctamente
    }

    private string GetEffectType(string skillName)
    {
        string[] physicalAbilities = { "Baluarte de Justicia", "Empujón", "Golpe de Escudo", "Acometida", "Acometida2", "Ataque desde las sombras", "Daga Envenenada", "Danza de sombras", "Degüello", "Finta", "Golpe Contundente", "Instinto asesino", "Puñalada Trapera", "Frenzy", "Cleaver Strike", "Fury Strike", "Sticky attack", "Serious Headbutt", "Headbutt", "Mushroom attack", "Vicious Headbutt", "Mycelus attack", "Infinite Hunger", "Harsh Bite" };
        string[] fireAbilities = { "Bola de Fuego", "Cometa Arcano", "Fuego Feerico", "Ignis", "Unknown gas", "Amaenita Caesara", "Fungus Wrath", "Plea of the Grimy Monarch" };
        string[] lightAbilities = { "Castigo Divino", "Espada Celestial", "Canalización Estelar", "Hipnosis", "Goblin bolt", "Mushroom trouble", "Sovereign Splash" };

        if (physicalAbilities.Contains(skillName)) return "physical";
        if (fireAbilities.Contains(skillName)) return "fire";
        if (lightAbilities.Contains(skillName)) return "magic";

        return enemyStats.isMagic ? "magic" : "physical";
    }

    private CharacterStats GetRandomTargetPlayer()
    {
        CharacterStats[] validPlayers = allPlayers.Where(player => player.CurrentHealth > 0).ToArray();

        if (validPlayers.Length == 0)
        {
            Debug.LogWarning("No hay jugadores con vida para recibir daño.");
            return null;
        }

        bool attackPlayerWithLeastHealth = Random.Range(0, 2) == 0;

        return attackPlayerWithLeastHealth
            ? validPlayers.OrderBy(player => player.CurrentHealth).First()
            : validPlayers.OrderByDescending(player => player.CurrentHealth).First();
    }
}