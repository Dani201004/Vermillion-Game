using UnityEngine;
using TMPro;

public class BattlePopUpStart : MonoBehaviour
{
    // Referencia al texto de TextMeshPro en la UI donde se mostrará el mensaje
    public TextMeshProUGUI battleMessage;

    // Referencia al Canvas que contiene el texto
    public Canvas battleCanvas;

    // Variable para controlar el tiempo de duración del mensaje
    public float messageDuration = 2f;

    void Start()
    {
        // Muestra el mensaje de inicio
        battleMessage.text = "DUEL UNTIL DEATH!";

        // Llama a la función que desactiva el Canvas después de un cierto tiempo
        Invoke("HideMessage", messageDuration);
    }

    // Función que desactiva el Canvas después de cierto tiempo
    void HideMessage()
    {
        // Desactiva el Canvas que contiene el texto
        battleCanvas.gameObject.SetActive(false);
    }
}
