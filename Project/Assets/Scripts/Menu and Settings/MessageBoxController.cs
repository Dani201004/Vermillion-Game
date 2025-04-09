using System.Collections;
using UnityEngine;
using TMPro;

public class MessageBoxController : MonoBehaviour
{
    public float moveSpeed = 5000f; // Velocidad del movimiento
    public float displayTime = 0.65f; // Tiempo visible
    private RectTransform rectTransform;
    private TMP_Text messageText;

    private Vector3 startPosition; // Posición inicial fuera del canvas
    private Vector3 endPosition;   // Posición visible dentro del canvas

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        messageText = GetComponentInChildren<TMP_Text>();

        // Definir la posición inicial y la final
        startPosition = rectTransform.anchoredPosition;
        endPosition = new Vector3(startPosition.x, 0, startPosition.z); // Centrar en la parte superior
    }

    public void ShowMessage(string message)
    {
        StopAllCoroutines(); // Detener cualquier animación en curso
        messageText.text = message; // Cambiar el texto del mensaje
        StartCoroutine(AnimateMessage());
    }

    private IEnumerator AnimateMessage()
    {
        // Mover hacia adentro
        yield return StartCoroutine(MoveToPosition(endPosition));

        // Esperar el tiempo visible
        yield return new WaitForSeconds(displayTime);

        // Mover hacia afuera
        yield return StartCoroutine(MoveToPosition(startPosition));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(rectTransform.anchoredPosition, targetPosition) > 0.1f)
        {
            rectTransform.anchoredPosition = Vector3.MoveTowards(rectTransform.anchoredPosition, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPosition;
    }
}
