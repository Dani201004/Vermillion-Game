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
            Debug.LogError("BattleCameraController no encontrado en la c�mara principal.");
            return;
        }

        int numberOfButtons = battleMenu.GetButtonsCount();
        for (int i = 0; i < numberOfButtons; i++)
        {
            UnityEvent newEvent = new UnityEvent();
            int index = i;
            newEvent.AddListener(() =>
            {
                // Por ejemplo, el bot�n 0 usa ataque b�sico.
                if (index == 0)
                {
                    ApplyDamageToTargetBasic();
                    cameraController.StopCameraRotation();
                }
                // Si quisieras a�adir habilidades, podr�as agregar otro �ndice aqu�.
            });
            onClickEvents.Add(newEvent);
            Debug.Log("Evento a�adido al bot�n con �xito");
        }

        battleMenu.RedefineOnClick(onClickEvents);
    }

    /// <summary>
    /// Aplica da�o usando ataque b�sico con cooldown.
    /// Se determina el tipo de efecto (magic o physical) en funci�n de si el atacante usa magia.
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
            Debug.LogWarning("No se encontr� la entidad actual del turno.");
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
                // Determinar si el ataque es m�gico o f�sico.
                bool isMagic = attackerStats.GetMagic();
                int damage = attackerStats.CalculateBasicAttackDamage(isMagic); // C�lculo del ataque b�sico.

                // Determinar el efecto: "magic" si el ataque es m�gico, "physical" si es f�sico.
                string effectType = isMagic ? "magic" : "physical";
                enemyStats.TakeDamage(damage, effectType); // Se aplica el da�o pasando el efecto.
                Debug.Log($"{currentEntity.name} infligi� {damage} de da�o a {targetEnemy.name} con efecto {effectType}.");
            }
            else
            {
                Debug.LogWarning($"El enemigo objetivo {targetEnemy.name} no tiene componente CharacterStats.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontr� un objetivo enemigo v�lido.");
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