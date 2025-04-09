using System.Collections;
using UnityEngine;

public class CanvasResize : MonoBehaviour
{
    private RectTransform canvasRectTransform; // Referencia al RectTransform del Canvas
    public float targetScale = 0.5f; // Escala objetivo (50% del tama�o original)
    public float duration = 1f; // Duraci�n de la animaci�n
    private bool isResized = false; // Para verificar si el canvas est� reducido o no



    private void Start()
    {
        canvasRectTransform = GetComponent<RectTransform>();
    }
    // M�todo que inicia la animaci�n de reducci�n o restauraci�n
    public void ToggleResize()
    {
        if (isResized)
        {
            StartCoroutine(ResizeCanvas(Vector3.one)); // Volver al tama�o original
        }
        else
        {
            StartCoroutine(ResizeCanvas(new Vector3(targetScale, targetScale, 1f))); // Reducir a la mitad
        }
        isResized = !isResized; // Cambiar el estado de si est� reducido o no
    }

    // Corutina para redimensionar el canvas
    private IEnumerator ResizeCanvas(Vector3 targetScaleVector)
    {
        Vector3 initialScale = canvasRectTransform.localScale; // Guarda el tama�o actual

        float timeElapsed = 0f;

        // Animaci�n de cambio de tama�o
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            // Interpolaci�n del tama�o (usamos Lerp para un cambio suave)
            canvasRectTransform.localScale = Vector3.Lerp(initialScale, targetScaleVector, timeElapsed / duration);

            yield return null;
        }

        // Asegurarse de que el canvas llegue exactamente al tama�o objetivo al final
        canvasRectTransform.localScale = targetScaleVector;
    }
}
