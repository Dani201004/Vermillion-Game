using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimpleConsole : MonoBehaviour
{
    // Referencia al GameObject que contiene el Panel de la consola
    public GameObject consolePanel;

    // Referencias a los campos de texto
    public TextMeshProUGUI consoleTextHit;
    public TextMeshProUGUI consoleTextDamage;
    public TextMeshProUGUI consoleTextName;

    // Bandera para saber si la consola está activa o no
    private bool isConsoleVisible = false;

    // Cola de mensajes
    private Queue<string> messageQueue = new Queue<string>();

    // Función para mostrar la consola
    public void ShowConsole()
    {
        consolePanel.SetActive(true);
        isConsoleVisible = true;
    }

    // Función para ocultar la consola
    public void HideConsole()
    {
        consolePanel.SetActive(false);
        isConsoleVisible = false;
    }

    // Función para agregar un nuevo mensaje a la cola
    public void AddMessageToQueue(string newMessage)
    {
        messageQueue.Enqueue(newMessage);
        if (messageQueue.Count == 1)  // Si es el primer mensaje en la cola, comenzamos a procesarlos
        {
            StartCoroutine(ProcessMessages());
        }
    }

    // Corutina que procesa los mensajes en la cola con un tiempo entre cada uno
    private IEnumerator ProcessMessages()
    {
        while (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();

            // Verificar el contenido de cada mensaje y actualizar los textos correspondientes
            if (message.Contains("HIT") || message.Contains("MISS"))
            {
                consoleTextHit.text = message; // Actualizar el texto de precisión (hit/miss)
            }
            else if (message.Contains("Golpe") || message.Contains("Habilidad"))
            {
                consoleTextDamage.text = message; // Actualizar el texto de daño
            }
            else
            {
                consoleTextName.text = message; // Actualizar el texto de nombre
            }

            // Esperar un segundo antes de mostrar el siguiente mensaje
            yield return new WaitForSeconds(1.2f);
        }

        // Ocultar la consola después de que se procesen todos los mensajes
        HideConsole();
    }

    // Función para cambiar el texto del campo de "hits"
    public void UpdateHitText(string newHitText)
    {
        if (consoleTextHit != null)
        {
            AddMessageToQueue(newHitText);  // Agregar mensaje a la cola para que se procese
        }
    }

    // Función para cambiar el texto del campo de "daño"
    public void UpdateDamageText(string newDamageText)
    {
        if (consoleTextDamage != null)
        {
            AddMessageToQueue(newDamageText);  // Agregar mensaje a la cola para que se procese
        }
    }

    // Función para cambiar el texto del campo de "nombre"
    public void UpdateNameText(string newNameText)
    {
        if (consoleTextName != null)
        {
            AddMessageToQueue(newNameText);  // Agregar mensaje a la cola para que se procese
        }
    }

    // Función para alternar la visibilidad de la consola
    public void ToggleConsole()
    {
        if (isConsoleVisible)
        {
            HideConsole();
        }
        else
        {
            ShowConsole();
        }
    }
}
