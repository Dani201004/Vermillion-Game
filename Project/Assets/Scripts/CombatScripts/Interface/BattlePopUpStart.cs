using UnityEngine;
using TMPro;

public class BattlePopUpStart : MonoBehaviour
{
    // Referencia al texto de TextMeshPro en la UI donde se mostrar� el mensaje
    public TextMeshProUGUI battleMessage;

    // Referencia al Canvas que contiene el texto
    public Canvas battleCanvas;

    // Variable para controlar el tiempo de duraci�n del mensaje
    public float messageDuration = 2f;

    void Start()
    {
        // Muestra el mensaje de inicio
        battleMessage.text = "DUEL UNTIL DEATH!";

        // Llama a la funci�n que desactiva el Canvas despu�s de un cierto tiempo
        Invoke("HideMessage", messageDuration);
    }

    // Funci�n que desactiva el Canvas despu�s de cierto tiempo
    void HideMessage()
    {
        // Desactiva el Canvas que contiene el texto
        battleCanvas.gameObject.SetActive(false);
    }
}
