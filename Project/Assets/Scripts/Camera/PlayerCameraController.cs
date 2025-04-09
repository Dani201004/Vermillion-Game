using System.Collections;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public float targetY = 12f; // Altura adicional deseada en el espacio local
    public float smoothTime = 2f; // Tiempo de suavizado para el movimiento

    private float originalY; // Posición original local de la cámara
    private float finalY; // La altura objetivo final en el espacio local
    private bool isMoving = false;
    private Camera cam; // Referencia a la cámara

    private void Start()
    {
        // Usar localPosition para obtener la posición relativa al padre
        originalY = transform.localPosition.y;
        finalY = originalY + targetY;

        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.enabled = false; // Desactiva la cámara al inicio
        }
    }

    public IEnumerator MoveCameraUp()
    {
        if (isMoving) yield break; // Evita múltiples llamadas simultáneas
        isMoving = true;

        if (cam != null)
        {
            cam.enabled = true; // Activa la cámara antes de moverla
        }

        float velocity = 0f;
        // Movimiento suave hacia la posición final local
        while (Mathf.Abs(transform.localPosition.y - finalY) > 0.05f)
        {
            float newY = Mathf.SmoothDamp(transform.localPosition.y, finalY, ref velocity, smoothTime);
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
            yield return null;
        }

        transform.localPosition = new Vector3(transform.localPosition.x, finalY, transform.localPosition.z);

        // Espera 1.6 segundos en la posición final
        yield return new WaitForSeconds(1.6f);

        if (cam != null)
        {
            cam.enabled = false;
        }

        // Regresar instantáneamente a la posición original sin mostrar el cambio
        transform.localPosition = new Vector3(transform.localPosition.x, originalY, transform.localPosition.z);
        isMoving = false;
    }
}