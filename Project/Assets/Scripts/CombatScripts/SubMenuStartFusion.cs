using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SubMenuStartFusion : MonoBehaviour
{
    private List<UnityEvent> onClickEvents = new List<UnityEvent>();
    private BattleMenu battleMenu;
    private BattleCameraController cameraController;
    private TurnManager turnManager;

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();
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
                if (index == 0)
                {
                    ActivateFusionWithPartner("Wicht");
                    Debug.Log($"Efecto de Fusión: Aumento de Daño. Ataque combinado:  de daño.");
                }
                if (index == 1)
                {
                    ActivateFusionWithPartner("Paladin");
                    Debug.Log("Efecto de Fusión: Los enemigos recibirán más daño.");
                }
                if (index == 2)
                {
                    ActivateFusionWithPartner("Cleric");
                    Debug.Log("Efecto de Fusión: Reducción de Coste de Mana.");
                }
            });
            onClickEvents.Add(newEvent);
        }

        battleMenu.RedefineOnClick(onClickEvents);
    }

    private void ActivateFusionWithPartner(string partnerName)
    {
        if (turnManager != null)
        {
            CharacterStats partner = turnManager.SelectPartyMember(partnerName);
            if (partner != null)
            {
                turnManager.FusionTurn(partner);
                Debug.Log($"Fusión activada con {partnerName}.");
            }
            else
            {
                Debug.LogWarning("No se encontró un compañero válido para la fusión.");
            }
        }
    }
}
