using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Turnindicator : MonoBehaviour
{
    public GameObject playerPanel;  // Panel para los aliados (izquierda)
    public GameObject dividerPanel; // Línea divisora (opcional)
    public GameObject enemyPanel;   // Panel para los enemigos (derecha)
    public GameObject namePrefab;   // Prefab para cada nombre en la lista
    public TurnManager turnManager; // Referencia al TurnManager

    private List<GameObject> playerNames = new List<GameObject>();
    private List<GameObject> enemyNames = new List<GameObject>();

    void Start()
    {
        // Inicializar el indicador con las entidades actuales
        UpdateTurnIndicator();
    }

    public void UpdateTurnIndicator()
    {
        // Limpia las listas actuales
        ClearPanel(playerPanel, playerNames);
        ClearPanel(enemyPanel, enemyNames);

        // Recorre el orden de turnos desde el TurnManager
        foreach (CharacterStats character in turnManager.GetTurnOrder())
        {
            // Determina la columna según el tipo de personaje
            GameObject parentPanel = (character is PlayerStats) ? playerPanel : enemyPanel;

            // Instancia el nombre en la columna correspondiente
            GameObject nameGO = Instantiate(namePrefab, parentPanel.transform);
            TMP_Text nameText = nameGO.GetComponent<TMP_Text>();
            nameText.text = character.gameObject.name;

            // Almacena la referencia para actualizaciones futuras
            if (character is PlayerStats)
                playerNames.Add(nameGO);
            else
                enemyNames.Add(nameGO);
        }

        // Destacar al personaje del turno actual
        HighlightCurrentTurn();
    }

    public void HighlightCurrentTurn()
    {
        // Obtén el personaje actual del TurnManager
        GameObject currentTurnEntity = turnManager.GetCurrentTurnEntity();

        // Destaca el nombre correspondiente
        foreach (GameObject nameGO in playerNames)
        {
            TMP_Text nameText = nameGO.GetComponent<TMP_Text>();
            if (nameGO.name == currentTurnEntity.name)
            {
                nameText.fontSize = 36; // Más grande
                nameText.fontStyle = FontStyles.Underline; // Subrayado
            }
            else
            {
                nameText.fontSize = 24; // Normal
                nameText.fontStyle = FontStyles.Normal;
            }
        }

        foreach (GameObject nameGO in enemyNames)
        {
            TMP_Text nameText = nameGO.GetComponent<TMP_Text>();
            if (nameGO.name == currentTurnEntity.name)
            {
                nameText.fontSize = 36; // Más grande
                nameText.fontStyle = FontStyles.Underline; // Subrayado
            }
            else
            {
                nameText.fontSize = 24; // Normal
                nameText.fontStyle = FontStyles.Normal;
            }
        }
    }

    private void ClearPanel(GameObject panel, List<GameObject> nameList)
    {
        // Elimina todos los elementos en un panel
        foreach (GameObject nameGO in nameList)
        {
            Destroy(nameGO);
        }

        nameList.Clear();
    }
}

