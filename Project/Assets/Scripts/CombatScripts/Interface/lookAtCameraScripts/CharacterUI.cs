using UnityEngine;

public class CharacterUI : BaseCharacterUI
{
    [Header("Flecha de turno")]
    [Tooltip("Canvas que contiene la flecha que indica el turno (solo para PlayerStats)")]
    public GameObject arrowCanvas;  // El Canvas que contiene la flecha

    [Header("Cámara para la flecha")]
    [Tooltip("Cámara hacia la que mirará la flecha")]
    [SerializeField] private Camera arrowCamera;

    private void LateUpdate()
    {
        // Verificamos si el Canvas de la flecha y la cámara están asignados
        if (arrowCanvas != null && arrowCamera != null)
        {
            // Hace que el Canvas que contiene la flecha siempre mire a la cámara
            arrowCanvas.transform.LookAt(arrowCanvas.transform.position + arrowCamera.transform.rotation * Vector3.forward, arrowCamera.transform.rotation * Vector3.up);

            // Rotamos 180° la flecha para que mire hacia abajo si es necesario
            arrowCanvas.transform.Rotate(180f, 0f, 0f);
        }
    }

    /// <summary>
    /// Activa o desactiva el Canvas que contiene la flecha de turno.
    /// </summary>
    public void SetTurnIndicator(bool active)
    {
        if (arrowCanvas != null)
        {
            arrowCanvas.SetActive(active);
        }
    }
}