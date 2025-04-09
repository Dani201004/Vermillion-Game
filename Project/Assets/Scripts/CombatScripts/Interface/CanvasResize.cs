using System.Collections;
using UnityEngine;

public class CanvasResize : MonoBehaviour
{
    private RectTransform canvasRectTransform; // Referencia al RectTransform del Canvas
    public float targetScale = 0.5f; // Escala objetivo (50% del tamaño original)
    public float duration = 1f; // Duración de la animación
    private bool isResized = false; // Para verificar si el canvas está reducido o no



    private void Start()
    {
        canvasRectTransform = GetComponent<RectTransform>();
    }
    // Método que inicia la animación de reducción o restauración
    public void ToggleResize()
    {
        if (isResized)
        {
            StartCoroutine(ResizeCanvas(Vector3.one)); // Volver al tamaño original
        }
        else
        {
            StartCoroutine(ResizeCanvas(new Vector3(targetScale, targetScale, 1f))); // Reducir a la mitad
        }
        isResized = !isResized; // Cambiar el estado de si está reducido o no
    }

    // Corutina para redimensionar el canvas
    private IEnumerator ResizeCanvas(Vector3 targetScaleVector)
    {
        Vector3 initialScale = canvasRectTransform.localScale; // Guarda el tamaño actual

        float timeElapsed = 0f;

        // Animación de cambio de tamaño
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            // Interpolación del tamaño (usamos Lerp para un cambio suave)
            canvasRectTransform.localScale = Vector3.Lerp(initialScale, targetScaleVector, timeElapsed / duration);

            yield return null;
        }

        // Asegurarse de que el canvas llegue exactamente al tamaño objetivo al final
        canvasRectTransform.localScale = targetScaleVector;
    }
}
