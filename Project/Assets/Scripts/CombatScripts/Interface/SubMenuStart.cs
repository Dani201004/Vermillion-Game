using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SubMenuStart : MonoBehaviour
{
    private List<UnityEvent> onClickEvents = new List<UnityEvent>();
    private BattleMenu battleMenu;
    private BattleCameraController cameraController;
    private FleeBehaviour fleeBehaviour;

    void Start()
    {
        battleMenu = GetComponent<BattleMenu>();
        fleeBehaviour = FindFirstObjectByType<FleeBehaviour>();

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
                    cameraController.FocusOnEnemy();
                }
                else if (index == 1) // Botón de Objetos
                {
                    // Buscamos el controlador del menú de consumibles
                    ConsumablesMenuController consumablesMenu = FindFirstObjectByType<ConsumablesMenuController>();
                    if (consumablesMenu != null)
                    {
                        // Obtenemos la lista de consumibles (cada instancia según su cantidad)
                        List<Item> consumables = InventoryManager.GetConsumableItemsList();
                        if (consumables.Count > 0)
                        {
                            consumablesMenu.DisplayConsumables(consumables);
                        }
                        else
                        {
                            Debug.Log("No hay consumibles en el inventario.");
                        }
                    }
                    else
                    {
                        Debug.LogError("ConsumablesMenuController no encontrado en la escena.");
                    }
                }
                else if (index == 2)
                {
                    fleeBehaviour.CheckDiceRoll();
                }
            });
            onClickEvents.Add(newEvent);
        }

        battleMenu.RedefineOnClick(onClickEvents);
    }
}
