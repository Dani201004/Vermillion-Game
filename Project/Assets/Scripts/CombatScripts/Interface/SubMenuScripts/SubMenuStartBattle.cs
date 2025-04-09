using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SubMenuStartBattle : MonoBehaviour
{
    private List<UnityEvent> onClickEvents = new List<UnityEvent>();
    private BattleMenu battleMenu;
    private BattleCameraController cameraController;
    private Transform targetEnemy;
    private TurnManager turnManager;

    // Variable para controlar el cooldown del ataque
    private bool isOnCooldown = false;

    void Start()
    {
        battleMenu = GetComponent<BattleMenu>();

        if (battleMenu == null)
        {
            Debug.LogError("BattleMenu no encontrado en la escena.");
            return;
        }

        cameraController = GameObject.FindWithTag("MainCamera")?.GetComponent<BattleCameraController>();

        if (cameraController == null)
        {
            Debug.LogError("BattleCameraController no encontrado en la cámara principal.");
            return;
        }

        int numberOfButtons = battleMenu.GetButtonsCount();
        for (int i = 0; i < numberOfButtons; i++)
        {
            UnityEvent newEvent = new UnityEvent();
            int index = i;
            newEvent.AddListener(() =>
            {
                // Por ejemplo, el botón 0 usa ataque básico.
                if (index == 0)
                {
                    ApplyDamageToTargetBasic();
                    cameraController.StopCameraRotation();
                }
                // Si quisieras añadir habilidades, podrías agregar otro índice aquí.
            });
            onClickEvents.Add(newEvent);
            Debug.Log("Evento añadido al botón con éxito");
        }

        battleMenu.RedefineOnClick(onClickEvents);
    }

    /// <summary>
    /// Aplica daño usando ataque básico con cooldown.
    /// Se determina el tipo de efecto (magic o physical) en función de si el atacante usa magia.
    /// </summary>
    private void ApplyDamageToTargetBasic()
    {
        if (isOnCooldown)
        {
            Debug.Log("Ataque en cooldown, espera 2 segundos.");
            return;
        }

        turnManager = FindFirstObjectByType<TurnManager>();

        if (turnManager == null)
        {
            Debug.LogWarning("TurnManager no encontrado.");
            return;
        }

        // Obtener la entidad del turno actual.
        GameObject currentEntity = turnManager.GetCurrentTurnEntity();

        if (currentEntity == null)
        {
            Debug.LogWarning("No se encontró la entidad actual del turno.");
            return;
        }

        // Obtener el componente CharacterStats del atacante.
        PlayerStats attackerStats = currentEntity.GetComponent<PlayerStats>();

        if (attackerStats == null)
        {
            Debug.LogWarning($"La entidad {currentEntity.name} no tiene componente CharacterStats.");
            return;
        }

        if (targetEnemy != null)
        {
            // Obtener el componente CharacterStats del enemigo.
            CharacterStats enemyStats = targetEnemy.GetComponent<CharacterStats>();

            if (enemyStats != null)
            {
                // Determinar si el ataque es mágico o físico.
                bool isMagic = attackerStats.GetMagic();
                int damage = attackerStats.CalculateBasicAttackDamage(isMagic); // Cálculo del ataque básico.

                // Determinar el efecto: "magic" si el ataque es mágico, "physical" si es físico.
                string effectType = isMagic ? "magic" : "physical";
                enemyStats.TakeDamage(damage, effectType); // Se aplica el daño pasando el efecto.
                Debug.Log($"{currentEntity.name} infligió {damage} de daño a {targetEnemy.name} con efecto {effectType}.");
            }
            else
            {
                Debug.LogWarning($"El enemigo objetivo {targetEnemy.name} no tiene componente CharacterStats.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un objetivo enemigo válido.");
        }

        // Finalizar el turno.
        turnManager.EndTurn();

        // Iniciar el cooldown de 2 segundos.
        StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(2f);
        isOnCooldown = false;
    }

    void Update()
    {
        targetEnemy = cameraController.GetTarget();
    }
}