using System.Collections;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public float targetY = 12f; // Altura adicional deseada en el espacio local
    public float smoothTime = 2f; // Tiempo de suavizado para el movimiento

    private float originalY; // Posici�n original local de la c�mara
    private float finalY; // La altura objetivo final en el espacio local
    private bool isMoving = false;
    private Camera cam; // Referencia a la c�mara

    private void Start()
    {
        // Usar localPosition para obtener la posici�n relativa al padre
        originalY = transform.localPosition.y;
        finalY = originalY + targetY;

        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.enabled = false; // Desactiva la c�mara al inicio
        }
    }

    public IEnumerator MoveCameraUp()
    {
        if (isMoving) yield break; // Evita m�ltiples llamadas simult�neas
        isMoving = true;

        if (cam != null)
        {
            cam.enabled = true; // Activa la c�mara antes de moverla
        }

        float velocity = 0f;
        // Movimiento suave hacia la posici�n final local
        while (Mathf.Abs(transform.localPosition.y - finalY) > 0.05f)
        {
            float newY = Mathf.SmoothDamp(transform.localPosition.y, finalY, ref velocity, smoothTime);
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
            yield return null;
        }

        transform.localPosition = new Vector3(transform.localPosition.x, finalY, transform.localPosition.z);

        // Espera 1.6 segundos en la posici�n final
        yield return new WaitForSeconds(1.6f);

        if (cam != null)
        {
            cam.enabled = false;
        }

        // Regresar instant�neamente a la posici�n original sin mostrar el cambio
        transform.localPosition = new Vector3(transform.localPosition.x, originalY, transform.localPosition.z);
        isMoving = false;
    }
}