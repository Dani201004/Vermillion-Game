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
                if (index == 0)
                {
                    ActivateFusionWithPartner("Wicht");
                    Debug.Log($"Efecto de Fusi�n: Aumento de Da�o. Ataque combinado:  de da�o.");
                }
                if (index == 1)
                {
                    ActivateFusionWithPartner("Paladin");
                    Debug.Log("Efecto de Fusi�n: Los enemigos recibir�n m�s da�o.");
                }
                if (index == 2)
                {
                    ActivateFusionWithPartner("Cleric");
                    Debug.Log("Efecto de Fusi�n: Reducci�n de Coste de Mana.");
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
                Debug.Log($"Fusi�n activada con {partnerName}.");
            }
            else
            {
                Debug.LogWarning("No se encontr� un compa�ero v�lido para la fusi�n.");
            }
        }
    }
}
