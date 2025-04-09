using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsumablesMenuController : MonoBehaviour
{
    [Header("Configuración de UI")]
    public GameObject consumableButtonPrefab; // Prefab del botón (debe tener un componente Button y un TextMeshProUGUI)
    public Transform buttonContainer;         // Contenedor donde se instanciarán los botones

    private List<GameObject> currentButtons = new List<GameObject>();

    public void DisplayConsumables(List<Item> consumables)
    {
        buttonContainer.gameObject.SetActive(true);
        ClearButtons();

        foreach (Item consumable in consumables)
        {
            GameObject btnObj = Instantiate(consumableButtonPrefab, buttonContainer);
            Button btn = btnObj.GetComponent<Button>();
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();

            if (btn == null || btnText == null)
            {
                Debug.LogError("El prefab del botón no tiene los componentes necesarios.");
                continue;
            }

            btnText.text = consumable.Name;
            Item capturedItem = consumable;
            btn.onClick.AddListener(() => { OnConsumableButtonClicked(capturedItem); });
            currentButtons.Add(btnObj);
        }
    }

    private void OnConsumableButtonClicked(Item consumable)
    {
        Debug.Log("Consumible usado: " + consumable.Name);

        TurnManager turnManager = FindObjectOfType<TurnManager>();
        if (turnManager == null)
        {
            Debug.LogError("TurnManager no encontrado en la escena.");
            return;
        }

        GameObject currentEntity = turnManager.GetCurrentTurnEntity();
        if (currentEntity == null)
        {
            Debug.LogError("No se encontró la entidad del turno actual.");
            return;
        }

        CharacterStats stats = currentEntity.GetComponent<CharacterStats>();
        if (stats == null)
        {
            Debug.LogError("La entidad del turno actual no tiene un componente CharacterStats.");
            return;
        }

        // Buscar el UI del personaje para mostrar efectos visuales
        CharacterUI characterUI = currentEntity.GetComponentInChildren<CharacterUI>();
        if (characterUI == null)
        {
            Debug.Log("character UI no encontrado");
        }

        if (consumable.Name.Equals("Pocion de Vida", System.StringComparison.OrdinalIgnoreCase))
        {
            float healAmount = Mathf.Min(100f, stats.MaxHealth - stats.CurrentHealth);
            stats.CurrentHealth = Mathf.Min(stats.CurrentHealth + 100f, stats.MaxHealth);
            Debug.Log("Poción de Vida aplicada. Salud: " + stats.CurrentHealth);

            if (characterUI != null && healAmount > 0)
            {
                characterUI.ShowHealth((int)healAmount);
            }
        }
        else if (consumable.Name.Equals("Pocion de Mana", System.StringComparison.OrdinalIgnoreCase))
        {
            float manaRestored = Mathf.Min(100f, stats.MaxMana - stats.CurrentMana);
            stats.CurrentMana = Mathf.Min(stats.CurrentMana + 100f, stats.MaxMana);
            Debug.Log("Poción de Mana aplicada. Maná: " + stats.CurrentMana);

            if (characterUI != null && manaRestored > 0)
            {
                characterUI.ShowMana((int)manaRestored);
            }
        }
        else if (consumable.Name.Equals("Pocion Maxima", System.StringComparison.OrdinalIgnoreCase))
        {
            float healthRestored = stats.MaxHealth - stats.CurrentHealth;
            float manaRestored = stats.MaxMana - stats.CurrentMana;

            stats.CurrentHealth = stats.MaxHealth;
            stats.CurrentMana = stats.MaxMana;
            Debug.Log("Poción Máxima aplicada. Salud y Maná al máximo.");

            if (characterUI != null)
            {
                if (healthRestored > 0)
                {
                    characterUI.ShowHealth((int)healthRestored);
                }
                if (manaRestored > 0)
                {
                    characterUI.ShowMana((int)manaRestored);
                }
            }
        }
        else
        {
            Debug.LogWarning("Consumible desconocido: " + consumable.Name);
        }

        InventoryManager.RemoveItemFromSave(consumable.Name);
        RefreshConsumables();
        turnManager.EndTurn();
    }

    public void RefreshConsumables()
    {
        List<Item> consumables = InventoryManager.GetConsumableItemsList();
        DisplayConsumables(consumables);
    }

    private void ClearButtons()
    {
        foreach (GameObject btn in currentButtons)
        {
            Destroy(btn);
        }
        currentButtons.Clear();
    }
}