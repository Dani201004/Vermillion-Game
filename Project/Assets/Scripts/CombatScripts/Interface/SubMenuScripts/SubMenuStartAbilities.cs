using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class SubMenuStartAbilities : MonoBehaviour
{
    public List<Button> skillButtons;  // Botones asignados en el Inspector
    private BattleCameraController cameraController;
    private TurnManager turnManager;
    private PlayerStats playerStats;      // Se actualizará con el jugador activo en turno
    private CharacterStats characterStats;
    private Transform targetEnemy;

    void Start()
    {
        // Obtener TurnManager y BattleCameraController
        turnManager = FindFirstObjectByType<TurnManager>();
        cameraController = GameObject.FindWithTag("MainCamera")?.GetComponent<BattleCameraController>();

        if (cameraController == null)
        {
            Debug.LogError("? BattleCameraController no encontrado en la escena.");
            return;
        }

        // Inicialmente, obtener el personaje del turno actual
        UpdateCurrentPlayer();

        // Asignar las habilidades aprendidas a los botones del menú
        AssignSkillsToButtons();
    }

    // Actualiza la referencia al jugador activo (según el turno)
    void UpdateCurrentPlayer()
    {
        GameObject currentEntity = turnManager.GetCurrentTurnEntity();
        if (currentEntity != null)
        {
            playerStats = currentEntity.GetComponent<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("PlayerStats no encontrado en el personaje del turno actual.");
                return;
            }
            characterStats = playerStats.GetComponent<CharacterStats>();
        }
        else
        {
            Debug.LogError("No se encontró ninguna entidad para el turno actual.");
        }
    }

    /// <summary>
    /// Asigna las habilidades aprendidas del jugador activo a los botones de la UI.
    /// </summary>
    private void AssignSkillsToButtons()
    {
        if (playerStats == null)
        {
            Debug.LogWarning("No hay PlayerStats asignado, no se pueden asignar habilidades.");
            return;
        }

        Debug.Log($"Asignando {playerStats.learnedSkills.Count} habilidades para {playerStats.characterName}");

        for (int i = 0; i < skillButtons.Count; i++)
        {
            if (i < playerStats.learnedSkills.Count)
            {
                Skill skill = playerStats.learnedSkills[i];
                Button button = skillButtons[i];

                Debug.Log($"Asignando {skill.Name} al botón {i}");

                // Mostrar el nombre de la habilidad en el botón
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = skill.Name;

                // Limpiar eventos previos
                button.onClick.RemoveAllListeners();

                cameraController.StopCameraRotation();

                // Asignar el evento de uso de la habilidad
                int skillIndex = i;
                button.onClick.AddListener(() => OnSkillButtonClicked(skillIndex));

                // Asegurar que el botón esté visible e interactivo
                button.interactable = true;
                button.gameObject.SetActive(true);
            }
            else
            {
                // Ocultar botones sin habilidad asignada
                skillButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Llamado cuando se presiona un botón de habilidad.
    /// </summary>
    public void OnSkillButtonClicked(int skillIndex)
    {
        Debug.Log($"Botón de habilidad presionado. Índice recibido: {skillIndex}");
        Skill[] playerSkills = playerStats.skills;
        if (skillIndex < 0 || skillIndex >= playerSkills.Length)
        {
            Debug.LogWarning($"Índice de habilidad inválido: {skillIndex}. Total habilidades: {playerSkills.Length}");
            return;
        }
        UseSkill(skillIndex, playerStats.GetMagic());
    }

    /// <summary>
    /// Usa la habilidad seleccionada y aplica sus efectos.
    /// </summary>
    private void UseSkill(int skillIndex, bool useMagic)
    {
        int damage = characterStats.UseSkill(skillIndex, useMagic);
        if (damage > 0)
        {
            Debug.Log($"Habilidad usada con éxito. Daño calculado: {damage}");
            // Obtener el skill usado para determinar el efecto.
            Skill selectedSkill = playerStats.learnedSkills[skillIndex];

            // Arrays para determinar el tipo de efecto
            string[] physicalAbilities = { "Baluarte de Justicia", "Empujón", "Golpe de Escudo", "Acometida", "Acometida2", "Ataque desde las sombras", "Daga Envenenada", "Danza de sombras", "Degüello", "Finta", "Golpe Contundente", "Instinto asesino", "Puñalada Trapera", "Frenzy", "Cleaver Strike", "Fury Strike", "Sticky attack", "Serious Headbutt", "Headbutt", "Mushroom attack", "Vicious Headbutt", "Mycelus attack", "Infinite Hunger", "Harsh Bite" };
            string[] fireAbilities = { "Bola de Fuego", "Cometa Arcano", "Fuego Feerico", "Ignis", "Unknown gas", "Amaenita Caesara", "Fungus Wrath", "Plea of the Grimy Monarch" };
            string[] lightAbilities = { "Castigo Divino", "Espada Celestial", "Canalización Estelar", "Hipnosis", "Goblin bolt", "Mushroom trouble", "Sovereign Splash" };

            string effectType = "";
            if (physicalAbilities.Contains(selectedSkill.Name))
            {
                effectType = "physical";
            }
            else if (fireAbilities.Contains(selectedSkill.Name))
            {
                effectType = "fire";
            }
            else if (lightAbilities.Contains(selectedSkill.Name))
            {
                effectType = "magic";
            }
            else
            {
                // Por defecto, usar el valor de useMagic
                effectType = useMagic ? "magic" : "physical";
            }

            ApplyDamageToTarget(damage, effectType);
        }
        else
        {
            Debug.LogWarning("La habilidad no causó daño.");
        }
        turnManager.EndTurn();
    }

    /// <summary>
    /// Aplica el daño al enemigo objetivo usando el efecto correspondiente.
    /// </summary>
    private void ApplyDamageToTarget(int damage, string effectType)
    {
        if (targetEnemy == null)
        {
            Debug.LogWarning("No se encontró un enemigo objetivo para aplicar daño.");
            return;
        }

        CharacterStats enemyStats = targetEnemy.GetComponent<CharacterStats>();
        if (enemyStats == null)
        {
            Debug.LogWarning($"El enemigo {targetEnemy.name} no tiene CharacterStats.");
            return;
        }
        enemyStats.TakeDamage(damage, effectType);
        Debug.Log($"Daño de {damage} infligido a {targetEnemy.name} con efecto {effectType}.");
    }

    void Update()
    {
        // Actualizamos el objetivo enemigo
        targetEnemy = cameraController.GetTarget();

        // Cada cierto tiempo (o cuando cambie el turno), actualizamos el PlayerStats actual
        UpdateCurrentPlayer();

        // Opcional: refrescar botones si el turno cambia.
    }
}
