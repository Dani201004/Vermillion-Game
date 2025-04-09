using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class SubMenuStartItems : MonoBehaviour
{
    private List<UnityEvent> onClickEvents = new List<UnityEvent>();
    private BattleMenu battleMenu;
    private BattleCameraController cameraController;
    private HealthBar health;
    GameObject HealthBar;
    private TurnManager turnManager;
    private TextMeshPro textMesh;
    private static bool healed = false;

    void Start()
    {
        HealthBar = GameObject.FindGameObjectWithTag("healthBar");
        turnManager = FindFirstObjectByType<TurnManager>();
        battleMenu = GetComponent<BattleMenu>();
        health = HealthBar.GetComponent<HealthBar>();

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
                    if (healed == false)
                    {
                        health.HealCharacter();
                        cameraController.StopCameraRotation();
                        turnManager.EndTurn();
                        healed = true;
                    }
                }
            });
            onClickEvents.Add(newEvent);
            Debug.Log("evento añadido al boton con exito");
        }


        battleMenu.RedefineOnClick(onClickEvents);
    }

    // Update is called once per frame
    void Update()
    {
        if (healed == true)
        {
            this.gameObject.SetActive(false);
        }
    }
}
