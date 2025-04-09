using UnityEngine;
using TMPro;

public class ChangeResult : MonoBehaviour
{
    private TextMeshProUGUI diceResultText;
    private static string diceValue;

    private void Awake()
    {
        // Obtiene el componente TextMeshProUGUI en este GameObject
        diceResultText = GetComponent<TextMeshProUGUI>();

        if (diceResultText == null)
        {
            Debug.LogWarning($"[{gameObject.name}] No se encontró TextMeshProUGUI en este GameObject.");
        }
        else if (!string.IsNullOrEmpty(diceValue))
        {
            diceResultText.text = ""; // Restaurar el último valor asignado
        }
    }


    public static void ChangeTextResult(string result)
    {
        diceValue = result; // Guarda el último valor

        // Encuentra todas las instancias de ChangeResult, incluyendo objetos desactivados
        ChangeResult[] instances = FindObjectsOfType<ChangeResult>(true);

        foreach (ChangeResult instance in instances)
        {
            if (instance.diceResultText != null)
            {
                instance.diceResultText.text = result;
            }
            else
            {
                Debug.LogWarning($"[{instance.gameObject.name}] No se puede cambiar el texto, TextMeshProUGUI es null.");
            }
        }
    }
}